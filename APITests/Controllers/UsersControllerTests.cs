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


    // Test login and user retrieval
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

    // Test login and user retrieval with bad token
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

    // Test login and user retrieval by username
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

    // Test updating user
    [TestMethod()]
    public async Task UpdateUserTest()
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

      // Arrange update user
      var updateUser = new MemberUpdateDto
      {
        Introduction = "Updated introduction",
        LookingFor = "Updated looking for",
        Interests = "Updated interests",
        City = "Updated city",
        Country = "Updated country"
      };

      var updateResult = await client.PutAsJsonAsync($"api/users", updateUser);
      Assert.IsTrue(updateResult.IsSuccessStatusCode);

      // Assert
      // Check if the user was updated successfully
      var updatedResult = await client.GetAsync($"api/users/{username}");
      Assert.IsTrue(updatedResult.IsSuccessStatusCode);
      var updatedUserContent = await updatedResult.Content.ReadFromJsonAsync<MemberDto>();
      Assert.IsNotNull(updatedUserContent);
      Assert.AreEqual(updateUser.Introduction, updatedUserContent.Introduction);
      Assert.AreEqual(updateUser.LookingFor, updatedUserContent.LookingFor);
      Assert.AreEqual(updateUser.Interests, updatedUserContent.Interests);
      Assert.AreEqual(updateUser.City, updatedUserContent.City);
      Assert.AreEqual(updateUser.Country, updatedUserContent.Country);
    }
  }
}