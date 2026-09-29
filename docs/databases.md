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
