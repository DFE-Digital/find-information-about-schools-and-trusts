using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.AcademiesDbServices;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Extensions;
using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Data.Repositories;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.School;
using DfE.FindInformationAcademiesTrusts.HttpServices;
using GovUK.Dfe.PersonsApi.Client.Contracts;
using Microsoft.Extensions.Caching.Memory;

namespace DfE.FindInformationAcademiesTrusts.Services.School;

public interface ISchoolService
{
    Task<SchoolSummaryServiceModel?> GetSchoolSummaryAsync(int urn);

    Task<bool> IsPartOfFederationAsync(int urn);

    Task<SchoolReferenceNumbersServiceModel> GetReferenceNumbersAsync(int urn);

    Task<SchoolGovernanceServiceModel> GetSchoolGovernanceAsync(int urn);

    Task<SchoolReligiousCharacteristicsServiceModel> GetReligiousCharacteristicsAsync(int urn);
}

public class SchoolService(
    IMemoryCache memoryCache,
    IGetEstablishmentsTemp getEstablishments,
    ISchoolRepository schoolRepository,
    IEstablishmentsClient establishmentsClient) : ISchoolService
{
    public async Task<bool> IsPartOfFederationAsync(int urn)
    {
        return await schoolRepository.IsPartOfFederationAsync(urn);
    }

    public async Task<SchoolSummaryServiceModel?> GetSchoolSummaryAsync(int urn)
    {
        var cacheKey = $"{nameof(GetSchoolSummaryAsync)}:{urn}";

        if (memoryCache.TryGetValue(cacheKey, out SchoolSummaryServiceModel? cachedTrustSummary))
        {
            return cachedTrustSummary;
        }
        
        var result = await getEstablishments.GetEstablishment(urn);

        var summary = new SchoolSummaryServiceModel(
            urn,
            result.Name,
            result.EstablishmentType.Name,
            result.EstablishmentGroupType.Name == "Academies"
                ? SchoolCategory.Academy
                : SchoolCategory.LaMaintainedSchool);

        memoryCache.Set(cacheKey, summary,
            new MemoryCacheEntryOptions { SlidingExpiration = TimeSpan.FromMinutes(10) });

        return summary;
    }

    public async Task<SchoolReferenceNumbersServiceModel> GetReferenceNumbersAsync(int urn)
    {
        var result = await getEstablishments.GetEstablishment(urn);
        var ukprn = result.Ukprn;

        var laestab = result.LocalAuthorityCode is not null && result.EstablishmentNumber is not null
            ? $"{result.LocalAuthorityCode}/{result.EstablishmentNumber}"
            : null;

        return new SchoolReferenceNumbersServiceModel(urn, laestab, ukprn);
    }

    public async Task<SchoolGovernanceServiceModel> GetSchoolGovernanceAsync(int urn)
    {
        var result = await establishmentsClient.GetAllPersonsAssociatedWithAcademyByUrnAsync(urn);
        
        var governors = new List<Governor>();

        foreach (var person in result)
        {
            governors.Add(new Governor(
                FullName: person.DisplayName,
                Role: person.Roles[0],
                AppointingBody: person.AppointingBody,
                DateOfAppointment: person.DateOfAppointment.ParseAsNullableDate(),
                DateOfTermEnd: person.DateTermOfOfficeEndsEnded.ParseAsNullableDate(),
                Email: person.Email));
        }

        return new SchoolGovernanceServiceModel(
            governors.Where(x => x.IsCurrentOrFutureGovernor).ToArray(),
            governors.Where(x => !x.IsCurrentOrFutureGovernor).ToArray());
    }

    public async Task<SchoolReligiousCharacteristicsServiceModel> GetReligiousCharacteristicsAsync(int urn)
    {
        var religiousCharacteristics = await schoolRepository.GetReligiousCharacteristicsAsync(urn);

        return new SchoolReligiousCharacteristicsServiceModel(
            GetCharacteristicsValue(nameof(religiousCharacteristics.ReligiousAuthority),
                religiousCharacteristics.ReligiousAuthority),
            GetCharacteristicsValue(nameof(religiousCharacteristics.ReligiousCharacter),
                religiousCharacteristics.ReligiousCharacter),
            GetCharacteristicsValue(nameof(religiousCharacteristics.ReligiousEthos),
                religiousCharacteristics.ReligiousEthos));
    }

    private static string GetCharacteristicsValue(string field, string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return "Not available";
        }

        if (field == nameof(ReligiousCharacteristics.ReligiousAuthority) &&
            value.Equals("Not applicable", StringComparison.CurrentCultureIgnoreCase))
        {
            return "Does not apply";
        }

        return value;
    }
}
