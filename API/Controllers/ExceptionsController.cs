using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class ExceptionsController : BaseApiController
{ 
  [AllowAnonymous]
  [HttpGet("server-error")]
  public ActionResult TestServerException()
  {
    throw new Exception("Server go explode");
  }
}
