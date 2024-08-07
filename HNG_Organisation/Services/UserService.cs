﻿using HNG_Organisation.Data;
using HNG_Organisation.Entities;
using HNG_Organisation.Models;
using HNG_Organisation.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HNG_Organisation.Services;

public partial class UserService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IConfiguration _config;
    private readonly ApplicationDbContext _context;
    private readonly IOrganisationService _organisationService;

    public UserService(UserManager<User> userManager,
        SignInManager<User> signInManager,
        IConfiguration config,
        ApplicationDbContext context,
        IOrganisationService organisationService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _config = config;
        _context = context;
        _organisationService = organisationService;
    }
        
    public async Task<SuccessResponse?> RegisterUserAsync(RegisterModel model)
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
            // Determine the user's name
            var name = user.FirstName.EndsWith('s') ?
                $"{user.FirstName}' Organisation" :
                $"{user.FirstName}'s Organisation";

            // Create an organisation model for the user
            var organisationModel = new OrganisationModel()
            {
                Name = name,
                Description = $"Created on {DateTime.Now:dd MMMM, yyyy}"
            };

            // Create an organisation for the user
            var organisation = await _organisationService
                .CreateOrganisationAsync(organisationModel);

            organisation.Users.Add(user);
            user.Organisations.Add(organisation);

            // Save changes
            _context.ApplicationUsers.Update(user);
            _context.Organisations.Update(organisation);
            await _context.SaveChangesAsync();

            // Generate token
            string token = GenerateToken(user, _config);

            var successResponse = new SuccessResponse()
            {
                Status = "success",
                Message = "Registration successful",
                Data = new SuccessData
                {
                    AccessToken = token,
                    User = new UserDto
                    {
                        UserId = user.Id,
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Email,
                        Phone = model.Phone
                    }
                }
            };

            return successResponse;
        }

        return null;
    }

    public async Task<SuccessResponse?> LoginUserAsync(LoginModel model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user is null) return null;

        var result = await _signInManager.PasswordSignInAsync(
            model.Email, model.Password, false, false);
        if (result is null) return null;

        var token = GenerateToken(user, _config);

        var successResponse = new SuccessResponse()
        {
            Status = "success",
            Message = "Login successful",
            Data = new SuccessData
            {
                AccessToken = token,
                User = new UserDto
                {
                    UserId = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email!, // Email is not null here
                    Phone = user.PhoneNumber! // PhoneNumber is not null here
                }
            }
        };

        return successResponse;
    }

    public string GenerateToken(User user, IConfiguration config)
    {
        var authClaims = new List<Claim>
        {
            new (ClaimTypes.NameIdentifier, user.Id),
            new (ClaimTypes.Name, user.Email!), // Email is not null here
            new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var authSigningKey = new SymmetricSecurityKey(
            // Key is not null
            Encoding.UTF8.GetBytes(config.GetValue<string>("Jwt:Key")));

        var signingCredentials = new SigningCredentials(
            authSigningKey, SecurityAlgorithms.HmacSha256);

        // Generate token
        var token = new JwtSecurityToken(
        issuer: config.GetValue<string>("Jwt:Issuer"),
        audience: config.GetValue<string>("Jwt:Audience"),
        claims: authClaims,
        notBefore: DateTime.UtcNow,
        expires: DateTime.UtcNow.AddMinutes(3),
        signingCredentials: signingCredentials);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return tokenString;
    }
}
