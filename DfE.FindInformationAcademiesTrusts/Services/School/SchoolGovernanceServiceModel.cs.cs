using DfE.FindInformationAcademiesTrusts.Data.Repositories;

namespace DfE.FindInformationAcademiesTrusts.Services.School;

public record SchoolGovernanceServiceModel(Governor[] Current, Governor[] Historic);
