using MCAQuincyApi.Application.Interfaces;
using MCAQuincyApi.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using System.Threading.Tasks;

namespace MCAQuincyApi.Infrastructure.Repositories;

public class Db2Repository : IDb2Repository {
    private readonly string _connectionString;
    private readonly AS400Settings _as400Settings;
    private readonly ILogger<Db2Repository> _logger;
    private readonly string _getSourceDataQuery;
    private readonly string _getPoliciesQuery;
    private readonly string _searchPoliciesQuery;
    private readonly string _getPolicyByIdQuery;
    private readonly string _updatePolicyPhoneQuery;
    private readonly string _apiUrl;
    private static readonly HttpClient _httpClient = new HttpClient();
    
    public Db2Repository(IConfiguration configuration, ILogger<Db2Repository> logger) {
        _logger = logger;
        _as400Settings = new AS400Settings
        {
            Host = configuration["AS400:Host"] ?? throw new InvalidOperationException("Missing AS400:Host configuration."),
            User = configuration["AS400:User"] ?? throw new InvalidOperationException("Missing AS400:User configuration."),
            Password = configuration["AS400:Password"] ?? throw new InvalidOperationException("Missing AS400:Password configuration."),
            Library = configuration["AS400:Library"] ?? throw new InvalidOperationException("Missing AS400:Library configuration."),
            Table = configuration["AS400:Table"] ?? throw new InvalidOperationException("Missing AS400:Table configuration.")
        };

        _connectionString = $"Driver={{iSeries Access ODBC Driver}};" +
                            $"System={_as400Settings.Host};" +
                            $"Uid={_as400Settings.User};" +
                            $"Pwd={_as400Settings.Password};" +
                            $"DefaultLibraries={_as400Settings.Library};";

        _getSourceDataQuery = configuration["Db2Queries:GetSourceData"] ?? throw new InvalidOperationException("Missing Db2Queries:GetSourceData configuration.");
        _getPoliciesQuery = configuration["Db2Queries:GetPolicies"] ?? throw new InvalidOperationException("Missing Db2Queries:GetPolicies configuration.");
        _searchPoliciesQuery = configuration["Db2Queries:SearchPolicies"] ?? throw new InvalidOperationException("Missing Db2Queries:SearchPolicies configuration.");
        _getPolicyByIdQuery = configuration["Db2Queries:GetPolicyById"] ?? throw new InvalidOperationException("Missing Db2Queries:GetPolicyById configuration.");
        _updatePolicyPhoneQuery = configuration["Db2Queries:UpdatePolicyPhone"] ?? throw new InvalidOperationException("Missing Db2Queries:UpdatePolicyPhone configuration.");
        
        _apiUrl = configuration["ExternalApi:PolicyUrl"] ?? throw new InvalidOperationException("Missing ExternalApi:PolicyUrl configuration.");
    }

    public async Task<IEnumerable<TempData>> GetSourceDataAsync() {
        try {
            var results = new List<TempData>();
            string sql = string.Format(_getSourceDataQuery, _as400Settings.Library);
            
            await using var connection = new OdbcConnection(_connectionString);
            await connection.OpenAsync();
            
            await using var command = new OdbcCommand(sql, connection);
            await using var reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
    
            while (await reader.ReadAsync()) {
                if (reader.FieldCount < 3) continue;

                results.Add(new TempData {
                    Id = reader.GetInt32(0), 
                    ProductName = reader.GetString(1), 
                    Price = reader.GetDecimal(2), 
                    LastRefreshed = DateTime.UtcNow
                });
            }
            return results;
        } catch (OdbcException ex) {
            _logger.LogError(ex, "ODBC error occurred while fetching source data.");
            throw;
        } catch (Exception ex) {
            _logger.LogError(ex, "An unexpected error occurred while fetching source data.");
            throw;
        }
    }

    public async Task<IEnumerable<Policy>> GetPoliciesAsync(string? search)
{
    try
    {
        var apiPolicies = await FetchPoliciesFromApiAsync();
        
        if (!string.IsNullOrWhiteSpace(search))
        {
            var normalizedSearch = search.Trim().Replace(" ", "").ToLowerInvariant();
            return apiPolicies.Where(p => 
                (p.InsuredName != null && p.InsuredName.Replace(" ", "").ToLowerInvariant().Contains(normalizedSearch)) ||
                (p.PolicyNo != null && p.PolicyNo.Replace(" ", "").ToLowerInvariant().Contains(normalizedSearch))
            ).ToList();
        }

        return apiPolicies;

#if false
        var policies = new List<Policy>();
        
        // Normalize search once — null if empty/whitespace
        // Spaces are stripped so "william smith" matches "WilliamSmith" etc.
        string? normalizedSearch = string.IsNullOrWhiteSpace(search)
            ? null
            : search.Trim().Replace(" ", "").ToLower();

        string sql = normalizedSearch is null
            ? string.Format(_getPoliciesQuery, _as400Settings.Library, _as400Settings.Table)
            : string.Format(_searchPoliciesQuery, _as400Settings.Library, _as400Settings.Table);

        await using var connection = new OdbcConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new OdbcCommand(sql, connection);
        
        if (normalizedSearch is not null)
        {
            AddSearchParameters(command, normalizedSearch);
        }

        await using var reader = await command.ExecuteReaderAsync();

        var ordinals = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        for (int i = 0; i < reader.FieldCount; i++)
        {
            ordinals[reader.GetName(i)] = i;
        }

        while (await reader.ReadAsync())
        {
            policies.Add(MapToPolicy(reader, ordinals));
        }

        return policies;
#endif
    }
#if false
    catch (OdbcException ex)
    {
        _logger.LogError(ex, "ODBC error occurred while fetching policies.");
        throw;
    }
#endif
    catch (Exception ex)
    {
        _logger.LogError(ex, "An unexpected error occurred while fetching policies.");
        throw;
    }
}
    private static void AddSearchParameters(OdbcCommand command, string? search)
    {
        // Strip spaces and lowercase to match REPLACE(LOWER(TRIM(col)),' ','') in the SQL query.
        // Only 2 positional parameters: one for INSUREDNAME LIKE, one for POLICYNO LIKE.
        string parameterValue = $"%{search!.Trim().Replace(" ", "").ToLowerInvariant()}%";
        command.Parameters.AddWithValue("?", parameterValue); // INSUREDNAME LIKE ?
        command.Parameters.AddWithValue("?", parameterValue); // POLICYNO LIKE ?
    }

    public async Task<Policy?> GetPolicyByIdAsync(string policyId)
    {
        try {
            var apiPolicies = await FetchPoliciesFromApiAsync();
            return apiPolicies.FirstOrDefault(p => 
                string.Equals(p.PolicyId, policyId, StringComparison.OrdinalIgnoreCase) || 
                string.Equals(p.PolicyNo, policyId, StringComparison.OrdinalIgnoreCase));

#if false
            string sql = string.Format(_getPolicyByIdQuery, _as400Settings.Library, _as400Settings.Table);
    
            await using var connection = new OdbcConnection(_connectionString);
            await connection.OpenAsync();
    
            await using var command = new OdbcCommand(sql, connection);
            command.Parameters.AddWithValue("?", policyId);
    
            await using var reader = await command.ExecuteReaderAsync();
    
            if (await reader.ReadAsync())
            {
                var ordinals = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                for (int i = 0; i < reader.FieldCount; i++) {
                    ordinals[reader.GetName(i)] = i;
                }

                return MapToPolicy(reader, ordinals);
            }
            return null;
#endif
        } catch (OdbcException ex) {
            _logger.LogError(ex, "ODBC error occurred while fetching policy by ID {PolicyId}.", policyId);
            throw;
        } catch (Exception ex) {
            _logger.LogError(ex, "An unexpected error occurred while fetching policy by ID {PolicyId}.", policyId);
            throw;
        }
    }

    public async Task<bool> UpdatePolicyPhoneAsync(string policyId, string phoneNumber)
    {
        try {
            string sql = string.Format(_updatePolicyPhoneQuery, _as400Settings.Library, _as400Settings.Table);
            
            await using var connection = new OdbcConnection(_connectionString);
            await connection.OpenAsync();

            await using var command = new OdbcCommand(sql, connection);
            
            // ODBC uses positional parameters `?`. The order added here must match the SQL query order exactly.
            command.Parameters.AddWithValue("?", (object?)phoneNumber ?? DBNull.Value);
            //command.Parameters.AddWithValue("?", (object?)mobileNumber ?? DBNull.Value);
            command.Parameters.AddWithValue("?", policyId);

            int rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        } catch (OdbcException ex) {
            _logger.LogError(ex, "ODBC error occurred while updating policy phone for ID {PolicyId}.", policyId);
            throw;
        } catch (Exception ex) {
            _logger.LogError(ex, "An unexpected error occurred while updating policy phone for ID {PolicyId}.", policyId);
            throw;
        }
    }

    private static Policy MapToPolicy(DbDataReader reader, Dictionary<string, int> ordinals)
    {
        try {
            return new Policy
            {
                PolicyId = GetString(reader, ordinals, "POLICYID") ?? string.Empty,
                InsuredName = GetString(reader, ordinals, "INSUREDNAME") ?? string.Empty,
                PhoneNumber = GetString(reader, ordinals, "PHONENUMBER"),
                MobileNumber = GetString(reader, ordinals, "MOBILENUMBER"),
                PolicyNo = GetString(reader, ordinals, "POLICYNO"),
                ProductCode = GetString(reader, ordinals, "PRODUCTCODE"),
                ProductName = GetString(reader, ordinals, "PRODUCTNAME"),
                State = GetString(reader, ordinals, "STATE"),
                InsuredCode = GetString(reader, ordinals, "INSUREDCODE"),
                AgentCode = GetString(reader, ordinals, "AGENTCODE"),
                AccountingDate = GetDate(reader, ordinals, "ACCOUNTINGDATE"),
                EffectiveDate = GetDate(reader, ordinals, "EFFECTIVEDATE"),
                ExpirationDate = GetDate(reader, ordinals, "EXPIRATIONDATE"),
                PolicyEffectiveDate = GetDate(reader, ordinals, "POLICYEFFECTIVEDATE"),
                ReferralCode = GetString(reader, ordinals, "REFERRALCODE"),
                NewRenew = GetString(reader, ordinals, "NEWRENEW"),
                NewVenture = GetString(reader, ordinals, "NEWVENTURE"),
                RenewalTerm = GetString(reader, ordinals, "RENEWALTERM"),
                AuthorityType = GetString(reader, ordinals, "AUTHORITYTYPE"),
                QuantumSubAgent = GetString(reader, ordinals, "QUANTUMSUBAGENT"),
                TransactionType = GetString(reader, ordinals, "TRANSACTIONTYPE"),
                StateShortName = GetString(reader, ordinals, "STATESHORTNAME"),
                PolicyIdOriginal = GetString(reader, ordinals, "PolicyId Original"),
                PolicyCancellationDate = GetDate(reader, ordinals, "POLICYCANCELLATIONDATE"),
                VehicleCount = GetInt(reader, ordinals, "Vehicle Count"),
                ClaimCount = GetInt(reader, ordinals, "Claim Count"),
                LiabilityLimit = GetString(reader, ordinals, "LIABILITYLIMIT"),
                PolicyExpirationDate = GetDate(reader, ordinals, "POLICYEXPIRATIONDATE"),
                QuoteId = GetString(reader, ordinals, "QUOTEID"),
                QuoteNumber = GetString(reader, ordinals, "QUOTENUMBER"),
                TransactionCode = GetString(reader, ordinals, "TRANSACTIONCODE"),
                EndorsementSeqNo = GetInt(reader, ordinals, "ENDORSEMENTSEQNO"),
                PolicyTerm = GetString(reader, ordinals, "POLICYTERM"),
                Address1 = GetString(reader, ordinals, "ADDRESS1"),
                Address2 = GetString(reader, ordinals, "ADDRESS2"),
                City = GetString(reader, ordinals, "CITY"),
                Zip = GetString(reader, ordinals, "ZIP"),
                Telephone = GetString(reader, ordinals, "TELEPHONE"),
                PhoneType = GetString(reader, ordinals, "PHONETYPE"),
                Email = GetString(reader, ordinals, "EMAIL"),
                ContactName = GetString(reader, ordinals, "CONTACTNAME"),
                SsnFein = GetString(reader, ordinals, "SSNFEIN"),
                BusinessType = GetString(reader, ordinals, "BUSINESSTYPE"),
                SubProducer = GetString(reader, ordinals, "SUBPRODUCER"),
                CompanyCode = GetString(reader, ordinals, "COMPANYCODE"),
                PolicyType = GetString(reader, ordinals, "POLICYTYPE"),
                AgencyType = GetString(reader, ordinals, "AGENCYTYPE"),
                PaymentPlan = GetString(reader, ordinals, "PAYMENTPLAN"),
                Fleet = GetString(reader, ordinals, "FLEET"),
                HiredAuto = GetString(reader, ordinals, "HIREDAUTO"),
                NonOwned = GetString(reader, ordinals, "NONOWNED"),
                DriveOtherCar = GetString(reader, ordinals, "DRIVEOTHERCAR"),
                AccountCredit = GetString(reader, ordinals, "ACCOUNTCREDIT"),
                RelatedPolicy = GetString(reader, ordinals, "RELATEDPOLICY"),
                RelatedPolicyNumber = GetString(reader, ordinals, "RELATEDPOLICYNUMBER"),
                EBill = GetString(reader, ordinals, "EBILL"),
                WrittenPremium = GetDecimal(reader, ordinals, "WRITTENPREMIUM"),
                TotalPremium = GetDecimal(reader, ordinals, "TOTALPREMIUM"),
                ClearanceStatus = GetString(reader, ordinals, "CLEARANCESTATUS"),
                QuoteLinkedPolicy = GetString(reader, ordinals, "QUOTELINKEDPOLICY"),
                ProrateReason = GetString(reader, ordinals, "PRORATEREASON"),
                NonRenewal = GetString(reader, ordinals, "NONRENEWAL"),
                NonRenewalReason = GetString(reader, ordinals, "NONRENEWALREASON"),
                PriorPolicy = GetString(reader, ordinals, "PRIORPOLICY"),
                TransactionDate = GetDate(reader, ordinals, "TRANSACTIONDATE"),
                UserId = GetString(reader, ordinals, "USERID"),
                UnderwriterId = GetString(reader, ordinals, "UNDERWRITERID"),
                Status = GetString(reader, ordinals, "STATUS"),
                LineOfBusiness = GetString(reader, ordinals, "LINEOFBUSINESS"),
                HeldBy = GetString(reader, ordinals, "HELDBY"),
                EndorseDate = GetDate(reader, ordinals, "ENDORSEDATE"),
                InsuredContactName = GetString(reader, ordinals, "INSUREDCONTACTNAME"),
                LicenseNumber = GetString(reader, ordinals, "LICENSENUMBER")
            };
        } catch (Exception ex) {
            throw new InvalidOperationException("An error occurred while mapping DbDataReader to Policy entity.", ex);
        }
    }

    private static string? GetString(DbDataReader reader, Dictionary<string, int> ordinals, string columnName)
    {
        try {
            if (!ordinals.TryGetValue(columnName, out int ordinal)) return null;
            return reader.IsDBNull(ordinal) ? null : reader.GetValue(ordinal).ToString();
        } catch { return null; }
    }

    private static DateTime? GetDate(DbDataReader reader, Dictionary<string, int> ordinals, string columnName)
    {
        try {
            if (!ordinals.TryGetValue(columnName, out int ordinal) || reader.IsDBNull(ordinal)) return null;

            var value = reader.GetValue(ordinal);
            if (value is DateTime dt) return dt;

            if (DateTime.TryParse(value.ToString(), out DateTime parsedDate))
            {
                return parsedDate;
            }
            return null;
        } catch { return null; }
    }

    private static int? GetInt(DbDataReader reader, Dictionary<string, int> ordinals, string columnName)
    {
        try {
            if (!ordinals.TryGetValue(columnName, out int ordinal) || reader.IsDBNull(ordinal)) return null;

            var value = reader.GetValue(ordinal);
            if (value is int i) return i;
            if (value is decimal d) return Convert.ToInt32(d);

            if (int.TryParse(value.ToString(), out int parsedInt))
            {
                return parsedInt;
            }
            return null;
        } catch { return null; }
    }

    private static decimal? GetDecimal(DbDataReader reader, Dictionary<string, int> ordinals, string columnName)
    {
        try {
            if (!ordinals.TryGetValue(columnName, out int ordinal) || reader.IsDBNull(ordinal)) return null;

            var value = reader.GetValue(ordinal);
            if (value is decimal d) return d;

            if (decimal.TryParse(value.ToString(), out decimal parsedDecimal))
            {
                return parsedDecimal;
            }
            return null;
        } catch { return null; }
    }

    private async Task<List<Policy>> FetchPoliciesFromApiAsync()
    {
        var responseJson = await _httpClient.GetStringAsync(_apiUrl);
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var apiResponse = JsonSerializer.Deserialize<PolicyApiResponse>(responseJson, options);
        
        var policies = new List<Policy>();
        if (apiResponse?.Result == null) return policies;

        foreach (var item in apiResponse.Result)
        {
            policies.Add(new Policy
            {
                QuoteId = item.QUOTEID.ToString(),
                PolicyId = item.POLICYNUMBER?.Trim() ?? string.Empty, // Map number as ID for endpoint lookups
                PolicyNo = item.POLICYNUMBER?.Trim(),
                InsuredName = item.INSUREDNAME?.Trim() ?? string.Empty,
                LineOfBusiness = item.LINEOFBUSINESS?.Trim(),
                EffectiveDate = ParseApiDate(item.EFFECTIVEDATE),
                ExpirationDate = ParseApiDate(item.EXPIRATIONDATE),
                Status = item.STATUS?.Trim(),
                TotalPremium = item.PREMIUM,
                AgentCode = item.AGENTCODE?.Trim(),
                TransactionDate = ParseApiDate(item.TRANSDATE),
                EndorseDate = ParseApiDate(item.ENDORSEDATE),
                TransactionType = item.TRANSACTIONTYPE?.Trim(),
                HeldBy = item.HELDBY?.Trim()
            });
        }
        return policies;
    }

    private static DateTime? ParseApiDate(int dateInt)
    {
        if (dateInt <= 0) return null;
        string dateStr = dateInt.ToString();
        if (dateStr.Length != 8) return null;
        if (DateTime.TryParseExact(dateStr, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out var dt))
        {
            return dt;
        }
        return null;
    }

    private class PolicyApiResponse
    {
        [JsonPropertyName("result")]
        public List<ApiPolicyDto>? Result { get; set; }
    }

    private class ApiPolicyDto
    {
        public int QUOTEID { get; set; }
        public string? POLICYNUMBER { get; set; }
        public string? INSUREDNAME { get; set; }
        public string? LINEOFBUSINESS { get; set; }
        public int EFFECTIVEDATE { get; set; }
        public int EXPIRATIONDATE { get; set; }
        public string? STATUS { get; set; }
        public decimal PREMIUM { get; set; }
        public string? AGENTCODE { get; set; }
        public int TRANSDATE { get; set; }
        public int ENDORSEDATE { get; set; }
        public string? TRANSACTIONTYPE { get; set; }
        public string? HELDBY { get; set; }
    }
}
