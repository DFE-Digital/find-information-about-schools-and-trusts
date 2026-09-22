using System.ComponentModel.DataAnnotations.Schema;

namespace DfE.FindInformationAcademiesTrusts.Domain.Common
{
    public abstract class BaseAggregateRoot : IAggregateRoot
    {
        private readonly List<IDomainEvent> _domainEvents = [];

        [NotMapped]
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        protected virtual void AddDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        protected virtual void RemoveDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents.Remove(domainEvent);
        }

        public virtual void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }
    }
}
