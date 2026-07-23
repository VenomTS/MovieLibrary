using Microsoft.AspNetCore.Http;

namespace App.APIResponses;

public class ErrorResponse
{
    public HttpValidationProblemDetails? ProblemDetails { get; set; }
    public string? Detail { get; set; }
}