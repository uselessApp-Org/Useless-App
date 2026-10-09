using MongoDB.Driver;
using Microsoft.Extensions.Options;
using UselessApp.Services;


namespace UselessApp.Services;


public class MongodbUserService
{
    private readonly IMongoCollection<User> _user;

    public MongodbUserService(IMongoClient client, IOptions<MongoDbSettings> settings)
    {
        try
{
    var names = client.ListDatabaseNames().ToList();
    Console.WriteLine("MongoDB connection SUCCESS");
    Console.WriteLine("Databases: " + string.Join(", ", names));
}
catch (Exception ex)
{
    Console.WriteLine("MongoDB connection FAILED: " + ex.Message);
}

    
        var db = client.GetDatabase(settings.Value.DatabaseName);
        _user = db.GetCollection<User>("users");
    }

    public async Task AddUserAsync(User user)
    {
        Console.WriteLine("INSERT METHOD RAN");
        await _user.InsertOneAsync(user);
    }
    //Validating Login
    public async Task<User> ValidateLoginAsync(string username, string password)
    {
        var user = await _user
        .Find(u => u.Username == username)
        .FirstOrDefaultAsync();
        if (user== null)
        return null;
        if (user.Password != password)
            return null;

        return user;

    }
}
