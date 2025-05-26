using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using API.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace API.Tests.Extensions
{
    [TestClass]
    public class IdentityServiceExtensionsTests
    {
        [TestMethod]
        public void AddIdentityServices_RegistersJwtBearerAuthentication()
        {
            // Arrange
            var services = new ServiceCollection();
            var tokenKey = "super_secret_test_key_1234567890";
            var inMemorySettings = new Dictionary<string, string?> { { "TokenKey", tokenKey } };
            IConfiguration config = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            // Act
            services.AddIdentityServices(config);
            var provider = services.BuildServiceProvider();

            // Assert
            var authOptions = provider.GetService<Microsoft.AspNetCore.Authentication.IAuthenticationSchemeProvider>();
            Assert.IsNotNull(authOptions, "Authentication scheme provider should be registered.");

            // Try to resolve JwtBearerOptions
            var jwtOptions = provider.GetService<IOptionsMonitor<JwtBearerOptions>>();
            Assert.IsNotNull(jwtOptions, "JwtBearerOptions should be registered.");
        }

        [TestMethod]
        public void AddIdentityServices_WithMissingTokenKey_DoesNotThrowImmediately()
        {
            // Arrange
            var services = new ServiceCollection();
            IConfiguration config = new ConfigurationBuilder().Build();

            // Act - This should NOT throw, just like in real applications
            services.AddIdentityServices(config);
            var provider = services.BuildServiceProvider();

            // Assert - Service registration succeeds
            var authSchemeProvider = provider.GetService<Microsoft.AspNetCore.Authentication.IAuthenticationSchemeProvider>();
            Assert.IsNotNull(authSchemeProvider, "Authentication scheme provider should be registered.");
            
            // The exception would only be thrown during actual authentication in a real request
            // Testing that scenario requires integration tests with TestServer, not unit tests
        }
    }
}
