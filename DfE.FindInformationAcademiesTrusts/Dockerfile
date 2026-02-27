# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0@sha256:88c86e5469c2b69969e07343193fd3dbe4ff6936d95b994dbfa27348299af111 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0@sha256:bfb6ed602caa605141700aea7dc7d42574b74b704368e67d683c71a002123808 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["DfE.FindInformationAcademiesTrusts/DfE.FindInformationAcademiesTrusts.csproj", "DfE.FindInformationAcademiesTrusts/"]
COPY ["DfE.FindInformationAcademiesTrusts.Data.AcademiesDb/DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.csproj", "DfE.FindInformationAcademiesTrusts.Data.AcademiesDb/"]
COPY ["DfE.FindInformationAcademiesTrusts.Data/DfE.FindInformationAcademiesTrusts.Data.csproj", "DfE.FindInformationAcademiesTrusts.Data/"]
COPY ["DfE.FindInformationAcademiesTrusts.Data.FiatDb/DfE.FindInformationAcademiesTrusts.Data.FiatDb.csproj", "DfE.FindInformationAcademiesTrusts.Data.FiatDb/"]
COPY ["DfE.FindInformationAcademiesTrusts.Data.Hardcoded/DfE.FindInformationAcademiesTrusts.Data.Hardcoded.csproj", "DfE.FindInformationAcademiesTrusts.Data.Hardcoded/"]
RUN dotnet restore "./DfE.FindInformationAcademiesTrusts/DfE.FindInformationAcademiesTrusts.csproj"
COPY . .
WORKDIR "/src/DfE.FindInformationAcademiesTrusts"
RUN dotnet build "./DfE.FindInformationAcademiesTrusts.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./DfE.FindInformationAcademiesTrusts.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DfE.FindInformationAcademiesTrusts.dll"]