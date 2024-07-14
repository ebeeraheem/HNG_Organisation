using HNG_Organisation.Data;
using HNG_Organisation.Entities;
using HNG_Organisation.Models;
using HNG_Organisation.Results;
using HNG_Organisation.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HNG_Organisation.Controllers;
[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public UsersController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUserInfo(string userId)
    {
        // Get the ID of the currently authenticated user
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        var currentUser = await _context.ApplicationUsers
            .Include(u => u.Organisations)
            .ThenInclude(o => o.Users)
            .FirstOrDefaultAsync(u => u.Id == currentUserId);
        if (currentUser is null)
        {
            return NotFound(new { Status = "error", Message = "User not found" });
        }

        // Check if the requested user ID is the same as the current user's ID
        if (userId == currentUserId)
        {
            return Ok(new GetUserResponse()
            {
                Status = "success",
                Message = "Request processed successfully",
                Data = new UserResponseData()
                {
                    UserId = currentUser.Id,
                    FirstName = currentUser.FirstName,
                    LastName = currentUser.LastName,
                    Email = currentUser.Email!,
                    Phone = currentUser.PhoneNumber!
                }
            });
        }

        // Check if the current user belongs to the same organization as the requested user
        var colleagues = currentUser.Organisations
            .SelectMany(o => o.Users)
            .DistinctBy(u => u.Id)
            .ToList();

        if (colleagues.Exists(c => c.Id == userId))
        {
            var requestedUser = await _context.ApplicationUsers.FindAsync(userId);
            if (requestedUser is null)
            {
                return NotFound(new { Status = "error", Message = "User not found" });
            }

            return Ok(new GetUserResponse()
            {
                Status = "success",
                Message = "Request processed successfully",
                Data = new UserResponseData()
                {
                    UserId = requestedUser.Id,
                    FirstName = requestedUser.FirstName,
                    LastName = requestedUser.LastName,
                    Email = requestedUser.Email!,
                    Phone = requestedUser.PhoneNumber!
                }
            });
        }

        // If the requested user is not the current user and not in the same organization, return forbidden
        return Forbid();
    }
}
