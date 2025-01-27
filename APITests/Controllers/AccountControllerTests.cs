using Microsoft.Extensions.DependencyInjection;
using API.DTOs;

namespace API.Controllers.Tests
{
  [TestClass()]
  public class AccountControllerTests
  {
    private static DatabaseFixture? _databaseFixture;

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
      _databaseFixture = new DatabaseFixture();
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
      Assert.IsNotNull(_databaseFixture);
      _databaseFixture.Dispose();
    }

     [TestMethod()]
    public void LoginSuccessTest()
    {
      // Arrange
      Assert.IsNotNull(_databaseFixture);
      using var scope = _databaseFixture.ServiceProvider.CreateScope();
      var controller = scope.ServiceProvider.GetRequiredService<AccountController>();
      var loginDto = new LoginDto
      {
        Username = "test",
        Password = "password"
      };

      // Act
      var result = controller.Login(loginDto);

      // Assert
      Assert.IsNotNull(result.Result.Value);
    }
    [TestMethod()]
    public void LoginFailureTest()
    {
      // Arrange
      Assert.IsNotNull(_databaseFixture);
      using var scope = _databaseFixture.ServiceProvider.CreateScope();
      var controller = scope.ServiceProvider.GetRequiredService<AccountController>();
      var loginDto = new LoginDto
      {
        Username = "falseuser",
        Password = "password"
      };

      // Act
      var result = controller.Login(loginDto);

      // Assert
      Assert.IsNull(result.Result.Value);
    }

    [TestMethod()]
    public void RegisterTest()
    {
      // Arrange
      Assert.IsNotNull(_databaseFixture);
      using var scope = _databaseFixture.ServiceProvider.CreateScope();
      var controller = scope.ServiceProvider.GetRequiredService<AccountController>();
      var registerDto = new RegisterDto
      {
        Username = "registerUser",
        Password = "password"
      };

      // Act
      var result = controller.Register(registerDto);

      // Assert
      Assert.IsNotNull(result.Result.Value);
      Assert.AreEqual(registerDto.Username.ToLower(), result.Result.Value.UserName);
      Assert.IsTrue(result.Result.Value.PasswordHash.Length > 0);
      Assert.IsTrue(result.Result.Value.PasswordSalt.Length > 0);
    }
  }
}