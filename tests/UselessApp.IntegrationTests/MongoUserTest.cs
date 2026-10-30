using Moq;
using MongoDB.Driver;
using UselessApp.Models;
using UselessApp.Services;
using Microsoft.Extensions.Options;
using System.Linq.Expressions;
using Xunit;

public class MongoUserServiceTests
{
    [Fact]
public async Task AddUserAsync_ShouldReturnCreatedUser()
{
    var mockClient = new Mock<IMongoClient>();
    var mockDatabase = new Mock<IMongoDatabase>();
    var mockCollection = new Mock<IMongoCollection<User>>();

    mockClient.Setup(c => c.GetDatabase("Users", null))
              .Returns(mockDatabase.Object);

    mockDatabase.Setup(d => d.GetCollection<User>("users", null))
                .Returns(mockCollection.Object);

    var settings = Options.Create(new MongoDbSettings { DatabaseName = "Users" });
    var service = new Mock<MongodbUserService>(mockClient.Object, settings) { CallBase = true };

    // ⭐ Mock the existence check instead of mocking MongoDB
    service.Setup(s => s.UserExistsAsync("Timmy")).ReturnsAsync(false);

    var newUser = new User
    {
        Username = "Timmy",
        Password = "Junior",
        DisplayName = "TimmyD"
    };

    var createdUser = await service.Object.AddUserAsync(newUser);

    mockCollection.Verify(c => c.InsertOneAsync(
        It.Is<User>(u => u.Username == "Timmy"),
        null,
        default
    ), Times.Once);

    Assert.Equal("Timmy", createdUser.Username);
    Assert.NotEqual("Junior", createdUser.Password); // hashed
    Assert.Equal("TimmyD", createdUser.DisplayName);
}

}
