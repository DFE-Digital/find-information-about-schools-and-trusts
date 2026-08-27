namespace DfE.FindInformationAcademiesTrusts.Pages.WatchList;

public record WatchListEntry(
    string Name,
    string DateAddedToMsi,
    string AssignedTo,
    string SupportingOrganisation,
    string Milestone,
    string Status,
    string StatusTagColour);
