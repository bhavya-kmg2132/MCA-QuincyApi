using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MCAQuincyApi.Application.Interfaces;
using MCAQuincyApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
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
    public async Task<IEnumerable<Policy>> GetPolicyQuotesAsync(string? policyNumber, string? insuredName, string? agentCode)
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
        return MapApiResponseToPolicies(responseBody);
    }

    /// <inheritdoc />
    public async Task<Policy?> GetPolicyByNumberAsync(string policyNumber)
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
        var policies = MapApiResponseToPolicies(responseBody);
        // Return first match since this is a single-policy lookup
        using var enumerator = policies.GetEnumerator();
        return enumerator.MoveNext() ? enumerator.Current : null;
    }

    /// <inheritdoc />
    public async Task<Policy?> UpdatePolicyPhoneAsync(string policyNumber, string telephone)
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
        var policies = MapApiResponseToPolicies(responseBody);
        using var enumerator = policies.GetEnumerator();
        return enumerator.MoveNext() ? enumerator.Current : null;
    }

    // ── Response mapping (same logic as Db2Repository.FetchPoliciesFromApiAsync) ──

    private List<Policy> MapApiResponseToPolicies(string responseJson)
    {
        var apiResponse = JsonSerializer.Deserialize<PolicyApiResponse>(responseJson, _jsonOptions);
        var policies = new List<Policy>();
        if (apiResponse?.Result == null) return policies;

        foreach (var item in apiResponse.Result)
        {
            policies.Add(new Policy
            {
                QuoteId = item.QUOTEID.ToString(),
                PolicyId = item.POLICYNUMBER?.Trim() ?? string.Empty,
                PolicyNo = item.POLICYNUMBER?.Trim(),
                InsuredName = item.INSUREDNAME?.Trim() ?? string.Empty,
                LineOfBusiness = item.LINEOFBUSINESS?.Trim(),
                EffectiveDate = ParseApiDate(item.EFFECTIVEDATE),
                ExpirationDate = ParseApiDate(item.EXPIRATIONDATE),
                Status = item.STATUS?.Trim(),
                TotalPremium = item.PREMIUM,
                AgentCode = item.AGENTCODE?.Trim(),
                TransactionDate = ParseApiDate(item.TRANSDATE),
                EndorseDate = ParseApiDate(item.ENDORSEDATE),
                TransactionType = item.TRANSACTIONTYPE?.Trim(),
                HeldBy = item.HELDBY?.Trim()
            });
        }
        return policies;
    }

    private static DateTime? ParseApiDate(int dateInt)
    {
        if (dateInt <= 0) return null;
        string dateStr = dateInt.ToString();
        if (dateStr.Length != 8) return null;
        if (DateTime.TryParseExact(dateStr, "yyyyMMdd", null, DateTimeStyles.None, out var dt))
        {
            return dt;
        }
        return null;
    }

    // ── DTOs matching the external API response shape ──

    private class PolicyApiResponse
    {
        [JsonPropertyName("result")]
        public List<ApiPolicyDto>? Result { get; set; }
    }

    private class ApiPolicyDto
    {
        public int QUOTEID { get; set; }
        public string? POLICYNUMBER { get; set; }
        public string? INSUREDNAME { get; set; }
        public string? LINEOFBUSINESS { get; set; }
        public int EFFECTIVEDATE { get; set; }
        public int EXPIRATIONDATE { get; set; }
        public string? STATUS { get; set; }
        public decimal PREMIUM { get; set; }
        public string? AGENTCODE { get; set; }
        public int TRANSDATE { get; set; }
        public int ENDORSEDATE { get; set; }
        public string? TRANSACTIONTYPE { get; set; }
        public string? HELDBY { get; set; }
    }
}
