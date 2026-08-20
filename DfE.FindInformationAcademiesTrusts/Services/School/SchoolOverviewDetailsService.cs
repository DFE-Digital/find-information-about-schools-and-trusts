using DfE.FindInformationAcademiesTrusts.Data.Repositories.School;

namespace DfE.FindInformationAcademiesTrusts.Services.School;

public interface ISchoolOverviewDetailsService
{
    Task<SchoolOverviewServiceModel> GetSchoolOverviewDetailsAsync(int urn);
}

public class SchoolOverviewDetailsService(ISchoolRepository schoolRepository) : ISchoolOverviewDetailsService
{
    public async Task<SchoolOverviewServiceModel> GetSchoolOverviewDetailsAsync(int urn)
    {
        var schoolDetails = await schoolRepository.GetSchoolDetailsAsync(urn);

        var nurseryProvision = GetNurseryProvision(schoolDetails.NurseryProvision);

        var overviewModel = new SchoolOverviewServiceModel(schoolDetails.Name, schoolDetails.Address,
            schoolDetails.Region, schoolDetails.LocalAuthority, schoolDetails.PhaseOfEducationName,
            schoolDetails.AgeRange, nurseryProvision, schoolDetails.TrustName, schoolDetails.DateJoinedTrust);

        return overviewModel;
    }

    public static NurseryProvision GetNurseryProvision(string? nurseryProvisionString)
    {
        return nurseryProvisionString?.ToLower() switch
        {
            "has nursery classes" => NurseryProvision.HasClasses,
            "no nursery classes" => NurseryProvision.NoClasses,
            _ => NurseryProvision.NotRecorded
        };
    }
}
