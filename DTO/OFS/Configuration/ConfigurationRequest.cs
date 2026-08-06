namespace DTO.OFS.Configuration;

public class ConfigurationRequest
{
    public string PrinterName { get; set; }
    public string PrinterType { get; set; }
    public string ReceiptLayout { get; set; }
    public int RecipeWidth { get; set; }
    public int ReceiptFontSize { get; set; }
    public int ReceiptFontSizeLarge { get; set; }
    public List<string> ReceiptHeaderTextLines { get; set; }
    public string ReceiptHeaderImage { get; set; }
    public List<string> ReceiptFooterTextLines { get; set; }
    public string ReceiptFooterImage { get; set; }
    public int QrCodeSize { get; set; }
    public int ReceiptFeedLinesEnd { get; set; }
    public string ReceiptCutPaper { get; set; }
    public string ReceiptOpenCashDrawer { get; set; }
    public bool receiptPrintGtin { get; set; }
    
}