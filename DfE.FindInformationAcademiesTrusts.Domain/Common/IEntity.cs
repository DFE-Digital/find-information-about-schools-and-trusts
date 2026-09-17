namespace DfE.FindInformationAcademiesTrusts.Domain.Common
{
    public interface IEntity<out TId> : IAuditableEntity where TId : IStronglyTypedId
    {
        TId? Id { get; }
    }
}
