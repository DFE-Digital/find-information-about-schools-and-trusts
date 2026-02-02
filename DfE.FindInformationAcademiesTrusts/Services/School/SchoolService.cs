using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.Ofsted;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.School;
using DfE.FindInformationAcademiesTrusts.Extensions;
using DfE.FindInformationAcademiesTrusts.Services.Academy;
using DfE.FindInformationAcademiesTrusts.Services.Ofsted;
using Microsoft.Extensions.Caching.Memory;

namespace DfE.FindInformationAcademiesTrusts.Services.School;

public interface ISchoolService
{
    Task<SchoolSummaryServiceModel?> GetSchoolSummaryAsync(int urn);

    Task<bool> IsPartOfFederationAsync(int urn);

    Task<SchoolReferenceNumbersServiceModel> GetReferenceNumbersAsync(int urn);

    Task<SchoolGovernanceServiceModel> GetSchoolGovernanceAsync(int urn);

    Task<OfstedHeadlineGradesServiceModel> GetOfstedHeadlineGrades(int urn);

    Task<OfstedOverviewInspectionServiceModel> GetOfstedOverviewInspection(int urn);

    Task<SchoolOfstedServiceModel> GetSchoolOfstedRatingsAsync(int urn);

    Task<SchoolOfstedServiceModel> GetSchoolOfstedRatingsAsBeforeAndAfterSeptemberGradeAsync(int urn);

    Task<SchoolReligiousCharacteristicsServiceModel> GetReligiousCharacteristicsAsync(int urn);
}

public class SchoolService(
    IMemoryCache memoryCache,
    ISchoolRepository schoolRepository,
    IOfstedRepository ofstedRepository,
    IReportCardsService reportCardsService) : ISchoolService
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

        var summary = await schoolRepository.GetSchoolSummaryAsync(urn);

        if (summary is null)
        {
            return null;
        }

        var schoolSummaryServiceModel =
            new SchoolSummaryServiceModel(urn, summary.Name, summary.Type, summary.Category);

        memoryCache.Set(cacheKey, schoolSummaryServiceModel,
            new MemoryCacheEntryOptions { SlidingExpiration = TimeSpan.FromMinutes(10) });

        return schoolSummaryServiceModel;
    }

    public async Task<SchoolReferenceNumbersServiceModel> GetReferenceNumbersAsync(int urn)
    {
        var referenceNumbers = await schoolRepository.GetReferenceNumbersAsync(urn);

        if (referenceNumbers is null) return new SchoolReferenceNumbersServiceModel(urn);

        var laestab = referenceNumbers.LaCode is not null && referenceNumbers.EstablishmentNumber is not null
            ? $"{referenceNumbers.LaCode}/{referenceNumbers.EstablishmentNumber}"
            : null;

        return new SchoolReferenceNumbersServiceModel(urn, laestab, referenceNumbers.Ukprn);
    }

    public async Task<SchoolGovernanceServiceModel> GetSchoolGovernanceAsync(int urn)
    {
        var governance = await schoolRepository.GetGovernanceAsync(urn);

        return new SchoolGovernanceServiceModel(
            governance.Where(x => x.IsCurrentOrFutureGovernor).ToArray(),
            governance.Where(x => !x.IsCurrentOrFutureGovernor).ToArray());
    }

    public async Task<OfstedHeadlineGradesServiceModel> GetOfstedHeadlineGrades(int urn)
    {
        var shortInspection = await ofstedRepository.GetOfstedShortInspectionAsync(urn);
        var inspectionHistorySummary = await ofstedRepository.GetOfstedInspectionHistorySummaryAsync(urn);

        return new OfstedHeadlineGradesServiceModel(shortInspection, inspectionHistorySummary.CurrentInspection,
            inspectionHistorySummary.PreviousInspection);
    }


    public async Task<SchoolOfstedServiceModel> GetSchoolOfstedRatingsAsync(int urn)
    {
        var schoolOfstedRatings = await ofstedRepository.GetSchoolOfstedRatingsAsync(urn);

        return new SchoolOfstedServiceModel(
            schoolOfstedRatings.Urn,
            schoolOfstedRatings.EstablishmentName,
            schoolOfstedRatings.DateAcademyJoinedTrust,
            schoolOfstedRatings.ShortInspection,
            schoolOfstedRatings.PreviousOfstedRating,
            schoolOfstedRatings.CurrentOfstedRating,
            schoolOfstedRatings.IsFurtherEducationalEstablishment
        );
    }

    public async Task<OfstedOverviewInspectionServiceModel> GetOfstedOverviewInspection(int urn)
    {
        var schoolOfstedRatings = await ofstedRepository.GetSchoolOfstedRatingsAsync(urn);
        var reportCards = await reportCardsService.GetReportCardsAsync(urn);

        var (before, after) = GetClassicBeforeAndAfter(schoolOfstedRatings);

        List<OverviewServiceModel> overviewModels = [];
        
        AddReportCard(reportCards.LatestReportCard);
        AddReportCard(reportCards.PreviousReportCard);
        AddOfstedRating(after);
        AddOfstedRating(before);


        void AddOfstedRating(OfstedRating? rating)
        {
            if (rating is { InspectionDate: not null })
            {
                overviewModels.Add(new OverviewServiceModel
                {
                    IsReportCard = false,
                    InspectionDate = DateOnly.FromDateTime(rating.InspectionDate.Value),
                    BeforeOrAfterJoining = schoolOfstedRatings.DateAcademyJoinedTrust
                        .GetBeforeOrAfterJoiningTrust(rating.InspectionDate)
                });
            }
        }

        void AddReportCard(ReportCardDetails? reportCard)
        {
            if (reportCard != null)
            {
                overviewModels.Add(new OverviewServiceModel
                {
                    IsReportCard = true,
                    InspectionDate = reportCard.InspectionDate,
                    BeforeOrAfterJoining = schoolOfstedRatings.DateAcademyJoinedTrust
                        .GetBeforeOrAfterJoiningTrust(reportCard.InspectionDate)
                });
            }
        }

        overviewModels = overviewModels.OrderByDescending(x => x.InspectionDate).ToList();

        ShortInspectionOverviewServiceModel? shortInspectionModel = GetShortInspectionModel(schoolOfstedRatings.ShortInspection, schoolOfstedRatings.DateAcademyJoinedTrust);

        return new OfstedOverviewInspectionServiceModel(overviewModels.FirstOrDefault(), overviewModels.Skip(1).FirstOrDefault(), shortInspectionModel);
    }

    private static ShortInspectionOverviewServiceModel? GetShortInspectionModel(OfstedShortInspection ofstedShortInspection, DateTime? dateAcademyJoinedTrust)
    {
        if (ofstedShortInspection.InspectionDate is null)
        {
            return null;
        }

        return new ShortInspectionOverviewServiceModel
        {
            IsReportCard = false,
            InspectionDate = DateOnly.FromDateTime(ofstedShortInspection.InspectionDate.Value),
            BeforeOrAfterJoining =
                dateAcademyJoinedTrust.GetBeforeOrAfterJoiningTrust(ofstedShortInspection.InspectionDate),
            InspectionOutcome = ofstedShortInspection.ToOutcomeDisplayString()
        };
    }


    private static (OfstedRating? before, OfstedRating? after) GetClassicBeforeAndAfter(SchoolOfsted schoolOfstedRatings)
    {
        DateTime cutOffDate = new(2024, 09, 01, 23, 59, 59, DateTimeKind.Unspecified);

        List<OfstedRating> ofstedRatings =
            [schoolOfstedRatings.CurrentOfstedRating, schoolOfstedRatings.PreviousOfstedRating];

        ofstedRatings = ofstedRatings.OrderByDescending(x => x.InspectionDate).ToList();

        var after = ofstedRatings.FirstOrDefault(x => x.InspectionDate > cutOffDate);
        var before = ofstedRatings.FirstOrDefault(x => x.InspectionDate <= cutOffDate);

        return (before, after);
    }

    public async Task<SchoolOfstedServiceModel> GetSchoolOfstedRatingsAsBeforeAndAfterSeptemberGradeAsync(int urn)
    {
        var schoolOfstedRatings = await ofstedRepository.GetSchoolOfstedRatingsAsync(urn);

        var (before, after) = GetClassicBeforeAndAfter(schoolOfstedRatings);

        return new SchoolOfstedServiceModel(
            schoolOfstedRatings.Urn,
            schoolOfstedRatings.EstablishmentName,
            schoolOfstedRatings.DateAcademyJoinedTrust,
            schoolOfstedRatings.ShortInspection,
            before,
            after,
            schoolOfstedRatings.IsFurtherEducationalEstablishment
        );
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
