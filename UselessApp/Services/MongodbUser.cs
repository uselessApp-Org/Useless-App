using MongoDB.Driver;
using Microsoft.Extensions.Options;
using UselessApp.Models;
using UselessApp.Services;
using BCrypt.Net;


namespace UselessApp.Services;


public class MongodbUserService
{
    private readonly IMongoCollection<User> _user;

    public MongodbUserService(IMongoClient client, IOptions<MongoDbSettings> settings)
    {
        var db = client.GetDatabase(settings.Value.DatabaseName);
        _user = db.GetCollection<User>("users");

    }

    private async Task EnsureIndexesAsync()
    {
        var indexKeys = Builders<User>.IndexKeys.Ascending(u => u.Username);
        var indexOptions = new CreateIndexOptions { Unique = true };
        await _user.Indexes.CreateOneAsync(new CreateIndexModel<User>(indexKeys, indexOptions));
    }
    public async Task<string?> GetRandomDisplayNameAsync(string currentUsername)
    {
        var displayNames = await _user
            .Find(u => u.Username != currentUsername)
            .Project(u => u.DisplayName)
            .ToListAsync();

        if (displayNames.Count == 0)
            return null;

        var random = new Random();
        return displayNames[random.Next(displayNames.Count)];
    }

    public async Task<(User? user, string? randomDisplayName)> ValidateLoginAsync(string username, string password)
    {
        var user = await _user
            .Find(u => u.Username == username)
            .FirstOrDefaultAsync();

        if (user == null)
            return (null, null);

        bool isValid = BCrypt.Net.BCrypt.Verify(password, user.Password);
        if (!isValid)
            return (null, null);

        // Get random display name from other users
        var randomDisplayName = await GetRandomDisplayNameAsync(username);

        return (user, randomDisplayName);
    }

    public async Task<User> AddUserAsync(User user)
{
    await EnsureIndexesAsync();

    // Check if username already exists
    var existing = await _user.Find(u => u.Username == user.Username).FirstOrDefaultAsync();
    if (existing != null)
        throw new Exception("Username already exists");

    if (string.IsNullOrEmpty(user.Password))
        throw new ArgumentException("Password cannot be null or empty");

    user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

    try
    {
        await _user.InsertOneAsync(user);
    }
    catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
    {
        throw new Exception("Username already exists");
    }

    return user;
}




}
