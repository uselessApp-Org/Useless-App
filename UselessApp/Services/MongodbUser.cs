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
        try
        {
            var user = await _user
                .Find(u => u.Username == username)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                Console.WriteLine("User not found");
                return (null, null);
            }

            var rehash = BCrypt.Net.BCrypt.HashPassword(password);

            Console.WriteLine($"New Hash: {rehash}");

            bool isvalid = BCrypt.Net.BCrypt.Verify(password, user.Password);

            Console.WriteLine($"Valid: {isvalid}");
            Console.WriteLine($"Password valid: {isvalid}");

            if (!isvalid)
                return (null, null);

            var randomDisplayName = await GetRandomDisplayNameAsync(username);

            return (user, randomDisplayName);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            throw;
        }
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
        {
            user.Username = "Default";
            return user;
        }

        // Hash password if present and not empty
        if (!string.IsNullOrEmpty(user.Password))
        {

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
    public async Task UpdateUserStatsAsync(string? username, UserStatsService stats)
    {
        var target = string.IsNullOrEmpty(username) ? "Default" : username;

        var ub = Builders<User>.Update;
        var updates = new List<UpdateDefinition<User>>();

        if (stats.CalcEnters > 0)
            updates.Add(ub.Set(u => u.Enters, stats.CalcEnters));
        if (!string.IsNullOrEmpty(stats.LastExpression))
            updates.Add(ub.Set(u => u.LastExpression, stats.LastExpression));
        if (!string.IsNullOrEmpty(stats.LastResult))
            updates.Add(ub.Set(u => u.LastResult, stats.LastResult));
        if (!string.IsNullOrEmpty(stats.WeatherLocation))
            updates.Add(ub.Set(u => u.Location, stats.WeatherLocation));
        if (stats.Latitude != 0.0f)
            updates.Add(ub.Set(u => u.Latitude, stats.Latitude));
        if (stats.Longitude != 0.0f)
            updates.Add(ub.Set(u => u.Longitude, stats.Longitude));
        if (!string.IsNullOrEmpty(stats.LastWordSearched))
            updates.Add(ub.Set(u => u.LastWordSearched, stats.LastWordSearched));
        if (!string.IsNullOrEmpty(stats.LastDefinition))
            updates.Add(ub.Set(u => u.LastDefinition, stats.LastDefinition));
        // add more stats here

        if (updates.Count == 0)
            return; // Nothing to save (Mongo rejects an empty update).

        var result = await _user.UpdateOneAsync(u => u.Username == target, ub.Combine(updates));

        if (result.MatchedCount == 0)
            Console.WriteLine($"WARNING: UpdateUserStatsAsync found no user named '{target}'.");
    }
    public async Task ChangeDisplayNameAsync(string username, string newDisplayName)
    {
        var update = Builders<User>.Update.Set(u => u.DisplayName, newDisplayName);
        await _user.UpdateOneAsync(u => u.Username == username, update);

    }
    public async Task ChangePasswordAsync(string username, string newPassword)
    {
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);
        var update = Builders<User>.Update.Set(u => u.Password, hashedPassword);
        await _user.UpdateOneAsync(u => u.Username == username, update);
    }


    public async Task<User?> GetUserDataForProfilePage(string? username)
    {
        return await _user.Find(u => u.Username == username).FirstOrDefaultAsync<User>();
    }
    public async Task UpdateUsersAsync()
    {
        var filter = Builders<User>.Filter.Or(
            Builders<User>.Filter.Exists("LastWordSearched", false),
            Builders<User>.Filter.Exists("LastDefinition", false),
            Builders<User>.Filter.Exists("LastMovieRecommended", false),
            Builders<User>.Filter.Exists("LastMoviePoster", false)

        );

        var update = Builders<User>.Update
            .Set(u => u.LastWordSearched, "")
            .Set(u => u.LastDefinition, "")
            .Set(u => u.LastMovieRecommended, "")
            .Set(u => u.LastMoviePoster, "");

        var before = await _user
            .Find(u => u.Username == "test")
            .FirstOrDefaultAsync();

        var result = await _user.UpdateManyAsync(filter, update);

        var after = await _user
            .Find(u => u.Username == "test")
            .FirstOrDefaultAsync();

        Console.WriteLine($"Updated {result.ModifiedCount} users.");
    }

    /// <summary>
    /// This function will be used to get any data we want displayed on
    /// the home page
    /// </summary>
    /// <returns></returns>
    public async Task UpdateGlobalUser()
    {
        int totalNums = 0; // add the variable you want

        var filter = Builders<User>.Filter.Empty;
        var result = _user.Find(filter).ToList();

        foreach(User curUser in result)
        {
            if(curUser.Username != "Default")
                totalNums += curUser.Enters; // add something like this to update your data
        }

        UserStatsService updateDefaultUser = new UserStatsService();

        updateDefaultUser.CalcEnters = totalNums; // add new stats to this

        await UpdateUserStatsAsync("Default", updateDefaultUser);
    }
}