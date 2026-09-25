using DfE.FindInformationAcademiesTrusts.Domain.ValueObjects;

namespace Dfe.FindInformationAcademiesTrusts.ViewModels;

public class TrustWatchlistViewModel
{
    public Guid WatchlistId  { get; set; }

    public DateTime CreatedOn { get; set; }
    
    public string? Name { get; set; }
    public string? ReferenceNumber { get; set; }
    
    public string? GroupUid { get; set; }
    
    public string? Region { get; set; }
    
    public string? CompaniesHouseNumber { get; set; }
    
}
