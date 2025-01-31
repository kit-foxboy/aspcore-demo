using System;

namespace API.Errors;

public class ApiException(int statusCode, string message, string? details = null)
{
  public int StatusCode { get; private set; } = statusCode;
  public string Message { get; private set; } = message;
  public string? Details { get; private set; } = details;
}
