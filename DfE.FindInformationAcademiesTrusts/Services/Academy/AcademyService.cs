using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.Academy;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.Ofsted;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.PipelineAcademy;

namespace DfE.FindInformationAcademiesTrusts.Services.Academy;

public interface IAcademyService
{
    Task<AcademyDetailsServiceModel[]> GetAcademiesInTrustDetailsAsync(string uid);
    Task<AcademyOfstedServiceModel[]> GetAcademiesInTrustOfstedAsync(string uid);
    Task<AcademyPupilNumbersServiceModel[]> GetAcademiesInTrustPupilNumbersAsync(string uid);
    Task<AcademyFreeSchoolMealsServiceModel[]> GetAcademiesInTrustFreeSchoolMealsAsync(string uid);
    Task<AcademyPipelineSummaryServiceModel> GetAcademiesPipelineSummaryAsync(string trustReferenceNumber);
    Task<AcademyPipelineServiceModel[]> GetAcademiesPipelinePreAdvisoryAsync(string trustReferenceNumber);
    Task<AcademyPipelineServiceModel[]> GetAcademiesPipelinePostAdvisoryAsync(string trustReferenceNumber);
    Task<AcademyPipelineServiceModel[]> GetAcademiesPipelineFreeSchoolsAsync(string trustReferenceNumber);
}

public class AcademyService(
    IAcademyRepository academyRepository,
    IOfstedRepository ofstedRepository,
    IPipelineEstablishmentRepository pipelineEstablishmentRepository,
    IFreeSchoolMealsAverageProvider freeSchoolMealsAverageProvider) : IAcademyService
{
    public async Task<AcademyDetailsServiceModel[]> GetAcademiesInTrustDetailsAsync(string uid)
    {
        var academies = await academyRepository.GetAcademiesInTrustDetailsAsync(uid);

        return academies.Select(a =>
            new AcademyDetailsServiceModel(a.Urn, a.EstablishmentName, a.LocalAuthority, a.TypeOfEstablishment,
                a.UrbanRural?.Replace("(England/Wales) ", ""), a.DateAcademyJoinedTrust)).ToArray();
    }

    public async Task<AcademyOfstedServiceModel[]> GetAcademiesInTrustOfstedAsync(string uid)
    {
        var academies = await ofstedRepository.GetAcademiesInTrustOfstedAsync(uid);

        return academies.Select(a =>
            new AcademyOfstedServiceModel(a.Urn, a.EstablishmentName, a.DateAcademyJoinedTrust, a.ShortInspection,
                a.PreviousOfstedRating, a.CurrentOfstedRating)).ToArray();
    }

    public async Task<AcademyPupilNumbersServiceModel[]> GetAcademiesInTrustPupilNumbersAsync(string uid)
    {
        var academies = await academyRepository.GetAcademiesInTrustPupilNumbersAsync(uid);

        return academies.Select(a =>
            new AcademyPupilNumbersServiceModel(a.Urn, a.EstablishmentName, a.PhaseOfEducation,
                a.AgeRange, a.NumberOfPupils,
                a.SchoolCapacity)).ToArray();
    }

    public async Task<AcademyFreeSchoolMealsServiceModel[]> GetAcademiesInTrustFreeSchoolMealsAsync(string uid)
    {
        var academies = await academyRepository.GetAcademiesInTrustFreeSchoolMealsAsync(uid);

        return academies.Select(a =>
                new AcademyFreeSchoolMealsServiceModel(
                    a.Urn,
                    a.EstablishmentName,
                    a.PercentageFreeSchoolMeals,
                    freeSchoolMealsAverageProvider.GetLaAverage(a.LocalAuthorityCode, a.PhaseOfEducation,
                        a.TypeOfEstablishment),
                    freeSchoolMealsAverageProvider.GetNationalAverage(a.PhaseOfEducation, a.TypeOfEstablishment)))
            .ToArray();
    }

    public async Task<AcademyPipelineSummaryServiceModel> GetAcademiesPipelineSummaryAsync(string trustReferenceNumber)
    {
        var repoSummary = await pipelineEstablishmentRepository.GetAcademiesPipelineSummaryAsync(trustReferenceNumber);

        return new AcademyPipelineSummaryServiceModel(
            repoSummary.PreAdvisoryCount,
            repoSummary.PostAdvisoryCount,
            repoSummary.FreeSchoolsCount
        );
    }

    public async Task<AcademyPipelineServiceModel[]> GetAcademiesPipelinePreAdvisoryAsync(string trustReferenceNumber)
    {
        var advisoryConversions =
            await pipelineEstablishmentRepository.GetAdvisoryConversionEstablishmentsAsync(trustReferenceNumber,
                AdvisoryType.PreAdvisory);

        var advisoryTransfers =
            await pipelineEstablishmentRepository.GetAdvisoryTransferEstablishmentsAsync(trustReferenceNumber,
                AdvisoryType.PreAdvisory);

        var preAdvisoryEstablishments = advisoryConversions.Concat(advisoryTransfers);

        return preAdvisoryEstablishments.Select(fs => new AcademyPipelineServiceModel(
            fs.Urn,
            fs.EstablishmentName,
            fs.AgeRange,
            fs.LocalAuthority,
            fs.ProjectType,
            fs.ChangeDate
        )).ToArray();
    }

    public async Task<AcademyPipelineServiceModel[]> GetAcademiesPipelinePostAdvisoryAsync(string trustReferenceNumber)
    {
        var advisoryConversions =
            await pipelineEstablishmentRepository.GetAdvisoryConversionEstablishmentsAsync(trustReferenceNumber,
                AdvisoryType.PostAdvisory);

        var advisoryTransfers =
            await pipelineEstablishmentRepository.GetAdvisoryTransferEstablishmentsAsync(trustReferenceNumber,
                AdvisoryType.PostAdvisory);

        var postAdvisoryEstablishments = advisoryConversions.Concat(advisoryTransfers);

        return postAdvisoryEstablishments.Select(fs => new AcademyPipelineServiceModel(
            fs.Urn,
            fs.EstablishmentName,
            fs.AgeRange,
            fs.LocalAuthority,
            fs.ProjectType,
            fs.ChangeDate
        )).ToArray();
    }

    public async Task<AcademyPipelineServiceModel[]> GetAcademiesPipelineFreeSchoolsAsync(string trustReferenceNumber)
    {
        var freeSchools =
            await pipelineEstablishmentRepository.GetPipelineFreeSchoolProjectsAsync(trustReferenceNumber);

        return freeSchools.Select(fs => new AcademyPipelineServiceModel(
            fs.Urn,
            fs.EstablishmentName,
            fs.AgeRange,
            fs.LocalAuthority,
            fs.ProjectType,
            fs.ChangeDate
        )).ToArray();
    }
}
