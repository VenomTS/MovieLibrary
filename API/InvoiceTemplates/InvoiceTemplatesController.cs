using DTO.InvoiceTemplates;
using Microsoft.AspNetCore.Mvc;

namespace API.InvoiceTemplates;

[ApiController]
[Route("api/v1/[controller]")]
public class InvoiceTemplatesController(InvoiceTemplateService invoiceTemplateService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateInvoiceTemplateRequest request)
    {
        var result = await invoiceTemplateService.CreateAsync(request);

        return Ok(result);
    }

    [HttpGet("{userId:guid}")]
    public async Task<IActionResult> GetByUserId([FromRoute] Guid userId)
    {
        var result = await invoiceTemplateService.GetByUserIdAsync(userId);
        return result.Match<IActionResult>(
            Ok,
            _ => NotFound());
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await invoiceTemplateService.GetAllAsync();
        return Ok(result);
    }
}