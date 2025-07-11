using System.Diagnostics.CodeAnalysis;
using DfE.FindInformationAcademiesTrusts.Data.Repositories;

namespace DfE.FindInformationAcademiesTrusts.Services.Trust;

[ExcludeFromCodeCoverage]
public record TrustGovernanceServiceModel(
    Governor[] CurrentTrustLeadership,
    Governor[] CurrentMembers,
    Governor[] CurrentTrustees,
    Governor[] HistoricMembers,
    decimal TurnoverRate);
