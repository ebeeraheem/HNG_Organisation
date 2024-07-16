using HNG_Organisation.Data;
using HNG_Organisation.Entities;
using HNG_Organisation.Models;
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
    private readonly Mock<ApplicationDbContext> _contextMock;
    private readonly Mock<IOrganisationService> _organisationServiceMock;
    private readonly IConfiguration _configuration;

    public UserServiceTests()
    {
        _userManagerMock = new Mock<UserManager<User>>(
            Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
        _signInManagerMock = new Mock<SignInManager<User>>(
            _userManagerMock.Object, Mock.Of<IHttpContextAccessor>(), Mock.Of<IUserClaimsPrincipalFactory<User>>(), null, null, null, null);

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "InMemoryDbForTesting")
            .Options;

        _contextMock = new Mock<ApplicationDbContext>(options);
        _organisationServiceMock = new Mock<IOrganisationService>();

        var inMemorySettings = new Dictionary<string, string>()
        {
            {"Jwt:Key", "0961024c-e6a6-44d5-bd3a-30b12afbb5a4"},
            {"Jwt:Issuer", "https://localhost:7204"},
            {"Jwt:Audience", "HNG_Organisation"}
        };

        _configuration = new ConfigurationBuilder()
        .AddInMemoryCollection(inMemorySettings)
        .Build();
    }

    [Fact]
    public void GenerateToken_ShouldExpireAfter30Minutes()
    {
        // Arrange
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
            _configuration,
            _contextMock.Object,
            _organisationServiceMock.Object);

        // Act
        var tokenString = userService.GenerateToken(user, _configuration);
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
            _configuration,
            _contextMock.Object,
            _organisationServiceMock.Object);

        // Act
        var tokenString = userService.GenerateToken(user, _configuration);
        var jwtToken = tokenHandler.ReadJwtToken(tokenString);

        // Assert
        Assert.Equal(user.Id, jwtToken.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
        Assert.Equal(user.Email, jwtToken.Claims.First(c => c.Type == ClaimTypes.Name).Value);
    }

    [Fact]
    public async Task RegisterUser_ShouldRegisterUserAndCreateNewOrganisation()
    {
        // Arrange
        var registerModel = new RegisterModel
        {
            FirstName = "Test",
            LastName = "User",
            Email = "test@example.com",
            Password = "Password123!",
            Phone = "1234567890"
        };

        _userManagerMock.Setup(um => um.CreateAsync(
            It.IsAny<User>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        _organisationServiceMock.Setup(os => os.CreateOrganisationAsync(
            It.IsAny<OrganisationModel>()))
            .ReturnsAsync(new Organisation { Id = "org-id", Name = "Test's Organisation" });

        var userService = new UserService(
            _userManagerMock.Object,
            _signInManagerMock.Object,
            _configuration,
            _contextMock.Object,
            _organisationServiceMock.Object);

        // Act
        var result = await userService.RegisterUserAsync(registerModel);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("success", result.Status);
        Assert.Equal("Test", result.Data.User.FirstName);
        Assert.Equal("User", result.Data.User.LastName);
        Assert.Equal("test@example.com", result.Data.User.Email);
        Assert.Equal("1234567890", result.Data.User.Phone);
        Assert.NotNull(result.Data.AccessToken);

        // Verify the organisation was created correctly in the context
        var organisation = await _contextMock.Object.Organisations
            .FirstOrDefaultAsync();

        Assert.NotNull(organisation);
        Assert.Equal("Test's Organisation", organisation.Name);
    }

    [Fact]
    public async Task RegisterUserAsync_ShouldCreateOrganisationWithCorrectName()
    {
        // Arrange
        var registerModel = new RegisterModel
        {
            FirstName = "Chris",
            LastName = "User",
            Email = "chris@example.com",
            Password = "Password123!",
            Phone = "1234567890"
        };

        _userManagerMock.Setup(um => um.CreateAsync(
            It.IsAny<User>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        _organisationServiceMock.Setup(os => os.CreateOrganisationAsync(
            It.IsAny<OrganisationModel>()))
            .ReturnsAsync(new Organisation { Id = "org-id", Name = "Chris' Organisation" });

        var userService = new UserService(
            _userManagerMock.Object,
            _signInManagerMock.Object,
            _configuration,
            _contextMock.Object,
            _organisationServiceMock.Object);

        // Act
        var result = await userService.RegisterUserAsync(registerModel);

        // Assert
        Assert.NotNull(result);
        //Assert.Equal("Chris' Organisation", result.Data.User.FirstName + "' Organisation");

        // Verify the organisation was created correctly in the context
        var organisation = await _contextMock.Object.Organisations
            .FirstOrDefaultAsync();

        Assert.NotNull(organisation);
        Assert.Equal("Chris' Organisation", organisation.Name);
    }

    [Fact]
    public async Task RegisterUserAsync_ShouldFailIfFirstNameIsMissing()
    {
        // Arrange
        var registerModel = new RegisterModel
        {
            LastName = "User",
            Email = "test@example.com",
            Password = "Password123!",
            Phone = "1234567890"
        };

        var userService = new UserService(
            _userManagerMock.Object,
            _signInManagerMock.Object,
            _configuration,
            _contextMock.Object,
            _organisationServiceMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<NullReferenceException>(() => userService.RegisterUserAsync(registerModel));
    }

    [Fact]
    public async Task RegisterUserAsync_ShouldFailIfEmailIsDuplicate()
    {
        // Arrange
        var registerModel = new RegisterModel
        {
            FirstName = "Test",
            LastName = "User",
            Email = "duplicate@example.com",
            Password = "Password123!",
            Phone = "1234567890"
        };

        _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Code = "DuplicateEmail", Description = "Email is already taken." }));

        var userService = new UserService(
            _userManagerMock.Object,
            _signInManagerMock.Object,
            _configuration,
            _contextMock.Object,
            _organisationServiceMock.Object);

        // Act
        var result = await userService.RegisterUserAsync(registerModel);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task RegisterUserAsync_ShouldReturn422ForDuplicateEmailOrUserID()
    {
        // Arrange
        var registerModel = new RegisterModel
        {
            FirstName = "Test",
            LastName = "User",
            Email = "duplicate@example.com",
            Password = "Password123!",
            Phone = "1234567890"
        };

        _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Code = "DuplicateEmail", Description = "Email is already taken." }));

        var userService = new UserService(
            _userManagerMock.Object,
            _signInManagerMock.Object,
            _configuration,
            _contextMock.Object,
            _organisationServiceMock.Object);

        // Act
        var result = await userService.RegisterUserAsync(registerModel);

        // Assert
        Assert.Null(result);
    }
}
