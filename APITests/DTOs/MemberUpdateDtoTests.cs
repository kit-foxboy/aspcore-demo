using Microsoft.VisualStudio.TestTools.UnitTesting;
using API.DTOs;

namespace API.Tests.DTOs
{
  [TestClass]
  public class MemberUpdateDtoTests
  {
    [TestMethod]
    public void MemberUpdateDto_Properties_CanBeSetAndRetrieved()
    {
      // Arrange & Act
      var memberUpdateDto = new MemberUpdateDto
      {
        Introduction = "Test introduction",
        LookingFor = "Test looking for",
        Interests = "Test interests",
        City = "Test City",
        Country = "Test Country"
      };

      // Assert
      Assert.AreEqual("Test introduction", memberUpdateDto.Introduction);
      Assert.AreEqual("Test looking for", memberUpdateDto.LookingFor);
      Assert.AreEqual("Test interests", memberUpdateDto.Interests);
      Assert.AreEqual("Test City", memberUpdateDto.City);
      Assert.AreEqual("Test Country", memberUpdateDto.Country);
    }

    [TestMethod]
    public void MemberUpdateDto_DefaultValues_AreEmpty()
    {
      // Arrange & Act
      var memberUpdateDto = new MemberUpdateDto();

      // Assert
      Assert.AreEqual(string.Empty, memberUpdateDto.Introduction);
      Assert.AreEqual(string.Empty, memberUpdateDto.LookingFor);
      Assert.AreEqual(string.Empty, memberUpdateDto.Interests);
      Assert.AreEqual(string.Empty, memberUpdateDto.City);
      Assert.AreEqual(string.Empty, memberUpdateDto.Country);
    }

    [TestMethod]
    public void MemberUpdateDto_Properties_CanBeSetToNull()
    {
      // Arrange & Act
      var memberUpdateDto = new MemberUpdateDto
      {
        Introduction = null,
        LookingFor = null,
        Interests = null,
        City = null,
        Country = null
      };

      // Assert
      Assert.IsNull(memberUpdateDto.Introduction);
      Assert.IsNull(memberUpdateDto.LookingFor);
      Assert.IsNull(memberUpdateDto.Interests);
      Assert.IsNull(memberUpdateDto.City);
      Assert.IsNull(memberUpdateDto.Country);
    }

    [TestMethod]
    public void MemberUpdateDto_Properties_CanBeSetToEmptyString()
    {
      // Arrange & Act
      var memberUpdateDto = new MemberUpdateDto
      {
        Introduction = "",
        LookingFor = "",
        Interests = "",
        City = "",
        Country = ""
      };

      // Assert
      Assert.AreEqual("", memberUpdateDto.Introduction);
      Assert.AreEqual("", memberUpdateDto.LookingFor);
      Assert.AreEqual("", memberUpdateDto.Interests);
      Assert.AreEqual("", memberUpdateDto.City);
      Assert.AreEqual("", memberUpdateDto.Country);
    }


    [TestMethod]
    public void MemberUpdateDto_PartialUpdate_OtherPropertiesUnchanged()
    {
      // Arrange
      var memberUpdateDto = new MemberUpdateDto
      {
        Introduction = "Original introduction"
      };

      // Act - Only update city
      memberUpdateDto.City = "New City";

      // Assert
      Assert.AreEqual("Original introduction", memberUpdateDto.Introduction);
      Assert.AreEqual("New City", memberUpdateDto.City);
      Assert.AreEqual(string.Empty, memberUpdateDto.LookingFor);
      Assert.AreEqual(string.Empty, memberUpdateDto.Interests);
      Assert.AreEqual(string.Empty, memberUpdateDto.Country);
    }

    [TestMethod]
    public void MemberUpdateDto_AllPropertiesSet_ToValidValues()
    {
      // Arrange & Act
      var memberUpdateDto = new MemberUpdateDto
      {
        Introduction = "I'm a software developer who loves outdoor activities.",
        LookingFor = "Someone who shares my passion for technology and adventure.",
        Interests = "Programming, Hiking, Photography, Gaming, Cooking",
        City = "San Francisco",
        Country = "United States"
      };

      // Assert
      Assert.AreEqual("I'm a software developer who loves outdoor activities.", memberUpdateDto.Introduction);
      Assert.AreEqual("Someone who shares my passion for technology and adventure.", memberUpdateDto.LookingFor);
      Assert.AreEqual("Programming, Hiking, Photography, Gaming, Cooking", memberUpdateDto.Interests);
      Assert.AreEqual("San Francisco", memberUpdateDto.City);
      Assert.AreEqual("United States", memberUpdateDto.Country);
    }
  }
}
