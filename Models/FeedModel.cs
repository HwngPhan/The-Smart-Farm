using TSF_mustidisProj.Models;

using System.Text.Json.Serialization;

namespace TSF_mustidisProj.Models; // Ensure this matches your project namespace

public class FeedModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    [JsonPropertyName("last_value")]
    public string LastValue { get; set; }
    public DateTime LastValueAt { get; set; }
    public string Key { get; set; }
    public FeedGroup Group { get; set; }
}

public class FeedGroup
{
    public int Id { get; set; }
    public string Key { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
}
