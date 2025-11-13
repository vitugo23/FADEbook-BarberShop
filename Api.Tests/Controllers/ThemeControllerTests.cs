using System.Linq;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Fadebook.Controllers;
using Fadebook.Exceptions;

namespace Api.Tests.Controllers;

public class ThemeControllerTests
{
    [Fact]
    public void GetTheme_ReturnsSystem_WhenCookieMissing()
    {
        // Arrange
        var controller = new ThemeController();
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };

        // Act
        var result = controller.GetTheme();

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var ok = result.Result as OkObjectResult;
        var prop = ok!.Value!.GetType().GetProperty("theme");
        ((string)prop!.GetValue(ok.Value)!).Should().Be("system");
    }

    [Fact]
    public void GetTheme_ReturnsCookieValue_WhenCookiePresent()
    {
        // Arrange
        var controller = new ThemeController();
        var http = new DefaultHttpContext();
        http.Request.Headers["Cookie"] = "fadebook_theme=dark";
        controller.ControllerContext = new ControllerContext { HttpContext = http };

        // Act
        var result = controller.GetTheme();

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var ok = result.Result as OkObjectResult;
        var prop = ok!.Value!.GetType().GetProperty("theme");
        ((string)prop!.GetValue(ok.Value)!).Should().Be("dark");
    }

    [Fact]
    public void SetTheme_SetsCookie_AndReturnsSelectedTheme()
    {
        // Arrange
        var controller = new ThemeController();
        var http = new DefaultHttpContext();
        controller.ControllerContext = new ControllerContext { HttpContext = http };

        // Act
        var result = controller.SetTheme(new ThemeController.ThemeRequest("dark"), null);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var ok = result.Result as OkObjectResult;
        var prop = ok!.Value!.GetType().GetProperty("theme");
        ((string)prop!.GetValue(ok.Value)!).Should().Be("dark");
        // Cookie header contains the set cookie
        http.Response.Headers["Set-Cookie"].ToString().Should().Contain("fadebook_theme=dark");
    }

    [Fact]
    public void SetTheme_InvalidValue_ThrowsBadRequest()
    {
        // Arrange
        var controller = new ThemeController();
        controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };

        // Act & Assert
        Assert.Throws<BadRequestException>(() => controller.SetTheme(new ThemeController.ThemeRequest("pink"), null));
    }
}
