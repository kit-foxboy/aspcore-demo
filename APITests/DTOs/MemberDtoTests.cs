using System.ComponentModel.DataAnnotations;
using API.DTOs;

namespace API.Tests.DTOs
{
  [TestClass]
  public class MemberDtoTests
  {
    [TestMethod]
    public void MemberDto_Properties_CanBeSetAndRetrieved()
    {
      // Arrange
      var photos = new List<PhotoDto>
      {
        new PhotoDto { Id = 1, Url = "photo1.jpg", IsMain = true },
        new PhotoDto { Id = 2, Url = "photo2.jpg", IsMain = false }
      };

      // Act
      var memberDto = new MemberDto
      {
        Id = 1,
        UserName = "testuser",
        Age = 25,
        KnownAs = "Test User",
        Created = DateTime.Parse("2023-01-01"),
        LastActive = DateTime.Parse("2024-01-01"),
        Gender = "male",
        IsPredator = false,
        Introduction = "Test introduction",
        Interests = "Test interests",
        Bio = "Test bio",
        LookingFor = "Test looking for",
        City = "Test City",
        Country = "Test Country",
        Photos = photos
      };

      // Assert
      Assert.AreEqual(1, memberDto.Id);
      Assert.AreEqual("testuser", memberDto.UserName);
      Assert.AreEqual(25, memberDto.Age);
      Assert.AreEqual("Test User", memberDto.KnownAs);
      Assert.AreEqual(DateTime.Parse("2023-01-01"), memberDto.Created);
      Assert.AreEqual(DateTime.Parse("2024-01-01"), memberDto.LastActive);
      Assert.AreEqual("male", memberDto.Gender);
      Assert.AreEqual(false, memberDto.IsPredator);
      Assert.AreEqual("Test introduction", memberDto.Introduction);
      Assert.AreEqual("Test interests", memberDto.Interests);
      Assert.AreEqual("Test bio", memberDto.Bio);
      Assert.AreEqual("Test looking for", memberDto.LookingFor);
      Assert.AreEqual("Test City", memberDto.City);
      Assert.AreEqual("Test Country", memberDto.Country);
      Assert.AreEqual(2, memberDto.Photos.Count);
    }

    [TestMethod]
    public void MemberDto_DefaultValues_AreCorrect()
    {
      // Arrange & Act
      var memberDto = new MemberDto
      {
        Gender = "male",
        IsPredator = false
      };

      // Assert
      Assert.AreEqual(0, memberDto.Id);
      Assert.IsNull(memberDto.UserName);
      Assert.AreEqual(0, memberDto.Age);
      Assert.IsNull(memberDto.KnownAs);
      Assert.AreEqual(default(DateTime), memberDto.Created);
      Assert.AreNotEqual(default(DateTime), memberDto.LastActive); // Has default value
      Assert.AreEqual("male", memberDto.Gender);
      Assert.AreEqual(false, memberDto.IsPredator);
      Assert.IsNull(memberDto.Introduction);
      Assert.IsNull(memberDto.Interests);
      Assert.IsNull(memberDto.Bio);
      Assert.IsNull(memberDto.LookingFor);
      Assert.IsNull(memberDto.City);
      Assert.IsNull(memberDto.Country);
      Assert.IsNotNull(memberDto.Photos);
      Assert.AreEqual(0, memberDto.Photos.Count);
    }

    // TODO: make the id always positive in the future
    [TestMethod]
    public void MemberDto_Id_CanBeNegative()
    {
      // Arrange & Act
      var memberDto = new MemberDto
      {
        Id = -1,
        Gender = "female",
        IsPredator = true
      };

      // Assert
      Assert.AreEqual(-1, memberDto.Id);
    }

    [TestMethod]
    public void MemberDto_Age_CanBeZero()
    {
      // Arrange & Act
      var memberDto = new MemberDto
      {
        Age = 0,
        Gender = "non-binary",
        IsPredator = false
      };

      // Assert
      Assert.AreEqual(0, memberDto.Age);
    }

    [TestMethod]
    public void MemberDto_Photos_CanBeEmpty()
    {
      // Arrange & Act
      var memberDto = new MemberDto
      {
        Gender = "female",
        IsPredator = false,
        Photos = new List<PhotoDto>()
      };

      // Assert
      Assert.IsNotNull(memberDto.Photos);
      Assert.AreEqual(0, memberDto.Photos.Count);
    }

    [TestMethod]
    public void MemberDto_Photos_CanContainMultipleItems()
    {
      // Arrange
      var photos = new List<PhotoDto>
      {
        new PhotoDto { Id = 1, Url = "photo1.jpg", IsMain = true },
        new PhotoDto { Id = 2, Url = "photo2.jpg", IsMain = false },
        new PhotoDto { Id = 3, Url = "photo3.jpg", IsMain = false }
      };

      // Act
      var memberDto = new MemberDto
      {
        Gender = "male",
        IsPredator = false,
        Photos = photos
      };

      // Assert
      Assert.AreEqual(3, memberDto.Photos.Count);
      Assert.IsTrue(memberDto.Photos.Any(p => p.IsMain));
      Assert.AreEqual(2, memberDto.Photos.Count(p => !p.IsMain));
    }

    [TestMethod]
    public void MemberDto_StringProperties_CanBeNull()
    {
      // Arrange & Act
      var memberDto = new MemberDto
      {
        UserName = null,
        KnownAs = null,
        Introduction = null,
        Interests = null,
        Bio = null,
        LookingFor = null,
        City = null,
        Country = null,
        Gender = "other",
        IsPredator = false
      };

      // Assert
      Assert.IsNull(memberDto.UserName);
      Assert.IsNull(memberDto.KnownAs);
      Assert.IsNull(memberDto.Introduction);
      Assert.IsNull(memberDto.Interests);
      Assert.IsNull(memberDto.Bio);
      Assert.IsNull(memberDto.LookingFor);
      Assert.IsNull(memberDto.City);
      Assert.IsNull(memberDto.Country);
    }

    [TestMethod]
    public void MemberDto_StringProperties_CanBeEmpty()
    {
      // Arrange & Act
      var memberDto = new MemberDto
      {
        UserName = "",
        KnownAs = "",
        Introduction = "",
        Interests = "",
        Bio = "",
        LookingFor = "",
        City = "",
        Country = "",
        Gender = "female",
        IsPredator = true
      };

      // Assert
      Assert.AreEqual("", memberDto.UserName);
      Assert.AreEqual("", memberDto.KnownAs);
      Assert.AreEqual("", memberDto.Introduction);
      Assert.AreEqual("", memberDto.Interests);
      Assert.AreEqual("", memberDto.Bio);
      Assert.AreEqual("", memberDto.LookingFor);
      Assert.AreEqual("", memberDto.City);
      Assert.AreEqual("", memberDto.Country);
    }

    [TestMethod]
    public void MemberDto_DateTimes_CanBeSetToSpecificValues()
    {
      // Arrange
      var created = new DateTime(2020, 1, 1, 10, 30, 0);
      var lastActive = new DateTime(2024, 12, 31, 23, 59, 59);

      // Act
      var memberDto = new MemberDto
      {
        Created = created,
        LastActive = lastActive,
        Gender = "male",
        IsPredator = false
      };

      // Assert
      Assert.AreEqual(created, memberDto.Created);
      Assert.AreEqual(lastActive, memberDto.LastActive);
    }

    [TestMethod]
    public void MemberDto_Photos_ModifyingCollection_WorksCorrectly()
    {
      // Arrange
      var memberDto = new MemberDto
      {
        Gender = "female",
        IsPredator = false
      };

      // Act - Add photos
      memberDto.Photos.Add(new PhotoDto { Id = 1, Url = "photo1.jpg", IsMain = true });
      memberDto.Photos.Add(new PhotoDto { Id = 2, Url = "photo2.jpg", IsMain = false });

      // Assert
      Assert.AreEqual(2, memberDto.Photos.Count);

      // Act - Remove a photo
      memberDto.Photos.RemoveAt(0);

      // Assert
      Assert.AreEqual(1, memberDto.Photos.Count);
      Assert.AreEqual("photo2.jpg", memberDto.Photos[0].Url);
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
