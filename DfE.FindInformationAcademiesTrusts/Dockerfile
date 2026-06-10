# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:10.0@sha256:ddcf70ad1ab963a4fcd41fbd722a6b660e404e87567cfbd46fd2809c21b02088 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0@sha256:548d93f8a18a1acbe6cc127bc4f47281430d34a9e35c18afa80a8d6741c2adc3 AS build
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