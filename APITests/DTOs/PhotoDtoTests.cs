using Microsoft.VisualStudio.TestTools.UnitTesting;
using API.DTOs;

namespace API.Tests.DTOs
{
  [TestClass]
  public class PhotoDtoTests
  {
    [TestMethod]
    public void PhotoDto_Properties_CanBeSetAndRetrieved()
    {
      // Arrange & Act
      var photoDto = new PhotoDto
      {
        Id = 1,
        Url = "https://example.com/photo.jpg",
        IsMain = true,
        IsNSFW = false
      };

      // Assert
      Assert.AreEqual(1, photoDto.Id);
      Assert.AreEqual("https://example.com/photo.jpg", photoDto.Url);
      Assert.IsTrue(photoDto.IsMain);
      Assert.IsFalse(photoDto.IsNSFW);
    }

    [TestMethod]
    public void PhotoDto_DefaultValues_AreCorrect()
    {
      // Arrange & Act
      var photoDto = new PhotoDto();

      // Assert
      Assert.AreEqual(0, photoDto.Id);
      Assert.IsNull(photoDto.Url);
      Assert.IsFalse(photoDto.IsMain);
      Assert.IsFalse(photoDto.IsNSFW);
    }
    
    [TestMethod]
    public void PhotoDto_Url_CanBeEmpty()
    {
      // Arrange & Act
      var photoDto = new PhotoDto { Url = "" };

      // Assert
      Assert.AreEqual("", photoDto.Url);
    }

    [TestMethod]
    public void PhotoDto_Url_CanBeValidHttpsUrl()
    {
      // Arrange
      var url = "https://cdn.example.com/images/photo123.jpg";

      // Act
      var photoDto = new PhotoDto { Url = url };

      // Assert
      Assert.AreEqual(url, photoDto.Url);
    }

    [TestMethod]
    public void PhotoDto_Url_CanBeValidHttpUrl()
    {
      // Arrange
      var url = "http://example.com/photo.png";

      // Act
      var photoDto = new PhotoDto { Url = url };

      // Assert
      Assert.AreEqual(url, photoDto.Url);
    }

    [TestMethod]
    public void PhotoDto_Url_CanBeRelativePath()
    {
      // Arrange
      var url = "/images/photo.jpg";

      // Act
      var photoDto = new PhotoDto { Url = url };

      // Assert
      Assert.AreEqual(url, photoDto.Url);
    }

    [TestMethod]
    public void PhotoDto_IsMain_True_IsSetCorrectly()
    {
      // Arrange & Act
      var photos = new List<PhotoDto>
      {
        new PhotoDto { Id = 1, Url = "https://example.com/photo1.jpg", IsMain = true },
        new PhotoDto { Id = 2, Url = "https://example.com/photo2.jpg", IsMain = false }
      };

      // Assert
      Assert.IsTrue(photos[0].IsMain);
    }

    [TestMethod]
    public void PhotoDto_IsMain_False_IsSetCorrectly()
    {
      // Arrange & Act
      var photos = new List<PhotoDto>
      {
        new PhotoDto { Id = 1, Url = "https://example.com/photo1.jpg", IsMain = false },
        new PhotoDto { Id = 2, Url = "https://example.com/photo2.jpg", IsMain = false }
      };

      // Assert
      Assert.IsFalse(photos[0].IsMain);
    }
  }
}
