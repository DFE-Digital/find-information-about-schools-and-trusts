using MediatR;

namespace DfE.FindInformationAcademiesTrusts.Domain.Common
{
    public interface IDomainEvent : INotification
    {
        DateTime OccurredOn { get; }
    }
}
