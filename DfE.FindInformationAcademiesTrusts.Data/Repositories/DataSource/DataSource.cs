using DfE.FindInformationAcademiesTrusts.Data.Enums;

namespace DfE.FindInformationAcademiesTrusts.Data.Repositories.DataSource;

public record DataSource(Source Source, DateTime? LastUpdated, UpdateFrequency? NextUpdated, string? UpdatedBy = null);
