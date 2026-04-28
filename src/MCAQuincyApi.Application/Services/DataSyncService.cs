using Microsoft.Extensions.Logging;
using MCAQuincyApi.Application.Interfaces;
using MCAQuincyApi.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace MCAQuincyApi.Application.Services;

public class DataSyncService : IDataSyncService {
    private readonly IDb2Repository _db2Repository;
    private readonly IPostgresRepository _postgresRepository;
    private readonly ILogger<DataSyncService> _logger;

    public DataSyncService(IDb2Repository db2Repository, IPostgresRepository postgresRepository, ILogger<DataSyncService> logger) {
        _db2Repository = db2Repository;
        _postgresRepository = postgresRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<TempData>> GetSyncedDataAsync() => await _postgresRepository.GetAllTempDataAsync();

    public async Task SynchronizeDataAsync() {
        _logger.LogInformation("Starting data sync from DB2 to PostgreSQL.");
        var sourceData = await _db2Repository.GetSourceDataAsync();
        var tempDataList = sourceData.ToList();
        await _postgresRepository.SyncDataAsync(tempDataList);
        _logger.LogInformation($"Successfully synced {tempDataList.Count} records to PostgreSQL.");
    }
}