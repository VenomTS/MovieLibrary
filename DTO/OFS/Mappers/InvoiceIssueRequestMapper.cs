using DTO.OFS.Fiscalization.InvoiceIssue;

namespace DTO.OFS.Mappers;

public static class InvoiceIssueRequestMapper
{
    public static InvoiceIssueRequest SetMailTo(this InvoiceIssueRequest source, string mail)
    {
        source.Email = mail;
        source.Print = false;
        return source;
    }
}