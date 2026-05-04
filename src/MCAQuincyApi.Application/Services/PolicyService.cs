using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MCAQuincyApi.Application.Interfaces;
using MCAQuincyApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
    private readonly string _apiKey;
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
        _apiKey = configuration["ExternalApi:ApiKey"]
            ?? throw new InvalidOperationException("Missing ExternalApi:ApiKey configuration.");
    }

    private void AddApiKeyHeader(HttpRequestMessage request)
    {
        request.Headers.Add("x-api-key", _apiKey);
    }

    // ──────────────────────────────────────────────────────────────
    //  Public API methods
    // ──────────────────────────────────────────────────────────────

    /// <inheritdoc />
    public async Task<IEnumerable<Policy>> GetPolicyQuotesAsync( string? insuredName, string? agentCode, string? policyNumber, int? limit = null)
    {
        _logger.LogInformation(
            "Calling external API: POST {BaseUrl}/api/v2/policy/quotes. InsuredName={InsuredName}, AgentCode={AgentCode}, QuoteNumber={QuoteNumber}, Limit={Limit}",
            _baseUrl, insuredName, agentCode, policyNumber, limit);

        var requestBody = new { policyNumber, insuredName, agentCode, limit };
        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}api/policy/GetQuotes") { Content = content };
        AddApiKeyHeader(request);

        var response = await _httpClient.SendAsync(request);

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            _logger.LogWarning(
                "External API returned 404 for POST /api/v2/policy/quotes. InsuredName={InsuredName}, AgentCode={AgentCode}, QuoteNumber={QuoteNumber}", insuredName, agentCode, policyNumber);
            return new List<Policy>();
        }

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            _logger.LogError(
                "External API returned {StatusCode} for POST /api/policy/GetQuotes. Response: {ErrorBody}",
                (int)response.StatusCode, errorBody);
            response.EnsureSuccessStatusCode();
        }

        var responseBody = await response.Content.ReadAsStringAsync();
        return MapApiResponseToPolicies(responseBody);
    }

    /// <inheritdoc />
    public async Task<Policy?> GetPolicyByNumberAsync(string policyNumber)
    {
        _logger.LogInformation(
            "Calling external API: GET {BaseUrl}/api/Policy/{PolicyNumber}",
            _baseUrl, policyNumber);

        var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUrl}api/Policy/{policyNumber}");
        AddApiKeyHeader(request);

        var response = await _httpClient.SendAsync(request);

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            _logger.LogWarning("Policy {PolicyNumber} not found in external API.", policyNumber);
            return null;
        }

        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();
        return MapPolicyInfoResponseToPolicy(responseBody);
    }

    /// <inheritdoc />
    public async Task<Policy?> UpdatePolicyPhoneAsync(string policyNumber, string telephone)
    {
        _logger.LogInformation(
            "Calling external API: POST {BaseUrl}/api/v2/policy/SavePolicyInfo. PolicyNumber={PolicyNumber}",
            _baseUrl, policyNumber);

        var requestBody = new { telephone, policyNumber };
        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}api/policy/SavePolicyInfo") { Content = content };
        AddApiKeyHeader(request);

        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            _logger.LogError(
                "External API returned {StatusCode} for POST /api/v2/policy/SavePolicyInfo. PolicyNumber={PolicyNumber}, Response: {ErrorBody}",
                (int)response.StatusCode, policyNumber, errorBody);
            response.EnsureSuccessStatusCode();
        }

        var responseBody = await response.Content.ReadAsStringAsync();
        return MapPolicyInfoResponseToPolicy(responseBody);
    }
     private class PolicyApiResponses
    {
        [JsonPropertyName("result")]
        public List<ApiPolicyDtos>? Result { get; set; }
    }
    private class ApiPolicyDtos
    {
        [JsonPropertyName("quoteid")]
        public object? QUOTEID { get; set; }
        [JsonPropertyName("policynumber")]
        public string? POLICYNUMBER { get; set; }
        [JsonPropertyName("insuredname")]
        public string? INSUREDNAME { get; set; }
        [JsonPropertyName("lineofbusiness")]
        public string? LINEOFBUSINESS { get; set; }
        [JsonPropertyName("effectivedate")]
        public object? EFFECTIVEDATE { get; set; }
        [JsonPropertyName("expirationdate")]
        public object? EXPIRATIONDATE { get; set; }
        [JsonPropertyName("status")]
        public string? STATUS { get; set; }
        [JsonPropertyName("premium")]
        public object? PREMIUM { get; set; }
        [JsonPropertyName("agentcode")]
        public string? AGENTCODE { get; set; }
        [JsonPropertyName("transdate")]
        public object? TRANSDATE { get; set; }
        [JsonPropertyName("endorsedate")]
        public object? ENDORSEDATE { get; set; }
        [JsonPropertyName("transactiontype")]
        public string? TRANSACTIONTYPE { get; set; }
        [JsonPropertyName("heldby")]
        public string? HELDBY { get; set; }
    }


    // ──────────────────────────────────────────────────────────────
    //  Mapping — flat list response (GetQuotes / UpdatePhone)
    // ──────────────────────────────────────────────────────────────

    private List<Policy> MapApiResponseToPolicies(string responseJson)
    {
        var policies = new List<Policy>();
        if (string.IsNullOrWhiteSpace(responseJson)) return policies;

        var wrapped = DeserializeOrDefault<PolicyApiResponses>(responseJson);
        if (wrapped?.Result != null)
        {
            return MapPoliciesFromList(wrapped.Result);
        }

        var directArray = DeserializeOrDefault<List<ApiPolicyDtos>>(responseJson);
        if (directArray != null)
        {
            return MapPoliciesFromList(directArray);
        }

        _logger.LogWarning("No policies found in response. Response: {Response}", responseJson.Substring(0, Math.Min(500, responseJson.Length)));
        return policies;
    }

    private List<Policy> MapPoliciesFromList(List<ApiPolicyDtos> items)
    {
        var policies = new List<Policy>();
        foreach (var item in items)
        {
            policies.Add(new Policy
            {
                QuoteId = ConvertToString(item.QUOTEID) ?? string.Empty,
                PolicyId = item.POLICYNUMBER?.Trim() ?? string.Empty,
                PolicyNo = item.POLICYNUMBER?.Trim(),
                InsuredName = item.INSUREDNAME?.Trim() ?? string.Empty,
                LineOfBusiness = item.LINEOFBUSINESS?.Trim(),
                EffectiveDate = ParsePolicyDate(item.EFFECTIVEDATE),
                ExpirationDate = ParsePolicyDate(item.EXPIRATIONDATE),
                Status = item.STATUS?.Trim(),
                TotalPremium = ParseDecimal(ConvertToString(item.PREMIUM)),
                AgentCode = item.AGENTCODE?.Trim(),
                TransactionDate = ParsePolicyDate(item.TRANSDATE),
                EndorseDate = ParsePolicyDate(item.ENDORSEDATE),
                TransactionType = item.TRANSACTIONTYPE?.Trim(),
                HeldBy = item.HELDBY?.Trim()
            });
        }
        return policies;
    }


    // ──────────────────────────────────────────────────────────────
    //  Mapping — nested detail response (GetPolicyByNumber)
    // ──────────────────────────────────────────────────────────────

    private Policy? MapPolicyInfoResponseToPolicy(string responseJson)
    {
        if (string.IsNullOrWhiteSpace(responseJson))
        {
            _logger.LogWarning("Empty response received for policy lookup");
            return null;
        }

        try
        {
            ApiPolicyDto? policyDto = null;

            // Try deserializing as direct object first
            var directObject = JsonSerializer.Deserialize<ApiPolicyDto>(responseJson, _jsonOptions);
            if (directObject != null && HasPolicyData(directObject))
            {
                policyDto = directObject;
            }
            else
            {
                // If that fails, try wrapped in "result" array
                var wrapped = JsonSerializer.Deserialize<PolicyInfoResponse>(responseJson, _jsonOptions);
                policyDto = wrapped?.Result?.FirstOrDefault();
            }

            if (policyDto == null)
            {
                _logger.LogWarning("No policy found in response");
                return null;
            }

            var primaryPhone   = policyDto.Contact?.Phones?.FirstOrDefault();
            var mailingAddress = policyDto.Contact?.MailingAddress;

            var policy = new Policy
            {
                // ── Identity ──
                PolicyId          = ConvertToString(policyDto.PolicyId) ?? string.Empty,
                PolicyIdOriginal  = ConvertToString(policyDto.PolicyId),
                PolicyNo          = policyDto.PolicyNumber?.Trim(),
                QuoteNumber       = policyDto.QuoteNumber?.Trim(),
                TransactionCode   = policyDto.TransactionCode?.Trim(),
                ProductCode       = policyDto.ProductCode?.Trim(),
                Status            = policyDto.Status?.Trim(),

                // ── Insured ──
                InsuredName        = policyDto.Insured?.NamedInsured?.Trim() ?? string.Empty,
                BusinessType       = policyDto.Insured?.BusinessType?.Trim(),
                LicenseNumber      = policyDto.Insured?.LicenseNumber?.Trim(),
                InsuredContactName = policyDto.Insured?.NamedInsured?.Trim(),
                ContactName        = policyDto.Insured?.NamedInsured?.Trim(),

                // ── Term ──
                EffectiveDate        = ParsePolicyDate(policyDto.Term?.EffectiveDate),
                ExpirationDate       = ParsePolicyDate(policyDto.Term?.ExpirationDate),
                PolicyEffectiveDate  = ParsePolicyDate(policyDto.Term?.EffectiveDate),
                PolicyExpirationDate = ParsePolicyDate(policyDto.Term?.ExpirationDate),

                // ── Contact ──
                PhoneNumber = primaryPhone?.PhoneNumber,
                Telephone   = primaryPhone?.PhoneNumber,
                PhoneType   = primaryPhone?.PhoneType,
                Email       = policyDto.Contact?.Email ?? string.Empty,

                // ── Address ──
                Address1 = mailingAddress?.Line1?.Trim(),
                Address2 = mailingAddress?.Line2?.Trim(),
                City     = mailingAddress?.City?.Trim(),
                State    = mailingAddress?.State?.Trim(),
                Zip      = mailingAddress?.PostalCode?.Trim(),

                // ── Policy Options ──
                HiredAuto     = FormatBool(policyDto.PolicyOptions?.HiredVehicle),
                NonOwned      = FormatBool(policyDto.PolicyOptions?.NonOwnedVehicle),
                DriveOtherCar = FormatBool(policyDto.PolicyOptions?.DriveOtherCar),

                // ── Vehicles ──
                VehicleCount = policyDto.Vehicles?.Count(v => v.Year.HasValue),

                // ── Rating ──
                TotalPremium   = policyDto.Rating?.TotalPremium,
                WrittenPremium = policyDto.Rating?.TotalPremium,

                // ── Line of Business ──
                LineOfBusiness = policyDto.ProductCode?.Trim(),
            };

            _logger.LogInformation("Successfully mapped policy {PolicyNumber}", policy.PolicyNo);
            return policy;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize policy response. Response: {Response}", responseJson.Substring(0, Math.Min(500, responseJson.Length)));
            return null;
        }
    }

    // ──────────────────────────────────────────────────────────────
    //  Helpers
    // ──────────────────────────────────────────────────────────────

    private static string? FormatBool(bool? value)
        => value.HasValue ? (value.Value ? "Y" : "N") : null;

    private static bool HasPolicyData(ApiPolicyDto policyDto)
        => policyDto.PolicyId != null
           || !string.IsNullOrWhiteSpace(policyDto.PolicyNumber)
           || !string.IsNullOrWhiteSpace(policyDto.QuoteNumber);

    private T? DeserializeOrDefault<T>(string responseJson)
    {
        try
        {
            return JsonSerializer.Deserialize<T>(responseJson, _jsonOptions);
        }
        catch (JsonException ex)
        {
            _logger.LogDebug(ex, "Response did not match {ResponseType}.", typeof(T).Name);
            return default;
        }
    }

    private static DateTime? ParsePolicyDate(object? value)
    {
        var dateStr = ConvertToString(value);
        if (string.IsNullOrWhiteSpace(dateStr)) return null;

        var trimmed = dateStr.Trim();
        string[] formats =
        [
            "yyyyMMdd",
            "yyyy-MM-dd",
            "yyyy-MM-ddTHH:mm:ss",
            "yyyy-MM-ddTHH:mm:ss.FFFFFFFK",
            "MM/dd/yyyy",
            "M/d/yyyy"
        ];

        return DateTime.TryParseExact(trimmed, formats, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var exactDate)
            ? exactDate
            : DateTime.TryParse(trimmed, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var parsedDate)
                ? parsedDate
                : null;
    }

    private static DateTime? ParseDateTime(string? dateStr)
    {
        if (string.IsNullOrWhiteSpace(dateStr)) return null;
        return DateTime.TryParseExact(
            dateStr, "yyyyMMdd", null, DateTimeStyles.None, out var dt) ? dt : null;
    }

    private static DateTime? ParseApiDate(int dateInt)
    {
        if (dateInt <= 0) return null;
        var dateStr = dateInt.ToString();
        return dateStr.Length == 8 &&
               DateTime.TryParseExact(dateStr, "yyyyMMdd", null, DateTimeStyles.None, out var dt)
            ? dt : null;
    }

    private static DateTime? ParseStringDate(string? dateStr)
    {
        if (string.IsNullOrWhiteSpace(dateStr)) return null;
        var trimmed = dateStr.Trim();
        if (trimmed.Length == 8 && DateTime.TryParseExact(trimmed, "yyyyMMdd", null, DateTimeStyles.None, out var dt))
            return dt;
        return null;
    }

    private static string? ConvertToString(object? value)
    {
        if (value == null) return null;
        if (value is JsonElement jsonElement)
        {
            return jsonElement.ValueKind switch
            {
                JsonValueKind.String => jsonElement.GetString(),
                JsonValueKind.Number => jsonElement.GetRawText(),
                JsonValueKind.True => bool.TrueString,
                JsonValueKind.False => bool.FalseString,
                JsonValueKind.Null => null,
                _ => jsonElement.ToString()
            };
        }
        if (value is string str) return str;
        if (value is int intVal) return intVal.ToString();
        if (value is long longVal) return longVal.ToString();
        if (value is decimal decVal) return decVal.ToString();
        if (value is double doubleVal) return doubleVal.ToString();
        return value.ToString();
    }

    private static decimal? ParseDecimal(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        return decimal.TryParse(value.Trim(), out var result) ? result : null;
    }

    // ──────────────────────────────────────────────────────────────
    //  DTOs — flat list response  (GetQuotes)
    // ──────────────────────────────────────────────────────────────

    private class PolicyApiResponse
    {
        [JsonPropertyName("result")]
        public List<ApiPolicyDto>? Result { get; set; }
    }

    // Reuse the same rich DTO for both endpoints since the API
    // returns the same envelope shape.
    private class ApiPolicyDto
    {
        [JsonPropertyName("POLICYID")]
        public object? PolicyId { get; set; }

        [JsonPropertyName("POLICYNUMBER")]
        public string? PolicyNumber { get; set; }

        [JsonPropertyName("QUOTENUMBER")]
        public string? QuoteNumber { get; set; }

        [JsonPropertyName("TRANSACTIONCODE")]
        public string? TransactionCode { get; set; }

        [JsonPropertyName("STATUS")]
        public string? Status { get; set; }

        [JsonPropertyName("PRODUCTCODE")]
        public string? ProductCode { get; set; }

        [JsonPropertyName("TERM")]
        public TermInfo? Term { get; set; }

        [JsonPropertyName("SOURCESYSTEM")]
        public string? SourceSystem { get; set; }

        [JsonPropertyName("RECORDVERSION")]
        public string? RecordVersion { get; set; }

        [JsonPropertyName("INSURED")]
        public InsuredInfo? Insured { get; set; }

        [JsonPropertyName("CONTACT")]
        public ContactInfo? Contact { get; set; }

        [JsonPropertyName("POLICYOPTIONS")]
        public PolicyOptions? PolicyOptions { get; set; }

        [JsonPropertyName("VEHICLES")]
        public List<VehicleInfo>? Vehicles { get; set; }

        [JsonPropertyName("DRIVERS")]
        public List<DriverInfo>? Drivers { get; set; }

        [JsonPropertyName("COVERAGES")]
        public List<object>? Coverages { get; set; }

        [JsonPropertyName("RATING")]
        public RatingInfo? Rating { get; set; }

        [JsonPropertyName("QUESTIONNAIRE")]
        public QuestionnaireInfo? Questionnaire { get; set; }

        [JsonPropertyName("MESSAGES")]
        public object? Messages { get; set; }
    }

    // ──────────────────────────────────────────────────────────────
    //  DTOs — nested detail response  (GetPolicyByNumber)
    // ──────────────────────────────────────────────────────────────

    private class PolicyInfoResponse
    {
        [JsonPropertyName("result")]
        public List<ApiPolicyDto>? Result { get; set; }
    }

    // ──────────────────────────────────────────────────────────────
    //  Shared nested DTOs
    // ──────────────────────────────────────────────────────────────

    private class TermInfo
    {
        [JsonPropertyName("EFFECTIVEDATE")]
        public object? EffectiveDate { get; set; }

        [JsonPropertyName("EXPIRATIONDATE")]
        public object? ExpirationDate { get; set; }
    }

    private class InsuredInfo
    {
        [JsonPropertyName("NAMEDINSURED")]
        public string? NamedInsured { get; set; }

        [JsonPropertyName("BUSINESSTYPE")]
        public string? BusinessType { get; set; }

        [JsonPropertyName("LICENSENUMBER")]
        public string? LicenseNumber { get; set; }
    }

    private class ContactInfo
    {
        [JsonPropertyName("PHONES")]
        public List<PhoneInfo>? Phones { get; set; }

        [JsonPropertyName("EMAIL")]
        public string? Email { get; set; }

        [JsonPropertyName("MAILINGADDRESS")]
        public AddressInfo? MailingAddress { get; set; }
    }

    private class PhoneInfo
    {
        [JsonPropertyName("PHONETYPE")]
        public string? PhoneType { get; set; }

        [JsonPropertyName("PHONENUMBER")]
        public string? PhoneNumber { get; set; }

        [JsonPropertyName("ISPRIMARY")]
        public object? IsPrimary { get; set; }
    }

    private class AddressInfo
    {
        [JsonPropertyName("LINE1")]
        public string? Line1 { get; set; }

        [JsonPropertyName("LINE2")]
        public string? Line2 { get; set; }

        [JsonPropertyName("CITY")]
        public string? City { get; set; }

        [JsonPropertyName("STATE")]
        public string? State { get; set; }

        [JsonPropertyName("POSTALCODE")]
        public string? PostalCode { get; set; }

        [JsonPropertyName("COUNTRY")]
        public string? Country { get; set; }
    }

    private class PolicyOptions
    {
        [JsonPropertyName("NONOWNEDVEHICLE")]
        public bool? NonOwnedVehicle { get; set; }

        [JsonPropertyName("HIREDVEHICLE")]
        public bool? HiredVehicle { get; set; }

        [JsonPropertyName("DRIVEOTHERCAR")]
        public bool? DriveOtherCar { get; set; }
    }

    private class VehicleInfo
    {
        [JsonPropertyName("VEHICLEID")]
        public string? VehicleId { get; set; }

        [JsonPropertyName("LOCNUM")]
        public string? LocNum { get; set; }

        [JsonPropertyName("YEAR")]
        public int? Year { get; set; }

        [JsonPropertyName("MAKE")]
        public string? Make { get; set; }

        [JsonPropertyName("MODEL")]
        public string? Model { get; set; }

        [JsonPropertyName("VIN")]
        public string? Vin { get; set; }

        [JsonPropertyName("PLATENUMBER")]
        public string? PlateNumber { get; set; }

        [JsonPropertyName("GARAGING")]
        public GaragingInfo? Garaging { get; set; }

        [JsonPropertyName("USECLASS")]
        public string? UseClass { get; set; }

        [JsonPropertyName("RADIUS")]
        public string? Radius { get; set; }

        [JsonPropertyName("COVERAGES")]
        public object? Coverages { get; set; }

        [JsonPropertyName("LOSSPAYEES")]
        public object? LossPayees { get; set; }
    }

    private class GaragingInfo
    {
        [JsonPropertyName("ZIPCODE")]
        public string? ZipCode { get; set; }

        [JsonPropertyName("CITY")]
        public string? City { get; set; }

        [JsonPropertyName("STATE")]
        public string? State { get; set; }
    }

    private class DriverInfo
    {
        [JsonPropertyName("DRIVERID")]
        public string? DriverId { get; set; }

        [JsonPropertyName("DRVNUM")]
        public string? DriverNum { get; set; }

        [JsonPropertyName("FIRSTNAME")]
        public string? FirstName { get; set; }

        [JsonPropertyName("LASTNAME")]
        public string? LastName { get; set; }

        [JsonPropertyName("DATEOFBIRTH")]
        public string? DateOfBirth { get; set; }

        [JsonPropertyName("LICENSENUMBER")]
        public string? LicenseNumber { get; set; }

        [JsonPropertyName("LICENSESTATE")]
        public string? LicenseState { get; set; }

        [JsonPropertyName("LICENSECLASS")]
        public string? LicenseClass { get; set; }

        [JsonPropertyName("SDIPSTEP")]
        public object? SdipStep { get; set; }

        [JsonPropertyName("ASSIGNEDVEHICLEIDS")]
        public List<string>? AssignedVehicleIds { get; set; }
    }

    private class RatingInfo
    {
        [JsonPropertyName("LASTCALCULATEDAT")]
        public string? LastCalculatedAt { get; set; }

        [JsonPropertyName("TOTALPREMIUM")]
        public decimal? TotalPremium { get; set; }

        [JsonPropertyName("CURRENCY")]
        public string? Currency { get; set; }

        [JsonPropertyName("IRPM")]
        public IrpmInfo? Irpm { get; set; }

        [JsonPropertyName("STATISTICALSUMMARY")]
        public List<object>? StatisticalSummary { get; set; }
    }

    private class IrpmInfo
    {
        [JsonPropertyName("LIABILITYADJUSTMENT")]
        public decimal? LiabilityAdjustment { get; set; }

        [JsonPropertyName("PHYSICALDAMAGEADJUSTMENT")]
        public decimal? PhysicalDamageAdjustment { get; set; }
    }

    private class QuestionnaireInfo
    {
        [JsonPropertyName("ACORDSTATUS")]
        public string? AcordStatus { get; set; }

        [JsonPropertyName("SUPPLEMENTALSTATUS")]
        public string? SupplementalStatus { get; set; }
    }
}
