#!/bin/bash

# exit on failures
set -e
set -o pipefail

# Apply migrations to FIAT db
ConnectionStrings__DefaultConnection=${ConnectionStrings__DefaultConnection:?}

declare -A mssqlconn

for keyvaluepair in $(echo "$ConnectionStrings__DefaultConnection" | sed "s/ //g; s/;/ /g")
do
  IFS=" " read -r -a ARR <<< "${keyvaluepair//=/ }"
  mssqlconn[${ARR[0]}]=${ARR[1]}
done

echo "Merging FIAT database migrations history ..."
if /opt/mssql-tools18/bin/sqlcmd -S "${mssqlconn[Server]}" -U "${mssqlconn[UserId]}" -P "${mssqlconn[Password]}" -d "${mssqlconn[Database]}" -b -C -I -i /app/sql/MergeMigrationsHistoryScript.sql -o /app/sql/MergeMigrationsHistoryScriptOutput.txt; then
  cat /app/sql/MergeMigrationsHistoryScriptOutput.txt
else
  cat /app/sql/MergeMigrationsHistoryScriptOutput.txt
  echo "FIAT database migrations history merge failed." >&2
  exit 1
fi

echo "Running FIAT database migrations ..."
if /opt/mssql-tools18/bin/sqlcmd -S "${mssqlconn[Server]}" -U "${mssqlconn[UserId]}" -P "${mssqlconn[Password]}" -d "${mssqlconn[Database]}" -b -C -I -i /app/sql/FiatDbMigrationScript.sql -o /app/sql/FiatDbMigrationScriptOutput.txt; then
  cat /app/sql/FiatDbMigrationScriptOutput.txt
else
  cat /app/sql/FiatDbMigrationScriptOutput.txt
  echo "FIAT database migrations failed." >&2
  exit 1
fi

echo "Done!"
