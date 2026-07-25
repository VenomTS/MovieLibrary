namespace App.APIResponses;

public class ErrorResponse
{
    public object? ProblemDetails { get; set; }
    public string? Detail { get; set; }
}