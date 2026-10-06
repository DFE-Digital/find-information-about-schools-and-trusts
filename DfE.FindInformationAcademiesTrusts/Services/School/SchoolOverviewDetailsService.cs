using System.Globalization;
using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.School;
using DfE.FindInformationAcademiesTrusts.HttpServices;

namespace DfE.FindInformationAcademiesTrusts.Services.School;

public interface ISchoolOverviewDetailsService
{
    Task<SchoolOverviewServiceModel> GetSchoolOverviewDetailsAsync(int urn);
}

public class SchoolOverviewDetailsService(IStringFormattingUtilities stringFormattingUtilities, IGetEstablishmentsTemp getEstablishments) : ISchoolOverviewDetailsService
{
    public async Task<SchoolOverviewServiceModel> GetSchoolOverviewDetailsAsync(int urn)
    {
        // var schoolDetails = await schoolRepository.GetSchoolDetailsAsync(urn);
        var schoolDetails = await getEstablishments.GetEstablishment(urn);
        
        DateTime? dateJoinedTrust = null;
        
        if (!string.IsNullOrEmpty(schoolDetails.DateJoinedTrust) &&
            DateTime.TryParseExact(
                schoolDetails.DateJoinedTrust,
                "dd/MM/yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var parsedDate))
        {
            dateJoinedTrust = parsedDate;
        }

        var nurseryProvision = GetNurseryProvision(schoolDetails.NurseryProvision);
        var address = stringFormattingUtilities.BuildAddressString(
            schoolDetails.Address.Street,
            schoolDetails.Address.Locality,
            schoolDetails.Address.Town,
            schoolDetails.Address.Postcode);

        var overviewModel = new SchoolOverviewServiceModel(schoolDetails.Name, address,
            schoolDetails.Gor.Name, schoolDetails.LocalAuthorityName, schoolDetails.PhaseOfEducation.Name,
            new AgeRange(schoolDetails.StatutoryLowAge, schoolDetails.StatutoryHighAge), nurseryProvision, schoolDetails.TrustName, dateJoinedTrust);

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
