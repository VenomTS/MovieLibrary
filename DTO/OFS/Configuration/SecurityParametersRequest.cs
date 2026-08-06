namespace DTO.OFS.Configuration;

public class SecurityParametersRequest
{
    public bool AuthorizeLocalClients { get; set; }
    public bool AuthorizeRemoteClients { get; set; }
    public string ApiKey { get; set; }
    public string WebserverAddress { get; set; }
}