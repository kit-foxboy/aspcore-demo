using API.DTOs;
using APITests.Factories;
using APITests.Helpers;
using System.Net.Http.Json;
using System.Net;

namespace API.Controllers.Tests
{
  [TestClass()]
  public class AccountControllerTests : DbTest
  {
    private static HttpClient? _client;

    [ClassInitialize]
    public static void Initialize(TestContext context)
    {
      var factory = new CustomWebApplicationFactory();
      _client = factory.CreateClient();
    }

    [TestMethod()]
    public async Task LoginSuccessTest()
    {
      // Arrange
      var client = _client!;

      // Act
      var result = await client.PostAsJsonAsync("api/account/login", new LoginDto
      {
        Username = "finley",
        Password = "pa$$w0rd"
      });

      // Assert
      Assert.IsTrue(result.IsSuccessStatusCode);
      var responseContent = await result.Content.ReadFromJsonAsync<UserDto>();
      Assert.IsNotNull(responseContent);
      Assert.AreEqual("finley", responseContent.Username);
      Assert.IsTrue(responseContent.Token.Length > 0);
    }
    [TestMethod()]
    public async Task LoginFailureTest()
    {
      // Arrange
      var client = _client!;

      // Act
      var result = await client.PostAsJsonAsync("api/account/login", new LoginDto
      {
        Username = "NOPE",
        Password = "wrong"
      });

      // Assert
      Assert.IsFalse(result.IsSuccessStatusCode);
      Assert.IsTrue(result.StatusCode == HttpStatusCode.Unauthorized);
    }

    [TestMethod()]
    public async Task RegisterSuccessTest()
    {
      // Arrange
      var client = _client!;

      // Act
      var result = await client.PostAsJsonAsync("api/account/register", new RegisterDto
      {
        Username = "newuser",
        Password = "pa$$w0rd"
      });


      // Assert
      Assert.IsTrue(result.IsSuccessStatusCode);
    }

    [TestMethod()]
    public async Task RegisterFailureTest()
    {
      // Arrange
      var client = _client!;

      // Act
      var result = await client.PostAsJsonAsync("api/account/register", new RegisterDto
      {
        Username = "finley",
        Password = "pa$$w0rd"
      });


      // Assert
      Assert.IsFalse(result.IsSuccessStatusCode);
      Assert.IsTrue(result.StatusCode == HttpStatusCode.BadRequest);
    }
  }
}