# Getting started

Use this documentation to configure your local development environment for .NET development.

In this doc:

- [Getting started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Configure local user secrets](#configure-local-user-secrets)
  - [Build and watch frontend assets](#build-and-watch-frontend-assets)
  - [Build and run dotnet project](#build-and-run-dotnet-project)

Other useful docs:

- [Run application locally using Docker](./docker.md#run-the-web-application-locally-in-docker)
- [Supercharge your dev environment](./supercharge-your-dev-environment.md)

## Prerequisites

- .NET 8 SDK
- [Node.js v22](https://nodejs.org/en)

## Configure local user secrets

Use the dotnet user secrets tool to set local secrets, any missing required secrets will cause the application to fail at startup with an exception detailing which secrets are missing.

```bash
cd DfE.FindInformationAcademiesTrusts

dotnet user-secrets set "AzureAd:ClientID" "[secret goes here]"
dotnet user-secrets set "AzureAd:ClientSecret" "[secret goes here]"
dotnet user-secrets set "AzureAd:TenantID" "[secret goes here]"

dotnet user-secrets set "ConnectionStrings:AcademiesDb" "[secret goes here for AcademiesDb]"
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "[secret goes here for FiatDb]"
```

See [database local development](./databases.md#local-development) for information on configuring a local database.

## Build and watch frontend assets

The frontend assets must be built before the .NET project. The assets are built into the `wwwroot` folder.

```bash
cd DfE.FindInformationAcademiesTrusts
npm install
npm run dev
```

The `npm run dev` script will watch for any changes to your sass and JavaScript entry files. Simply refresh the browser window to reflect changes whilst your dotnet project is running.

You will need to stop (`ctrl-c`) and rerun the script if you change any saved images.

## Build and run dotnet project

- Install project tools by running `dotnet tool restore` from project directory.
- Ensure you have [built the frontend assets](#build-and-watch-frontend-assets) before building the dotnet project.
- Ensure you have access to the DfE nuget repository `dotnet nuget add source https://nuget.pkg.github.com/DFE-Digital/index.json -n DfE`
- Run/debug project as normal in your chosen IDE

Note that the default `ASPNETCORE_ENVIRONMENT` for local development is `"LocalDevelopment"` (configured in `launchsettings.json`)
