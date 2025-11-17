### Create a database migration image ###
FROM mcr.microsoft.com/mssql-tools@sha256:62556500522072535cb3df2bb5965333dded9be47000473e9e0f84118e248642 AS db-migration

WORKDIR /src

COPY ["/DfE.FindInformationAcademiesTrusts.Data.FiatDb/Migrations/FiatDbMigrationScript.sql", "/src/sql/FiatDbMigrationScript.sql"]

# Entrypoint to run the migration script against the SQL Server
ENTRYPOINT /bin/bash -c "/opt/mssql-tools/bin/sqlcmd -S $SQL_SERVER -d $SQL_DATABASE -U $SQL_USER -P $SQL_PASSWORD -i /src/sql/FiatDbMigrationScript.sql"
