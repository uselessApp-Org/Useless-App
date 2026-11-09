using UselessApp.Components;
using UselessApp.Models;
using Microsoft.AspNetCore.Authentication;
using MongoDB.Driver;
using Microsoft.AspNetCore.Authentication.Google;
using System.Security.Claims;
namespace UselessApp.Services;
// ... other usings

public async Task<UselessApp.Models.User?> ProcessGoogleUserAsync(ClaimsPrincipal principal)
{
    var email = principal.FindFirstValue(ClaimTypes.Email);
    var googleId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
    var name = principal.FindFirstValue(ClaimTypes.Name) ?? email;

    if (string.IsNullOrEmpty(email)) return null;

    // 1. Check if user already exists by email
    var user = await GetByEmailAsync(email); // Ensure you have a helper like this, or use your existing query

    if (user == null)
    {
        // --- SIGN UP LOGIC (New User) ---
        user = new User 
        {
            Username = email,
            Email = email,
            DisplayName = name,
            GoogleId = googleId,
            CreatedAt = DateTime.UtcNow
        };
        await CreateUserAsync(user);
    }
    else if (string.IsNullOrEmpty(user.GoogleId))
    {
        // --- ACCOUNT LINKING ---
        user.GoogleId = googleId;
        await UpdateUserAsync(user);
    }

    return user;
}
