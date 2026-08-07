using Services;
using Services.OFS;
using Services.OFS.Fiscalization;

namespace OFSApp;

public partial class FiscalSettings : Form
{
    private readonly IHttpService _httpService;
    private readonly ConfigurationService _configService;
    private readonly InitializationService _initService;
    
    public FiscalSettings(
        IHttpService httpService,
        ConfigurationService configService,
        InitializationService initService)
    {
        InitializeComponent();
        
        _httpService = httpService;
        _configService = configService;
        _initService = initService;
    }

    private async void saveButton_Click(object sender, EventArgs e)
    {
        var ipAddress = ipInput.Text;
        var port = portInput.Text;
        var key = keyInput.Text;
        var pin = pinInput.Text;

        if (string.IsNullOrWhiteSpace(ipAddress) ||
            string.IsNullOrWhiteSpace(port) ||
            string.IsNullOrWhiteSpace(key) ||
            string.IsNullOrWhiteSpace(pin))
        {
            MessageBox.Show("Sva polja moraju biti ispunjena", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        if (pin.Length != 4)
        {
            MessageBox.Show("Dužina pin koda mora biti 4 znamenke", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        _httpService.SetBaseAddress(ipAddress, port);
        _httpService.SetBearerToken(key);

        try
        {
            // Check Availability
            var availabilityResponse = await _configService.CheckAvailabilityAsync();
            if (!availabilityResponse.IsSuccess)
            {
                MessageBox.Show($"Greška pri provjeri dostupnosti: {availabilityResponse.Message}");
                return;
            }
        
            // Check Status
            var statusResponse = await _initService.CheckStatusAsync();
            if (!statusResponse.IsSuccess)
            {
                MessageBox.Show($"Greška pri provjeri statusa: {statusResponse.Message}");
                return;
            }
        
            // Get Configuration
            // var configResponse = await _configService.GetConfigurationAsync();
            // if (!configResponse.IsSuccess)
            // {
            //     MessageBox.Show($"Greška pri očitavanju konfiguracije: {configResponse.Message}");
            //     return;
            // }
        
            MessageBox.Show("");
        }
        catch (Exception exception)
        {
            MessageBox.Show($"Neočekivana greška: {exception.Message}", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}