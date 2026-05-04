using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using MCAQuincyApi.Application.Interfaces;
using MCAQuincyApi.Application.Services;
using MCAQuincyApi.Infrastructure.Persistence;
using MCAQuincyApi.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);
const string LocalFrontendCorsPolicy = "LocalFrontendCorsPolicy";

var postgresConnectionString = builder.Configuration.GetConnectionString("PostgresConnection");
builder.Services.AddDbContext<PostgresDbContext>(options => options.UseNpgsql(postgresConnectionString));

builder.Services.AddScoped<IDb2Repository, Db2Repository>();

builder.Services.AddScoped<IPostgresRepository, PostgresRepository>();
builder.Services.AddScoped<IDataSyncService, DataSyncService>();
builder.Services.AddHttpClient<IPolicyService, PolicyService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy(LocalFrontendCorsPolicy, policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseCors(LocalFrontendCorsPolicy);
app.UseAuthorization();
app.MapControllers();
app.Run();
