using Core;
using Services;

namespace OFSApp;

public partial class FiscalSettings : Form
{

    private readonly OFSService _ofsService;
    
    public FiscalSettings(OFSService ofsService)
    {
        InitializeComponent();
        
        _ofsService = ofsService;
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

        _ofsService.Initialize(ipAddress, port, key, pin);

        try
        {
            var availabilityResponse = await _ofsService.VerifyAvailabilityAsync();
            if (!availabilityResponse.IsSuccess)
            {
                MessageBox.Show($"Greška pri provjeri dostupnosti: {availabilityResponse.Message}");
                return;
            }
        
            // Check Status
            var statusResponse = await _ofsService.VerifyStatusAsync();
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