namespace MCAQuincyApi.Domain.Entities;

using System;

public class Policy
{
    public string PolicyId { get; set; } = string.Empty;
    public string InsuredName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? MobileNumber { get; set; }
    public string? PolicyNo { get; set; }
    public string? ProductCode { get; set; }
    public string? ProductName { get; set; }
    public string? State { get; set; }
    public string? InsuredCode { get; set; }
    public string? AgentCode { get; set; }
    public DateTime? AccountingDate { get; set; }
    public DateTime? EffectiveDate { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public DateTime? PolicyEffectiveDate { get; set; }
    public string? ReferralCode { get; set; }
    public string? NewRenew { get; set; }
    public string? NewVenture { get; set; }
    public string? RenewalTerm { get; set; }
    public string? AuthorityType { get; set; }
    public string? QuantumSubAgent { get; set; }
    public string? TransactionType { get; set; }
    public string? StateShortName { get; set; }
    public string? PolicyIdOriginal { get; set; }
    public DateTime? PolicyCancellationDate { get; set; }
    public int? VehicleCount { get; set; }
    public int? ClaimCount { get; set; }
    public string? LiabilityLimit { get; set; }
    public DateTime? PolicyExpirationDate { get; set; }
    public string? QuoteId { get; set; }
    public string? QuoteNumber { get; set; }
    public string? TransactionCode { get; set; }
    public int? EndorsementSeqNo { get; set; }
    public string? PolicyTerm { get; set; }
    public string? Address1 { get; set; }
    public string? Address2 { get; set; }
    public string? City { get; set; }
    public string? Zip { get; set; }
    public string? Telephone { get; set; }
    public string? PhoneType { get; set; }
    public string? Email { get; set; }
    public string? ContactName { get; set; }
    public string? SsnFein { get; set; }
    public string? BusinessType { get; set; }
    public string? SubProducer { get; set; }
    public string? CompanyCode { get; set; }
    public string? PolicyType { get; set; }
    public string? AgencyType { get; set; }
    public string? PaymentPlan { get; set; }
    public string? Fleet { get; set; }
    public string? HiredAuto { get; set; }
    public string? NonOwned { get; set; }
    public string? DriveOtherCar { get; set; }
    public string? AccountCredit { get; set; }
    public string? RelatedPolicy { get; set; }
    public string? RelatedPolicyNumber { get; set; }
    public string? EBill { get; set; }
    public decimal? WrittenPremium { get; set; }
    public decimal? TotalPremium { get; set; }
    public string? ClearanceStatus { get; set; }
    public string? QuoteLinkedPolicy { get; set; }
    public string? ProrateReason { get; set; }
    public string? NonRenewal { get; set; }
    public string? NonRenewalReason { get; set; }
    public string? PriorPolicy { get; set; }
    public DateTime? TransactionDate { get; set; }
    public string? UserId { get; set; }
    public string? UnderwriterId { get; set; }
    public string? Status { get; set; }
    public string? LineOfBusiness { get; set; }
    public string? HeldBy { get; set; }
    public DateTime? EndorseDate { get; set; }
    public string? InsuredContactName { get; set; }
    public string? LicenseNumber { get; set; }
    public string? VehicleInfo { get; set; }
    public string? DriverInfo { get; set; }
}