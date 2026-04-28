using Microsoft.Extensions.Logging;
using MCAQuincyApi.Application.Interfaces;
using MCAQuincyApi.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MCAQuincyApi.Application.Services;

public class PolicyService : IPolicyService
{
    private readonly IDb2Repository _db2Repository;
    private readonly ILogger<PolicyService> _logger;

    public PolicyService(IDb2Repository db2Repository, ILogger<PolicyService> logger)
    {
        _db2Repository = db2Repository;
        _logger = logger;
    }

    public async Task<IEnumerable<Policy>> GetPoliciesAsync(string? search)
    {
        _logger.LogInformation(
            "Fetching policies from DB2 with search filter. Search: {Search}.",
            search);
        return await _db2Repository.GetPoliciesAsync(search);
    }

    public async Task<Policy?> GetPolicyByIdAsync(string policyId)
    {
        _logger.LogInformation("Fetching policy with ID {PolicyId} from DB2.", policyId);
        return await _db2Repository.GetPolicyByIdAsync(policyId);
    }

    public async Task<bool> UpdatePolicyPhoneAsync(string policyId, string phoneNumber)
    {
        _logger.LogInformation("Updating phone numbers for policy with ID {PolicyId}.", policyId);
        var success = await _db2Repository.UpdatePolicyPhoneAsync(policyId, phoneNumber);
        if (!success) {
            _logger.LogWarning("Failed to update phone numbers for policy {PolicyId}. Policy may not exist.", policyId);
        }
        return success;
    }
}
