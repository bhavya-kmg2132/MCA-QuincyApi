using System.Text.Json.Serialization;

namespace MCAQuincyApi.Domain.Entities;

public class PolicyApiResponse
{
    [JsonPropertyName("result")]
    public List<ApiPolicyDto>? Result { get; set; }
}

public class PolicyInfoResponse
{
    [JsonPropertyName("result")]
    public List<ApiPolicyDto>? Result { get; set; }
}

public class ApiPolicyDto
{
    [JsonPropertyName("QUOTEID")]
    public int QuoteId { get; set; }

    [JsonPropertyName("POLICYID")]
    public int? PolicyId { get; set; }

    [JsonPropertyName("POLICYNUMBER")]
    public string? PolicyNumber { get; set; }

    [JsonPropertyName("INSUREDNAME")]
    public string? InsuredName { get; set; }

    [JsonPropertyName("LINEOFBUSINESS")]
    public string? LineOfBusiness { get; set; }

    [JsonPropertyName("EFFECTIVEDATE")]
    public int EffectiveDate { get; set; }

    [JsonPropertyName("EXPIRATIONDATE")]
    public int ExpirationDate { get; set; }

    [JsonPropertyName("PREMIUM")]
    public decimal Premium { get; set; }

    [JsonPropertyName("AGENTCODE")]
    public string? AgentCode { get; set; }

    [JsonPropertyName("TRANSDATE")]
    public int TransDate { get; set; }

    [JsonPropertyName("ENDORSEDATE")]
    public int EndorseDate { get; set; }

    [JsonPropertyName("TRANSACTIONTYPE")]
    public string? TransactionType { get; set; }

    [JsonPropertyName("HELDBY")]
    public string? HeldBy { get; set; }

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
    public string? Messages { get; set; }
}

public class TermInfo
{
    [JsonPropertyName("EFFECTIVEDATE")]
    public string? EffectiveDate { get; set; }      // "20250426" — string, parse to DateOnly if needed

    [JsonPropertyName("EXPIRATIONDATE")]
    public string? ExpirationDate { get; set; }
}

public class InsuredInfo
{
    [JsonPropertyName("NAMEDINSURED")]
    public string? NamedInsured { get; set; }

    [JsonPropertyName("BUSINESSTYPE")]
    public string? BusinessType { get; set; }

    [JsonPropertyName("LICENSENUMBER")]
    public string? LicenseNumber { get; set; }
}

public class ContactInfo
{
    [JsonPropertyName("PHONES")]
    public List<PhoneInfo>? Phones { get; set; }

    [JsonPropertyName("EMAIL")]
    public string? Email { get; set; }

    [JsonPropertyName("MAILINGADDRESS")]
    public AddressInfo? MailingAddress { get; set; }
}

public class PhoneInfo
{
    [JsonPropertyName("PHONETYPE")]
    public string? PhoneType { get; set; }          // contains phone number value in this API

    [JsonPropertyName("PHONENUMBER")]
    public string? PhoneNumber { get; set; }

    [JsonPropertyName("ISPRIMARY")]
    public string? IsPrimary { get; set; }          // "4135227575" — API sends string, not bool
}

public class AddressInfo
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

public class PolicyOptions
{
    [JsonPropertyName("NONOWNEDVEHICLE")]
    public bool? NonOwnedVehicle { get; set; }      // true/false/null

    [JsonPropertyName("HIREDVEHICLE")]
    public bool? HiredVehicle { get; set; }

    [JsonPropertyName("DRIVEOTHERCAR")]
    public bool? DriveOtherCar { get; set; }
}

public class VehicleInfo
{
    [JsonPropertyName("VEHICLEID")]
    public string? VehicleId { get; set; }

    [JsonPropertyName("LOCNUM")]
    public string? LocNum { get; set; }

    [JsonPropertyName("YEAR")]
    public int? Year { get; set; }                  // 2021 — integer in JSON

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
    public object? Coverages { get; set; }          // null in sample; use object until schema is known

    [JsonPropertyName("LOSSPAYEES")]
    public object? LossPayees { get; set; }
}

public class GaragingInfo
{
    [JsonPropertyName("ZIPCODE")]
    public string? ZipCode { get; set; }

    [JsonPropertyName("CITY")]
    public string? City { get; set; }

    [JsonPropertyName("STATE")]
    public string? State { get; set; }
}

public class DriverInfo
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
    public string? DateOfBirth { get; set; }        // string — parse to DateOnly if needed

    [JsonPropertyName("LICENSENUMBER")]
    public string? LicenseNumber { get; set; }

    [JsonPropertyName("LICENSESTATE")]
    public string? LicenseState { get; set; }

    [JsonPropertyName("LICENSECLASS")]
    public string? LicenseClass { get; set; }

    [JsonPropertyName("SDIPSTEP")]
    public string? SdipStep { get; set; }

    [JsonPropertyName("ASSIGNEDVEHICLEIDS")]
    public List<string>? AssignedVehicleIds { get; set; }   // null in sample; List<string> is the safe bet
}

public class RatingInfo
{
    [JsonPropertyName("LASTCALCULATEDAT")]
    public string? LastCalculatedAt { get; set; }

    [JsonPropertyName("TOTALPREMIUM")]
    public decimal? TotalPremium { get; set; }      // numeric in real data, null here

    [JsonPropertyName("CURRENCY")]
    public string? Currency { get; set; }

    [JsonPropertyName("IRPM")]
    public IrpmInfo? Irpm { get; set; }

    [JsonPropertyName("STATISTICALSUMMARY")]
    public List<object>? StatisticalSummary { get; set; }
}

public class IrpmInfo
{
    [JsonPropertyName("LIABILITYADJUSTMENT")]
    public decimal? LiabilityAdjustment { get; set; }

    [JsonPropertyName("PHYSICALDAMAGEADJUSTMENT")]
    public decimal? PhysicalDamageAdjustment { get; set; }
}

public class QuestionnaireInfo
{
    [JsonPropertyName("ACORDSTATUS")]
    public string? AcordStatus { get; set; }

    [JsonPropertyName("SUPPLEMENTALSTATUS")]
    public string? SupplementalStatus { get; set; }
}
