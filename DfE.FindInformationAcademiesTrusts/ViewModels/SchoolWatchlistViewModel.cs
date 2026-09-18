using DfE.FindInformationAcademiesTrusts.Domain.ValueObjects;

namespace Dfe.FindInformationAcademiesTrusts.ViewModels;

public class SchoolWatchlistViewModel
{
    public Guid WatchlistId  { get; set; }

    public DateTime CreatedOn { get; set; }
    
    public string? Urn { get; set; }
    
    public string? Name { get; set; }
    
    public string? TrustName { get; set; }
    
    public string? LocalAuthority { get; set; }
}
