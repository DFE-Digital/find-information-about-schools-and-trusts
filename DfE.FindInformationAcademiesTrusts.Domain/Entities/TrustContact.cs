using DfE.FindInformationAcademiesTrusts.Domain.Common;
using DfE.FindInformationAcademiesTrusts.Domain.Enums;

namespace DfE.FindInformationAcademiesTrusts.Domain.Entities;

public class TrustContact : BaseEntity
{
    public int Id { get; set; }
    public required int Uid { get; set; }
    public required TrustContactRole Role { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    
    
}
