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
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
            return BadRequest(new UnsuccessfulResponse());
        }

        return Created("", response);
    }
}
