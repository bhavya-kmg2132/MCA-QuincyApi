using System.Text.Json.Serialization;

namespace MCAQuincyApi.API.Models;

public class SavePolicyInfoV2Request
{
    [JsonPropertyName("telephone")]
    public string Telephone { get; set; } = string.Empty;

    [JsonPropertyName("policyNumber")]
    public string PolicyNumber { get; set; } = string.Empty;
}
