using System;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class RegisterDto
{
  [Required]
  [MaxLength(100)]
  public string Username { get; set; } = String.Empty;
  
  [Required]
  public string Password { get; set; } = String.Empty;
}
