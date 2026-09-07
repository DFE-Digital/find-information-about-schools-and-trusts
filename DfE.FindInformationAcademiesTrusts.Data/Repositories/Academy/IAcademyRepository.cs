namespace DfE.FindInformationAcademiesTrusts.Data.Repositories.Academy;

public interface IAcademyRepository
{
    Task<string?> GetSingleAcademyTrustAcademyUrnAsync(string uid);
    Task<int> GetNumberOfAcademiesInTrustAsync(string referenceNumber);
    Task<AcademyDetails[]> GetAcademiesInTrustDetailsAsync(string uid);

    Task<AcademyFreeSchoolMeals[]> GetAcademiesInTrustFreeSchoolMealsAsync(string referenceNumber);
    Task<AcademyOverview[]> GetOverviewOfAcademiesInTrustAsync(string referenceNumber);
    Task<string?> GetTrustUidFromAcademyUrnAsync(int urn);

    Task<AcademyPupilNumbers[]> GetAcademiesInTrustPupilNumbersByTrnAsync(string referenceNumber);
}
