using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace UselessApp.Models;
public class User
{
    [BsonId]
    public ObjectId Id { get; set; }

    [BsonElement("Username")]
    public required string Username { get; set; } = string.Empty;

    [BsonElement("Password")]
    public required string Password { get; set; } = string.Empty;
    
    [BsonElement("DisplayName")]
    public string DisplayName { get; set; } = string.Empty;

    [BsonElement("WeatherLocation")]
    public string Location { get; set; } = string.Empty;

    [BsonElement("Latitude")]
    public float Latitude { get; set; }

    [BsonElement("Longitude")]
    public float Longitude { get; set; }

    [BsonElement("Enters")]
    public int Enters { get; set; }

    [BsonElement("LastExpression")]
    public string LastExpression { get; set; } = string.Empty;

    [BsonElement("LastResult")]
    public string LastResult { get; set; } = string.Empty;

    [BsonElement("GoogleId")]
    public string GoogleId { get; set; } = string.Empty;
    
    [BsonElement("LastWordSearched")]
    public string LastWordSearched { get; set; } = string.Empty;

    [BsonElement("LastDefinition")]
    public string LastDefinition { get; set; } = string.Empty;
}
