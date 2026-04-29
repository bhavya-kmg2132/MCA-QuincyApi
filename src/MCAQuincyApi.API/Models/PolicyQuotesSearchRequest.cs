namespace MCAQuincyApi.API.Models;

/// <summary>
/// Request model for searching policy quotes via the external API.
/// Maps to: POST /api/v2/policy/quotes
/// </summary>
public class PolicyQuotesSearchRequest
{
    public string? PolicyNumber { get; set; }
    public string? InsuredName { get; set; }
    public string? AgentCode { get; set; }
}
