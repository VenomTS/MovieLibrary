namespace DTO.OFS.Configuration;

public class ConfigurationResponse
{
    public List<int> AllowedPaymentTypes { get; set; }
    public string ApiKey { get; set; } = string.Empty;
    public string ApplicationLanguage { get; set; }
    public bool AuthorizeLocalClients { get; set; }
    public bool AuthorizeRemoteClients { get; set; }
    public List<string> AvailableDisplayDevices { get; set; }
    public List<string> AvailableEftPosDevices { get; set; }
    public List<string> AvailableEftPosProtocols { get; set; }
    public List<string> AvailablePrinters { get; set; }
    public List<string> AvailableScaleDevices { get; set; }
    public List<string> AvailableScaleProtocols { get; set; }
    public string? CustomTabName { get; set; }
    public string? CustomTabUrl { get; set; }
    public string? DisplayDeviceName { get; set; }
}