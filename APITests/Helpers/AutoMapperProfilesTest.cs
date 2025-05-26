using API.DTOs;
using API.Entities;
using API.Extensions;
using AutoMapper;

namespace API.Helpers.Tests
{
  [TestClass]
  public class AutoMapperProfilesTest
  {
    private IMapper _mapper = null!;

    [TestInitialize]
    public void Setup()
    {
      var mapperConfig = new MapperConfiguration(cfg =>
      {
        cfg.AddProfile(new AutoMapperProfiles());
      });

      _mapper = mapperConfig.CreateMapper();
    }

    [TestMethod]
    public void AutoMapper_Configuration_IsValid()
    {
      // This will throw if there are any configuration errors
      _mapper.ConfigurationProvider.AssertConfigurationIsValid();
    }

    [TestMethod]
    public void Map_AppUserToMemberDto_MapsCorrectly()
    {
      // Arrange
      var user = new AppUser
      {
        Id = 1,
        UserName = "testuser",
        Gender = "male",
        IsPredator = false,
        DateOfBirth = DateOnly.Parse("1990-01-01"),
        KnownAs = "Test User",
        Created = DateTime.UtcNow,
        LastActive = DateTime.UtcNow,
        Introduction = "Test introduction",
        LookingFor = "Test looking for",
        Interests = "Test interests",
        City = "Test City",
        Country = "Test Country",
        Photos = new List<Photo>
        {
          new Photo { Id = 1, PublicId = "testId", Url = "test.jpg", IsMain = true }
        }
      };

      // Act
      var result = _mapper.Map<MemberDto>(user);

      // Assert
      Assert.AreEqual(user.Id, result.Id);
      Assert.AreEqual(user.UserName, result.UserName);
      Assert.AreEqual(user.DateOfBirth.CalculateAge(), result.Age);
      Assert.IsNotNull(result.Photos);
      Assert.AreEqual(1, result.Photos.Count());
    }

    [TestMethod]
    public void Map_AppUserToMemberDto_WithEmptyPhotos_MapsCorrectly()
    {
      // Arrange
      var user = new AppUser
      {
        Id = 1,
        UserName = "testuser",
        Gender = "male",
        IsPredator = false,
        DateOfBirth = DateOnly.Parse("1987-05-15"),
        KnownAs = "Test User",
        Created = DateTime.UtcNow,
        LastActive = DateTime.UtcNow,
        Introduction = "Test introduction",
        LookingFor = "Test looking for",
        Interests = "Test interests",
        City = "Test City",
        Country = "Test Country",
        Photos = new List<Photo>() // Empty list
      };

      // Act
      var result = _mapper.Map<MemberDto>(user);

      // Assert
      Assert.AreEqual(user.Id, result.Id);
      Assert.AreEqual(user.UserName, result.UserName);
      Assert.AreEqual(user.DateOfBirth.CalculateAge(), result.Age);
      Assert.IsNotNull(result.Photos);
      Assert.AreEqual(0, result.Photos.Count());
    }

    [TestMethod]
    public void Map_AppUserToMemberDto_WithMultiplePhotos_MapsCorrectly()
    {
      // Arrange
      var user = new AppUser
      {
        Id = 1,
        UserName = "testuser",
        Gender = "female",
        IsPredator = false,
        DateOfBirth = DateOnly.Parse("1995-12-04"),
        KnownAs = "Test User",
        Created = DateTime.UtcNow,
        LastActive = DateTime.UtcNow,
        Introduction = "Test introduction",
        LookingFor = "Test looking for",
        Interests = "Test interests",
        City = "Test City",
        Country = "Test Country",
        Photos = new List<Photo>
        {
          new Photo { Id = 1, PublicId = "testId", Url = "test.jpg", IsMain = true },
          new Photo { Id = 2, PublicId = "testId2", Url = "test2.jpg", IsMain = false },
          new Photo { Id = 3, PublicId = "testId3", Url = "test3.jpg", IsMain = false }
        }
      };

      // Act
      var result = _mapper.Map<MemberDto>(user);

      // Assert
      Assert.AreEqual(user.Id, result.Id);
      Assert.AreEqual(user.UserName, result.UserName);
      Assert.AreEqual(user.DateOfBirth.CalculateAge(), result.Age);
      Assert.IsNotNull(result.Photos);
      Assert.AreEqual(3, result.Photos.Count());
    }

    [TestMethod]
    public void Map_PhotoToPhotoDto_MapsCorrectly()
    {
      // Arrange
      var photo = new Photo
      {
        Id = 1,
        PublicId = "testId",
        Url = "test.jpg",
        IsMain = true
      };

      // Act
      var result = _mapper.Map<PhotoDto>(photo);

      // Assert
      Assert.AreEqual(photo.Id, result.Id);
      Assert.AreEqual(photo.Url, result.Url);
      Assert.AreEqual(photo.IsMain, result.IsMain);
    }

    [TestMethod]
    public void Map_PhotoToPhotoDto_WithNonMainPhoto_MapsCorrectly()
    {
      // Arrange
      var photo = new Photo
      {
        Id = 2,
        PublicId = "secondaryTestId",
        Url = "secondary.jpg",
        IsMain = false
      };

      // Act
      var result = _mapper.Map<PhotoDto>(photo);

      // Assert
      Assert.AreEqual(photo.Id, result.Id);
      Assert.AreEqual(photo.Url, result.Url);
      Assert.AreEqual(photo.IsMain, result.IsMain);
    }

    [TestMethod]
    public void Map_MemberUpdateDtoToAppUser_MapsCorrectly()
    {
      // Arrange
      var updateDto = new MemberUpdateDto
      {
        Introduction = "Test intro",
        LookingFor = "Test looking for",
        Interests = "Test interests",
        City = "Test city",
        Country = "Test country"
      };

      var user = new AppUser
      {
        Id = 1,
        UserName = "testuser",
        Gender = "male",
        IsPredator = true,
        City = "Old city",
        Country = "Old country",
        DateOfBirth = DateOnly.Parse("1990-01-01"),
        KnownAs = "Test User",
        Created = DateTime.UtcNow,
        LastActive = DateTime.UtcNow,
        Photos = new List<Photo>()
      };

      // Act
      _mapper.Map(updateDto, user);

      // Assert
      Assert.AreEqual(updateDto.Introduction, user.Introduction);
      Assert.AreEqual(updateDto.LookingFor, user.LookingFor);
      Assert.AreEqual(updateDto.Interests, user.Interests);
      Assert.AreEqual(updateDto.City, user.City);
      Assert.AreEqual(updateDto.Country, user.Country);
    }

    [TestMethod]
    public void Map_MemberUpdateDtoToAppUser_WithNullValues_MapsCorrectly()
    {
      // Arrange
      var updateDto = new MemberUpdateDto
      {
        Introduction = null,
        LookingFor = null,
        Interests = null,
        City = null,
        Country = null
      };

      var user = new AppUser
      {
        Id = 1,
        UserName = "testuser",
        Gender = "male",
        IsPredator = true,
        City = "Old city",
        Country = "Old country",
        DateOfBirth = DateOnly.Parse("1990-01-01"),
        KnownAs = "Test User",
        Created = DateTime.UtcNow,
        LastActive = DateTime.UtcNow,
        Photos = new List<Photo>()
      };

      // Act
      _mapper.Map(updateDto, user);

      // Assert
      Assert.IsNull(user.Introduction);
      Assert.IsNull(user.LookingFor);
      Assert.IsNull(user.Interests);
      Assert.IsNull(user.City);
      Assert.IsNull(user.Country);
    }

    [TestMethod]
    public void Map_MemberUpdateDtoToAppUser_WithEmptyStrings_MapsCorrectly()
    {
      // Arrange
      var updateDto = new MemberUpdateDto
      {
        Introduction = "",
        LookingFor = "",
        Interests = "",
        City = "",
        Country = ""
      };

      var user = new AppUser
      {
        Id = 1,
        UserName = "testuser",
        Gender = "male",
        IsPredator = true,
        City = "Old city",
        Country = "Old country",
        DateOfBirth = DateOnly.Parse("1991-03-03"),
        KnownAs = "Test User",
        Created = DateTime.UtcNow,
        LastActive = DateTime.UtcNow,
        Photos = new List<Photo>()
      };

      // Act
      _mapper.Map(updateDto, user);

      // Assert
      Assert.AreEqual(string.Empty, user.Introduction);
      Assert.AreEqual(string.Empty, user.LookingFor);
      Assert.AreEqual(string.Empty, user.Interests);
      Assert.AreEqual(string.Empty, user.City);
      Assert.AreEqual(string.Empty, user.Country);
    }

    [TestMethod]
    public void Map_MemberUpdateDtoToExistingAppUser_OverwritesValues()
    {
      // Arrange
      var user = new AppUser
      {
        Id = 1,
        UserName = "testuser",
        Gender = "male",
        IsPredator = false,
        DateOfBirth = DateOnly.Parse("1990-06-04"),
        KnownAs = "Test User",
        Created = DateTime.UtcNow,
        LastActive = DateTime.UtcNow,
        Introduction = "Old intro",
        LookingFor = "Old looking for",
        Interests = "Old interests",
        City = "Old city",
        Country = "Old country",
        Photos = new List<Photo>()
      };

      var updateDto = new MemberUpdateDto
      {
        Introduction = "New intro",
        LookingFor = "New looking for",
        Interests = "New interests",
        City = "New city",
        Country = "New country"
      };

      // Act
      _mapper.Map(updateDto, user);

      // Assert
      Assert.AreEqual("New intro", user.Introduction);
      Assert.AreEqual("New looking for", user.LookingFor);
      Assert.AreEqual("New interests", user.Interests);
      Assert.AreEqual("New city", user.City);
      Assert.AreEqual("New country", user.Country);
    }
  }
}