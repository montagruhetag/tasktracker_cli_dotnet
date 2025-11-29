namespace Models;

using System.Text.Json.Serialization;

public class Task
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("description")]
    public string Description { get; set; }
    [JsonPropertyName("status")]
    public Status Status { get; set; }
    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }
    [JsonPropertyName("updatedAt")]
    public DateTime UpdatedAt { get; set; }
    public Task(int id, string description, Status status, DateTime createdAt, DateTime updatedAt)
    {
        Id = id;
        Description = description;
        Status = status;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

}

public enum Status
{
    Todo,
    InProgress,
    Done
}