namespace DTO.OFS.ResponseObject;

public class ApiResponseObject<T> : ApiResponse
{
    public T? Content { get; set; }
}