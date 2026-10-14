using MongoDB.Driver;
using UselessApp.Models;
using UselessApp.Services;
using Microsoft.Extensions.Options;
using Xunit;

public class MongoUserTest
{
    private readonly IMongoClient client;
    private readonly MongodbUserService service;
    private readonly string testusername = "Timmy";
    private readonly string testpassword = "Junior";

    public MongoUserTest()
    {
        client = new MongoClient("mongodb+srv://server:dontusethisplz@uselessapp.zs41pju.mongodb.net/Users?appName=UselessApp");
        var settings = Options.Create(new MongoDbSettings
        {
            DatabaseName = "Users"
        });
        service = new MongodbUserService(client, settings);
    }

    [Fact]
   public async Task AddUser()
    {
        var user = new User()
        {
            Username = testusername,
            Password = testpassword
        };
//Creates the user
        await service.AddUserAsync(user);
//this is fetching the users
        var database = client.GetDatabase("Users");
        var users = database.GetCollection<User>("users");
//Checks to see if the user was added to the database
        var found = await users.Find(u => u.Username == testusername).FirstOrDefaultAsync();
        Assert.NotNull(found);
//This checks to see if the password got put in the database, it should not be equal to the original password
        Assert.NotEqual(testpassword, found.Password);
//Checks to see if the password is hashed correctly
        Assert.StartsWith("$2", found.Password); //The hash will always start wiht $2
//deletes the user from the database
        await users.DeleteOneAsync(u => u.Username == testusername);
        //Verifies that the user has been deleted
        var deleted = await users.Find(u => u.Username == testusername).FirstOrDefaultAsync();
        Assert.Null(deleted);

    }

}