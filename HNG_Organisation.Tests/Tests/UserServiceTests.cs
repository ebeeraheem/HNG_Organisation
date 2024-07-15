using HNG_Organisation.Data;
using HNG_Organisation.Entities;
using HNG_Organisation.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory.Internal;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HNG_Organisation.Tests.Tests;
public class UserServiceTests
{
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly Mock<SignInManager<User>> _signInManagerMock;
    private readonly ApplicationDbContext _context;
    private readonly Mock<OrganisationService> _organisationServiceMock;   

    public UserServiceTests()
    {
        _userManagerMock = new Mock<UserManager<User>>(
            Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
        _signInManagerMock = new Mock<SignInManager<User>>(
            _userManagerMock.Object, Mock.Of<IHttpContextAccessor>(), Mock.Of<IUserClaimsPrincipalFactory<User>>(), null, null, null, null);

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "InMemoryDbForTesting")
            .Options;

        _context = new ApplicationDbContext(options);
        _organisationServiceMock = new Mock<OrganisationService>(_context);
    }

    [Fact]
    public void GenerateToken_ShouldExpireAfter30Minutes()
    {
        // Arrange
        var inMemorySettings = new Dictionary<string, string>()
        {
            {"Jwt:Key", "0961024c-e6a6-44d5-bd3a-30b12afbb5a4"},
            {"Jwt:Issuer", "https://localhost:7204"},
            {"Jwt:Audience", "HNG_Organisation"}
        };

        IConfiguration configuration = new ConfigurationBuilder()
        .AddInMemoryCollection(inMemorySettings)
        .Build();

        var user = new User()
        {
            Id = "sdfghjkl-dfghjk567-dfgh456",
            Email = "sample@email.com",
            FirstName = "Habu",
            LastName = "Garba",
            PhoneNumber = "08012121212"
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var userService = new UserService(
            _userManagerMock.Object,
            _signInManagerMock.Object,
            configuration,
            _context,
            _organisationServiceMock.Object);

        // Act
        var tokenString = userService.GenerateToken(user, configuration);
        var jwtToken = tokenHandler.ReadJwtToken(tokenString);

        // Assert
        var tokenExpiration = jwtToken.ValidTo;
        var expectedExpiration = DateTime.UtcNow.AddMinutes(30);
        Assert.True((tokenExpiration - expectedExpiration).TotalSeconds < 1);
    }

    [Fact]
    public void GenerateToken_ShouldContainCorrectUserDetails()
    {
        // Arrange
        var inMemorySettings = new Dictionary<string, string>()
        {
            {"Jwt:Key", "0961024c-e6a6-44d5-bd3a-30b12afbb5a4"},
            {"Jwt:Issuer", "https://localhost:7204"},
            {"Jwt:Audience", "HNG_Organisation"}
        };

        IConfiguration configuration = new ConfigurationBuilder()
        .AddInMemoryCollection(inMemorySettings)
        .Build();

        var user = new User()
        {
            Id = "sdfghjkl-dfghjk567-dfgh456",
            Email = "sample@email.com",
            FirstName = "Habu",
            LastName = "Garba",
            PhoneNumber = "08012121212"
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var userService = new UserService(
            _userManagerMock.Object,
            _signInManagerMock.Object,
            configuration,
            _context,
            _organisationServiceMock.Object);

        // Act
        var tokenString = userService.GenerateToken(user, configuration);
        var jwtToken = tokenHandler.ReadJwtToken(tokenString);

        // Assert
        Assert.Equal(user.Id, jwtToken.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
        Assert.Equal(user.Email, jwtToken.Claims.First(c => c.Type == ClaimTypes.Name).Value);
    }
}
