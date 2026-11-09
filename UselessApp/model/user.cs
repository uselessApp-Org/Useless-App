using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace UselessApp.Models;
public class User
{
    [BsonId]
    public ObjectId Id { get; set; }

    [BsonElement("Username")]
    public required string Username { get; set; } = "";

    [BsonElement("Password")]
    public required string Password { get; set; } = "";
    
    [BsonElement("DisplayName")]
    public string DisplayName { get; set; } = "";

    [BsonElement("Email")]
    public string Email { get; set; } = "";

    [BsonElement("GoogleId")]
    public string? GoogleId { get; set; }

   
}
