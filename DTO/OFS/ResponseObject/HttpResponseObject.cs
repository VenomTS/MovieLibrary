using Services.ResponseObject;

namespace DTO.OFS.ResponseObject;

public class HttpResponseObject<TResponse> : HttpResponse
{
    public TResponse? Content { get; set; }
}