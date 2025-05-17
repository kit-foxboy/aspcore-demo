using System.Text.Json;
using API.Data;
using API.Entities;
using APITests.Factories;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace API.Tests.Data
{
  [TestClass]
  public class SeedTests
  {
    private DataContext _context = null!;

    [TestInitialize]
    public async Task InitializeAsync()
    {
      var factory = new CustomWebApplicationFactory();
      _context = factory.Services.GetRequiredService<DataContext>();

      await _context.Database.EnsureDeletedAsync();
      await _context.Database.EnsureCreatedAsync();
    }

    [TestMethod]
    public async Task SeedUsers_WhenUsersExist_DoesNotAddUsers()
    {
      // Arrange
      var users = new List<AppUser> { new AppUser { Id = 1, UserName = "testuser", City = "Test City", Country = "Test Country", Gender = "Test Gender", IsPredator = false, KnownAs = "Test User" } };
      _context.Users.AddRange(users);
      await _context.SaveChangesAsync();

      
      // Act
      await Seed.SeedUsers(_context);

      // Assert
      Assert.AreEqual(1, _context.Users.Count());
      Assert.IsTrue(_context.Users.Any(u => u.UserName == "testuser"));
      //Todo: Figure out how to assert that readFileAsync was not called
      
    }

    [TestMethod]
    public async Task SeedUsers_WhenNoUsers_AddsUsersFromJsonFile()
    {
      // Arrange
      var users = new List<AppUser>();
      _context.Users.AddRange(users);
      await _context.SaveChangesAsync();

      // Create test JSON file content
      var testUserData = "[{\"UserName\":\"Test1\", \"City\":\"Test City\", \"Country\":\"Test Country\", \"Gender\":\"Test Gender\", \"IsPredator\":false, \"KnownAs\":\"Test User\"},{\"UserName\":\"Test2\", \"City\":\"Test City\", \"Country\":\"Test Country\", \"Gender\":\"Test Gender\", \"IsPredator\":false, \"KnownAs\":\"Test User\"}]";
      File.WriteAllText("Data/UserSeedData.json", testUserData);

      // Act
      await Seed.SeedUsers(_context);

      // Assert
      Assert.AreEqual(2, _context.Users.Count());
      Assert.IsTrue(_context.Users.Any(u => u.UserName == "test1"));
      Assert.IsTrue(_context.Users.Any(u => u.UserName == "test2"));

      // Cleanup
      if (File.Exists("Data/UserSeedData.json"))
        File.Delete("Data/UserSeedData.json");
    }

    [TestMethod]
    public async Task SeedUsers_ShouldConvertUsernamesToLowercase()
    {
      // Arrange
      var users = new List<AppUser> { new AppUser { Id = 1, UserName = "TEST", City = "Test City", Country = "Test Country", Gender = "Test Gender", IsPredator = false, KnownAs = "Test User" } };

      // Create test JSON file with uppercase username
      File.WriteAllText("Data/UserSeedData.json", JsonSerializer.Serialize(users, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }));

      // Act
      await Seed.SeedUsers(_context);

      // Assert
      Assert.AreEqual("test", _context.Users.First().UserName);
      Assert.IsTrue(_context.Users.First().PasswordHash.Length > 0);
      Assert.IsTrue(_context.Users.First().PasswordSalt.Length > 0);

      // Cleanup
      if (File.Exists("Data/UserSeedData.json"))
        File.Delete("Data/UserSeedData.json");
    }
  }
}
