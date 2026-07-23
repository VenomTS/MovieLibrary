namespace App.APIResponses;

public class ErrorResponse
{
    public List<string> Errors { get; set; } = [];
    public string? Detail { get; set; }
}