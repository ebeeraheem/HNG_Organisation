using HNG_Organisation.Data;
using HNG_Organisation.Entities;
using HNG_Organisation.Models;
using Microsoft.AspNetCore.Identity;

namespace HNG_Organisation.Services;

public class UserServices
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IConfiguration config;
    private readonly ApplicationDbContext _context;

    public UserServices(UserManager<User> userManager,
        SignInManager<User> signInManager,
        IConfiguration config,
        ApplicationDbContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        this.config = config;
        _context = context;
    }

    public async Task<IdentityResult> RegisterUserAsync(RegisterModel model)
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

            // Identity hashes passwords by default
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await transaction.CommitAsync();
            }

            return result;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
