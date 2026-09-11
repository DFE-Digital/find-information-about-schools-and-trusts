using DfE.FindInformationAcademiesTrusts.Domain.Common;
using DfE.FindInformationAcademiesTrusts.Domain.ValueObjects;

namespace DfE.FindInformationAcademiesTrusts.Domain.Entities;

public class Watchlist : BaseAggregateRoot, IEntity<WatchlistId>
{
    
    public Watchlist(WatchlistId id, string? establishmentId, string? trustId, bool isTrust, string user)
    {
        Id = id;
        EstablishmentId = establishmentId;
        TrustId = trustId;
        IsTrust = isTrust;
        User = user;
    }
    
    
    public WatchlistId Id { get; set; }
    
    
    
    public int ReadableId { get; }
    public string? EstablishmentId { get; set; }
    public string? TrustId { get; set; }
    public bool IsTrust{ get; set; }
    public string? User { get; set; }
    public DateTime CreatedOn { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    
    public DateTime? LastModifiedOn { get; set; }
    
    public string? LastModifiedBy { get; set; }
}
