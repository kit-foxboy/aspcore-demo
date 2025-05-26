using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.DataAnnotations;
using API.DTOs;

namespace API.Tests.DTOs
{
  [TestClass]
  public class RegisterDtoTests
  {
    [TestMethod]
    public void RegisterDto_Properties_CanBeSetAndRetrieved()
    {
      // Arrange & Act
      var registerDto = new RegisterDto
      {
        Username = "testuser",
        Password = "testpassword"
      };

      // Assert
      Assert.AreEqual("testuser", registerDto.Username);
      Assert.AreEqual("testpassword", registerDto.Password);
    }

    [TestMethod]
    public void RegisterDto_DefaultValues_AreEmpty()
    {
      // Arrange & Act
      var registerDto = new RegisterDto();

      // Assert
      Assert.AreEqual(string.Empty, registerDto.Username);
      Assert.AreEqual(string.Empty, registerDto.Password);
    }

    [TestMethod]
    public void RegisterDto_Username_IsRequired()
    {
      // Arrange
      var registerDto = new RegisterDto
      {
        Username = "",
        Password = "testpassword"
      };

      // Act
      var validationResults = ValidateModel(registerDto);

      // Assert
      Assert.IsTrue(validationResults.Any(v => v.MemberNames.Contains("Username")));
    }

    [TestMethod]
    public void RegisterDto_Password_IsRequired()
    {
      // Arrange
      var registerDto = new RegisterDto
      {
        Username = "testuser",
        Password = ""
      };

      // Act
      var validationResults = ValidateModel(registerDto);

      // Assert
      Assert.IsTrue(validationResults.Any(v => v.MemberNames.Contains("Password")));
    }

    [TestMethod]
    public void RegisterDto_Username_MaxLength100_Valid()
    {
      // Arrange
      var longUsername = new string('a', 100); // Exactly 100 characters
      var registerDto = new RegisterDto
      {
        Username = longUsername,
        Password = "testpassword"
      };

      // Act
      var validationResults = ValidateModel(registerDto);

      // Assert
      Assert.IsFalse(validationResults.Any(v => v.MemberNames.Contains("Username")));
    }

    [TestMethod]
    public void RegisterDto_Username_ExceedsMaxLength_Invalid()
    {
      // Arrange
      var longUsername = new string('a', 101); // 101 characters - exceeds limit
      var registerDto = new RegisterDto
      {
        Username = longUsername,
        Password = "testpassword"
      };

      // Act
      var validationResults = ValidateModel(registerDto);

      // Assert
      Assert.IsTrue(validationResults.Any(v => v.MemberNames.Contains("Username")));
    }

    [TestMethod]
    public void RegisterDto_ValidData_PassesValidation()
    {
      // Arrange
      var registerDto = new RegisterDto
      {
        Username = "validuser",
        Password = "validpassword123"
      };

      // Act
      var validationResults = ValidateModel(registerDto);

      // Assert
      Assert.AreEqual(0, validationResults.Count);
    }

    [TestMethod]
    public void RegisterDto_MinimumValidInput_PassesValidation()
    {
      // Arrange
      var registerDto = new RegisterDto
      {
        Username = "a",
        Password = "p"
      };

      // Act
      var validationResults = ValidateModel(registerDto);

      // Assert
      Assert.AreEqual(0, validationResults.Count);
    }

    [TestMethod]
    public void RegisterDto_Username_WhitespaceOnly_IsInvalid()
    {
      // Arrange
      var registerDto = new RegisterDto
      {
        Username = "   ",
        Password = "testpassword"
      };

      // Act
      var validationResults = ValidateModel(registerDto);

      // Assert
      Assert.IsTrue(validationResults.Any(v => v.MemberNames.Contains("Username")));
    }

    [TestMethod]
    public void RegisterDto_BothFieldsEmpty_BothInvalid()
    {
      // Arrange
      var registerDto = new RegisterDto
      {
        Username = "",
        Password = ""
      };

      // Act
      var validationResults = ValidateModel(registerDto);

      // Assert
      Assert.IsTrue(validationResults.Any(v => v.MemberNames.Contains("Username")));
      Assert.IsTrue(validationResults.Any(v => v.MemberNames.Contains("Password")));
      Assert.AreEqual(2, validationResults.Count);
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
