using Microsoft.EntityFrameworkCore;
using MCAQuincyApi.Domain.Entities;
namespace MCAQuincyApi.Infrastructure.Persistence;

public class PostgresDbContext : DbContext {
    public PostgresDbContext(DbContextOptions<PostgresDbContext> options) : base(options) { }
    public DbSet<TempData> TempDataItems { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.Entity<TempData>().ToTable("TempData");
    }
}