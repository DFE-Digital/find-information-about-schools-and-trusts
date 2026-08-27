using DfE.FindInformationAcademiesTrusts.Application.Watchlist.Models;

namespace DfE.FindInformationAcademiesTrusts.Pages.WatchList;

public static class WatchListDummyData
{
    public static IReadOnlyList<EstablishmentWatchlistDto> Schools { get; } =
    [
        new("Oakwood Primary School", 100001, "Education Support Services", "Local Authority"),
        new("Riverside Academy", 100002, "Regional Schools Trust", "Local Authority"),
        new("St Mary's High School", 100003, "National Education Partners", "Local Authority"),
        new("Meadow View Primary", 100004, "Local Authority Support", "Local Authority"),
        new("Hillcrest Community School", 100005, "Education Partnership UK", "Local Authority")
    ];

    public static IReadOnlyList<WatchListEntry> Trusts { get; } =
    [
        new("Oakfield Learning Trust", "14 August 2026", "Rebecca Hughes", "Education Support Services",
            "Growth plan review", "In progress", "blue"),
        new("Northgate Multi-Academy Trust", "9 August 2026", "Michael Turner", "Regional Schools Trust",
            "Due diligence", "Complete", "green"),
        new("Bridgewater Trust", "4 August 2026", "Amelia Foster", "National Education Partners",
            "Consultation", "Awaiting response", "yellow"),
        new("Silverdale Education Trust", "29 July 2026", "Daniel Price", "Local Authority Support",
            "Initial assessment", "Action required", "red"),
        new("Elmwood Academies Trust", "22 July 2026", "Grace Mitchell", "Education Partnership UK",
            "Final decision", "Not started", "grey")
    ];
}
