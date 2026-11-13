/*using UselessApp.Models;
using MongoDB.Driver;
using MongoDB.Bson;
using UselessApp.Services;

namespace Update;

public class UserUpdate
{
    
    
    public async Task UpdateUser(IMongoCollection<User> _user)

    {
         var db = client.GetDatabase(settings.Value.DatabaseName, null);
        _user = db.GetCollection<User>("users");
        var filter = Builders<User>.Filter.Or(
            Builders<User>.Filter.Exists(u => u.LastWordSearched, false),
            Builders<User>.Filter.Exists(u => u.LastDefinition, false));

        var update = Builders<User>.Update
            .Set(u => u.LastWordSearched, string.Empty)
            .Set(u => u.LastDefinition, string.Empty);

        // Assuming you have an IMongoCollection<User> instance named _user
        Console.WriteLine("Running Update...");
        var result = await collection.UpdateManyAsync(filter, update);
        Console.WriteLine($"Updated {result.ModifiedCount} users.");
        Console.WriteLine("Update complete.");
    }


}
*/
