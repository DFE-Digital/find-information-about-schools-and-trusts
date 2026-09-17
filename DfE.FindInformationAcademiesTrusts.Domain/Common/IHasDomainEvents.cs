namespace DfE.FindInformationAcademiesTrusts.Domain.Common

{
    public interface IHasDomainEvents
    {
        IReadOnlyCollection<IDomainEvent> DomainEvents { get; }
        void ClearDomainEvents();
    }
}
