using MCAQuincyApi.Domain.Entities;
using System.Linq;
using System.Text.Json.Serialization;

namespace MCAQuincyApi.API.Models;

public class SavePolicyInfoV3Response
{
    [JsonPropertyName("policyNumber")]
    public string? PolicyNumber { get; set; }

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

    public static SavePolicyInfoV3Response? FromPolicyV3(PolicyV3Response? policy)
    {
        if (policy == null)
        {
            return null;
        }

        return new SavePolicyInfoV3Response
        {
            PolicyNumber = policy.PolicyNumber,
            Telephone = policy.Contact?.Phones?.FirstOrDefault()?.PhoneNumber,
            Email = policy.Contact?.Email,
            Insured = policy.Insured == null
                ? null
                : new PolicyV3UpdateInsured
                {
                    NamedInsured = policy.Insured.NamedInsured,
                    BusinessType = policy.Insured.BusinessType,
                    LicenseNumber = policy.Insured.LicenseNumber
                },
            MailingAddress = policy.Contact?.MailingAddress == null
                ? null
                : new PolicyV3UpdateMailingAddress
                {
                    Line1 = policy.Contact.MailingAddress.Line1,
                    Line2 = policy.Contact.MailingAddress.Line2,
                    City = policy.Contact.MailingAddress.City,
                    State = policy.Contact.MailingAddress.State,
                    PostalCode = policy.Contact.MailingAddress.PostalCode,
                    Country = policy.Contact.MailingAddress.Country
                },
            CoverageIndicators = policy.CoverageIndicators == null
                ? null
                : new PolicyV3UpdateCoverageIndicators
                {
                    NonOwnedAuto = policy.CoverageIndicators.NonOwnedAuto,
                    HiredAuto = policy.CoverageIndicators.HiredAuto,
                    DriveOtherCar = policy.CoverageIndicators.DriveOtherCar,
                    FleetStatus = policy.CoverageIndicators.FleetStatus
                },
            UnderwriterQuestions = policy.UnderwriterQuestions == null
                ? null
                : new PolicyV3UpdateUnderwriterQuestions
                {
                    HazardousMaterialsTransport = policy.UnderwriterQuestions.HazardousMaterialsTransport,
                    ValidFeinFid = policy.UnderwriterQuestions.ValidFeinFid,
                    SnowRemovalForFee = policy.UnderwriterQuestions.SnowRemovalForFee,
                    IccPucFilings = policy.UnderwriterQuestions.IccPucFilings
                }
        };
    }
}
