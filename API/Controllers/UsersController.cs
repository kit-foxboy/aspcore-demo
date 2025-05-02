using System.Security.Claims;
using API.DTOs;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize]
public class UsersController(IUserRepository userRepository) : BaseApiController
{
  [HttpGet]
  public async Task<ActionResult<IEnumerable<MemberDto?>>> GetUsers()
  {
    var users = await userRepository.GetMembersAsync();

    return Ok(users);
  }

  [HttpGet("{username}")]
  public async Task<ActionResult<MemberDto>> GetUser(string username)
  {
    var user = await userRepository.GetMemberAsync(username);

    if (user == null) return NotFound();

    return user;
  }

  [HttpPut]
  public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto, IMapper mapper)
  {
    if (memberUpdateDto == null) return BadRequest("Member update data is null");

    // Get the username from the token
    var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    if (username == null) return BadRequest("Username not found in token");

    // Get the user from the repository using the username
    var user = await userRepository.GetUserByNameAsync(username);
    if (user == null) return BadRequest("User not found");

    // Map the updated properties to the user entity
    mapper.Map(memberUpdateDto, user);

    // save the changes to the database
    if (await userRepository.SaveAllAsync()) return NoContent();

    return BadRequest("Failed to update user");
  }
}
