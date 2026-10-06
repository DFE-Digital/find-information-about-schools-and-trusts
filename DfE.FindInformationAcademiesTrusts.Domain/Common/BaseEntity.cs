namespace DfE.FindInformationAcademiesTrusts.Domain.Common;

public class BaseEntity
{
    public string LastModifiedByName { get; set; } = null!;
    public string LastModifiedByEmail { get; set; } = null!;
    public DateTime LastModifiedAtTime { get; set; }
}
