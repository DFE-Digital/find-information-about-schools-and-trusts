# Set the major version of dotnet
ARG DOTNET_VERSION=8.0
# Set the major version of nodejs
ARG NODEJS_VERSION_MAJOR=22

# Note: For optimal Docker caching, ensure all steps within a build stage are arranged in order of "least likely to be impacted by changes"

### Build frontend assets using node js ###
FROM node:${NODEJS_VERSION_MAJOR}-bullseye-slim AS assets
WORKDIR /app

# - Copy over and restore npm packages separately so they can be cached by Docker
COPY ./DfE.FindInformationAcademiesTrusts/package.json      /app
COPY ./DfE.FindInformationAcademiesTrusts/package-lock.json /app
# - Only copy over frontend assets so this can be cached independently to other app changes
COPY ./DfE.FindInformationAcademiesTrusts/webpack.config.js /app
COPY ./DfE.FindInformationAcademiesTrusts/assets/           /app/assets/
RUN npm ci --ignore-scripts && \
    npm run build

### Build the app using the dotnet SDK ###
FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION}-azurelinux3.0 AS build
WORKDIR /build

# Mount GitHub Token as a Docker secret so that NuGet Feed can be accessed
RUN --mount=type=secret,id=github_token dotnet nuget add source --username USERNAME --password $(cat /run/secrets/github_token) --store-password-in-clear-text --name github "https://nuget.pkg.github.com/DFE-Digital/index.json"

# - Copy over csprojs and restore nuget packages separately so they can be cached by Docker
COPY ./DfE.FindInformationAcademiesTrusts.Data.Hardcoded/DfE.FindInformationAcademiesTrusts.Data.Hardcoded.csproj       /build/DfE.FindInformationAcademiesTrusts.Data.Hardcoded/
COPY ./DfE.FindInformationAcademiesTrusts.Data.FiatDb/DfE.FindInformationAcademiesTrusts.Data.FiatDb.csproj             /build/DfE.FindInformationAcademiesTrusts.Data.FiatDb/
COPY ./DfE.FindInformationAcademiesTrusts.Data/DfE.FindInformationAcademiesTrusts.Data.csproj                           /build/DfE.FindInformationAcademiesTrusts.Data/
COPY ./DfE.FindInformationAcademiesTrusts.Data.AcademiesDb/DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.csproj   /build/DfE.FindInformationAcademiesTrusts.Data.AcademiesDb/
COPY ./DfE.FindInformationAcademiesTrusts/DfE.FindInformationAcademiesTrusts.csproj                                     /build/DfE.FindInformationAcademiesTrusts/
RUN dotnet restore DfE.FindInformationAcademiesTrusts

# - Only copy over relevant C# code so this can be cached independently to other app changes
COPY ./DfE.FindInformationAcademiesTrusts.Data.Hardcoded/   /build/DfE.FindInformationAcademiesTrusts.Data.Hardcoded/
COPY ./DfE.FindInformationAcademiesTrusts.Data.FiatDb/      /build/DfE.FindInformationAcademiesTrusts.Data.FiatDb/
COPY ./DfE.FindInformationAcademiesTrusts.Data/             /build/DfE.FindInformationAcademiesTrusts.Data/
COPY ./DfE.FindInformationAcademiesTrusts.Data.AcademiesDb/ /build/DfE.FindInformationAcademiesTrusts.Data.AcademiesDb/
COPY ./DfE.FindInformationAcademiesTrusts/                  /build/DfE.FindInformationAcademiesTrusts/
RUN dotnet build DfE.FindInformationAcademiesTrusts --no-restore -c Release && \
    dotnet publish DfE.FindInformationAcademiesTrusts --no-build -o /app

### Entity Framework: Migration Runner ###
FROM ubuntu:22.04@sha256:3c61d3759c2639d4b836d32a2d3c83fa0214e36f195a3421018dbaaf79cbe37f AS initcontainer
RUN apt-get update && \
    apt-get install ca-certificates -y

# - Install upstream requirements
ADD https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb /usr/share/microsoft.deb
RUN dpkg -i /usr/share/microsoft.deb && \
    rm /usr/share/microsoft.deb && \
    apt-get update && \
    apt-get upgrade -y && \
    ACCEPT_EULA=Y apt-get install -y \
    mssql-tools18 \
    msodbcsql18 && \
    apt-get clean && \
    rm -rf /var/lib/apt/lists/*

# - Copy and configure sql migration script
WORKDIR /app
COPY ./DfE.FindInformationAcademiesTrusts.Data.FiatDb/Migrations/FiatDbMigrationScript.sql /app/sql/FiatDbMigrationScript.sql
COPY ./docker/init-docker-entrypoint.sh /app/migratedb

# - Set permissions, create user, and change ownership
RUN chmod +x /app/migratedb && \
    touch /app/sql/FiatDbMigrationScriptOutput.txt && \
    useradd -r -m -U app && \
    chown -R app:app /app

USER app
ENV PATH="$PATH:/opt/mssql-tools18/bin"

### Build a runtime environment ###
FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_VERSION}-azurelinux3.0 AS final
WORKDIR /app
LABEL org.opencontainers.image.source="https://github.com/DFE-Digital/find-information-about-academies-and-trusts"

# - Copy and configure docker entrypoint script
COPY ./docker/web-docker-entrypoint.sh /app/docker-entrypoint.sh

# - Copy and configure sql migration script
COPY ./DfE.FindInformationAcademiesTrusts.Data.FiatDb/Migrations/FiatDbMigrationScript.sql /app/sql/FiatDbMigrationScript.sql

RUN chmod +x /app/docker-entrypoint.sh && \
    touch /app/sql/FiatDbMigrationScriptOutput.txt && \
    chown "$APP_UID" "/app/sql" -R

# - Copy frontend assets
COPY --from=assets /app/wwwroot /app/wwwroot

# - Copy dotnet app
COPY --from=build /app /app

USER $APP_UID
