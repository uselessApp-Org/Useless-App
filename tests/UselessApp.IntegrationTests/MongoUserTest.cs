using Moq;
using MongoDB.Driver;
using UselessApp.Models;
using UselessApp.Services;
using Microsoft.Extensions.Options;
using Xunit;

public class MongoUserServiceTests
{
    [Fact]
    public async Task AddUser_ShouldInsertUser()
    {
        // Arrange
        var mockClient = new Mock<IMongoClient>();
        var mockDatabase = new Mock<IMongoDatabase>();
        var mockCollection = new Mock<IMongoCollection<User>>();

        mockClient.Setup(c => c.GetDatabase("Users", null))
                  .Returns(mockDatabase.Object);

        mockDatabase.Setup(d => d.GetCollection<User>("users", null))
                    .Returns(mockCollection.Object);

        var settings = Options.Create(new MongoDbSettings { DatabaseName = "Users" });
        var service = new MongodbUserService(mockClient.Object, settings);

        var user = new User { Username = "Timmy", Password = "Junior" };

        // Act
        await service.AddUserAsync(user);

        // Assert
        mockCollection.Verify(c => c.InsertOneAsync(
            It.Is<User>(u => u.Username == "Timmy"),
            null,
            default
        ), Times.Once);
    }
}
