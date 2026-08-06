namespace DTO.OFS.Fiscalization.Status;

public class StatusResponse
{
    public List<TaxRateResponse> AllTaxRates { get; set; }
    public TaxRateResponse CurrentTaxRates { get; set; }
    public string DeviceSerialNumber { get; set; }
    public List<string> Gsc { get; set; }
    public string HardwareVersion { get; set; }
    public string LastInvoiceNumber { get; set; }
    public string Make { get; set; }
    public string Model { get; set; }
    
    // Provjeriti
    public List<string> Mssc { get; set; }
    public string ProtocolVersion { get; set; }
    public DateTimeOffset sdcDateTime { get; set; }
    public string SoftwareVersion { get; set; }
    public List<string> SupportedLanguages { get; set; }
}