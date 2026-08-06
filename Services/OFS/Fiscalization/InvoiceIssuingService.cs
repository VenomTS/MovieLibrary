using DTO.OFS.Fiscalization.InvoiceIssue;
using DTO.OFS.ResponseObject;

namespace Services.OFS.Fiscalization;

public class InvoiceIssuingService(IHttpService httpService)
{
    public async Task<HttpResponseObject<InvoiceIssueResponse>> IssueInvoice(InvoiceIssueRequest request, string? requestId = null)
    {

        Dictionary<string, string>? headers = null;
        if (requestId != null)
        {
            headers = new Dictionary<string, string>
            {
                ["RequestId"] = requestId
            };
        }
        
        var response = await httpService.PostJsonAsync<InvoiceIssueRequest, InvoiceIssueResponse>
            ("invoices", request, headers);

        return response;
    }
}