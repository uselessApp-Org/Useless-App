using MongoDB.Driver;
using Microsoft.Extensions.Options;
using UselessApp.Models;
namespace UselessApp.Services;

public class MongodbUserService(IMongoClient client, IOptions<MongoDbSettings> settings)
{
    private IMongoCollection<User> Users => client.GetDatabase(settings.Value.DatabaseName).GetCollection<User>("users");
    public async Task AddUserAsync(User user)
    {
        if (string.IsNullOrWhiteSpace(user.Username) || user.Username.Length > 64) throw new ArgumentException("Username must contain 1–64 characters.");
        if (string.IsNullOrWhiteSpace(user.Password) || user.Password.Length < 8) throw new ArgumentException("Password must be at least 8 characters.");
        if (await Users.Find(u => u.Username == user.Username).AnyAsync()) throw new InvalidOperationException("That username is already taken.");
        var stored = new User { Username = user.Username.Trim(), Password = BCrypt.Net.BCrypt.HashPassword(user.Password) };
        await Users.InsertOneAsync(stored);
    }
    public async Task<User?> ValidateLoginAsync(string username, string password)
    {
        var user = await Users.Find(u => u.Username == username).FirstOrDefaultAsync();
        return user != null && BCrypt.Net.BCrypt.Verify(password, user.Password) ? user : null;
    }
}
