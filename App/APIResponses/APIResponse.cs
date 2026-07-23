using System.Net;

namespace App.APIResponses;

public class APIResponse<T>
{
    public HttpStatusCode Status { get; set; }
    public T? Content { get; set; }
    public ErrorResponse? ErrorResponse { get; set; }
}