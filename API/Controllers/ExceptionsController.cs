using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class ExceptionsController : BaseApiController
{
  [AllowAnonymous]
  [HttpGet]
  public ActionResult<string> TestException()
  {
    throw new Exception("Test exception");
  } 
}
