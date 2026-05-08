using MCAQuincyApi.API.Models;
using MCAQuincyApi.Application.Interfaces;
using MCAQuincyApi.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace MCAQuincyApi.API.Controllers;

[ApiController]
[Route("api/v3/policy")]
public class PolicyV3Controller : ControllerBase
{
    private readonly IPolicyService _policyService;

    public PolicyV3Controller(IPolicyService policyService)
    {
        _policyService = policyService;
    }

    [HttpGet("/api/v3/Policy/{policyNumber}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetPolicyDetails(string policyNumber)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            var result = await _policyService.GetPolicyByNumberV3Async(policyNumber);
            if (result == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse(
                    "Policy not found",
                    "POLICY_NOT_FOUND",
                    stopwatch.ElapsedMilliseconds));
            }

            return Ok(ApiResponse<PolicyV3Response>.SuccessResponse(
                "Policy details retrieved successfully",
                stopwatch.ElapsedMilliseconds,
                result,
                1));
        }
        catch (Exception ex)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResponse(
                    $"Unable to retrieve policy details. {ex.Message}",
                    "POLICY_DETAILS_RETRIEVAL_FAILED",
                    stopwatch.ElapsedMilliseconds));
        }
    }

    [HttpPost("SavePolicyInfo")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SavePolicyInfo([FromBody] PolicyV3Response request)
    {
        var stopwatch = Stopwatch.StartNew();
        if (string.IsNullOrWhiteSpace(request.PolicyNumber))
        {
            return BadRequest(ApiResponse<object>.ErrorResponse(
                "Policy number is required",
                "INVALID_SAVE_POLICY_INFO_REQUEST",
                stopwatch.ElapsedMilliseconds));
        }

        try
        {
            var updateRequest = PolicyV3UpdateRequest.FromPolicyV3Response(request);
            var result = await _policyService.UpdatePolicyInfoV3Async(updateRequest);
            var response = SavePolicyInfoV3Response.FromPolicyV3(result);
            return Ok(ApiResponse<SavePolicyInfoV3Response>.SuccessResponse(
                "Policy information updated successfully",
                stopwatch.ElapsedMilliseconds,
                response,
                response == null ? 0 : 1));
        }
        catch (Exception ex)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResponse(
                    $"Unable to update policy information. {ex.Message}",
                    "POLICY_INFO_UPDATE_FAILED",
                    stopwatch.ElapsedMilliseconds));
        }
    }
}
