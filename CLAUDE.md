# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
# Build the solution
dotnet build

# Run the API (Development profile, http://localhost:5010)
cd src/MCAQuincyApi.API && dotnet run --launch-profile http

# Run with HTTPS (https://localhost:7039)
cd src/MCAQuincyApi.API && dotnet run --launch-profile https

# Restore dependencies
dotnet restore
```

No test projects exist in this solution.

## Architecture

Clean Architecture with 4 layers:

```
MCAQuincyApi.API           → Controllers, Swagger, DI wiring (Program.cs)
MCAQuincyApi.Application   → Interfaces + Services (business logic)
MCAQuincyApi.Domain        → Entities only (Policy, TempData)
MCAQuincyApi.Infrastructure → Repositories, DbContext, ODBC driver
```

Dependency direction: API → Application ← Infrastructure, both depend on Domain.

### Two data sources

**DB2/AS400** (read-only policy data):
- Connected via ODBC using `iSeries Access ODBC Driver`
- `Db2Repository` executes raw SQL through `System.Data.Odbc`
- All SQL queries are stored in `appsettings.json` under `Db2Queries` and formatted at runtime with `{0}` = Library, `{1}` = Table
- Column mapping is done manually in `MapToPolicy()` using an ordinals dictionary (`Dictionary<string, int>`) to avoid positional fragility
- Some DB2 column names contain spaces (e.g., `"PolicyId Original"`, `"Vehicle Count"`) — these require double-quote escaping in SQL and exact string matching in the ordinals lookup
- ODBC uses positional `?` parameters, not named parameters

**PostgreSQL** (sync target):
- Connected via EF Core + Npgsql (`PostgresDbContext`)
- Only used for `TempData` sync via `DataSyncService` — the sync flow pulls from DB2 and writes to Postgres
- Azure-hosted: `pg-server-1.postgres.database.azure.com`

### Key config sections (`appsettings.json` / `appsettings.Development.json`)

| Key | Purpose |
|-----|---------|
| `ConnectionStrings:PostgresConnection` | EF Core Npgsql connection |
| `AS400:Host/User/Password/Library/Table` | ODBC connection to AS400 |
| `Db2Queries:GetPolicies` | Full SELECT for all policies |
| `Db2Queries:GetPolicyById` | SELECT with `WHERE POLICYID = ?` |
| `Db2Queries:UpdatePolicyPhone` | UPDATE for phone field |

`appsettings.Development.json` overrides `appsettings.json` and contains the extended column list (address, contact, premium fields, and the 6 new columns: `STATUS`, `LINEOFBUSINESS`, `HELDBY`, `ENDORSEDATE`, `INSUREDCONTACTNAME`, `LICENSENUMBER`). The production `appsettings.json` has a shorter column list — keep both in sync when adding columns.

### Adding a new Policy column

1. `ALTER TABLE DB2DEV1.POLICY_NEW ADD COLUMN ...` on the DB2 side
2. Add the property to `Policy.cs` (Domain)
3. Add mapping in `Db2Repository.MapToPolicy()` using the appropriate `GetString/GetDate/GetInt/GetDecimal` helper
4. Add the column name to both `GetPolicies` and `GetPolicyById` queries in **both** appsettings files

### API surface

| Method | Route | Handler |
|--------|-------|---------|
| GET | `/api/policies` | Returns all policies (capped at 100 rows by DB2 query) |
| GET | `/api/policies/{id}` | Returns single policy by `POLICYID` |
| PUT | `/api/policies/{id}/phone` | Updates `PHONENUMBER` on DB2 |
| POST | `/api/data/sync` | Pulls `TempData` from DB2 → writes to PostgreSQL |
| GET | `/api/data` | Returns synced `TempData` from PostgreSQL |

Swagger UI is available at `/swagger` in all environments (enabled unconditionally in `Program.cs`).
