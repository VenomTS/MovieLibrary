using DTO.OFS.Fiscalization.Status;
using DTO.OFS.ResponseObject;

namespace Services.OFS.Fiscalization;

public class InitializationService(IHttpService httpService)
{
    public async Task<HttpResponseObject<StatusResponse>> CheckStatusAsync()
    {
        var response = await httpService.GetAsync<StatusResponse>("status");

        return response;
    }
}