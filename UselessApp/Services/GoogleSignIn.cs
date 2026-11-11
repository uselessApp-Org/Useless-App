using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using UselessApp.Models;

namespace UselessApp.Services;

public class GoogleSignInService
{
    private readonly MongodbUserService _userService;

    public GoogleSignInService(MongodbUserService userService)
    {
        _userService = userService;
    }

    public async Task<(User? user, bool needsOnboarding)> ProcessGoogleUserAsync(ClaimsPrincipal principal)
    {
        // Use the Google email or unique identifier as the lookup username key
        var identifier = principal.FindFirstValue(ClaimTypes.Email) ?? principal.FindFirstValue(ClaimTypes.NameIdentifier);
        var googleName = principal.FindFirstValue(ClaimTypes.Name);

        if (string.IsNullOrEmpty(identifier))
            return (null, false);

        var user = await _userService.GetByUsernameAsync(identifier);
        bool needsOnboarding = false;

        if (user == null)
        {
            // Create minimal user record
            user = new User
            {
                Username = identifier,
                Password = string.Empty,
                DisplayName = string.Empty // Blank signifies they need to set it
            };
            
            await _userService.AddUserAsync(user);
            needsOnboarding = true;
        }
        else if (string.IsNullOrEmpty(user.DisplayName))
        {
            needsOnboarding = true;
        }

        return (user, needsOnboarding);
    }
}