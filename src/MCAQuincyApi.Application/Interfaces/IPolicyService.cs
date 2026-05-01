using MCAQuincyApi.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MCAQuincyApi.Application.Interfaces;

public interface IPolicyService
{
    /// <summary>
    /// Search policy quotes via the external API.
    /// POST /api/v2/policy/quotes
    /// </summary>
    Task<IEnumerable<Policy>> GetPolicyQuotesAsync( string? insuredName, string? agentCode, string? quoteNumber, int? limit = null);

    /// <summary>
    /// Get policy details by policy number from the external API.
    /// GET /api/v2/policy/{policyNumber}
    /// </summary>
    Task<Policy?> GetPolicyByNumberAsync(string policyNumber);

    /// <summary>
    /// Update the phone number for a policy via the external API.
    /// POST /api/v2/policy/SavePolicyInfo
    /// </summary>
    Task<bool> UpdatePolicyPhoneAsync(string policyNumber, string telephone);
}
