using APITests.Helpers;
using APITests.Factories;
using System.Net.Http.Json;
using API.DTOs;
using System.Net.Http.Headers;
using System.Net;

namespace API.Controllers.Tests
{
  [TestClass()]
  public class UsersControllerTests : DbTest
  {
    private static HttpClient? _client;

    private async Task<HttpResponseMessage> login(LoginDto loginDto)
    {
      var client = _client!;

      // Make Http request
      var result = await client.PostAsJsonAsync("api/account/login", loginDto);

      return result;
    }

    [ClassInitialize]
    public static void Initialize(TestContext context)
    {
      var factory = new CustomWebApplicationFactory();
      _client = factory.CreateClient();
    }

    [TestMethod()]
    public async Task GetUsersTest()
    {
      // Arrange
      var client = _client!;

      // Act
      var result = await login(new LoginDto
      {
        Username = "finley",
        Password = "pa$$w0rd"
      });

      // Assert
      var responseContent = await result.Content.ReadFromJsonAsync<UserDto>();
      Assert.IsNotNull(responseContent);

      client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", responseContent.Token);

      var usersResult = await client.GetAsync("api/users");


      Assert.IsTrue(usersResult.IsSuccessStatusCode);
      var usersContent = await usersResult.Content.ReadFromJsonAsync<List<MemberDto>>();
      Assert.IsNotNull(usersContent);
      Assert.IsTrue(usersContent.Count > 0);
    }

    [TestMethod()]
    public async Task FailGetUsersTest()
    {
      // Arrange
      var client = _client!;
      client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "bad token");

      // Act
      var usersResult = await client.GetAsync("api/users");

      // Assert
      Assert.IsFalse(usersResult.IsSuccessStatusCode);
      Assert.IsTrue(usersResult.StatusCode == HttpStatusCode.Unauthorized);
    }

    [TestMethod()]
    public async Task GetUserTest()
    {
      // Arrange
      var client = _client!;
      var username = "finley";

      // Act
      var loginResult = await login(new LoginDto
      {
        Username = "finley",
        Password = "pa$$w0rd"
      });
      var user = await loginResult.Content.ReadFromJsonAsync<UserDto>();
      Assert.IsNotNull(user);

      client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", user.Token);
      var result = await client.GetAsync($"api/users/{username}");

      // Assert
      Assert.IsTrue(result.IsSuccessStatusCode);
      var userContent = await result.Content.ReadFromJsonAsync<MemberDto>();
      Assert.IsNotNull(userContent);
      Assert.AreEqual(username, userContent.UserName);
    }
    [TestMethod()]
    public async Task FailGetUserTest()
    {
      // Arrange
      var client = _client!;
      var username = "finley";

      // Act
      client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "bad token");
      var result = await client.GetAsync($"api/users/{username}");

      // Assert
      Assert.IsFalse(result.IsSuccessStatusCode);
      Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode);
    }
  }
}