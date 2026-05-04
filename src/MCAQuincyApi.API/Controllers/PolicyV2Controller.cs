using MCAQuincyApi.API.Models;
using MCAQuincyApi.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace MCAQuincyApi.API.Controllers;

[ApiController]
[Route("api/v2/policy")]
public class PolicyV2Controller : ControllerBase
{
    private readonly IPolicyService _policyService;

    public PolicyV2Controller(IPolicyService policyService)
    {
        _policyService = policyService;
    }

    [HttpPost("quotes")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetQuotes([FromBody] PolicyQuotesV2SearchRequest? request)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            var result = await _policyService.GetPolicyQuotesV2Async(
                request?.InsuredName,
                request?.AgentCode,
                request?.PolicyNumber,
                request?.Limit);

            var quotes = result.Select(PolicyQuoteResponse.FromPolicy).ToList();
            return Ok(ApiResponse<List<PolicyQuoteResponse>>.SuccessResponse(
                "Quotes retrieved successfully",
                stopwatch.ElapsedMilliseconds,
                quotes,
                quotes.Count));
        }
        catch (Exception ex)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ApiResponse<List<PolicyQuoteResponse>>.ErrorResponse(
                    $"Unable to retrieve quotes. {ex.Message}",
                    "QUOTES_RETRIEVAL_FAILED",
                    stopwatch.ElapsedMilliseconds,
                    []));
        }
    }

    [HttpGet("/api/v2/Policy/{policyNumber}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetPolicyDetails(string policyNumber)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            var result = await _policyService.GetPolicyByNumberV2Async(policyNumber);
            if (result == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse(
                    "Policy not found",
                    "POLICY_NOT_FOUND",
                    stopwatch.ElapsedMilliseconds));
            }

            return Ok(ApiResponse<object>.SuccessResponse(
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
    public async Task<IActionResult> SavePolicyInfo([FromBody] SavePolicyInfoV2Request request)
    {
        var stopwatch = Stopwatch.StartNew();
        if (string.IsNullOrWhiteSpace(request.PolicyNumber) || string.IsNullOrWhiteSpace(request.Telephone))
        {
            return BadRequest(ApiResponse<object>.ErrorResponse(
                "Policy number and telephone are required",
                "INVALID_SAVE_POLICY_INFO_REQUEST",
                stopwatch.ElapsedMilliseconds));
        }

        try
        {
            var result = await _policyService.UpdatePolicyPhoneV2Async(request.PolicyNumber, request.Telephone);
            return Ok(ApiResponse<object>.SuccessResponse(
                "Policy phone number updated successfully",
                stopwatch.ElapsedMilliseconds,
                result,
                result == null ? 0 : 1));
        }
        catch (Exception ex)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResponse(
                    $"Unable to update policy phone number. {ex.Message}",
                    "POLICY_PHONE_UPDATE_FAILED",
                    stopwatch.ElapsedMilliseconds));
        }
    }
}
