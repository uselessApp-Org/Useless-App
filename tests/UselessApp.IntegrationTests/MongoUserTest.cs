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
        var mockIndexManager = new Mock<IMongoIndexManager<User>>();
        var mockCursor = new Mock<IAsyncCursor<User>>();

        mockClient.Setup(c => c.GetDatabase("Users", null))
            .Returns(mockDatabase.Object);

        mockDatabase.Setup(d => d.GetCollection<User>("users", null))
            .Returns(mockCollection.Object);

        // create index for user
        mockCollection.Setup(c => c.Indexes)
            .Returns(mockIndexManager.Object);

        mockCursor.Setup(c => c.Current)
            .Returns(new List<User>());
        mockCursor.Setup(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        mockCollection.Setup(c => c.FindAsync(
            It.IsAny<FilterDefinition<User>>(),
            It.IsAny<FindOptions<User, User>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockCursor.Object);

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
