using System.Net.Http.Headers;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Fadebook.DTOs;
using Fadebook.Repositories;
using System.Web;
using Microsoft.AspNetCore.WebUtilities;
using Fadebook.Services;

namespace Fadebook.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GoogleCalendarController(
    IHttpClientFactory _httpClientFactory,
    IAppointmentManagementService _appointmentManagementService,
    IConfiguration _configuration
    ) : ControllerBase
{
    [HttpGet("google-auth", Name = "GoogleAuth")]
    // /google-auth?apptId=1
    public IActionResult GoogleAuth([FromQuery] int apptId)
    {
        var clientId = _configuration["Google:ClientId"];
        var redirectUri = Url.Link("GoogleCallback", new {});
        var scope = HttpUtility.UrlEncode("https://www.googleapis.com/auth/calendar.events");
        var state = $"apptId={apptId}";

        var url = $"https://accounts.google.com/o/oauth2/v2/auth" +
                  $"?client_id={clientId}" +
                  $"&redirect_uri={redirectUri}" +
                  $"&response_type=code" +
                  $"&scope={scope}" +
                  $"&access_type=offline" +
                  $"&prompt=consent" +
                  $"&state={Uri.EscapeDataString(state)}";

        return Redirect(url);
    }

    [HttpGet("google-callback", Name = "GoogleCallback")]
    public async Task<IActionResult> Callback([FromQuery] string code, [FromQuery] string state)
    {
        var stateValues = QueryHelpers.ParseQuery(state);
        var apptIdStr = stateValues["apptId"].ToString();
        int apptId = int.Parse(apptIdStr);
        
        
        var client = _httpClientFactory.CreateClient();
        var clientId = _configuration["Google:ClientId"];
        var clientSecret = _configuration["Google:ClientSecret"];
        // var redirectUri = _config["Google:RedirectUri"];
        var redirectUri = Url.Link("GoogleCallback", new {});

        var body = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("code", code),
            new KeyValuePair<string, string>("client_id", clientId),
            new KeyValuePair<string, string>("client_secret", clientSecret),
            new KeyValuePair<string, string>("redirect_uri", redirectUri),
            new KeyValuePair<string, string>("grant_type", "authorization_code")
        });

        var response = await client.PostAsync("https://oauth2.googleapis.com/token", body);
        var json = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {

            // _logger.LogDebug("--------------------JSON TOKENS--------------------");
            // _logger.LogDebug(json.ToString());
            return BadRequest(json);
        }

        var tokenData = JsonSerializer.Deserialize<JsonElement>(json);
        var accessToken = tokenData.GetProperty("access_token").GetString();
        var refreshToken = tokenData.TryGetProperty("refresh_token", out var rt) ? rt.GetString() : null;

        // Store tokens as secure, HttpOnly cookies
        // Response.Cookies.Append("google_access_token", accessToken!, new CookieOptions
        // {
        //     HttpOnly = true,
        //     Secure = true,
        //     SameSite = SameSiteMode.Strict,
        //     Expires = DateTime.UtcNow.AddHours(1)
        // });

        // if (refreshToken != null)
        // {
        //     Response.Cookies.Append("google_refresh_token", refreshToken, new CookieOptions
        //     {
        //         HttpOnly = true,
        //         Secure = true,
        //         SameSite = SameSiteMode.Strict,
        //         Expires = DateTime.UtcNow.AddDays(30)
        //     });
        // }

        await AddEventAsync(apptId, accessToken);

        await RevokeTokenAsync(refreshToken);
        // await RevokeTokenAsync(accessToken);

        return Redirect($"localhost:3000/book/confirmation/{apptId}");
    }
 
    [HttpPost("add-event")]
    public async Task AddEventAsync(int apptId, string accessToken)
    {
        // retrieve appointment from the service
        var appointment = await _appointmentManagementService.GetAppointmentByIdAsync(apptId);
        // if (appointment == null)
        //     return NotFound(new { message = $"Appointment {apptId} not found." });
        

        // Build event payload: start uses AppointmentDate, end = start + 1 hour
        var eventData = new
        {
            summary = $"Fadebook Appointment number: {appointment.AppointmentId}",
            description = $"Appointment with Barber: {appointment.BarberId}",
            start = new {
                dateTime = appointment.AppointmentDate.ToString("o"),
                timeZone = "America/New_York"
            },
            end = new {
                // set end time to start + 1 hour
                dateTime = appointment.AppointmentDate.AddHours(1).ToString("o"),
                timeZone = "America/New_York"
            }
        };

        // Send to Google Calendar API
        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var json = JsonSerializer.Serialize(eventData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var resp = await client.PostAsync("https://www.googleapis.com/calendar/v3/calendars/primary/events", content);
        if (!resp.IsSuccessStatusCode)
        {
            var body = await resp.Content.ReadAsStringAsync();
            // return StatusCode((int)resp.StatusCode, new { message = "Failed to create Google Calendar event", details = body });
        }

        var responseBody = await resp.Content.ReadAsStringAsync();
        // return Ok(JsonSerializer.Deserialize<JsonElement>(responseBody));
    }

    [HttpPost("revoke-token")]
    private async Task RevokeTokenAsync(string token)
    {
        // revoke tokens: https://oauth2.googleapis.com/revoke?token={token}
        if (string.IsNullOrEmpty(token))
        return; // Nothing to revoke

        var client = _httpClientFactory.CreateClient();

        // Google revocation endpoint
        var revokeUrl = $"https://oauth2.googleapis.com/revoke?token={token}";

        // Make POST request (Google accepts GET or POST)
        var response = await client.PostAsync(revokeUrl, null);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Failed to revoke token: {error}");
            // You can throw or log the error depending on your needs
        }
        else
        {
            Console.WriteLine("Token revoked successfully.");
        }
    }
}