using DTO.OFS.Fiscalization.InvoiceIssue;

namespace Core;

public class InvoiceIssueBuilder
{
    private InvoiceIssueRequest _invoiceIssueRequest;

    public InvoiceIssueBuilder()
    {
        _invoiceIssueRequest = new InvoiceIssueRequest
        {
            InvoiceRequest = new InvoiceRequest
            {
                Payment = [],
                Items = []
            },
            ReceiptHeaderTextLines = [],
            ReceiptFooterTextLines = []
        };
    }

    public InvoiceIssueBuilder SendToMail(string mail)
    {
        _invoiceIssueRequest.Email = mail;
        _invoiceIssueRequest.Print = false;
        return this;
    }

    public InvoiceIssueBuilder RenderReceiptImage()
    {
        _invoiceIssueRequest.RenderReceiptImage = true;
        return this;
    }

    public InvoiceIssueBuilder SetLayoutAndImageFormat(InvoiceLayout layout, InvoiceImageFormat format)
    {
        _invoiceIssueRequest.ReceiptLayout = layout.ToString();
        if(layout == InvoiceLayout.Invoice && format == InvoiceImageFormat.Png)
            format = InvoiceImageFormat.Pdf;
        
        _invoiceIssueRequest.ReceiptImageFormat = format.ToString();
        return this;
    }

    public InvoiceIssueBuilder SetHeaderImage(string headerImageBase64)
    {
        _invoiceIssueRequest.ReceiptHeaderImage = headerImageBase64;
        return this;
    }

    public InvoiceIssueBuilder SetFooterImage(string footerImageBase64)
    {
        _invoiceIssueRequest.ReceiptFooterImage = footerImageBase64;
        return this;
    }

    public InvoiceIssueBuilder AddHeaderTextLine(string text)
    {
        _invoiceIssueRequest.ReceiptHeaderTextLines.Add(text);
        return this;
    }
    
    public InvoiceIssueBuilder AddFooterTextLine(string text)
    {
        _invoiceIssueRequest.ReceiptFooterTextLines.Add(text);
        return this;
    }

    public InvoiceIssueBuilder SetAdvancePaid(decimal advancePaid)
    {
        _invoiceIssueRequest.AdvancePaid = advancePaid;
        return this;
    }

    public InvoiceIssueBuilder SetAdvanceTax(decimal advanceTax)
    {
        _invoiceIssueRequest.AdvanceTax = advanceTax;
        return this;
    }

    public InvoiceIssueBuilder SetInvoiceType(InvoiceType type)
    {
        _invoiceIssueRequest.InvoiceRequest.InvoiceType = type.ToString();
        return this;
    }

    public InvoiceIssueBuilder SetInvoiceTransactionType(InvoiceTransactionType type)
    {
        _invoiceIssueRequest.InvoiceRequest.TransactionType = type.ToString();
        return this;
    }

    public InvoiceIssueBuilder AddPaymentMethod(InvoicePaymentType paymentType, decimal amount)
    {
        _invoiceIssueRequest.InvoiceRequest.Payment.Add(new PaymentRequest
        {
            PaymentType = paymentType.ToString(),
            Amount = amount
        });
        return this;
    }

    public InvoiceIssueBuilder SetDateAndTimeOfIssue(DateTimeOffset dateTimeOffset)
    {
        _invoiceIssueRequest.InvoiceRequest.DateAndTimeOfIssue = dateTimeOffset;
        return this;
    }

    public InvoiceIssueBuilder SetCashier(string cashier)
    {
        _invoiceIssueRequest.InvoiceRequest.Cashier = cashier;
        return this;
    }
    
    public InvoiceIssueBuilder SetBuyerId(string buyerId)
    {
        _invoiceIssueRequest.InvoiceRequest.BuyerId = buyerId;
        return this;
    }
    
    public InvoiceIssueBuilder SetBuyerCostCenterId(string buyerCostCenterId)
    {
        _invoiceIssueRequest.InvoiceRequest.BuyerCostCenterId = buyerCostCenterId;
        return this;
    }
    
    public InvoiceIssueBuilder SetReferentDocumentNumber(string referentDocumentNumber)
    {
        _invoiceIssueRequest.InvoiceRequest.ReferentDocumentNumber = referentDocumentNumber;
        return this;
    }
    
    public InvoiceIssueBuilder SetReferentDocumentDT(DateTimeOffset referentDocumentDT)
    {
        _invoiceIssueRequest.InvoiceRequest.ReferentDocumentDT = referentDocumentDT;
        return this;
    }

    public InvoiceIssueBuilder AddItem(string name, string gtin, List<string> labels, decimal unitPrice,
        decimal quantity, decimal totalAmount, decimal discount = 0, decimal discountAmount = 0)
    {
        _invoiceIssueRequest.InvoiceRequest.Items.Add(new ItemRequest
        {
            Name = name,
            Gtin = gtin,
            Labels = labels,
            UnitPrice = unitPrice,
            Quantity = quantity,
            TotalAmount = totalAmount,
            Discount = discount,
            DiscountAmount = discountAmount
        });
        return this;
    }

    public InvoiceIssueRequest Build()
    {
        return _invoiceIssueRequest;
    }
}

public enum InvoiceLayout
{
    Slip,
    Invoice
}

public enum InvoiceImageFormat
{
    Png,
    Pdf,
    Html
}

public enum InvoiceType
{
    Normal,
    Proforma,
    Copy,
    Training,
    Advance
}

public enum InvoiceTransactionType
{
    Sale,
    Refund
}

public enum InvoicePaymentType
{
    Cash,
    Card,
    Check,
    WireTransfer,
    Voucher,
    MobileMoney,
    Other
}