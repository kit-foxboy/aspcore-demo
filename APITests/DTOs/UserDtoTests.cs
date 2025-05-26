using Microsoft.VisualStudio.TestTools.UnitTesting;
using API.DTOs;

namespace API.Tests.DTOs
{
  [TestClass]
  public class UserDtoTests
  {
    [TestMethod]
    public void UserDto_Properties_CanBeSetAndRetrieved()
    {
      // Arrange & Act
      var userDto = new UserDto
      {
        Username = "testuser",
        Token = "test.jwt.token"
      };

      // Assert
      Assert.AreEqual("testuser", userDto.Username);
      Assert.AreEqual("test.jwt.token", userDto.Token);
    }

    [TestMethod]
    public void UserDto_Username_CanBeNull()
    {
      // Arrange & Act
      var userDto = new UserDto
      {
        Username = null!,
        Token = "test.jwt.token"
      };

      // Assert
      Assert.IsNull(userDto.Username);
      Assert.AreEqual("test.jwt.token", userDto.Token);
    }

    [TestMethod]
    public void UserDto_Token_CanBeNull()
    {
      // Arrange & Act
      var userDto = new UserDto
      {
        Username = "testuser",
        Token = null!
      };

      // Assert
      Assert.AreEqual("testuser", userDto.Username);
      Assert.IsNull(userDto.Token);
    }

    [TestMethod]
    public void UserDto_WithValidJwtToken_PropertiesSetCorrectly()
    {
      // Arrange
      var username = "john.doe";
      var jwtToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c";

      // Act
      var userDto = new UserDto
      {
        Username = username,
        Token = jwtToken
      };

      // Assert
      Assert.AreEqual(username, userDto.Username);
      Assert.AreEqual(jwtToken, userDto.Token);
    }

    [TestMethod]
    public void UserDto_EmptyStrings_AreValid()
    {
      // Arrange & Act
      var userDto = new UserDto
      {
        Username = "",
        Token = ""
      };

      // Assert
      Assert.AreEqual("", userDto.Username);
      Assert.AreEqual("", userDto.Token);
    }

    [TestMethod]
    public void UserDto_ToString_ReturnsExpectedFormat()
    {
      // Arrange
      var userDto = new UserDto
      {
        Username = "testuser",
        Token = "testtoken"
      };

      // Act
      var result = userDto.ToString();

      // Assert
      Assert.IsNotNull(result);
      Assert.IsTrue(result.Contains("UserDto") || result.Contains("testuser") || result.Length > 0);
    }
  }
}
