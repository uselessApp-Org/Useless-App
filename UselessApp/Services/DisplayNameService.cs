using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using UselessApp.Models;

public class DisplayNameService
{
    private readonly ProtectedSessionStorage _session;

    public DisplayNameService(ProtectedSessionStorage session)
    {
        _session = session;
    }

    public string? Username { get; private set; }
    public string? DisplayName { get; private set; }
    public string? RandomDisplayName { get; private set; }

    public bool IsLoggedIn => Username != null;
public event Action? OnChange;

private void NotifyStateChanged() => OnChange?.Invoke();


    public async Task SetUserAsync(string username, string displayName, string randomDisplayName)
{
    Username = username;
    DisplayName = displayName;
    RandomDisplayName = randomDisplayName;

    await _session.SetAsync("username", username);
    await _session.SetAsync("displayName", displayName);
    await _session.SetAsync("randomDisplayName", randomDisplayName);

    NotifyStateChanged();
}

    public async Task LoadUserAsync()
{
    var usernameResult = await _session.GetAsync<string>("username");
    Console.WriteLine($"DEBUG SESSION username: success={usernameResult.Success}, value={usernameResult.Value}");

    var displayNameResult = await _session.GetAsync<string>("displayName");
    Console.WriteLine($"DEBUG SESSION displayName: success={displayNameResult.Success}, value={displayNameResult.Value}");

    var randomDisplayNameResult = await _session.GetAsync<string>("randomDisplayName");
    Console.WriteLine($"DEBUG SESSION randomDisplayName: success={randomDisplayNameResult.Success}, value={randomDisplayNameResult.Value}");

    Username = usernameResult.Value;
    DisplayName = displayNameResult.Value;
    RandomDisplayName = randomDisplayNameResult.Value;

    NotifyStateChanged();
}
    public async Task LogoutAsync()
    {
        Username = null;
        DisplayName = null;
        RandomDisplayName = null;

        await _session.DeleteAsync("username");
        await _session.DeleteAsync("displayName");
        await _session.DeleteAsync("randomDisplayName");

        NotifyStateChanged();
    }

   
}
