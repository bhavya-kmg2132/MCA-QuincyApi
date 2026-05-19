using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MCAQuincyApi.API.Models;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MCAQuincyApi.API.Controllers;

[ApiController]
[Route("api/v2/policy")]
public class ChangePolicyController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ChangePolicyController> _logger;
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public ChangePolicyController(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<ChangePolicyController> logger)
    {
        _httpClient = httpClientFactory.CreateClient();
        _configuration = configuration;
        _logger = logger;
    }

    [HttpGet("ChangePolicyExclude131/{policyNumber}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ChangePolicyExclude131(string policyNumber)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            var baseUrl = _configuration["ExternalApi:BaseUrlPY"] ?? "http://10.1.29.18/";
            var url = $"{baseUrl}api/v2/policy/ChangePolicyExclude131/{Uri.EscapeDataString(policyNumber)}";

            _logger.LogInformation("Calling external API: GET {Url}", url);

            var request = new HttpRequestMessage(HttpMethod.Get, url);

            var response = await _httpClient.SendAsync(request);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return NotFound(ApiResponse<object>.ErrorResponse(
                    "Policy not found",
                    "POLICY_NOT_FOUND",
                    stopwatch.ElapsedMilliseconds));
            }

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                _logger.LogError(
                    "External API returned {StatusCode}. Response: {ErrorBody}",
                    (int)response.StatusCode, errorBody);
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<object>.ErrorResponse(
                        $"External API returned {(int)response.StatusCode}",
                        "EXTERNAL_API_ERROR",
                        stopwatch.ElapsedMilliseconds));
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            stopwatch.Stop();

            return Content(responseBody, "application/json");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling ChangePolicyExclude131 API for policy {PolicyNumber}", policyNumber);
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResponse(
                    $"Unable to retrieve policy exclude data. {ex.Message}",
                    "POLICY_EXCLUDE_RETRIEVAL_FAILED",
                    stopwatch.ElapsedMilliseconds));
        }
    }

    

[HttpGet("ChangePolicy/{policyNumber}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ChangePolicy(string policyNumber)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
           //var baseUrl = "http://127.0.0.1:8000/";
            var baseUrl = _configuration["ExternalApi:BaseUrlPY"] ?? "http://10.1.29.18/";
            var url = $"{baseUrl}api/v2/policy/ChangePolicy/{Uri.EscapeDataString(policyNumber)}";

            _logger.LogInformation("Calling external API: GET {Url}", url);

            var request = new HttpRequestMessage(HttpMethod.Get, url);

            var response = await _httpClient.SendAsync(request);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return NotFound(ApiResponse<object>.ErrorResponse(
                    "Policy not found",
                    "POLICY_NOT_FOUND",
                    stopwatch.ElapsedMilliseconds));
            }

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                _logger.LogError(
                    "External API returned {StatusCode}. Response: {ErrorBody}",
                    (int)response.StatusCode, errorBody);
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<object>.ErrorResponse(
                        $"External API returned {(int)response.StatusCode}",
                        "EXTERNAL_API_ERROR",
                        stopwatch.ElapsedMilliseconds));
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            stopwatch.Stop();

            // Return the external API response directly without wrapping
            return Content(responseBody, "application/json");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling ChangeChangePolicy131 API for policy {PolicyNumber}", policyNumber);
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResponse(
                    $"Unable to retrieve policy exclude data. {ex.Message}",
                    "POLICY_EXCLUDE_RETRIEVAL_FAILED",
                    stopwatch.ElapsedMilliseconds));
        }
    }

    [HttpPost("RateMCAData")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RateMCAData([FromBody] List<RateMcaDataTable> requestData)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            if (requestData == null || requestData.Count == 0)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(
                    "Request data is required",
                    "INVALID_REQUEST",
                    stopwatch.ElapsedMilliseconds));
            }

            var baseUrl = "http://127.0.0.1:8000/";
            var url = $"{baseUrl}api/v2/policy/RateMCAData";

            _logger.LogInformation("Calling external API: POST {Url}", url);

            var json = JsonSerializer.Serialize(requestData, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, url) { Content = content };

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                _logger.LogError(
                    "External API returned {StatusCode}. Response: {ErrorBody}",
                    (int)response.StatusCode, errorBody);
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<object>.ErrorResponse(
                        $"External API returned {(int)response.StatusCode}",
                        "EXTERNAL_API_ERROR",
                        stopwatch.ElapsedMilliseconds));
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            stopwatch.Stop();

            // Return the external API response directly without wrapping
            return Content(responseBody, "application/json");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling RateMCAData API");
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResponse(
                    $"Unable to rate MCA data. {ex.Message}",
                    "RATE_MCA_FAILED",
                    stopwatch.ElapsedMilliseconds));
        }
    }
}

public class ChangeChangePolicyApiResponse
{
    [JsonPropertyName("result")]
    public ChangeChangePolicyResult? Result { get; set; }

    [JsonPropertyName("process time")]
    public string? ProcessTime { get; set; }
}

public class ChangeChangePolicyResult
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("errorCode")]
    public string? ErrorCode { get; set; }

    [JsonPropertyName("durationMs")]
    public long? DurationMs { get; set; }

    [JsonPropertyName("data")]
    public List<ChangeChangePolicyData?> Data { get; set; } = new();
}

public class ChangeChangePolicyResponse
{
    [JsonPropertyName("data")]
    public List<ChangeChangePolicyData?> Data { get; set; } = new();
}

[JsonDerivedType(typeof(TotalPremiumData), "TOTALPREMIUM")]
[JsonDerivedType(typeof(TableData), "tableName")]
public class ChangeChangePolicyData
{
    [JsonPropertyName("TOTALPREMIUM")]
    public object? TOTALPREMIUM { get; set; }

    [JsonPropertyName("tableName")]
    public string? TableName { get; set; }

    [JsonPropertyName("tableValue")]
    public List<Dictionary<string, object>>? TableValue { get; set; }
}

public class TotalPremiumData : ChangeChangePolicyData
{
    [JsonPropertyName("TOTALPREMIUM")]
    public new object? TOTALPREMIUM { get; set; }
}

public class TableData : ChangeChangePolicyData
{
    [JsonPropertyName("tableName")]
    public new string? TableName { get; set; }

    [JsonPropertyName("tableValue")]
    public new List<Dictionary<string, object>>? TableValue { get; set; }
}

public class RateMcaDataTable
{
    [JsonPropertyName("tableName")]
    public string? TableName { get; set; }

    [JsonPropertyName("tableValue")]
    public List<Dictionary<string, object>>? TableValue { get; set; }
}

public class RateMcaDataResponse
{
    [JsonPropertyName("data")]
    public List<ChangeChangePolicyData?> Data { get; set; } = new();
}

public class RateMcaDataApiResponse
{
    [JsonPropertyName("result")]
    public RateMcaDataResult? Result { get; set; }

    [JsonPropertyName("process time")]
    public string? ProcessTime { get; set; }
}

public class RateMcaDataResult
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("errorCode")]
    public string? ErrorCode { get; set; }

    [JsonPropertyName("durationMs")]
    public long? DurationMs { get; set; }

    [JsonPropertyName("data")]
    public List<RateMcaDataItem?> Data { get; set; } = new();
}

public class RateMcaDataApiResponseOriginal
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("errorCode")]
    public string? ErrorCode { get; set; }

    [JsonPropertyName("durationMs")]
    public long? DurationMs { get; set; }

    [JsonPropertyName("data")]
    public List<RateMcaDataItem?> Data { get; set; } = new();
}

public class RateMcaDataItem
{
    [JsonPropertyName("RatingData")]
    public RatingData? RatingData { get; set; }
}

public class RatingData
{
    [JsonPropertyName("TOTALPREMIUM")]
    public object? TOTALPREMIUM { get; set; }

    [JsonPropertyName("DMBPRATPY")]
    public Dictionary<string, object>? DMBPRATPY { get; set; }

    [JsonPropertyName("DWXP110PY")]
    public Dictionary<string, object>? DWXP110PY { get; set; }

    [JsonPropertyName("DMBPSTATPY")]
    public List<Dictionary<string, object>>? DMBPSTATPY { get; set; }

    [JsonPropertyName("DMBP130P")]
    public List<Dictionary<string, object>>? DMBP130P { get; set; }
}