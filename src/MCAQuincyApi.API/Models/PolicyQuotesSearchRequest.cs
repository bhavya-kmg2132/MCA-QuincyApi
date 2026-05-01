namespace MCAQuincyApi.API.Models;

/// <summary>
/// Request model for searching policy quotes via the external API.
/// Maps to: POST /api/v2/policy/quotes
/// </summary>
public class PolicyQuotesSearchRequest
{
    public string? PolicyNumber { get; set; }
    public string? quoteNumber { get; set; }
    public string? insuredName { get; set; }
    public string? agentCode { get; set; }
    public int? limit { get; set; }
}
