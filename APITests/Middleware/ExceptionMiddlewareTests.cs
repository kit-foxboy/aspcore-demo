using System.Net;
using System.Text.Json;
using API.Errors;
using API.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace API.Tests.Middleware
{
    [TestClass]
    public class ExceptionMiddlewareTests
    {
        private Mock<RequestDelegate> _mockNext = null!;
        private Mock<ILogger<ExceptionMiddleware>> _mockLogger = null!;
        private Mock<IHostEnvironment> _mockHostEnvironment = null!;
        private DefaultHttpContext _httpContext = null!;
        private ExceptionMiddleware _middleware = null!;

        [TestInitialize]
        public void Setup()
        {
            _mockNext = new Mock<RequestDelegate>();
            _mockLogger = new Mock<ILogger<ExceptionMiddleware>>();
            _mockHostEnvironment = new Mock<IHostEnvironment>();
            _httpContext = new DefaultHttpContext();
            _httpContext.Response.Body = new MemoryStream();
        }

        private void SetupMiddlewareForEnvironment(string environmentName)
        {
            _mockHostEnvironment.Setup(x => x.EnvironmentName).Returns(environmentName);
            _middleware = new ExceptionMiddleware(_mockNext.Object, _mockLogger.Object, _mockHostEnvironment.Object);
        }

        private void SetupExceptionThrow(Exception exception)
        {
            _mockNext.Setup(x => x(_httpContext)).ThrowsAsync(exception);
        }

        private async Task<ApiException> GetResponseAsApiException()
        {
            _httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(_httpContext.Response.Body).ReadToEndAsync();
            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            return JsonSerializer.Deserialize<ApiException>(responseBody, options)!;
        }

        [TestMethod]
        public async Task InvokeAsync_NoException_CallsNextDelegate()
        {
            // Arrange
            SetupMiddlewareForEnvironment("Production");

            // Act
            await _middleware.InvokeAsync(_httpContext);

            // Assert
            _mockNext.Verify(x => x(_httpContext), Times.Once);
        }

        [TestMethod]
        public async Task InvokeAsync_ExceptionThrown_InDevelopment_ReturnsDetailedError()
        {
            // Arrange
            var exception = new InvalidOperationException("Test exception");
            SetupExceptionThrow(exception);
            SetupMiddlewareForEnvironment("Development");

            // Act
            await _middleware.InvokeAsync(_httpContext);

            // Assert
            Assert.AreEqual((int)HttpStatusCode.InternalServerError, _httpContext.Response.StatusCode);
            Assert.AreEqual("application/json", _httpContext.Response.ContentType);

            var apiException = await GetResponseAsApiException();
            Assert.IsNotNull(apiException);
            Assert.AreEqual((int)HttpStatusCode.InternalServerError, apiException.StatusCode);
            Assert.AreEqual("Test exception", apiException.Message);
            Assert.IsNotNull(apiException.Details);
        }

        [TestMethod]
        public async Task InvokeAsync_ExceptionThrown_InProduction_ReturnsGenericError()
        {
            // Arrange
            var exception = new InvalidOperationException("Sensitive information");
            SetupExceptionThrow(exception);
            SetupMiddlewareForEnvironment("Production");

            // Act
            await _middleware.InvokeAsync(_httpContext);

            // Assert
            Assert.AreEqual((int)HttpStatusCode.InternalServerError, _httpContext.Response.StatusCode);
            Assert.AreEqual("application/json", _httpContext.Response.ContentType);

            var apiException = await GetResponseAsApiException();
            Assert.IsNotNull(apiException);
            Assert.AreEqual((int)HttpStatusCode.InternalServerError, apiException.StatusCode);
            Assert.AreEqual("Internal Server Error", apiException.Message);
            Assert.IsNull(apiException.Details);
        }

        [TestMethod]
        public async Task InvokeAsync_ExceptionThrown_LogsError()
        {
            // Arrange
            var exception = new InvalidOperationException("Test exception");
            SetupExceptionThrow(exception);
            SetupMiddlewareForEnvironment("Production");

            // Act
            await _middleware.InvokeAsync(_httpContext);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Test exception")),
                    exception,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [TestMethod]
        public async Task InvokeAsync_ExceptionThrown_SetsCorrectResponseHeaders()
        {
            // Arrange
            var exception = new ArgumentException("Test argument exception");
            SetupExceptionThrow(exception);
            SetupMiddlewareForEnvironment("Development");

            // Act
            await _middleware.InvokeAsync(_httpContext);

            // Assert
            Assert.AreEqual("application/json", _httpContext.Response.ContentType);
            Assert.AreEqual((int)HttpStatusCode.InternalServerError, _httpContext.Response.StatusCode);
        }

        [TestMethod]
        public async Task InvokeAsync_ExceptionThrown_ReturnsValidJson()
        {
            // Arrange
            var exception = new NotImplementedException("Feature not implemented");
            SetupExceptionThrow(exception);
            SetupMiddlewareForEnvironment("Development");

            // Act
            await _middleware.InvokeAsync(_httpContext);

            // Assert
            _httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(_httpContext.Response.Body).ReadToEndAsync();
            
            // Should not throw when deserializing
            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var apiException = JsonSerializer.Deserialize<ApiException>(responseBody, options);
            Assert.IsNotNull(apiException);
            Assert.IsTrue(responseBody.Length > 0);
        }

        [TestMethod]
        public async Task InvokeAsync_DifferentExceptionTypes_AllHandledCorrectly()
        {
            // Arrange
            var exceptions = new Exception[]
            {
                new ArgumentNullException("param"),
                new InvalidOperationException("invalid op"),
                new NotSupportedException("not supported"),
                new UnauthorizedAccessException("unauthorized")
            };

            foreach (var exception in exceptions)
            {
                // Reset the context for each test
                _httpContext = new DefaultHttpContext();
                _httpContext.Response.Body = new MemoryStream();
                
                SetupExceptionThrow(exception);
                SetupMiddlewareForEnvironment("Development");

                // Act
                await _middleware.InvokeAsync(_httpContext);

                // Assert
                Assert.AreEqual((int)HttpStatusCode.InternalServerError, _httpContext.Response.StatusCode);
                Assert.AreEqual("application/json", _httpContext.Response.ContentType);

                _httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
                var responseBody = await new StreamReader(_httpContext.Response.Body).ReadToEndAsync();
                Assert.IsTrue(responseBody.Length > 0, $"Response body should not be empty for {exception.GetType().Name}");
                
                // Reset mock for next iteration
                _mockNext.Reset();
            }
        }
    }
}
