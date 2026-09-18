using DfE.FindInformationAcademiesTrusts.Domain.Common;

namespace DfE.FindInformationAcademiesTrusts.Domain.ValueObjects

{
    public record WatchlistId(Guid Value) : IStronglyTypedId;
}