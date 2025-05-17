namespace API.Extensions.Tests;

[TestClass]
public class DateTimeTests
{
  [TestMethod]
  public void CalculateBirthdayOccurredAgeTest()
  {
    // Arrange
    var today = DateOnly.FromDateTime(DateTime.Today);
    var dateOfBirth = new DateOnly(today.Year - 30, today.Month, 1);
    if (dateOfBirth > today)
    {
      dateOfBirth = dateOfBirth.AddYears(-1);
    }

    // Act
    var age = dateOfBirth.CalculateAge();

    // Assert
    Assert.AreEqual(30, age);
  }

  [TestMethod]
  public void CalculateBirthdayNotOccurredAgeTest()
  {
    // Arrange
    var today = DateOnly.FromDateTime(DateTime.Today);
    var dateOfBirth = new DateOnly(today.Year - 30, today.Month + 1, 1);
    if (dateOfBirth > today)
    {
      dateOfBirth = dateOfBirth.AddYears(-1);
    }

    // Act
    var age = dateOfBirth.CalculateAge();

    // Assert
    Assert.AreEqual(29, age);
  }

  [TestMethod]
  public void CalculateBirthdayIsTodayTest()
  {
    // Arrange
    var today = DateOnly.FromDateTime(DateTime.Today);
    var dateOfBirth = new DateOnly(today.Year - 25, today.Month, today.Day);

    // Act
    var age = dateOfBirth.CalculateAge();

    // Assert
    Assert.AreEqual(25, age);
  }

}
