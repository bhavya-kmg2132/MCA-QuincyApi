using MCAQuincyApi.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MCAQuincyApi.Application.Interfaces;

public interface IDb2Repository
{
    Task<IEnumerable<TempData>> GetSourceDataAsync();
    Task<IEnumerable<Policy>> GetPoliciesAsync(string? search);
    Task<Policy?> GetPolicyByIdAsync(string policyId);
    Task<bool> UpdatePolicyPhoneAsync(string policyId, string phoneNumber);
}
