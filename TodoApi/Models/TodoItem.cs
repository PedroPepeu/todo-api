using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TodoApi.Models;

public class TodoItem
{
    public int Id { get; set; }
    [Required]
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    // FK
    public int UserId { get; set; }
    [JsonIgnore]
    public User? User { get; set; }
}
