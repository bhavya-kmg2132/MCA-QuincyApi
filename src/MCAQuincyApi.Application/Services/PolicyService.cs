using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MCAQuincyApi.Application.Interfaces;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MCAQuincyApi.Application.Services;

public class PolicyService : IPolicyService
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;
    private readonly ILogger<PolicyService> _logger;
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public PolicyService(HttpClient httpClient, IConfiguration configuration, ILogger<PolicyService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _baseUrl = configuration["ExternalApi:BaseUrl"]
            ?? throw new InvalidOperationException("Missing ExternalApi:BaseUrl configuration.");
    }

    /// <inheritdoc />
    public async Task<object> GetPolicyQuotesAsync(string? policyNumber, string? insuredName, string? agentCode)
    {
        _logger.LogInformation(
            "Calling external API: POST {BaseUrl}/quotes. PolicyNumber={PolicyNumber}, InsuredName={InsuredName}, AgentCode={AgentCode}",
            _baseUrl, policyNumber, insuredName, agentCode);

        var requestBody = new
        {
            PolicyNumber = policyNumber ?? string.Empty,
            insuredName = insuredName ?? string.Empty,
            agentCode = agentCode ?? string.Empty
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync($"{_baseUrl}/quotes", content);
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<object>(responseBody, _jsonOptions)!;
    }

    /// <inheritdoc />
    public async Task<object?> GetPolicyByNumberAsync(string policyNumber)
    {
        _logger.LogInformation(
            "Calling external API: GET {BaseUrl}/{PolicyNumber}", _baseUrl, policyNumber);

        var response = await _httpClient.GetAsync($"{_baseUrl}/{policyNumber}");

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            _logger.LogWarning("Policy {PolicyNumber} not found in external API.", policyNumber);
            return null;
        }

        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<object>(responseBody, _jsonOptions);
    }

    /// <inheritdoc />
    public async Task<object> UpdatePolicyPhoneAsync(string policyNumber, string telephone)
    {
        _logger.LogInformation(
            "Calling external API: POST {BaseUrl}/SavePolicyInfo. PolicyNumber={PolicyNumber}",
            _baseUrl, policyNumber);

        var requestBody = new
        {
            telephone,
            policyNumber
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync($"{_baseUrl}/SavePolicyInfo", content);
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<object>(responseBody, _jsonOptions)!;
    }
}
