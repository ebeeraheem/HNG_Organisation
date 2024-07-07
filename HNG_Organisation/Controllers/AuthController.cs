using HNG_Organisation.Entities;
using HNG_Organisation.Extensions;
using HNG_Organisation.Models;
using HNG_Organisation.Results;
using HNG_Organisation.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HNG_Organisation.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserService _userService;

    public AuthController(UserService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        var registerResult = model.IsValidRegisterModel();
        if (registerResult.Errors.Count > 0)
        {
            return UnprocessableEntity(registerResult);
        }

        var response = await _userService.RegisterUserAsync(model);
        if (response is null)
        {
            return BadRequest(new UnsuccessfulResponse()
            {
                Status = "Bad Request",
                Message = "Registration unsuccessful",
                StatusCode = 400
            });
        }

        return Created("", response);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        var successResponse = await _userService.LoginUserAsync(model);

        if (successResponse is null)
        {
            return Unauthorized(new UnsuccessfulResponse()
            {
                Status = "Bad Request",
                Message = "Authentication failed",
                StatusCode = 401
            });
        }

        return Ok(successResponse);
    }
}
