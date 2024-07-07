using HNG_Organisation.Data;
using HNG_Organisation.Entities;
using HNG_Organisation.Models;
using HNG_Organisation.Results;
using Microsoft.AspNetCore.Identity;

namespace HNG_Organisation.Services;

public partial class UserService
{
    private readonly UserManager<User> _userManager;
    private readonly ApplicationDbContext _context;

    public UserService(UserManager<User> userManager,
        ApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public async Task<SuccessResponse?> RegisterUserAsync(RegisterModel model)
    {
        using var transaction = await _context.Database
            .BeginTransactionAsync();

        try
        {
            var user = new User()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                UserName = model.Email,
                PhoneNumber = model.Phone
            };

            // Password is hashed by default
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Create an organisation for the user

                // Generate token
                string token = string.Empty;

                await transaction.CommitAsync();

                var successResponse = new SuccessResponse()
                {
                    Status = "success",
                    Message = "Registration successful",
                    Data = new SuccessData
                    {
                        AccessToken = token,
                        User = new User
                        {
                            Id = user.Id,
                            FirstName = model.FirstName,
                            LastName = model.LastName,
                            Email = model.Email,
                            PhoneNumber = model.Phone
                        }
                    }
                };

                return successResponse;
            }

            return null;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task LoginUserAsync(LoginModel model)
    {

    }
}
