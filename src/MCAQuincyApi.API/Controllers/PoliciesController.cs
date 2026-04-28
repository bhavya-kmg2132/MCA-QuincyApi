using Microsoft.AspNetCore.Mvc;
using MCAQuincyApi.Application.Interfaces;
using MCAQuincyApi.API.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MCAQuincyApi.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PoliciesController : ControllerBase
{
    private readonly IPolicyService _policyService;

    public PoliciesController(IPolicyService policyService)
    {
        _policyService = policyService;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchPolicies([FromBody] PolicySearchRequest? request)
    {
        var policies = await _policyService.GetPoliciesAsync(request?.Search);
        return Ok(policies);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPolicyById(string id)
    {
        var policy = await _policyService.GetPolicyByIdAsync(id);
        return policy == null ? NotFound() : Ok(policy);
    }

    [HttpPut("{id}/phone")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePolicyPhone(string id, [FromBody] UpdatePhoneRequest request)
    {
        var success = await _policyService.UpdatePolicyPhoneAsync(id, request.PhoneNumber);
        return !success ? NotFound() : NoContent();
    }
}
