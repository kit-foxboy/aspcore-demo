using System;
using System.Net;
using API.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace API.Tests.Controllers
{
  [TestClass()]
  public class UsersControllerTests
  {
    public class ExceptionsControllerTests
    {
      private readonly ExceptionsController _controller;

      public ExceptionsControllerTests()
      {
        _controller = new ExceptionsController();
      }

      // Test method to check if the server exception is thrown correctly
      [TestMethod()]
      public void TestServerException_ThrowsException()
      {
        // Act & Assert
        var exception = Assert.ThrowsException<Exception>(() => _controller.TestServerException());
        Assert.AreEqual("Server go explode", exception.Message);
      }
    }
  }
}