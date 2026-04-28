using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MCAQuincyApi.Application.Interfaces;
using System;
using System.Threading.Tasks;
namespace MCAQuincyApi.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DataController : ControllerBase {
    private readonly IDataSyncService _dataSyncService;
    public DataController(IDataSyncService dataSyncService) { _dataSyncService = dataSyncService; }

    [HttpPost("sync")]
    public async Task<IActionResult> TriggerSync() {
        try {
            await _dataSyncService.SynchronizeDataAsync();
            return Accepted(new { message = "Data synchronization process completed successfully." });
        } catch (Exception ex) { return StatusCode(500, $"An error occurred: {ex.Message}"); }
    }

    [HttpGet]
    public async Task<IActionResult> Get() {
        var data = await _dataSyncService.GetSyncedDataAsync();
        return Ok(data);
    }
}