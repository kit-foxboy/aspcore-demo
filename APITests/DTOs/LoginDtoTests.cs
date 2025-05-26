using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.DataAnnotations;
using API.DTOs;

namespace API.Tests.DTOs
{
  [TestClass]
  public class LoginDtoTests
  {
    [TestMethod]
    public void LoginDto_Properties_CanBeSetAndRetrieved()
    {
      // Arrange & Act
      var loginDto = new LoginDto
      {
        Username = "testuser",
        Password = "testpassword"
      };

      // Assert
      Assert.AreEqual("testuser", loginDto.Username);
      Assert.AreEqual("testpassword", loginDto.Password);
    }

    [TestMethod]
    public void LoginDto_Username_IsRequired()
    {
      // Arrange
      var loginDto = new LoginDto
      {
        Username = "",
        Password = "testpassword"
      };

      // Act
      var validationResults = ValidateModel(loginDto);

      // Assert
      Assert.IsTrue(validationResults.Any(v => v.MemberNames.Contains("Username")));
    }

    [TestMethod]
    public void LoginDto_Password_IsRequired()
    {
      // Arrange
      var loginDto = new LoginDto
      {
        Username = "testuser",
        Password = ""
      };

      // Act
      var validationResults = ValidateModel(loginDto);

      // Assert
      Assert.IsTrue(validationResults.Any(v => v.MemberNames.Contains("Password")));
    }

    [TestMethod]
    public void LoginDto_Username_MaxLength100_Valid()
    {
      // Arrange
      var longUsername = new string('a', 100); // Exactly 100 characters
      var loginDto = new LoginDto
      {
        Username = longUsername,
        Password = "testpassword"
      };

      // Act
      var validationResults = ValidateModel(loginDto);

      // Assert
      Assert.IsFalse(validationResults.Any(v => v.MemberNames.Contains("Username")));
    }

    [TestMethod]
    public void LoginDto_Username_ExceedsMaxLength_Invalid()
    {
      // Arrange
      var longUsername = new string('a', 101); // 101 characters - exceeds limit
      var loginDto = new LoginDto
      {
        Username = longUsername,
        Password = "testpassword"
      };

      // Act
      var validationResults = ValidateModel(loginDto);

      // Assert
      Assert.IsTrue(validationResults.Any(v => v.MemberNames.Contains("Username")));
    }

    [TestMethod]
    public void LoginDto_ValidData_PassesValidation()
    {
      // Arrange
      var loginDto = new LoginDto
      {
        Username = "validuser",
        Password = "validpassword"
      };

      // Act
      var validationResults = ValidateModel(loginDto);

      // Assert
      Assert.AreEqual(0, validationResults.Count);
    }

    [TestMethod]
    public void LoginDto_Username_Null_FailsValidation()
    {
      // Arrange
      var loginDto = new LoginDto
      {
        Username = null!,
        Password = "testpassword"
      };

      // Act
      var validationResults = ValidateModel(loginDto);

      // Assert
      Assert.IsTrue(validationResults.Any(v => v.MemberNames.Contains("Username")));
    }

    [TestMethod]
    public void LoginDto_Password_Null_FailsValidation()
    {
      // Arrange
      var loginDto = new LoginDto
      {
        Username = "testuser",
        Password = null!
      };

      // Act
      var validationResults = ValidateModel(loginDto);

      // Assert
      Assert.IsTrue(validationResults.Any(v => v.MemberNames.Contains("Password")));
    }

    [TestMethod]
    public void LoginDto_Username_WhitespaceOnly_IsInvalid()
    {
      // Arrange
      var loginDto = new LoginDto
      {
        Username = "   ",
        Password = "testpassword"
      };

      // Act
      var validationResults = ValidateModel(loginDto);

      // Assert
      Assert.IsTrue(validationResults.Any(v => v.MemberNames.Contains("Username")));
    }

    [TestMethod]
    public void LoginDto_SpecialCharacters_InUsername_Valid()
    {
      // Arrange
      var loginDto = new LoginDto
      {
        Username = "user@domain.com",
        Password = "testpassword"
      };

      // Act
      var validationResults = ValidateModel(loginDto);

      // Assert
      Assert.IsFalse(validationResults.Any(v => v.MemberNames.Contains("Username")));
    }

    private static List<ValidationResult> ValidateModel(object model)
    {
      var validationResults = new List<ValidationResult>();
      var validationContext = new ValidationContext(model, null, null);
      Validator.TryValidateObject(model, validationContext, validationResults, true);
      return validationResults;
    }
  }
}
