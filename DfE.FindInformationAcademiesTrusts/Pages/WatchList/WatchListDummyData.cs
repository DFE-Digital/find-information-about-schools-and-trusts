using DfE.FindInformationAcademiesTrusts.Application.Watchlist.Models;

namespace DfE.FindInformationAcademiesTrusts.Pages.WatchList;

public static class WatchListDummyData
{
    public static IReadOnlyList<EstablishmentWatchlistDto> Schools { get; } =
    [
        new("Oakwood Primary School", "100001", "Education Support Services", "Local Authority"),
        new("Riverside Academy", "100002", "Regional Schools Trust", "Local Authority"),
        new("St Mary's High School", "100003", "National Education Partners", "Local Authority"),
        new("Meadow View Primary", "100004", "Local Authority Support", "Local Authority"),
        new("Hillcrest Community School", "100005", "Education Partnership UK", "Local Authority")
    ];

    public static IReadOnlyList<TrustWatchlistDto> Trusts { get; } =
    [
        new("Oakfield Learning Trust", 100001, "Education Support Services", "CH100001"),
        new("Northgate Multi-Academy Trust", 100002, "Regional Schools Trust", "CH100002"),
        new("Bridgewater Trust", 100003, "National Education Partners", "CH100003"),
        new("Silverdale Education Trust", 100004, "Local Authority Support", "CH100004"),
        new("Elmwood Academies Trust", 100005, "Education Partnership UK", "CH100005")
    ];

}
