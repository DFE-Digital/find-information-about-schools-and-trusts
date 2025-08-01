using System.Globalization;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Contexts;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Extensions;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.Ofsted;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Repositories;

public class OfstedRepository(IAcademiesDbContext academiesDbContext, ILogger<AcademyRepository> logger)
    : IOfstedRepository
{
    private static readonly DateTime
        SingleHeadlineGradesPolicyChangeDate = new(2024, 09, 02, 0, 0, 0, DateTimeKind.Utc);

    public async Task<SchoolOfsted[]> GetAcademiesInTrustOfstedAsync(string uid)
    {
        var giasGroupLinkData = await academiesDbContext.GiasGroupLinks
            .Where(gl => gl.GroupUid == uid)
            .Select(gl => new
            {
                Urn = gl.Urn!,
                gl.EstablishmentName,
                gl.JoinedDate
            })
            .ToListAsync();

        var ofstedRatings = await GetOfstedRatings(giasGroupLinkData.Select(gl => gl.Urn).ToArray());

        var academyOfsteds = giasGroupLinkData.Select(gl =>
                new SchoolOfsted(gl.Urn,
                    gl.EstablishmentName,
                    DateTime.ParseExact(gl.JoinedDate!, "dd/MM/yyyy", CultureInfo.InvariantCulture),
                    ofstedRatings[gl.Urn].ShortInspection,
                    ofstedRatings[gl.Urn].Previous,
                    ofstedRatings[gl.Urn].Current
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

    private async Task<Dictionary<string, AcademyOfstedRatings>> GetOfstedRatings(string[] urns)
    {
        // First pass at getting ofsted ratings from the db
        var allOfstedRatings = await GetOfstedRatingsFromDb(urns);

        // If any missing then this could be a school that has recently changed URN
        // try to get ofsted rating using predecessor URN
        var missingUrns = urns.Except(allOfstedRatings.Keys).ToArray();
        if (missingUrns.Length > 0)
        {
            var previousUrnMapping = await GetPredecessorUrns(missingUrns);
            var oldOfstedRatings = await GetOfstedRatingsFromDb(previousUrnMapping);

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
                    new AcademyOfstedRatings(int.Parse(urn), OfstedShortInspection.Unknown, OfstedRating.Unknown, OfstedRating.Unknown, "none"));
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
            if (foundRating.FromTable == nameof(academiesDbContext.MisMstrEstablishmentsFiat) &&
                HasShgIssuedAfterPolicyChange(foundRating.Current))
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
            if (foundRating.FromTable == nameof(academiesDbContext.MisMstrEstablishmentsFiat) &&
                HasShgIssuedAfterPolicyChange(foundRating.Previous))
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
        var allPredecessorsForUrns = academiesDbContext.GiasEstablishmentLink
            .Where(gel => gel.LinkType == "Predecessor" && currentUrns.Contains(gel.Urn));

        var currentUrnsWithOneClearPredecessor = await allPredecessorsForUrns
            .GroupBy(gel => Convert.ToInt32(gel.Urn))
            .Where(group => group.Count() == 1)
            .Select(group => new { PreviousUrn = Convert.ToInt32(group.Single().LinkUrn), CurrentUrn = group.Key })
            .ToDictionaryAsync(u => u.PreviousUrn, u => u.CurrentUrn);

        return currentUrnsWithOneClearPredecessor;
    }

    private async Task<Dictionary<string, AcademyOfstedRatings>> GetOfstedRatingsFromDb(IEnumerable<string> urns)
    {
        // URN to search by and URN to return are the same
        var urnMapping = urns.ToDictionary(int.Parse, int.Parse);
        return await GetOfstedRatingsFromDb(urnMapping);
    }

    /// <param name="urnMapping">Key: URN to search by, Value: URN to return</param>
    private async Task<Dictionary<string, AcademyOfstedRatings>> GetOfstedRatingsFromDb(Dictionary<int, int> urnMapping)
    {
        // Ofsted data is held in MisEstablishments for most academies
        var ofstedRatings = await academiesDbContext.MisMstrEstablishmentsFiat
            .Where(me => urnMapping.Keys.Contains(me.Urn))
            .Select(me => new AcademyOfstedRatings(
                urnMapping[me.Urn],
                new OfstedShortInspection(
                    me.DateOfLatestSection8Inspection.ParseAsNullableDate(),
                    me.Section8InspectionOverallOutcome),
                new OfstedRating(
                    me.OverallEffectiveness.ConvertOverallEffectivenessToOfstedRatingScore(),
                    me.QualityOfEducation.ToOfstedRatingScore(),
                    me.BehaviourAndAttitudes.ToOfstedRatingScore(),
                    me.PersonalDevelopment.ToOfstedRatingScore(),
                    me.EffectivenessOfLeadershipAndManagement.ToOfstedRatingScore(),
                    me.EarlyYearsProvisionWhereApplicable.ToOfstedRatingScore(),
                    me.SixthFormProvisionWhereApplicable.ToOfstedRatingScore(),
                    me.CategoryOfConcern.ToCategoriesOfConcern(),
                    me.SafeguardingIsEffective.ToSafeguardingScore(),
                    me.InspectionStartDate.ParseAsNullableDate()),
                new OfstedRating(
                    me.PreviousFullInspectionOverallEffectiveness.ConvertOverallEffectivenessToOfstedRatingScore(),
                    me.PreviousQualityOfEducation.ToOfstedRatingScore(),
                    me.PreviousBehaviourAndAttitudes.ToOfstedRatingScore(),
                    me.PreviousPersonalDevelopment.ToOfstedRatingScore(),
                    me.PreviousEffectivenessOfLeadershipAndManagement.ToOfstedRatingScore(),
                    me.PreviousEarlyYearsProvisionWhereApplicable.ToOfstedRatingScore(),
                    me.PreviousSixthFormProvisionWhereApplicable.ConvertNullableStringToOfstedRatingScore(),
                    me.PreviousCategoryOfConcern.ToCategoriesOfConcern(),
                    me.PreviousSafeguardingIsEffective.ToSafeguardingScore(),
                    me.PreviousInspectionStartDate.ParseAsNullableDate()),
                nameof(academiesDbContext.MisMstrEstablishmentsFiat)
            ))
            .ToListAsync();

        // Check to see if all ratings have been found in MisEstablishments, if not search in MisFurtherEducationEstablishments
        // Note: if an entry is in MisEstablishments then it will not be in MisFurtherEducationEstablishments, even if it has no ofsted data
        var missingUrns = urnMapping.Where(kvp => ofstedRatings.All(o => o.Urn != kvp.Key)).ToDictionary();
        if (missingUrns.Count != 0)
        {
            ofstedRatings.AddRange(await academiesDbContext.MisMstrFurtherEducationEstablishmentsFiat
                .Where(mfe => missingUrns.Keys.Contains(mfe.ProviderUrn))
                .Select(mfe => new AcademyOfstedRatings(
                    missingUrns[mfe.ProviderUrn],
                    new OfstedShortInspection(
                        mfe.DateOfLatestShortInspection.ParseAsNullableDate(),
                        null),
                    new OfstedRating(
                        mfe.OverallEffectiveness.ConvertOverallEffectivenessToOfstedRatingScore(),
                        mfe.QualityOfEducation.ToOfstedRatingScore(),
                        mfe.BehaviourAndAttitudes.ToOfstedRatingScore(),
                        mfe.PersonalDevelopment.ToOfstedRatingScore(),
                        mfe.EffectivenessOfLeadershipAndManagement.ToOfstedRatingScore(),
                        OfstedRatingScore.NotInspected,
                        OfstedRatingScore.NotInspected,
                        CategoriesOfConcern.DoesNotApply,
                        mfe.IsSafeguardingEffective.ToSafeguardingScore(),
                        mfe.LastDayOfInspection.ParseAsNullableDate()),
                    new OfstedRating(
                        mfe.PreviousOverallEffectiveness.ConvertOverallEffectivenessToOfstedRatingScore(),
                        mfe.PreviousQualityOfEducation.ToOfstedRatingScore(),
                        mfe.PreviousBehaviourAndAttitudes.ToOfstedRatingScore(),
                        mfe.PreviousPersonalDevelopment.ToOfstedRatingScore(),
                        mfe.PreviousEffectivenessOfLeadershipAndManagement.ToOfstedRatingScore(),
                        OfstedRatingScore.NotInspected,
                        OfstedRatingScore.NotInspected,
                        CategoriesOfConcern.DoesNotApply,
                        mfe.PreviousSafeguarding.ToSafeguardingScore(),
                        mfe.PreviousLastDayOfInspection.ParseAsNullableDate()),
                    nameof(academiesDbContext.MisMstrFurtherEducationEstablishmentsFiat)
                ))
                .ToArrayAsync()
            );
        }

        return ofstedRatings.ToDictionary(o => o.Urn.ToString(), o => o);
    }

    private static bool HasShgIssuedAfterPolicyChange(OfstedRating rating)
    {
        return rating.InspectionDate >= SingleHeadlineGradesPolicyChangeDate &&
               rating.OverallEffectiveness != OfstedRatingScore.SingleHeadlineGradeNotAvailable;
    }

    private sealed record AcademyOfstedRatings(int Urn, OfstedShortInspection ShortInspection, OfstedRating Current, OfstedRating Previous, string FromTable);
}
