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
       


        var db = client.GetDatabase(settings.Value.DatabaseName, null);
        _user = db.GetCollection<User>("users");


          

    }
    public void EnsureIndexes()
    {
        var indexKeysDefinition = Builders<User>.IndexKeys.Ascending(u => u.Username);
        var indexModel = new CreateIndexModel<User>(indexKeysDefinition, new CreateIndexOptions { Unique = true });
        _user.Indexes.CreateOne(indexModel);
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
public virtual async Task<bool> UserExistsAsync(string username)
{
    var existing = await _user.Find(u => u.Username == username).FirstOrDefaultAsync();
    return existing != null;
}

public async Task<User?> GetByUsernameAsync(string username)
{
    return await _user.Find(u => u.Username == username).FirstOrDefaultAsync();
}

public async Task<User> AddUserAsync(User user)
{
    if (string.IsNullOrEmpty(user.Username))
        throw new ArgumentException("Username cannot be null or empty");

    if (await UserExistsAsync(user.Username))
        throw new Exception("Username already exists");

    // Hash password if present and not empty
    if (!string.IsNullOrEmpty(user.Password))
    {
        user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
    }
    else
    {
        user.Password = string.Empty;
    }

    await _user.InsertOneAsync(user);

    return user;
}

public async Task UpdateUserAsync(User user)
{
    var filter = Builders<User>.Filter.Eq(u => u.Id, user.Id);
    await _user.ReplaceOneAsync(filter, user);
}




}
