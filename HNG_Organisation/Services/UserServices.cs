using HNG_Organisation.Data;
using HNG_Organisation.Entities;
using HNG_Organisation.Extensions;
using HNG_Organisation.Models;
using HNG_Organisation.Results;
using Microsoft.AspNetCore.Identity;

namespace HNG_Organisation.Services;

public partial class UserServices
{
    private readonly UserManager<User> _userManager;
    private readonly ApplicationDbContext _context;

    public UserServices(UserManager<User> userManager,
        ApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public async Task<RegisterResult?> RegisterUserAsync(RegisterModel model)
    {
        var registerResult = model.IsValidRegisterModel();

        if (registerResult.Errors.Count > 0)
        {
            return registerResult;
        }

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
            await _userManager.CreateAsync(user, model.Password);

            // Create an organisation for the user

            await transaction.CommitAsync();

            return null;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
