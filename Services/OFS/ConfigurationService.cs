using DTO.OFS.Configuration;
using DTO.OFS.ResponseObject;
using Services.ResponseObject;

namespace Services.OFS;

public class ConfigurationService(IHttpService httpService)
{
    public async Task<HttpResponse> CheckAvailabilityAsync()
    {
        var response = await httpService.GetAsync("attention");

        return response;
    }

    public async Task<HttpResponseObject<ConfigurationResponse>> GetConfigurationAsync()
    {
        var response = await httpService.GetAsync<ConfigurationResponse>("settings");
        
        return response;
    }

    public async Task<HttpResponse> SetSecurityParametersAsync(SecurityParametersRequest request)
    {
        var response = await httpService.PostJsonAsync("settings", request);

        return response;
    }

    public async Task<HttpResponse> SetConfigurationAsync(ConfigurationRequest request)
    {
        var response = await httpService.PostJsonAsync("settings", request);

        return response;
    }
}