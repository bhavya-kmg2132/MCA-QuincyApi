using MCAQuincyApi.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace MCAQuincyApi.Application.Interfaces;

public interface IDataSyncService {
    Task SynchronizeDataAsync();
    Task<IEnumerable<TempData>> GetSyncedDataAsync();
}
public interface IPostgresRepository {
    Task SyncDataAsync(IEnumerable<TempData> data);
    Task<IEnumerable<TempData>> GetAllTempDataAsync();
}