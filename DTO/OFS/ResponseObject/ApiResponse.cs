using System.Net;

namespace DTO.OFS.ResponseObject;

public class ApiResponse
{
    public bool IsSuccess { get; set; }
    public HttpStatusCode StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
}