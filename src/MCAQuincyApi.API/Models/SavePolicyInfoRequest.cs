namespace MCAQuincyApi.API.Models;

/// <summary>
/// Request model for updating policy phone number via the external API.
/// Maps to: POST /api/v2/policy/SavePolicyInfo
/// </summary>
public class SavePolicyInfoRequest
{
    public string Telephone { get; set; } = string.Empty;
    public string PolicyNumber { get; set; } = string.Empty;
}
