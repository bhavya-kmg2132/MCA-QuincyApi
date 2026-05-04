using System.Text.Json.Serialization;

namespace MCAQuincyApi.API.Models;

public class PolicyQuotesV2SearchRequest
{
    [JsonPropertyName("PolicyNumber")]
    public string? PolicyNumber { get; set; }

    [JsonPropertyName("insuredName")]
    public string? InsuredName { get; set; }

    [JsonPropertyName("agentCode")]
    public string? AgentCode { get; set; }

    [JsonPropertyName("limit")]
    public int? Limit { get; set; }
}
