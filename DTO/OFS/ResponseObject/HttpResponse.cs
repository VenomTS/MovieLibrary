using System.Net;

namespace Services.ResponseObject;

public class HttpResponse
{
    public bool IsSuccess { get; set; }
    public HttpStatusCode StatusCode { get; set; }
    public string? Message { get; set; }
}