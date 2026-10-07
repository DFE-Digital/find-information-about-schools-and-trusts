# Databases

Find information about academies and trusts (FIAT) uses two databases:

- Academies Db - a cross RSD SQL Server database which collates data from several different sources. It is maintained in code as part of the [academies api repository](https://github.com/DFE-Digital/academies-api). FIAT reads information from the database but doesn't update it.
- Fiat Db - a code first SQL Server database maintained by this repository in `DfE.FindInformationAcademiesTrusts.Data.FiatDb`. FIAT reads and writes information in this database.

## Local development

For local development you can either connect to the databases in the Development environment or you can use local databases by installing SQL Server or using a SQL Server Docker container. The connection strings should be set in user secrets.

```bash
cd DfE.FindInformationAcademiesTrusts

dotnet user-secrets set "ConnectionStrings:AcademiesDb" "[secret goes here for AcademiesDb]"
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "[secret goes here for FiatDb]"
```

## Migrations for Fiat Db

### One-off: merge the migrations history (existing databases only)

`FiatDbContext` and `FindInformationAcademiesTrustsContext` used to track their migrations in separate history tables. They have been merged into a single `FindInformationAcademiesTrustsContext`, which only uses `__EFMigrationsHistory`. Any database created before the merge must run `MergeMigrationsHistoryScript.sql` **once** before running `dotnet ef database update`, otherwise it fails with `There is already an object named 'Watchlist' in the database`.

Deployed environments do this automatically (see `docker/init-docker-entrypoint.sh`), so this is only needed for local databases. The script is safe to run more than once and does nothing on a new database.

If you don't have `sqlcmd`, install it with `winget install sqlcmd` (Windows) or `brew install sqlcmd` (macOS).

Replace `<server>` and `<database>` with the values from your `DefaultConnection` connection string:

```bash
### These commands should be run from repository root

# Local SQL Server using Windows authentication
sqlcmd -S <server> -d <database> -E -C -i ./DfE.FindInformationAcademiesTrusts.Data.FiatDb/Migrations/MergeMigrationsHistoryScript.sql

# SQL Server using a SQL login (e.g. SQL Server in Docker: -S localhost,1433 -U sa)
sqlcmd -S <server> -d <database> -U <user> -P <password> -C -i ./DfE.FindInformationAcademiesTrusts.Data.FiatDb/Migrations/MergeMigrationsHistoryScript.sql

# Check it worked - __EFMigrationsHistory should include 20260909123351_InitialCreate
# (swap -E for -U <user> -P <password> if you use a SQL login)
sqlcmd -S <server> -d <database> -E -C -Q "SELECT MigrationId FROM __EFMigrationsHistory"
```

If `__FindInformationAcademiesTrustsContextMigrationsHistory` does not exist but the `Watchlist` table does, record the migration by hand instead:

```bash
sqlcmd -S <server> -d <database> -E -C -Q "INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion) VALUES (N'20260909123351_InitialCreate', N'8.0.22')"
```

#### Running it manually (no install)

If you'd rather not install `sqlcmd`, run the SQL directly against your local database using any SQL tool (SSMS, Azure Data Studio, Rider/Visual Studio database tools, etc.):

1. Connect to your local SQL Server using the details from your `DefaultConnection` connection string (e.g. SQL Server in Docker: server `localhost,1433`, SQL Server Authentication, user `sa`).
2. Select your FIAT database (e.g. `FiatDb`) - new query windows often default to `master`.
3. Run the contents of `MergeMigrationsHistoryScript.sql`, or if `__FindInformationAcademiesTrustsContextMigrationsHistory` does not exist, run:

```sql
INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion)
VALUES (N'20260909123351_InitialCreate', N'8.0.22');
```

Then confirm `20260909123351_InitialCreate` is no longer shown as `(Pending)`:

```bash
dotnet ef migrations list --context FindInformationAcademiesTrustsContext --project DfE.FindInformationAcademiesTrusts.Data.FiatDb --startup-project DfE.FindInformationAcademiesTrusts
```

### Adding and applying migrations

```bash
### These commands should be run from repository root

# Ensure dotnet ef is installed and up to date
dotnet tool restore

# Check whether there needs to be a new migration
dotnet ef migrations has-pending-model-changes --context FindInformationAcademiesTrustsContext --project DfE.FindInformationAcademiesTrusts.Data.FiatDb --startup-project DfE.FindInformationAcademiesTrusts

# Add new migration
dotnet ef migrations add NameOfMigrationGoesHere --context FindInformationAcademiesTrustsContext --project DfE.FindInformationAcademiesTrusts.Data.FiatDb --startup-project DfE.FindInformationAcademiesTrusts
```

A new migration and snapshot should be created in `DfE.FindInformationAcademiesTrusts.Data.FiatDb/Migrations`. Look to ensure that only the changes you were expecting are there and add any custom migration code to this new migration file.

Ensure that you test the new migration on a local copy of the database (which contains pre-existing data) before committing the code to ensure that there is no data loss.

```bash
# Update db to latest migration
dotnet ef database update --context FindInformationAcademiesTrustsContext --project DfE.FindInformationAcademiesTrusts.Data.FiatDb --startup-project DfE.FindInformationAcademiesTrusts

# Undo all known updates to db (note that removed migrations can't be removed from db by EF)
dotnet ef database update 0 --context FindInformationAcademiesTrustsContext --project DfE.FindInformationAcademiesTrusts.Data.FiatDb --startup-project DfE.FindInformationAcademiesTrusts
```

Once happy, **run the script below** to generate the SQL script which is used by the pipeline to deploy the migrations. You may want to double check that the SQL output is as expected but do not alter this SQL script directly as it will be overwritten by the next migration.

```bash
# Turn migrations into SQL
dotnet ef migrations script --idempotent -o ./DfE.FindInformationAcademiesTrusts.Data.FiatDb/Migrations/FiatDbMigrationScript.sql --context FindInformationAcademiesTrustsContext --project DfE.FindInformationAcademiesTrusts.Data.FiatDb --startup-project DfE.FindInformationAcademiesTrusts
```
