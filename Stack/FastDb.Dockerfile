### Create a database migration image ###
FROM mcr.microsoft.com/mssql-tools AS db-migration

WORKDIR /src

COPY ["/DfE.FindInformationAcademiesTrusts.Data.FiatDb/Migrations/FiatDbMigrationScript.sql", "/src/sql/FiatDbMigrationScript.sql"]

# Entrypoint to run the migration script against the SQL Server
ENTRYPOINT /bin/bash -c "/opt/mssql-tools/bin/sqlcmd -S $SQL_SERVER -d $SQL_DATABASE -U $SQL_USER -P $SQL_PASSWORD -i /src/sql/FiatDbMigrationScript.sql"
