using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Contexts;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Extensions;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.Academy;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.AcademiesDbServices;

namespace DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Repositories;

public class AcademyRepository(IAcademiesDbContext academiesDbContext, IGetEstablishments getEstablishments)
    : IAcademyRepository
{
    public async Task<AcademyDetails[]> GetAcademiesInTrustDetailsAsync(string uid)
    {
        return await academiesDbContext.GiasGroupLinks
            .Where(gl => gl.GroupUid == uid)
            .Join(academiesDbContext.GiasEstablishments,
                gl => gl.Urn!, e => e.Urn.ToString(),
                (gl, e) =>
                    new AcademyDetails(e.Urn.ToString(),
                        e.EstablishmentName,
                        e.TypeOfEstablishmentName,
                        e.LaName,
                        e.UrbanRuralName,
                        DateOnly.ParseExact(gl.JoinedDate!, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None)))
            .ToArrayAsync();
    }
    
    public async Task<AcademyPupilNumbers[]> GetAcademiesInTrustPupilNumbersByTrnAsync(string referenceNumber)
    {
        var details = await getEstablishments.GetEstablishmentsByTrustReferenceNumber(referenceNumber);

        return details
            .Select(e => new AcademyPupilNumbers(
                e.Urn.ToString(),
                e.Name,
                e.PhaseOfEducation.Name,
                new AgeRange(e.StatutoryLowAge, e.StatutoryHighAge),
                e.Census.NumberOfPupils.ParseAsNullableInt(),
                e.SchoolCapacity.ParseAsNullableInt()))
            .ToArray();
    }

    public async Task<AcademyFreeSchoolMeals[]> GetAcademiesInTrustFreeSchoolMealsAsync(string referenceNumber)
    {
        var result = await getEstablishments.GetEstablishmentsByTrustReferenceNumber(referenceNumber);
        
        return result
            .Select(e => new AcademyFreeSchoolMeals(
                e.Urn.ToString(),
                e.Name,
                e.Census.PercentageFsm.ParseAsNullableDouble(),
                e.LocalAuthorityCode.ParseAsNullableInt(),
                e.EstablishmentType.Name,
                e.PhaseOfEducation.Name))
            .ToArray();
    }

    public async Task<int> GetNumberOfAcademiesInTrustAsync(string referenceNumber)
    {
        var result =  await getEstablishments.GetEstablishmentsByTrustReferenceNumber(referenceNumber);
        return result.Count();
    }

    public async Task<string?> GetSingleAcademyTrustAcademyUrnAsync(string uid)
    {
        return await academiesDbContext.GiasGroupLinks.SingleAcademyTrusts()
            .Where(gl => gl.GroupUid == uid)
            .Select(gl => gl.Urn)
            .FirstOrDefaultAsync();
    }

    public async Task<AcademyOverview[]> GetOverviewOfAcademiesInTrustAsync(string referenceNumber)
    {
        var result = await getEstablishments.GetEstablishmentsByTrustReferenceNumber(referenceNumber);

        return result.Select(e => new AcademyOverview(
                e.Urn.ToString(),
                e.LocalAuthorityName,
                e.Census.NumberOfPupils.ParseAsNullableInt(),
                e.SchoolCapacity.ParseAsNullableInt()
            )).ToArray();
    }
}
