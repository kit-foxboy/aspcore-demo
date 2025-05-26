using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using API.Data;
using API.Entities;
using API.DTOs;
using API.Helpers;
using API.Interfaces;
using API.Extensions;

namespace API.Tests.Data
{
  [TestClass]
  public class UserRepositoryTests
  {
    private DataContext _context = null!;
    private IMapper _mapper = null!;
    private UserRepository _repository = null!;

    [TestInitialize]
    public void Setup()
    {
      // Create in-memory database
      var options = new DbContextOptionsBuilder<DataContext>()
        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
        .Options;

      _context = new DataContext(options);

      // Setup AutoMapper
      var mapperConfig = new MapperConfiguration(cfg =>
      {
        cfg.AddProfile(new AutoMapperProfiles());
      });
      _mapper = mapperConfig.CreateMapper();

      _repository = new UserRepository(_context, _mapper);

      // Seed test data
      SeedTestData();
    }

    [TestCleanup]
    public void Cleanup()
    {
      _context.ChangeTracker.Clear();
      _context.Dispose();
    }

    private void SeedTestData()
    {
      var users = new List<AppUser>
      {
        new AppUser
        {
          Id = 1,
          UserName = "testuser1",
          KnownAs = "Test User 1",
          Gender = "male",
          IsPredator = false,
          DateOfBirth = DateOnly.Parse("1990-01-01"),
          City = "Test City 1",
          Country = "Test Country 1",
          Created = DateTime.UtcNow.AddDays(-30),
          LastActive = DateTime.UtcNow.AddDays(-1),
          Introduction = "Test introduction 1",
          LookingFor = "Test looking for 1",
          Interests = "Test interests 1",
          Photos = new List<Photo>
          {
            new Photo { Id = 1, Url = "photo1.jpg", IsMain = true, PublicId = "public1" },
            new Photo { Id = 2, Url = "photo2.jpg", IsMain = false, PublicId = "public2" }
          }
        },
        new AppUser
        {
          Id = 2,
          UserName = "testuser2",
          KnownAs = "Test User 2",
          Gender = "female",
          IsPredator = false,
          DateOfBirth = DateOnly.Parse("1985-05-15"),
          City = "Test City 2",
          Country = "Test Country 2",
          Created = DateTime.UtcNow.AddDays(-20),
          LastActive = DateTime.UtcNow.AddDays(-2),
          Introduction = "Test introduction 2",
          LookingFor = "Test looking for 2",
          Interests = "Test interests 2",
          Photos = new List<Photo>
          {
            new Photo { Id = 3, Url = "photo3.jpg", IsMain = true, PublicId = "public3" }
          }
        },
        new AppUser
        {
          Id = 3,
          UserName = "testuser3",
          KnownAs = "Test User 3",
          Gender = "male",
          IsPredator = true,
          DateOfBirth = DateOnly.Parse("1995-12-25"),
          City = "Test City 3",
          Country = "Test Country 3",
          Created = DateTime.UtcNow.AddDays(-10),
          LastActive = DateTime.UtcNow,
          Introduction = "Test introduction 3",
          LookingFor = "Test looking for 3",
          Interests = "Test interests 3",
          Photos = new List<Photo>()
        }
      };

      _context.Users.AddRange(users);
      _context.SaveChanges();
    }

    [TestMethod]
    public async Task GetMemberAsync_WithValidUsername_ReturnsMemberDto()
    {
      // Arrange
      var username = "testuser1";

      // Act
      var result = await _repository.GetMemberAsync(username);

      // Assert
      Assert.IsNotNull(result);
      Assert.AreEqual(username, result.UserName);
      Assert.AreEqual("Test User 1", result.KnownAs);
      Assert.AreEqual(2, result.Photos.Count());
      Assert.IsTrue(result.Photos.Any(p => p.IsMain));
    }

    [TestMethod]
    public async Task GetMemberAsync_WithInvalidUsername_ReturnsNull()
    {
      // Arrange
      var username = "nonexistentuser";

      // Act
      var result = await _repository.GetMemberAsync(username);

      // Assert
      Assert.IsNull(result);
    }

    [TestMethod]
    public async Task GetMemberAsync_WithNullUsername_ReturnsNull()
    {
      // Arrange
      string username = null!;

      // Act
      var result = await _repository.GetMemberAsync(username);

      // Assert
      Assert.IsNull(result);
    }

    [TestMethod]
    public async Task GetMembersAsync_ReturnsAllMembers()
    {
      // Act
      var result = await _repository.GetMembersAsync();

      // Assert
      Assert.IsNotNull(result);
      Assert.AreEqual(3, result.Count());
      
      var membersList = result.ToList();
      Assert.IsTrue(membersList.Any(m => m.UserName == "testuser1"));
      Assert.IsTrue(membersList.Any(m => m.UserName == "testuser2"));
      Assert.IsTrue(membersList.Any(m => m.UserName == "testuser3"));
    }

    [TestMethod]
    public async Task GetMembersAsync_WithEmptyDatabase_ReturnsEmptyCollection()
    {
      // Arrange - Clear database
      _context.Users.RemoveRange(_context.Users);
      await _context.SaveChangesAsync();

      // Act
      var result = await _repository.GetMembersAsync();

      // Assert
      Assert.IsNotNull(result);
      Assert.AreEqual(0, result.Count());
    }

    [TestMethod]
    public async Task GetUserByIdAsync_WithValidId_ReturnsAppUser()
    {
      // Arrange
      var userId = 1;

      // Act
      var result = await _repository.GetUserByIdAsync(userId);

      // Assert
      Assert.IsNotNull(result);
      Assert.AreEqual(userId, result.Id);
      Assert.AreEqual("testuser1", result.UserName);
      Assert.AreEqual("Test User 1", result.KnownAs);
    }

    [TestMethod]
    public async Task GetUserByIdAsync_WithInvalidId_ReturnsNull()
    {
      // Arrange
      var userId = 999;

      // Act
      var result = await _repository.GetUserByIdAsync(userId);

      // Assert
      Assert.IsNull(result);
    }

    [TestMethod]
    public async Task GetUserByNameAsync_WithValidUsername_ReturnsAppUserWithPhotos()
    {
      // Arrange
      var username = "testuser1";

      // Act
      var result = await _repository.GetUserByNameAsync(username);

      // Assert
      Assert.IsNotNull(result);
      Assert.AreEqual(username, result.UserName);
      Assert.AreEqual("Test User 1", result.KnownAs);
      Assert.IsNotNull(result.Photos);
      Assert.AreEqual(2, result.Photos.Count);
      Assert.IsTrue(result.Photos.Any(p => p.IsMain));
    }

    [TestMethod]
    public async Task GetUserByNameAsync_WithInvalidUsername_ReturnsNull()
    {
      // Arrange
      var username = "nonexistentuser";

      // Act
      var result = await _repository.GetUserByNameAsync(username);

      // Assert
      Assert.IsNull(result);
    }

    [TestMethod]
    public async Task GetUserByNameAsync_WithUserHavingNoPhotos_ReturnsUserWithEmptyPhotos()
    {
      // Arrange
      var username = "testuser3";

      // Act
      var result = await _repository.GetUserByNameAsync(username);

      // Assert
      Assert.IsNotNull(result);
      Assert.AreEqual(username, result.UserName);
      Assert.IsNotNull(result.Photos);
      Assert.AreEqual(0, result.Photos.Count);
    }

    [TestMethod]
    public async Task GetUsersAsync_ReturnsAllUsersWithPhotos()
    {
      // Act
      var result = await _repository.GetUsersAsync();

      // Assert
      Assert.IsNotNull(result);
      Assert.AreEqual(3, result.Count());

      var usersList = result.ToList();
      
      // Verify all users are returned with their photos loaded
      var user1 = usersList.First(u => u.UserName == "testuser1");
      Assert.AreEqual(2, user1.Photos.Count);
      
      var user2 = usersList.First(u => u.UserName == "testuser2");
      Assert.AreEqual(1, user2.Photos.Count);
      
      var user3 = usersList.First(u => u.UserName == "testuser3");
      Assert.AreEqual(0, user3.Photos.Count);
    }

    [TestMethod]
    public async Task GetUsersAsync_WithEmptyDatabase_ReturnsEmptyCollection()
    {
      // Arrange - Clear database
      _context.Users.RemoveRange(_context.Users);
      await _context.SaveChangesAsync();

      // Act
      var result = await _repository.GetUsersAsync();

      // Assert
      Assert.IsNotNull(result);
      Assert.AreEqual(0, result.Count());
    }

    [TestMethod]
    public async Task SaveAllAsync_WithChanges_ReturnsTrue()
    {
      // Arrange
      var user = await _repository.GetUserByIdAsync(1);
      Assert.IsNotNull(user);
      user.KnownAs = "Updated Name";
      _repository.Update(user);

      // Act
      var result = await _repository.SaveAllAsync();

      // Assert
      Assert.IsTrue(result);
      
      // Verify the change was saved
      var updatedUser = await _repository.GetUserByIdAsync(1);
      Assert.AreEqual("Updated Name", updatedUser!.KnownAs);
    }

    [TestMethod]
    public async Task SaveAllAsync_WithoutChanges_ReturnsFalse()
    {
      // Act
      var result = await _repository.SaveAllAsync();

      // Assert
      Assert.IsFalse(result);
    }

    [TestMethod]
    public async Task Update_MarksEntityAsModified()
    {
      // Arrange - Get an existing user from the database
      var user = await _repository.GetUserByIdAsync(1);
      Assert.IsNotNull(user);
      
      // Detach the entity to simulate getting it from another context
      _context.Entry(user).State = EntityState.Detached;

      // Act
      _repository.Update(user);

      // Assert
      var entry = _context.Entry(user);
      Assert.AreEqual(EntityState.Modified, entry.State);
    }

    [TestMethod]
    public async Task GetMemberAsync_MapsAgeCorrectly()
    {
      // Arrange
      var username = "testuser1";
      var expectedAge = DateOnly.Parse("1990-01-01").CalculateAge();

      // Act
      var result = await _repository.GetMemberAsync(username);

      // Assert
      Assert.IsNotNull(result);
      Assert.AreEqual(expectedAge, result.Age);
    }

    [TestMethod]
    public async Task GetMemberAsync_MapsPhotosCorrectly()
    {
      // Arrange
      var username = "testuser1";

      // Act
      var result = await _repository.GetMemberAsync(username);

      // Assert
      Assert.IsNotNull(result);
      Assert.AreEqual(2, result.Photos.Count());
      
      var mainPhoto = result.Photos.FirstOrDefault(p => p.IsMain);
      Assert.IsNotNull(mainPhoto);
      Assert.AreEqual("photo1.jpg", mainPhoto.Url);
      
      var secondaryPhoto = result.Photos.FirstOrDefault(p => !p.IsMain);
      Assert.IsNotNull(secondaryPhoto);
      Assert.AreEqual("photo2.jpg", secondaryPhoto.Url);
    }

    [TestMethod]
    public async Task Integration_AddUpdateAndRetrieveUser()
    {
      // Arrange - Add a new user
      var newUser = new AppUser
      {
        UserName = "newuser",
        KnownAs = "New User",
        Gender = "female",
        IsPredator = false,
        DateOfBirth = DateOnly.Parse("1992-06-15"),
        City = "New City",
        Country = "New Country",
        Introduction = "New introduction",
        LookingFor = "New looking for",
        Interests = "New interests"
      };

      _context.Users.Add(newUser);
      await _context.SaveChangesAsync();

      // Act & Assert - Retrieve the user
      var retrievedUser = await _repository.GetUserByNameAsync("newuser");
      Assert.IsNotNull(retrievedUser);
      Assert.AreEqual("newuser", retrievedUser.UserName);

      // Act & Assert - Update the user
      retrievedUser.Introduction = "Updated introduction";
      _repository.Update(retrievedUser);
      var saveResult = await _repository.SaveAllAsync();
      Assert.IsTrue(saveResult);

      // Act & Assert - Verify the update
      var updatedUser = await _repository.GetUserByNameAsync("newuser");
      Assert.IsNotNull(updatedUser);
      Assert.AreEqual("Updated introduction", updatedUser.Introduction);

      // Act & Assert - Get as MemberDto
      var memberDto = await _repository.GetMemberAsync("newuser");
      Assert.IsNotNull(memberDto);
      Assert.AreEqual("newuser", memberDto.UserName);
      Assert.AreEqual("Updated introduction", memberDto.Introduction);
    }
  }
}
