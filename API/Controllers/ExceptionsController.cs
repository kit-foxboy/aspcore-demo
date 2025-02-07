using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
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

  [AllowAnonymous]
  [HttpGet("server-error")]
  public ActionResult TestServerException()
  {
    throw new Exception("Server go explode");
  }
}
