namespace DfE.FindInformationAcademiesTrusts.Domain.Entities;

public class Watchlist
{
    public Guid Id { get; set; }
    public int ReadableId { get; }
    public string? EstablishmentId { get; set; }
    public String? TrustId { get; set; }
    public bool IsTrust{ get; set; }
    public string? User { get; set; }
    public DateTime CreatedOn { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime? LastModifiedOn { get; set; }
    public string? LastModifiedBy { get; set; }
}
