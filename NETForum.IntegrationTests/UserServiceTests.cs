using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using NETForum.Data;
using NETForum.Errors;
using NETForum.Models.DTOs;
using NETForum.Models.Entities;
using NETForum.Services;

namespace NETForum.IntegrationTests;

public class UserServiceTests : ServiceTests
{
    private readonly IUserService _userService;
    private UserManager<User> _userManager;
    private readonly Mock<IFileStorageService> _mockFileStorageService;
    private readonly Mock<LinkGenerator> _mockLinkGenerator;
    private readonly Mock<IEmailSender> _mockEmailSender;
    private readonly Mock<ILogger<UserService>> _mockLogger;
    
    public UserServiceTests()
    {
        InitializeUserManager();
        _mockFileStorageService = new Mock<IFileStorageService>();
        _mockLinkGenerator = new Mock<LinkGenerator>();
        var fakeHttpContextAccessor = new FakeHttpContextAccessor();
        var fakeLinkGenerator = new FakeLinkGenerator();
        
        _mockEmailSender = new Mock<IEmailSender>();
        _mockLogger = new Mock<ILogger<UserService>>();
        
        _userService = new UserService(
            _userManager,
            _mockFileStorageService.Object,
            _db,
            fakeLinkGenerator,
            fakeHttpContextAccessor,
            _mockEmailSender.Object,
            _mockLogger.Object
        );
    }

    private void InitializeUserManager()
    {
        var userStore = new UserStore<User, Role, AppDbContext, int>(_db);
        var passwordHasher = new PasswordHasher<User>();
        var userValidators = new List<IUserValidator<User>> { new UserValidator<User>() };
        var passwordValidators = new List<IPasswordValidator<User>> { new PasswordValidator<User>() };
        var lookupNormalizer = new UpperInvariantLookupNormalizer();
        
        _userManager = new UserManager<User>(
            userStore,
            Options.Create(new IdentityOptions()),
            passwordHasher,
            userValidators,
            passwordValidators,
            lookupNormalizer,
            new IdentityErrorDescriber(),
            null,
            new NullLogger<UserManager<User>>()
        );
        
        _userManager.RegisterTokenProvider("Default", new FakeTokenProvider());
        
        SeedRoles();
        SeedUsers();
    }

    private void SeedUsers()
    {
        var existingUser = new User()
        {
            Id = 1,
            UserName = "existingUser",
            Email = "existingUser@email.com",
            NormalizedUserName = "EXISTINGUSER",
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            SecurityStamp = Guid.NewGuid().ToString(),
            ConcurrencyStamp = Guid.NewGuid().ToString()
        };

        var existingUserRoles = new List<IdentityUserRole<int>>()
        {
            new() { RoleId = 1, UserId = 1 },
        };
        
        _db.Users.AddRange(existingUser);
        _db.UserRoles.AddRange(existingUserRoles);
        
        _db.SaveChanges();
    }

    private void SeedRoles()
    {
        var memberRole = new Role()
        {
            Name = "Member",
            NormalizedName = "MEMBER",
            Description = "Member role"
        };

        var adminRole = new Role()
        {
            Name = "Admin",
            NormalizedName = "ADMIN",
            Description = "Admin role"
        };

        var moderatorRole = new Role()
        {
            Name = "Moderator",
            NormalizedName = "MODERATOR",
            Description = "Moderator role"
        };
        
        _db.AddRange(memberRole, moderatorRole, adminRole);
        _db.SaveChanges();
    }

    [Fact]
    public async Task RegisterUserAsync_WithValidDto_ReturnsSuccessResult()
    {
        var userRegistrationDto = new UserRegistrationDto()
        {
            Username = "new_user",
            Email = "new_user@email.com",
            Password = "@Password123",
            ConfirmPassword = "@Password123"
        };
        
        var result = await _userService.RegisterUserAsync(userRegistrationDto);
        
        result.IsSuccess.Should().BeTrue();
        
        var userLookupResult = await _userService.GetUserAsync("new_user");
        userLookupResult.IsSuccess.Should().BeTrue();
        userLookupResult.Value.UserName.Should().Be("new_user");
        userLookupResult.Value.Email.Should().Be("new_user@email.com");
        
        var userRoleLookupResult = _db.UserRoles.Where(ur => ur.UserId == userLookupResult.Value.Id).ToList();
        userRoleLookupResult.Count.Should().Be(1);
    }

    [Fact]
    public async Task RegisterUserAsync_WithTakenUsername_ReturnsFailureResult()
    {
        var userRegistrationDto = new UserRegistrationDto()
        {
            Username = "existingUser",
            Email = "new_user@email.com",
            Password = "@Password123",
            ConfirmPassword = "@Password123"
        };
        
        var result = await _userService.RegisterUserAsync(userRegistrationDto);
        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteUserAsync_WithValidUserId_DeletesUser()
    {
        var userId = 1;
        
        var result = await _userService.DeleteUserAsync(userId);
        result.IsSuccess.Should().BeTrue();
        
        _db.Users.Any(u => u.Id == userId).Should().BeFalse();
    }

    [Fact]
    public async Task DeleteUserAsync_WithInvalidUserId_ReturnsFailureResult()
    {
        var result = await _userService.DeleteUserAsync(-1);
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].Message.Should().Be(UserErrors.NotFound(-1).Message);
    }

    [Fact]
    public async Task CreateUserAsync_WithExistingUsername_ReturnsFailureResult()
    {
        var userCreateDto = new CreateUserDto()
        {
            Username = "existingUser",
            Email = "email@email.com"
        };
        
        var result = await _userService.CreateUserAsync(userCreateDto);
        
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].Message.Should().Be(UserErrors.NameTaken(userCreateDto.Username).Message);
    }

    [Fact]
    public async Task CreateUserAsync_WithValidDto_ReturnsSuccessResult()
    {
        var userCreateDto = new CreateUserDto()
        {
            Username = "nonExistingUser",
            Email = "nonExistingUser@email.com"
        };
        
        var result = await _userService.CreateUserAsync(userCreateDto);
        
        result.IsSuccess.Should().BeTrue();
        result.Value.UserName.Should().Be("nonExistingUser");
        result.Value.Email.Should().Be("nonExistingUser@email.com");
    }

    [Fact]
    public async Task GetUserForEdit_WithValidUserId_ReturnsDto()
    {
        var result = await _userService.GetUserForEditAsync(1);
        result.IsSuccess.Should().BeTrue();
        result.Value.Email.Should().Be("existingUser@email.com");
        result.Value.Username.Should().Be("existingUser");
    }
    
    [Fact]
    public async Task GetUserForEdit_WithInvalidUserId_ReturnsFailureResult()
    {
        var result = await _userService.GetUserForEditAsync(-1);
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].Message.Should().Be(UserErrors.NotFound(-1).Message);
    }

    [Fact]
    public async Task GetUserAsync_WithValidUsername_ReturnsUser()
    {
        var result = await _userService.GetUserAsync("existingUser");
        result.IsSuccess.Should().BeTrue();
        result.Value.Email.Should().Be("existingUser@email.com");
        result.Value.UserName.Should().Be("existingUser");
    }

    [Fact]
    public async Task GetUserAsync_WithInvalidUsername_ReturnsFailureResult()
    {
        var result = await _userService.GetUserAsync("userdoesnotexist");
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].Message.Should().Be(UserErrors.NotFound("userdoesnotexist").Message);
    }

    [Fact]
    public async Task GetUserAsync_WithValidId_ReturnsUser()
    {
        var result = await _userService.GetUserAsync(1);
        result.IsSuccess.Should().BeTrue();
        result.Value.Email.Should().Be("existingUser@email.com");
        result.Value.UserName.Should().Be("existingUser");
    }

    [Fact]
    public async Task GetUserAsync_WithInvalidId_ReturnsFailureResult()
    {
        var result = await _userService.GetUserAsync(-1);
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].Message.Should().Be(UserErrors.NotFound(-1).Message);
    }

    [Fact]
    public async Task UpdateUserRolesAsync_WithValidUserIdAndRoles_ShouldUpdateRoles()
    {
        var userId = 1;
        var roles = new List<string> { "Admin", "Moderator" };
        
        var result = await _userService.UpdateUserRolesAsync(userId, roles);
        
        result.IsSuccess.Should().BeTrue();

        var userRoles = await _db.UserRoles.Where(ur => ur.UserId == userId).ToListAsync();
        userRoles.Count.Should().Be(roles.Count);
    }
}