
namespace DfE.FindInformationAcademiesTrusts.Domain.Common
{
    public interface IAuditableEntity
   {
        DateTime CreatedOn { get; set; }
        string CreatedBy { get; set; }
        DateTime? LastModifiedOn { get; set; }
        string? LastModifiedBy { get; set; }
    }
}
