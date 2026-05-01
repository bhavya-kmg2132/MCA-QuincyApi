using Microsoft.EntityFrameworkCore;
using MCAQuincyApi.Application.Interfaces;
using MCAQuincyApi.Domain.Entities;
using MCAQuincyApi.Infrastructure.Persistence;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MCAQuincyApi.Infrastructure.Repositories;

public class PostgresRepository : IPostgresRepository {
    private readonly PostgresDbContext _context;
    
    public PostgresRepository(PostgresDbContext context) { 
        _context = context; 
    }

    public async Task<IEnumerable<TempData>> GetAllTempDataAsync() => 
        await _context.TempDataItems.AsNoTracking().ToListAsync();

    public async Task SyncDataAsync(IEnumerable<TempData> data) {
        await using var transaction = await _context.Database.BeginTransactionAsync();
        
        // Fixed: Escaped quotes around TempData
        await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"TempData\" RESTART IDENTITY");
        
        await _context.TempDataItems.AddRangeAsync(data);
        await _context.SaveChangesAsync();
        await transaction.CommitAsync();
    }
}
