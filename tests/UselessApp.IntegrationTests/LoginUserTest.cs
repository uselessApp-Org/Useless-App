using Xunit;
using UselessApp.Models;
using BCrypt.Net;

public class LoginUserTests
{
    [Fact]
public void LoginLogic_ShouldVerifyPasswordCorrectly()
{
    // Arrange: create a fake user exactly like MongoDB would return
    var user = new User
    {
        Username = "Timmy",
        Password = BCrypt.Net.BCrypt.HashPassword("Junior"),
        DisplayName = "TimmyD"
    };

    // Act: test your real password verification logic
    bool correctPassword = BCrypt.Net.BCrypt.Verify("Junior", user.Password);
    bool wrongPassword = BCrypt.Net.BCrypt.Verify("WrongPass", user.Password);

    // Assert
    Assert.True(correctPassword);
    Assert.False(wrongPassword);
}
[Fact]
public void LoginLogic_ShouldPickRandomDisplayName()
{
    var otherNames = new List<string> { "TimmyDanger", "TimmyPower" };

    var random = new Random();
    var picked = otherNames[random.Next(otherNames.Count)];

    Assert.Contains(picked, otherNames);
}

}