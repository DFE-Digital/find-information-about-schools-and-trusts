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

echo "Running FIAT database migrations ..."
until /opt/mssql-tools18/bin/sqlcmd -S "${mssqlconn[Server]}" -U "${mssqlconn[UserId]}" -P "${mssqlconn[Password]}" -d "${mssqlconn[Database]}" -C -I -i /app/sql/FiatDbMigrationScript.sql -o /app/sql/FiatDbMigrationScriptOutput.txt
do
  cat /app/sql/FiatDbMigrationScriptOutput.txt
  echo "Retrying FIAT database migrations ..."
  sleep 5
done

echo "Done!"
