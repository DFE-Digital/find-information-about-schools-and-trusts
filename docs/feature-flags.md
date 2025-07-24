# Feature flags

Feature flags can be used to programatically turn features on and off. We can use them to control the release of features in different environments or at different times.

## List of current flags

- `TestFlag` (Default: false): Flag added to test the functionality of feature flags. Adds a line to the layout that shows flags are working when set to true.
- `UpdatedFooterHelpLink` (Default: true): Flag added to control when we release the updated help link in the footer.
- `GovernanceTurnover` (Default: false): Flag to hide the governance turnover calculation until a bug fix can be determined.
- `ContactsInDfeForSchools` (Default: false): This flag controls the visibility of the 'Contacts in DfE' page for a school or academy. The page is accessible and appears in the school nav menu when the flag is set to true.

## Implementation

We have followed what is more or less the default DotNet appoach for adding feature flags to our application. More documentation on using feature flags can be found on the [Microsoft docs here](https://learn.microsoft.com/en-us/azure/azure-app-configuration/use-feature-flags-dotnet-core).

### Adding a flag

To create a flag you need to add it as a constant to `DfE.FindInformationAcadmiesTrusts/Configuration/FeatureFlags.cs` and add a default value to the appsettings.json file. Finally add the flag name, default value (used in appsettings.json) and reason for creating the flag/what the flag controls to the list above.

### Using a flag

Flags can be used in 3 main ways to restrict parts of the code:

- Using the tag helper in a razor page
- Using an attribute on a class or function
- Using dependency injection with the IFeatureManagement class

They can then be toggled using enviroment variables in each of the different environments, or using the appsettings.json file to override them when developing locally.
