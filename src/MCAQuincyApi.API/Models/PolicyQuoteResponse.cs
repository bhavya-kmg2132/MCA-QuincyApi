using MCAQuincyApi.Domain.Entities;

namespace MCAQuincyApi.API.Models;

public class PolicyQuoteResponse
{
    public string? QuoteId { get; set; }
    public string? PolicyNumber { get; set; }
    public string InsuredName { get; set; } = string.Empty;
    public string? LineOfBusiness { get; set; }
    public string? EffectiveDate { get; set; }
    public string? ExpirationDate { get; set; }
    public string? Status { get; set; }
    public decimal? Premium { get; set; }
    public string? AgentCode { get; set; }
    public string? TransDate { get; set; }
    public string? EndorseDate { get; set; }
    public string? TransactionType { get; set; }
    public string? HeldBy { get; set; }

    public static PolicyQuoteResponse FromPolicy(Policy policy)
        => new()
        {
            QuoteId = policy.QuoteId,
            PolicyNumber = policy.PolicyNo,
            InsuredName = policy.InsuredName,
            LineOfBusiness = policy.LineOfBusiness,
            EffectiveDate = FormatDate(policy.EffectiveDate),
            ExpirationDate = FormatDate(policy.ExpirationDate),
            Status = policy.Status,
            Premium = policy.TotalPremium,
            AgentCode = policy.AgentCode,
            TransDate = FormatDate(policy.TransactionDate),
            EndorseDate = FormatDate(policy.EndorseDate),
            TransactionType = policy.TransactionType,
            HeldBy = policy.HeldBy
        };

    private static string? FormatDate(DateTime? value)
        => value?.ToString("yyyy-MM-dd");
}
