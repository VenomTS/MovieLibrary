namespace DTO;

public class DefaultResponse
{
    public string? Detail { get; set; }
    public IEnumerable<string> Errors { get; set; } = [];
    public string? Instance { get; set; }
    public int? Status { get; set; }
    public string? Title { get; set; }
    public string? Type { get; set; }
}