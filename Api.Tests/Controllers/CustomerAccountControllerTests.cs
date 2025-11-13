using System.Threading.Tasks;
using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Fadebook.Controllers;
using Fadebook.Services;
using AutoMapper;

namespace Api.Tests.Controllers;

public class CustomerAccountControllerTests
{
    private readonly Mock<IUserAccountService> _mockUserAccountService;
    private readonly Mock<IMapper> _mockMapper;
    private readonly CustomerAccountController _controller;

    public CustomerAccountControllerTests()
    {
        _mockUserAccountService = new Mock<IUserAccountService>();
        _mockMapper = new Mock<IMapper>();
        _controller = new CustomerAccountController(
            _mockUserAccountService.Object,
            _mockMapper.Object
        );
    }

    [Fact]
    public async Task UsernameExists_ReturnsOk_WithTrue_WhenUsernameExists()
    {
        // Arrange
        _mockUserAccountService.Setup(s => s.CheckIfUsernameExistsAsync("existinguser")).ReturnsAsync(true);

        // Act
        var result = await _controller.UsernameExists("existinguser");

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var existsProp = okResult!.Value!.GetType().GetProperty("exists");
        ((bool)existsProp!.GetValue(okResult.Value)!).Should().BeTrue();
    }

    [Fact]
    public async Task UsernameExists_ReturnsOk_WithFalse_WhenUsernameDoesNotExist()
    {
        // Arrange
        _mockUserAccountService.Setup(s => s.CheckIfUsernameExistsAsync("newuser")).ReturnsAsync(false);

        // Act
        var result = await _controller.UsernameExists("newuser");

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var existsProp = okResult!.Value!.GetType().GetProperty("exists");
        ((bool)existsProp!.GetValue(okResult.Value)!).Should().BeFalse();
    }

    [Fact]
    public async Task UsernameExists_CallsServiceWithCorrectUsername()
    {
        // Arrange
        _mockUserAccountService.Setup(s => s.CheckIfUsernameExistsAsync("testuser")).ReturnsAsync(false);

        // Act
        await _controller.UsernameExists("testuser");

        // Assert
        _mockUserAccountService.Verify(s => s.CheckIfUsernameExistsAsync("testuser"), Times.Once);
    }
}
