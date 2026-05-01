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

<<<<<<< HEAD
    /// <summary>
    /// Search policy quotes.
    /// Proxies to: POST http://10.1.16.145:8020/api/v2/policy/quotes
    /// </summary>
    [HttpPost("quotes")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchPolicyQuotes([FromBody] PolicyQuotesSearchRequest? request)
    {
        var result = await _policyService.GetPolicyQuotesAsync(
            request?.insuredName,
            request?.agentCode,
            request?.quoteNumber
           // request?.searchkey
           );
        return Ok(result);
    }

    /// <summary>
    /// Get policy details by policy number.
    /// Proxies to: GET http://10.1.16.145:8020/api/v2/policy/{policyNumber}
    /// </summary>
    [HttpGet("{policyNumber}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPolicyDetails(string policyNumber)
    {
        var result = await _policyService.GetPolicyByNumberAsync(policyNumber);
        return result == null ? NotFound() : Ok(result);
    }

    /// <summary>
    /// Update phone number for a policy.
    /// Proxies to: POST http://10.1.16.145:8020/api/v2/policy/SavePolicyInfo
    /// </summary>
    [HttpPost("SavePolicyInfo")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> SavePolicyInfo([FromBody] SavePolicyInfoRequest request)
    {
        var result = await _policyService.UpdatePolicyPhoneAsync(request.PolicyNumber, request.Telephone);
        return Ok(result);
=======
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
>>>>>>> 43cc1a5f5aff4817593a6c753ccbc1c2b4edec73
    }
}
