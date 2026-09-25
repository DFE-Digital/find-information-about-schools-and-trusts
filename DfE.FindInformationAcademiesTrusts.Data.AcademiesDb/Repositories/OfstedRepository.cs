using System.Globalization;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.AcademiesDbServices;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Contexts;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Extensions;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.Ofsted;
using GovUK.Dfe.AcademiesApi.Client.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Repositories;

public class OfstedRepository(
    IAcademiesDbContext academiesDbContext,
    IGetEstablishments getEstablishments,
    ILogger<AcademyRepository> logger)
    : IOfstedRepository
{
    private static readonly DateTime
        SingleHeadlineGradesPolicyChangeDate = new(2024, 09, 02, 0, 0, 0, DateTimeKind.Utc);

    public async Task<SchoolOfsted[]> GetAcademiesInTrustOfstedAsync(string trustReferenceNumber)
    {
        var academies = await getEstablishments.GetEstablishmentsByTrustReferenceNumber(trustReferenceNumber);
        var academiesData = academies.Select(a => new
            {
                a.Urn,
                a.Name,
                a.DateJoinedTrust
            })
            .ToList();

        var ofstedRatings = await GetOfstedRatings(academiesData.Select(a => a.Urn).ToArray());

        var academyOfsteds = academiesData.Select(a =>
                new SchoolOfsted(a.Urn,
                    a.Name,
                    DateTime.ParseExact(a.DateJoinedTrust!, "dd/MM/yyyy", CultureInfo.InvariantCulture),
                    ofstedRatings[a.Urn].ShortInspection,
                    ofstedRatings[a.Urn].Previous,
                    ofstedRatings[a.Urn].Current,
                    ofstedRatings[a.Urn].IsFurtherEducationalEstablishment
                ))
            .ToArray();

        return academyOfsteds;
    }

    public async Task<OfstedInspectionHistorySummary> GetOfstedInspectionHistorySummaryAsync(int urn)
    {
        var urnString = urn.ToString();
        var rating = (await GetOfstedRatings([urnString]))[urnString];

        var currentFullInspection =
            new OfstedFullInspectionSummary(rating.Current.InspectionDate, rating.Current.OverallEffectiveness);
        var previousFullInspection =
            new OfstedFullInspectionSummary(rating.Previous.InspectionDate, rating.Previous.OverallEffectiveness);

        return new OfstedInspectionHistorySummary(currentFullInspection, previousFullInspection);
    }

    public async Task<OfstedShortInspection> GetOfstedShortInspectionAsync(int urn)
    {
        return await academiesDbContext.MisMstrEstablishmentsFiat.Where(est => est.Urn == urn).Select(est =>
            new OfstedShortInspection(est.DateOfLatestSection8Inspection.ParseAsNullableDate(),
                est.Section8InspectionOverallOutcome)).FirstOrDefaultAsync() ?? OfstedShortInspection.Unknown;
    }

    public async Task<SchoolOfsted> GetSchoolOfstedRatingsAsync(int urn)
    {
        var urnString = urn.ToString();

        var result = await GetOfstedRatings([urnString]);
        var ofstedRatings = result[urnString];

        var schoolDetails = await getEstablishments.GetEstablishment(urn);

        return new SchoolOfsted(
            urnString,
            schoolDetails.Name,
            schoolDetails.DateJoinedTrust.ParseAsNullableDate(),
            ofstedRatings.ShortInspection,
            ofstedRatings.Previous,
            ofstedRatings.Current,
            ofstedRatings.IsFurtherEducationalEstablishment
        );
    }

    private async Task<Dictionary<string, AcademyOfstedRatings>> GetOfstedRatings(string[] urns)
    {
        // First pass at getting ofsted ratings from the db
        var allOfstedRatings = await GetOfstedRatingsForTrust(urns);

        // If any missing then this could be a school that has recently changed URN
        // try to get ofsted rating using predecessor URN
        var missingUrns = urns.Except(allOfstedRatings.Keys).ToArray();
        if (missingUrns.Length > 0)
        {
            var previousUrnMapping = await GetPredecessorUrns(missingUrns);
            
            var oldOfstedRatings = await GetOfstedRatingsForTrust(previousUrnMapping);

            allOfstedRatings = allOfstedRatings.Concat(oldOfstedRatings).ToDictionary();
        }

        // Validate that all the ofsted ratings are found and valid
        foreach (var urn in urns)
        {
            allOfstedRatings.TryGetValue(urn, out var foundRating);

            // Log any URNs that couldn't be found and default to unknown
            if (foundRating is null)
            {
                logger.LogError(
                    "URN {Urn} was not found in Mis.Establishments or Mis.FurtherEducationEstablishments. This indicates a data integrity issue with the Ofsted data in Academies Db.",
                    urn);
                allOfstedRatings.Add(urn,
                    new AcademyOfstedRatings(int.Parse(urn), OfstedShortInspection.Unknown, OfstedRating.Unknown,
                        OfstedRating.Unknown, false));
                continue;
            }

            //Log any errors that occured during parsing
            if (foundRating.Current.HasAnyUnknownRating || foundRating.Previous.HasAnyUnknownRating)
            {
                logger.LogError(
                    "URN {Urn} has some unrecognised ofsted ratings. This could be a data integrity issue with the Ofsted data in Academies Db.",
                    urn);
            }

            // Ensure that there are no current single headline grades after policy change on 2nd September 2024 - only applies to non-further ed establishments
            if (!foundRating.IsFurtherEducationalEstablishment && HasShgIssuedAfterPolicyChange(foundRating.Current))
            {
                logger.LogError(
                    "URN {Urn} has a current Ofsted single headline grade of {score} issued on {InspectionDate} which was after single headline grades stopped being issued on 2nd September. This could be a data integrity issue with the Ofsted data in Academies Db.",
                    urn, foundRating.Current.OverallEffectiveness, foundRating.Current.InspectionDate);

                allOfstedRatings[urn] = foundRating with
                {
                    Current = foundRating.Current with
                    {
                        OverallEffectiveness = OfstedRatingScore.SingleHeadlineGradeNotAvailable
                    }
                };
            }

            // Ensure that there are no previous single headline grades after policy change on 2nd September 2024 - only applies to non-further ed establishments
            if (!foundRating.IsFurtherEducationalEstablishment && HasShgIssuedAfterPolicyChange(foundRating.Previous))
            {
                logger.LogError(
                    "URN {Urn} has a previous Ofsted single headline grade of {score} issued on {InspectionDate} which was after single headline grades stopped being issued on 2nd September. This could be a data integrity issue with the Ofsted data in Academies Db.",
                    urn, foundRating.Previous.OverallEffectiveness, foundRating.Previous.InspectionDate);

                allOfstedRatings[urn] = foundRating with
                {
                    Previous = foundRating.Previous with
                    {
                        OverallEffectiveness = OfstedRatingScore.SingleHeadlineGradeNotAvailable
                    }
                };
            }
        }

        return allOfstedRatings;
    }

    /// <summary>
    /// Attempts to find previous urns for given urns
    /// If more than one match is found then no urn is returned as we don't know which one (if any) the old Ofsted report is against
    /// </summary>
    /// <param name="currentUrns"></param>
    /// <returns>Key: Previous URN, Value: Current URN</returns>
    private async Task<Dictionary<int, int>> GetPredecessorUrns(string[] currentUrns)
    {
        var currentUrnIds = currentUrns.Select(int.Parse).ToList();
        var schools = await getEstablishments.GetEstablishmentsByUrns(currentUrnIds);

        return schools
            .Where(s => s.PreviousEstablishment?.Urn is not null && currentUrns.Contains(s.Urn))
            .GroupBy(s => Convert.ToInt32(s.Urn))
            .Where(group => group.Count() == 1)
            .ToDictionary(
                group => Convert.ToInt32(group.Single().PreviousEstablishment!.Urn),
                group => group.Key);
    }

    private async Task<Dictionary<string, AcademyOfstedRatings>> GetOfstedRatingsForTrust(string[] urns)
    {
        var parsedUrns = urns.Select(u => int.Parse(u)).ToArray();
        var establishments = await getEstablishments.GetEstablishmentsWithOfstedData(parsedUrns);

        return CreateOfstedRatings(establishments, parsedUrns);
    }

    /// <param name="urnMapping">Key: URN to search by, Value: URN to return</param>
    private async Task<Dictionary<string, AcademyOfstedRatings>> GetOfstedRatingsForTrust(Dictionary<int, int> urnMapping)
    {
        var urns = urnMapping.Keys.ToArray();
        var establishments = await getEstablishments.GetEstablishmentsWithOfstedData(urns);

        var ofstedRatings = CreateOfstedRatings(establishments, urns);

        return ofstedRatings.ToDictionary(
            rating => urnMapping[int.Parse(rating.Key)].ToString(),
            rating => rating.Value);
    }
    
    private static Dictionary<string, AcademyOfstedRatings> CreateOfstedRatings(List<EstablishmentResponse> establishments, int[] urns)
    {
        establishments ??= [];

        var ofstedRatings = establishments
            .Where(e => e.MisEstablishment is not null)
            .Select(e => new AcademyOfstedRatings(
                int.Parse(e.Urn!),
                new OfstedShortInspection(
                    e.MisEstablishment!.DateOfLatestSection8Inspection.ParseAsNullableDate(),
                    e.MisEstablishment.Section8InspectionOverallOutcome
                    ),
                new OfstedRating(
                    e.MisEstablishment.OverallEffectiveness.ConvertOverallEffectivenessToOfstedRatingScore(),
                    e.MisEstablishment.QualityOfEducation.ToOfstedRatingScore(),
                    e.MisEstablishment.BehaviourAndAttitudes.ToOfstedRatingScore(),
                    e.MisEstablishment.PersonalDevelopment.ToOfstedRatingScore(),
                    e.MisEstablishment.EffectivenessOfLeadershipAndManagement.ToOfstedRatingScore(),
                    e.MisEstablishment.EarlyYearsProvision.ToOfstedRatingScore(),
                    e.MisEstablishment.SixthFormProvision.ToOfstedRatingScore(),
                    e.MisEstablishment.CategoryOfConcern.ToCategoriesOfConcern(),
                    e.MisEstablishment.SafeguardingIsEffective.ToSafeguardingScore(),
                    e.MisEstablishment.InspectionStartDate.ParseAsNullableDate()),
                new OfstedRating(
                    e.MisEstablishment.PreviousFullInspectionOverallEffectiveness.ConvertOverallEffectivenessToOfstedRatingScore(),
                    e.MisEstablishment.PreviousQualityOfEducation.ToOfstedRatingScore(),
                    e.MisEstablishment.PreviousBehaviourAndAttitudes.ToOfstedRatingScore(),
                    e.MisEstablishment.PreviousPersonalDevelopment.ToOfstedRatingScore(),
                    e.MisEstablishment.PreviousEffectivenessOfLeadershipAndManagement.ToOfstedRatingScore(),
                    e.MisEstablishment.PreviousEarlyYearsProvision.ToOfstedRatingScore(),
                    e.MisEstablishment.PreviousSixthFormProvision.ConvertNullableStringToOfstedRatingScore(),
                    e.MisEstablishment.PreviousCategoryOfConcern.ToCategoriesOfConcern(),
                    e.MisEstablishment.PreviousIsSafeguardingEffective.ToSafeguardingScore(),
                    e.MisEstablishment.PreviousInspectionStartDate.ParseAsNullableDate()),
                false
                ))
            .ToList();

        // Check to see if all ratings have been found in MisEstablishments, if not search in MisFurtherEducationEstablishments
        // Note: if an entry is in MisEstablishments then it will not be in MisFurtherEducationEstablishments, even if it has no ofsted data
        var missingUrns = urns.Where(urn => ofstedRatings.All(o => o.Urn != urn)).ToHashSet();
        if (missingUrns.Count != 0)
        {
            ofstedRatings.AddRange(establishments
                .Where(e => e.MisFurtherEducationEstablishment is not null
                            && int.TryParse(e.Urn, out var urn)
                            && missingUrns.Contains(urn))
                .Select(e => new AcademyOfstedRatings(
                    int.Parse(e.Urn!),
                    new OfstedShortInspection(
                        e.MisFurtherEducationEstablishment!.DateOfLatestShortInspection.ParseAsNullableDate(),
                        null),
                    new OfstedRating(
                        e.MisFurtherEducationEstablishment.OverallEffectiveness.ConvertOverallEffectivenessToOfstedRatingScore(),
                        e.MisFurtherEducationEstablishment.QualityOfEducation.ToOfstedRatingScore(),
                        e.MisFurtherEducationEstablishment.BehaviourAndAttitudes.ToOfstedRatingScore(),
                        e.MisFurtherEducationEstablishment.PersonalDevelopment.ToOfstedRatingScore(),
                        e.MisFurtherEducationEstablishment.EffectivenessOfLeadershipAndManagement.ToOfstedRatingScore(),
                        OfstedRatingScore.NotInspected,
                        OfstedRatingScore.NotInspected,
                        CategoriesOfConcern.DoesNotApply,
                        e.MisFurtherEducationEstablishment.IsSafeguardingEffective.ToSafeguardingScore(),
                        e.MisFurtherEducationEstablishment.LastDayOfInspection.ParseAsNullableDate()),
                    new OfstedRating(
                        e.MisFurtherEducationEstablishment.PreviousOverallEffectiveness.ConvertOverallEffectivenessToOfstedRatingScore(),
                        e.MisFurtherEducationEstablishment.PreviousQualityOfEducation.ToOfstedRatingScore(),
                        e.MisFurtherEducationEstablishment.PreviousBehaviourAndAttitudes.ToOfstedRatingScore(),
                        e.MisFurtherEducationEstablishment.PreviousPersonalDevelopment.ToOfstedRatingScore(),
                        e.MisFurtherEducationEstablishment.PreviousEffectivenessOfLeadershipAndManagement.ToOfstedRatingScore(),
                        OfstedRatingScore.NotInspected,
                        OfstedRatingScore.NotInspected,
                        CategoriesOfConcern.DoesNotApply,
                        e.MisFurtherEducationEstablishment.PreviousSafeguarding.ToSafeguardingScore(),
                        e.MisFurtherEducationEstablishment.PreviousLastDayOfInspection.ParseAsNullableDate()),
                    true
                ))
                .ToArray()
            );
        }

        return ofstedRatings.ToDictionary(o => o.Urn.ToString(), o => o);
    }

    private static bool HasShgIssuedAfterPolicyChange(OfstedRating rating)
    {
        return rating.InspectionDate >= SingleHeadlineGradesPolicyChangeDate &&
               rating.OverallEffectiveness != OfstedRatingScore.SingleHeadlineGradeNotAvailable;
    }

    private sealed record AcademyOfstedRatings(
        int Urn,
        OfstedShortInspection ShortInspection,
        OfstedRating Current,
        OfstedRating Previous,
        bool IsFurtherEducationalEstablishment);
}
