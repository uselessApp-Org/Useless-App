using MongoDB.Driver;
using Microsoft.Extensions.Options;
using UselessApp.Models;




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
    if (string.IsNullOrEmpty(user.Password))
        throw new ArgumentException("Password cannot be null or empty");

    user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

    await _user.InsertOneAsync(user);

    return user;
}

public async Task UpdateUserAsync(User user)
{
    var filter = Builders<User>.Filter.Eq(u => u.Id, user.Id);
    await _user.ReplaceOneAsync(filter, user);
}

    /// <summary>
    /// This function updates user stats that will be displayed on the profile page
    /// add a new guard and then the updates statement helps ensure we dont overwrite with null/""/0
    /// </summary>
    /// <param name="username"></param>
    /// <param name="stats"></param>
    /// <returns></returns>
    public async Task UpdateUserStatsAsync(string? username, UserStatsService stats)
    {
        var target = string.IsNullOrEmpty(username) ? "Default" : username;

        var ub = Builders<User>.Update;
        var updates = new List<UpdateDefinition<User>>();

        if (stats.CalcEnters > 0)
            updates.Add(ub.Inc(u => u.Enters, stats.CalcEnters));
        if (!string.IsNullOrEmpty(stats.LastExpression))
            updates.Add(ub.Set(u => u.LastExpression, stats.LastExpression));
        if (!string.IsNullOrEmpty(stats.LastResult))
            updates.Add(ub.Set(u => u.LastResult, stats.LastResult));
        // Future stats: add one guarded updates.Add(...) line each.

        if (updates.Count == 0)
            return; // Nothing to save (Mongo rejects an empty update).

        var result = await _user.UpdateOneAsync(u => u.Username == target, ub.Combine(updates));

        if (result.MatchedCount == 0)
            Console.WriteLine($"WARNING: UpdateUserStatsAsync found no user named '{target}'.");
    }

    public async Task<User?> GetUserDataForProfilePage(string? username)
    {
        return await _user.Find(u => u.Username == username).FirstOrDefaultAsync<User>();
    }
}