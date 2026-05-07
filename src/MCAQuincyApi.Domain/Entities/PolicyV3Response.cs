using System.Text.Json.Serialization;

namespace MCAQuincyApi.Domain.Entities;

public class PolicyV3Envelope
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("errorCode")]
    public string? ErrorCode { get; set; }

    [JsonPropertyName("durationMs")]
    public long? DurationMs { get; set; }

    [JsonPropertyName("count")]
    public int? Count { get; set; }

    [JsonPropertyName("data")]
    public PolicyV3Response? Data { get; set; }
}

public class PolicyV3ListEnvelope
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("errorCode")]
    public string? ErrorCode { get; set; }

    [JsonPropertyName("durationMs")]
    public long? DurationMs { get; set; }

    [JsonPropertyName("count")]
    public int? Count { get; set; }

    [JsonPropertyName("data")]
    public List<PolicyV3Response>? Data { get; set; }
}

public class PolicyV3Response
{
    [JsonPropertyName("policyId")]
    public string? PolicyId { get; set; }

    [JsonPropertyName("policyNumber")]
    public string? PolicyNumber { get; set; }

    [JsonPropertyName("quoteNumber")]
    public string? QuoteNumber { get; set; }

    [JsonPropertyName("transactionCode")]
    public string? TransactionCode { get; set; }

    [JsonPropertyName("status")]
    public string? Status { get; set; }

    [JsonPropertyName("productCode")]
    public string? ProductCode { get; set; }

    [JsonPropertyName("sourceSystem")]
    public string? SourceSystem { get; set; }

    [JsonPropertyName("recordVersion")]
    public string? RecordVersion { get; set; }

    [JsonPropertyName("term")]
    public PolicyV3Term? Term { get; set; }

    [JsonPropertyName("insured")]
    public PolicyV3Insured? Insured { get; set; }

    [JsonPropertyName("contact")]
    public PolicyV3Contact? Contact { get; set; }

    [JsonPropertyName("policyOptions")]
    public PolicyV3Options? PolicyOptions { get; set; }

    [JsonPropertyName("vehicles")]
    public List<PolicyV3Vehicle>? Vehicles { get; set; }

    [JsonPropertyName("drivers")]
    public List<PolicyV3Driver>? Drivers { get; set; }

    [JsonPropertyName("coverages")]
    public List<PolicyV3Coverage>? Coverages { get; set; }

    [JsonPropertyName("coverageIndicators")]
    public PolicyV3CoverageIndicators? CoverageIndicators { get; set; }

    [JsonPropertyName("rating")]
    public PolicyV3Rating? Rating { get; set; }

    [JsonPropertyName("questionnaire")]
    public PolicyV3Questionnaire? Questionnaire { get; set; }

    [JsonPropertyName("underwriterQuestions")]
    public PolicyV3UnderwriterQuestions? UnderwriterQuestions { get; set; }

    [JsonPropertyName("messages")]
    public List<object>? Messages { get; set; }
}

public class PolicyV3Term
{
    [JsonPropertyName("effectiveDate")]
    public string? EffectiveDate { get; set; }

    [JsonPropertyName("expirationDate")]
    public string? ExpirationDate { get; set; }
}

public class PolicyV3Insured
{
    [JsonPropertyName("namedInsured")]
    public string? NamedInsured { get; set; }

    [JsonPropertyName("businessType")]
    public string? BusinessType { get; set; }

    [JsonPropertyName("licenseNumber")]
    public string? LicenseNumber { get; set; }
}

public class PolicyV3Contact
{
    [JsonPropertyName("phones")]
    public List<PolicyV3Phone>? Phones { get; set; }

    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("mailingAddress")]
    public PolicyV3Address? MailingAddress { get; set; }
}

public class PolicyV3Phone
{
    [JsonPropertyName("phoneType")]
    public string? PhoneType { get; set; }

    [JsonPropertyName("phoneNumber")]
    public string? PhoneNumber { get; set; }

    [JsonPropertyName("isPrimary")]
    public bool? IsPrimary { get; set; }
}

public class PolicyV3Address
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

public class PolicyV3Options
{
    [JsonPropertyName("nonOwnedVehicle")]
    public bool? NonOwnedVehicle { get; set; }

    [JsonPropertyName("hiredVehicle")]
    public bool? HiredVehicle { get; set; }

    [JsonPropertyName("driveOtherCar")]
    public bool? DriveOtherCar { get; set; }
}

public class PolicyV3Vehicle
{
    [JsonPropertyName("vehicleId")]
    public string? VehicleId { get; set; }

    [JsonPropertyName("locnum")]
    public string? LocNum { get; set; }

    [JsonPropertyName("year")]
    public int? Year { get; set; }

    [JsonPropertyName("make")]
    public string? Make { get; set; }

    [JsonPropertyName("model")]
    public string? Model { get; set; }

    [JsonPropertyName("vin")]
    public string? Vin { get; set; }

    [JsonPropertyName("plateNumber")]
    public string? PlateNumber { get; set; }

    [JsonPropertyName("garaging")]
    public PolicyV3Garaging? Garaging { get; set; }

    [JsonPropertyName("useClass")]
    public string? UseClass { get; set; }

    [JsonPropertyName("radius")]
    public string? Radius { get; set; }

    [JsonPropertyName("coverages")]
    public List<PolicyV3Coverage>? Coverages { get; set; }

    [JsonPropertyName("lossPayees")]
    public object? LossPayees { get; set; }
}

public class PolicyV3Coverage
{
    [JsonPropertyName("code")]
    public string? Code { get; set; }

    [JsonPropertyName("limit")]
    public string? Limit { get; set; }

    [JsonPropertyName("deductible")]
    public decimal? Deductible { get; set; }
}

public class PolicyV3Garaging
{
    [JsonPropertyName("zipCode")]
    public string? ZipCode { get; set; }

    [JsonPropertyName("city")]
    public string? City { get; set; }

    [JsonPropertyName("state")]
    public string? State { get; set; }
}

public class PolicyV3Driver
{
    [JsonPropertyName("driverId")]
    public string? DriverId { get; set; }

    [JsonPropertyName("drvnum")]
    public string? DriverNum { get; set; }

    [JsonPropertyName("firstName")]
    public string? FirstName { get; set; }

    [JsonPropertyName("lastName")]
    public string? LastName { get; set; }

    [JsonPropertyName("dateOfBirth")]
    public string? DateOfBirth { get; set; }

    [JsonPropertyName("licenseNumber")]
    public string? LicenseNumber { get; set; }

    [JsonPropertyName("licenseState")]
    public string? LicenseState { get; set; }

    [JsonPropertyName("licenseClass")]
    public string? LicenseClass { get; set; }

    [JsonPropertyName("sdipStep")]
    public int? SdipStep { get; set; }

    [JsonPropertyName("assignedVehicleIds")]
    public List<string>? AssignedVehicleIds { get; set; }
}

public class PolicyV3Rating
{
    [JsonPropertyName("lastCalculatedAt")]
    public string? LastCalculatedAt { get; set; }

    [JsonPropertyName("totalPremium")]
    public decimal? TotalPremium { get; set; }

    [JsonPropertyName("currency")]
    public string? Currency { get; set; }

    [JsonPropertyName("irpm")]
    public PolicyV3Irpm? Irpm { get; set; }

    [JsonPropertyName("statisticalSummary")]
    public List<object>? StatisticalSummary { get; set; }
}

public class PolicyV3Irpm
{
    [JsonPropertyName("liabilityAdjustment")]
    public decimal? LiabilityAdjustment { get; set; }

    [JsonPropertyName("physicalDamageAdjustment")]
    public decimal? PhysicalDamageAdjustment { get; set; }
}

public class PolicyV3Questionnaire
{
    [JsonPropertyName("acordStatus")]
    public string? AcordStatus { get; set; }

    [JsonPropertyName("supplementalStatus")]
    public string? SupplementalStatus { get; set; }
}

public class PolicyV3CoverageIndicators
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

public class PolicyV3UnderwriterQuestions
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
