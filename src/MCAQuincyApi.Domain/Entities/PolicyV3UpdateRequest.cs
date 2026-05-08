using System.Text.Json.Serialization;
using System.Linq;

namespace MCAQuincyApi.Domain.Entities;

public class PolicyV3UpdateRequest
{
    [JsonPropertyName("policyNumber")]
    public string PolicyNumber { get; set; } = string.Empty;

    [JsonPropertyName("insured")]
    public PolicyV3UpdateInsured? Insured { get; set; }

    [JsonPropertyName("telephone")]
    public string? Telephone { get; set; }

    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("mailingAddress")]
    public PolicyV3UpdateMailingAddress? MailingAddress { get; set; }

    [JsonPropertyName("coverageIndicators")]
    public PolicyV3UpdateCoverageIndicators? CoverageIndicators { get; set; }

    [JsonPropertyName("underwriterQuestions")]
    public PolicyV3UpdateUnderwriterQuestions? UnderwriterQuestions { get; set; }

    public static PolicyV3UpdateRequest FromPolicyV3Response(PolicyV3Response source)
    {
        return new PolicyV3UpdateRequest
        {
            PolicyNumber = source.PolicyNumber ?? string.Empty,
            Telephone = source.Contact?.Phones?.FirstOrDefault()?.PhoneNumber,
            Email = source.Contact?.Email,
            Insured = source.Insured == null
                ? null
                : new PolicyV3UpdateInsured
                {
                    NamedInsured = source.Insured.NamedInsured,
                    BusinessType = source.Insured.BusinessType,
                    LicenseNumber = source.Insured.LicenseNumber
                },
            MailingAddress = source.Contact?.MailingAddress == null
                ? null
                : new PolicyV3UpdateMailingAddress
                {
                    Line1 = source.Contact.MailingAddress.Line1,
                    Line2 = source.Contact.MailingAddress.Line2,
                    City = source.Contact.MailingAddress.City,
                    State = source.Contact.MailingAddress.State,
                    PostalCode = source.Contact.MailingAddress.PostalCode,
                    Country = source.Contact.MailingAddress.Country
                },
            CoverageIndicators = source.CoverageIndicators == null
                ? null
                : new PolicyV3UpdateCoverageIndicators
                {
                    NonOwnedAuto = source.CoverageIndicators.NonOwnedAuto,
                    HiredAuto = source.CoverageIndicators.HiredAuto,
                    DriveOtherCar = source.CoverageIndicators.DriveOtherCar,
                    FleetStatus = source.CoverageIndicators.FleetStatus
                },
            UnderwriterQuestions = source.UnderwriterQuestions == null
                ? null
                : new PolicyV3UpdateUnderwriterQuestions
                {
                    HazardousMaterialsTransport = source.UnderwriterQuestions.HazardousMaterialsTransport,
                    ValidFeinFid = source.UnderwriterQuestions.ValidFeinFid,
                    SnowRemovalForFee = source.UnderwriterQuestions.SnowRemovalForFee,
                    IccPucFilings = source.UnderwriterQuestions.IccPucFilings
                }
        };
    }
}

public class PolicyV3UpdateInsured
{
    [JsonPropertyName("namedInsured")]
    public string? NamedInsured { get; set; }

    [JsonPropertyName("businessType")]
    public string? BusinessType { get; set; }

    [JsonPropertyName("licenseNumber")]
    public string? LicenseNumber { get; set; }
}

public class PolicyV3UpdateMailingAddress
{
    [JsonPropertyName("line1")]
    public string? Line1 { get; set; }

    [JsonPropertyName("line2")]
    public string? Line2 { get; set; }

    [JsonPropertyName("city")]
    public string? City { get; set; }

    [JsonPropertyName("state")]
    public string? State { get; set; }

    [JsonPropertyName("postalCode")]
    public string? PostalCode { get; set; }

    [JsonPropertyName("country")]
    public string? Country { get; set; }
}

public class PolicyV3UpdateCoverageIndicators
{
    [JsonPropertyName("nonOwnedAuto")]
    public string? NonOwnedAuto { get; set; }

    [JsonPropertyName("hiredAuto")]
    public string? HiredAuto { get; set; }

    [JsonPropertyName("driveOtherCar")]
    public string? DriveOtherCar { get; set; }

    [JsonPropertyName("fleetStatus")]
    public string? FleetStatus { get; set; }
}

public class PolicyV3UpdateUnderwriterQuestions
{
    [JsonPropertyName("hazardousMaterialsTransport")]
    public string? HazardousMaterialsTransport { get; set; }

    [JsonPropertyName("validFeinFid")]
    public string? ValidFeinFid { get; set; }

    [JsonPropertyName("snowRemovalForFee")]
    public string? SnowRemovalForFee { get; set; }

    [JsonPropertyName("iccPucFilings")]
    public string? IccPucFilings { get; set; }
}
