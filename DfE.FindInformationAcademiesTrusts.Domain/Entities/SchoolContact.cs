using DfE.FindInformationAcademiesTrusts.Domain.Common;
using DfE.FindInformationAcademiesTrusts.Domain.Enums;

namespace DfE.FindInformationAcademiesTrusts.Domain.Entities;

public class SchoolContact : BaseEntity
{
    public int Id { get; set; }
    public required int Urn { get; set; }
    public required SchoolContactRole Role { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
}
