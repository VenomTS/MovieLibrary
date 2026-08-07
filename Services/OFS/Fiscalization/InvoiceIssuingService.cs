using DTO.OFS;
using DTO.OFS.Fiscalization.InvoiceIssue;
using DTO.OFS.ResponseObject;

namespace Services.OFS.Fiscalization;

public class InvoiceIssuingService(IHttpService httpService)
{
    public async Task<HttpResponseObject<InvoiceIssueResponse>> IssueInvoice(InvoiceIssueRequest request, params InvoiceHeader[] headers)
    {
        var headerDictionary = new Dictionary<string, string>();
        foreach(var header in headers)
            headerDictionary.Add(header.Name, header.Value);
        
        var response = await httpService.PostJsonAsync<InvoiceIssueRequest, InvoiceIssueResponse>
            ("invoices", request, headerDictionary);

        return response;
    }
}