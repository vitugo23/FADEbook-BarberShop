using Microsoft.AspNetCore.Mvc;
using Fadebook.Exceptions;
using Microsoft.AspNetCore.Http;

namespace Fadebook.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ThemeController : ControllerBase
{
    private const string ThemeCookieName = "fadebook_theme";
    private static readonly string[] AllowedThemes = ["light", "dark", "system"]; // C# 12 collection expression

    // GET: api/theme
    [HttpGet]
    public ActionResult<object> GetTheme()
    {
        var theme = Request.Cookies[ThemeCookieName];
        if (string.IsNullOrWhiteSpace(theme)) theme = "system";
        return Ok(new { theme });
    }

    // POST: api/theme
    // Accepts { theme: "light"|"dark"|"system" } or query ?theme=
    public record ThemeRequest(string? theme);

    [HttpPost]
    public ActionResult<object> SetTheme([FromBody] ThemeRequest? body, [FromQuery] string? theme)
    {
        var value = (body?.theme ?? theme ?? string.Empty).Trim().ToLowerInvariant();
        if (!AllowedThemes.Contains(value))
            throw new BadRequestException("Theme must be one of: light, dark, system.");

        var cookieOptions = new CookieOptions
        {
            // Persist for 1 year
            Expires = DateTimeOffset.UtcNow.AddYears(1),
            // Lax works for top-level navigations; avoids Secure requirement on http://localhost
            SameSite = SameSiteMode.Lax,
            HttpOnly = false, // allow client-side scripts to read if needed in the future
            Secure = Request.IsHttps // true in prod
        };
        Response.Cookies.Append(ThemeCookieName, value, cookieOptions);
        return Ok(new { theme = value });
    }
}
