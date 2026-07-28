using DTO.Schedules;
using Microsoft.AspNetCore.Mvc;

namespace API.Schedules;

[ApiController]
[Route("api/v1/[controller]")]
public class SchedulesController(ScheduleService scheduleService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await scheduleService.GetAllAsync();

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateScheduleRequest request)
    {
        await scheduleService.CreateAsync(request);
        
        return Ok();
    }
}