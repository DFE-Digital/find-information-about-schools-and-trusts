using System.Globalization;
using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Extensions;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.Ofsted;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.School;
using DfE.FindInformationAcademiesTrusts.Extensions;
using DfE.FindInformationAcademiesTrusts.HttpServices;
using GovUK.Dfe.CoreLibs.Contracts.Academies.V4.Establishments;

namespace DfE.FindInformationAcademiesTrusts.Services.Ofsted
{
    public interface IOfstedHelperService
    {
        SafeGuardingAndConcernsServiceModel GetLatestSafeGuardingInspection(ReportCardServiceModel reportCards, SchoolOfsted schoolOfstedRating);
        Task<SchoolOfsted[]> GetAcademiesOfstedAsync(string trustReferenceNumber);
        Task<SchoolOfsted> GetSchoolOfstedAsync(int urn);
    }

    public class OfstedHelperService(
        IGetEstablishmentsTemp getEstablishments,
        ISchoolRepository schoolRepository,
        ILogger<OfstedHelperService> logger) : IOfstedHelperService
    {
        public async Task<SchoolOfsted[]> GetAcademiesOfstedAsync(string trustReferenceNumber)
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

            return academiesData.Select(a =>
                    new SchoolOfsted(a.Urn,
                        a.Name,
                        DateTime.ParseExact(a.DateJoinedTrust!, "dd/MM/yyyy", CultureInfo.InvariantCulture),
                        ofstedRatings[a.Urn].ShortInspection,
                        ofstedRatings[a.Urn].Previous,
                        ofstedRatings[a.Urn].Current,
                        ofstedRatings[a.Urn].IsFurtherEducationalEstablishment
                    ))
                .ToArray();
        }

        public async Task<SchoolOfsted> GetSchoolOfstedAsync(int urn)
        {
            var urnString = urn.ToString();
            
            var result = await GetOfstedRatings([urnString]);
            var ofstedRatings = result[urnString];
            
            var schoolDetails = await getEstablishments.GetEstablishment(urn);
            
            // temporary fix to get joined date for foundations and federations as well as academies
            var joinedDate = await schoolRepository.GetJoinedDate(urnString);
            
            return new SchoolOfsted(
                urnString,
                schoolDetails.Name,
                joinedDate.ParseAsNullableDate(),
                ofstedRatings.ShortInspection,
                ofstedRatings.Previous,
                ofstedRatings.Current,
                ofstedRatings.IsFurtherEducationalEstablishment
            );
        }
        
        public SafeGuardingAndConcernsServiceModel GetLatestSafeGuardingInspection(
            ReportCardServiceModel reportCards, SchoolOfsted schoolOfstedRating)
        {
            var safeGuardingAndConcerns =
                new SafeGuardingAndConcernsServiceModel(schoolOfstedRating.DateEstablishmentJoinedGroup!.Value);

            if (TryGetSafeGuarding(safeGuardingAndConcerns, reportCards))
            {
                return safeGuardingAndConcerns;
            }

            return GetSafeGuardingFromOlderInspections(safeGuardingAndConcerns, schoolOfstedRating);
        }

        private static bool TryGetSafeGuarding(SafeGuardingAndConcernsServiceModel safeGuardingAndConcerns,
            ReportCardServiceModel reportCards)
        {
            var recentReportCard = reportCards.LatestReportCard;
            var previousReportCard = reportCards.PreviousReportCard;

            var latestInspection = recentReportCard;

            if (previousReportCard?.InspectionDate is { } prevDate &&
                recentReportCard?.InspectionDate is { } recentDate &&
                prevDate > recentDate)
            {
                latestInspection = previousReportCard;
            }

            if (latestInspection == null)
            {
                return false;
            }

            safeGuardingAndConcerns.Concerns = latestInspection.CategoryOfConcern;
            safeGuardingAndConcerns.SafeGuarding = latestInspection.Safeguarding;
            safeGuardingAndConcerns.InspectionDate = latestInspection.InspectionDate;

            return true;
        }

        private static SafeGuardingAndConcernsServiceModel GetSafeGuardingFromOlderInspections(
            SafeGuardingAndConcernsServiceModel safeGuardingAndConcerns, SchoolOfsted schoolOfstedRating)
        {
            var recent = schoolOfstedRating.CurrentOfstedRating;
            var previous = schoolOfstedRating.PreviousOfstedRating;

            var latestInspection = recent;

            if (previous.InspectionDate is { } prevDate &&
                recent.InspectionDate is { } recentDate &&
                prevDate > recentDate)
            {
                latestInspection = previous;
            }

            if (latestInspection.InspectionDate == null)
            {
                return safeGuardingAndConcerns;
            }

            safeGuardingAndConcerns.Concerns = latestInspection.CategoryOfConcern.ToDisplayString();
            safeGuardingAndConcerns.SafeGuarding = latestInspection.SafeguardingIsEffective.ToDisplayString();
            safeGuardingAndConcerns.InspectionDate = DateOnly.FromDateTime(latestInspection.InspectionDate.Value);

            return safeGuardingAndConcerns;
        }

        private async Task<Dictionary<string, AcademyOfstedRatings>> GetOfstedRatings(string[] urns)
        {
            // First pass at getting ofsted ratings from the db
            var allOfstedRatings = await GetOfstedRatingsForTrustOrSchool(urns);
            await AddPredecessorRatingsForMissingUrns(urns, allOfstedRatings);

            // Validate that all the ofsted ratings are found and valid
            foreach (var urn in urns)
            {
                ValidateOfstedRating(urn, allOfstedRatings);
            }

            return allOfstedRatings;
        }

        /// <summary>
        /// A recently changed URN can be identified when it was not returned, or when
        /// short, current and previous inspection dates are all null. Try the predecessor URN.
        /// </summary>
        private async Task AddPredecessorRatingsForMissingUrns(string[] urns,
            Dictionary<string, AcademyOfstedRatings> allOfstedRatings)
        {
            var missingUrns = urns.Where(urn => IsMissingOfstedData(allOfstedRatings, urn)).ToArray();
            if (missingUrns.Length == 0)
            {
                return;
            }

            var previousUrnMapping = await GetPredecessorUrns(missingUrns);
            if (previousUrnMapping.Count == 0)
            {
                return;
            }

            var oldOfstedRatings = await GetOfstedRatingsForTrustOrSchool(previousUrnMapping);
            foreach (var (urn, rating) in oldOfstedRatings)
            {
                allOfstedRatings[urn] = rating;
            }
        }

        private static bool IsMissingOfstedData(Dictionary<string, AcademyOfstedRatings> allOfstedRatings, string urn)
        {
            return !allOfstedRatings.TryGetValue(urn, out var ratings) || HasNoInspectionDates(ratings);
        }

        private static bool HasNoInspectionDates(AcademyOfstedRatings ratings)
        {
            return ratings.ShortInspection.InspectionDate is null
                   && ratings.Current.InspectionDate is null
                   && ratings.Previous.InspectionDate is null;
        }

        private void ValidateOfstedRating(string urn, Dictionary<string, AcademyOfstedRatings> allOfstedRatings)
        {
            // Log any URNs that couldn't be found and default to unknown
            if (!allOfstedRatings.TryGetValue(urn, out var foundRating))
            {
                logger.LogError(
                    "URN {Urn} was not found in Mis.Establishments or Mis.FurtherEducationEstablishments. This indicates a data integrity issue with the Ofsted data in Academies Db.",
                    urn);
                allOfstedRatings.Add(urn,
                    new AcademyOfstedRatings(int.Parse(urn), OfstedShortInspection.Unknown, OfstedRating.Unknown,
                        OfstedRating.Unknown, false));
                return;
            }

            //Log any errors that occured during parsing
            if (foundRating.Current.HasAnyUnknownRating || foundRating.Previous.HasAnyUnknownRating)
            {
                logger.LogError(
                    "URN {Urn} has some unrecognised ofsted ratings. This could be a data integrity issue with the Ofsted data in Academies Db.",
                    urn);
            }

            ReplaceSingleHeadlineGradesIssuedAfterPolicyChange(urn, foundRating, allOfstedRatings);
        }

        private void ReplaceSingleHeadlineGradesIssuedAfterPolicyChange(string urn, AcademyOfstedRatings foundRating,
            Dictionary<string, AcademyOfstedRatings> allOfstedRatings)
        {
            // Single headline grade checks only apply to non-further education establishments
            if (foundRating.IsFurtherEducationalEstablishment)
            {
                return;
            }

            var updatedRating = foundRating;

            // Ensure that there are no current single headline grades after policy change on 2nd September 2024
            if (HasShgIssuedAfterPolicyChange(foundRating.Current))
            {
                logger.LogError(
                    "URN {Urn} has a current Ofsted single headline grade of {score} issued on {InspectionDate} which was after single headline grades stopped being issued on 2nd September. This could be a data integrity issue with the Ofsted data in Academies Db.",
                    urn, foundRating.Current.OverallEffectiveness, foundRating.Current.InspectionDate);

                updatedRating = updatedRating with
                {
                    Current = foundRating.Current with
                    {
                        OverallEffectiveness = OfstedRatingScore.SingleHeadlineGradeNotAvailable
                    }
                };
            }

            // Ensure that there are no previous single headline grades after policy change on 2nd September 2024
            if (HasShgIssuedAfterPolicyChange(foundRating.Previous))
            {
                logger.LogError(
                    "URN {Urn} has a previous Ofsted single headline grade of {score} issued on {InspectionDate} which was after single headline grades stopped being issued on 2nd September. This could be a data integrity issue with the Ofsted data in Academies Db.",
                    urn, foundRating.Previous.OverallEffectiveness, foundRating.Previous.InspectionDate);

                updatedRating = updatedRating with
                {
                    Previous = foundRating.Previous with
                    {
                        OverallEffectiveness = OfstedRatingScore.SingleHeadlineGradeNotAvailable
                    }
                };
            }

            if (!ReferenceEquals(updatedRating, foundRating))
            {
                allOfstedRatings[urn] = updatedRating;
            }
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

        private async Task<Dictionary<string, AcademyOfstedRatings>> GetOfstedRatingsForTrustOrSchool(string[] urns)
        {
            if (urns.Length == 0)
            {
                return [];
            }

            var parsedUrns = urns.Select(u => int.Parse(u)).ToList();
            var establishments = await getEstablishments.GetEstablishmentsByUrns(parsedUrns);

            return CreateOfstedRatings(establishments, parsedUrns);
        }

        /// <param name="urnMapping">Key: URN to search by, Value: URN to return</param>
        private async Task<Dictionary<string, AcademyOfstedRatings>> GetOfstedRatingsForTrustOrSchool(
            Dictionary<int, int> urnMapping)
        {
            var urns = urnMapping.Keys.ToList();
            var establishments = await getEstablishments.GetEstablishmentsByUrns(urns);

            var ofstedRatings = CreateOfstedRatings(establishments, urns);

            return ofstedRatings.ToDictionary(
                rating => urnMapping[int.Parse(rating.Key)].ToString(),
                rating => rating.Value);
        }

        
        private static Dictionary<string, AcademyOfstedRatings> CreateOfstedRatings(
            List<EstablishmentDto> establishments,
            List<int> urns)
        {
            var ofstedRatings = establishments
                .Where(e => !string.IsNullOrEmpty(e.MISEstablishment.DateOfLatestSection8Inspection) || !string.IsNullOrEmpty(e.MISEstablishment.InspectionStartDate))
                .Select(e => new AcademyOfstedRatings(
                    int.Parse(e.Urn),
                    new OfstedShortInspection(
                        e.MISEstablishment.DateOfLatestSection8Inspection.ParseAsNullableDate(),
                        e.MISEstablishment.Section8InspectionOverallOutcome
                    ),
                    new OfstedRating(
                        e.MISEstablishment.OverallEffectiveness.ConvertOverallEffectivenessToOfstedRatingScore(),
                        e.MISEstablishment.QualityOfEducation.ToOfstedRatingScore(),
                        e.MISEstablishment.BehaviourAndAttitudes.ToOfstedRatingScore(),
                        e.MISEstablishment.PersonalDevelopment.ToOfstedRatingScore(),
                        e.MISEstablishment.EffectivenessOfLeadershipAndManagement.ToOfstedRatingScore(),
                        e.MISEstablishment.EarlyYearsProvision.ToOfstedRatingScore(),
                        e.MISEstablishment.SixthFormProvision.ToOfstedRatingScore(),
                        e.MISEstablishment.CategoryOfConcern.ToCategoriesOfConcern(),
                        e.MISEstablishment.SafeguardingIsEffective.ToSafeguardingScore(),
                        e.MISEstablishment.InspectionStartDate.ParseAsNullableDate()),
                    new OfstedRating(
                        e.MISEstablishment.PreviousFullInspectionOverallEffectiveness
                            .ConvertOverallEffectivenessToOfstedRatingScore(),
                        e.MISEstablishment.PreviousQualityOfEducation.ToOfstedRatingScore(),
                        e.MISEstablishment.PreviousBehaviourAndAttitudes.ToOfstedRatingScore(),
                        e.MISEstablishment.PreviousPersonalDevelopment.ToOfstedRatingScore(),
                        e.MISEstablishment.PreviousEffectivenessOfLeadershipAndManagement.ToOfstedRatingScore(),
                        e.MISEstablishment.PreviousEarlyYearsProvision.ToOfstedRatingScore(),
                        e.MISEstablishment.PreviousSixthFormProvision.ConvertNullableStringToOfstedRatingScore(),
                        e.MISEstablishment.PreviousCategoryOfConcern.ToCategoriesOfConcern(),
                        e.MISEstablishment.PreviousSafeguardingIsEffective.ToSafeguardingScore(),
                        e.MISEstablishment.PreviousInspectionStartDate.ParseAsNullableDate()),
                    false
                ))
                .ToList();

            // Check to see if all ratings have been found in MisEstablishments, if not search in MisFurtherEducationEstablishments
            // Note: if an entry is in MisEstablishments then it will not be in MisFurtherEducationEstablishments, even if it has no ofsted data
            var missingUrns = urns.Where(urn => ofstedRatings.All(o => o.Urn != urn)).ToHashSet();
            if (missingUrns.Count != 0)
            {
                ofstedRatings.AddRange(establishments
                    .Where(e => !string.IsNullOrEmpty(e.MisFurtherEducationEstablishment.DateOfLatestSection8Inspection) || !string.IsNullOrEmpty(e.MisFurtherEducationEstablishment.LastDayOfInspection)
                                && int.TryParse(e.Urn, out var urn)
                                && missingUrns.Contains(urn))
                    .Select(e => new AcademyOfstedRatings(
                        int.Parse(e.Urn),
                        new OfstedShortInspection(
                            e.MisFurtherEducationEstablishment.DateOfLatestSection8Inspection.ParseAsNullableDate(),
                            null),
                        new OfstedRating(
                            e.MisFurtherEducationEstablishment.OverallEffectiveness
                                .ConvertOverallEffectivenessToOfstedRatingScore(),
                            e.MisFurtherEducationEstablishment.QualityOfEducation.ToOfstedRatingScore(),
                            e.MisFurtherEducationEstablishment.BehaviourAndAttitudes.ToOfstedRatingScore(),
                            e.MisFurtherEducationEstablishment.PersonalDevelopment.ToOfstedRatingScore(),
                            e.MisFurtherEducationEstablishment.EffectivenessOfLeadershipAndManagement
                                .ToOfstedRatingScore(),
                            OfstedRatingScore.NotInspected,
                            OfstedRatingScore.NotInspected,
                            CategoriesOfConcern.DoesNotApply,
                            e.MisFurtherEducationEstablishment.SafeguardingIsEffective.ToSafeguardingScore(),
                            e.MisFurtherEducationEstablishment.LastDayOfInspection.ParseAsNullableDate()),
                        new OfstedRating(
                            e.MisFurtherEducationEstablishment.PreviousOverallEffectiveness
                                .ConvertOverallEffectivenessToOfstedRatingScore(),
                            e.MisFurtherEducationEstablishment.PreviousQualityOfEducation.ToOfstedRatingScore(),
                            e.MisFurtherEducationEstablishment.PreviousBehaviourAndAttitudes.ToOfstedRatingScore(),
                            e.MisFurtherEducationEstablishment.PreviousPersonalDevelopment.ToOfstedRatingScore(),
                            e.MisFurtherEducationEstablishment.PreviousEffectivenessOfLeadershipAndManagement
                                .ToOfstedRatingScore(),
                            OfstedRatingScore.NotInspected,
                            OfstedRatingScore.NotInspected,
                            CategoriesOfConcern.DoesNotApply,
                            e.MisFurtherEducationEstablishment.PreviousSafeguardingIsEffective.ToSafeguardingScore(),
                            e.MisFurtherEducationEstablishment.PreviousLastDayOfInspection.ParseAsNullableDate()),
                        true
                    ))
                    .ToArray()
                );
            }

            return ofstedRatings.ToDictionary(o => o.Urn.ToString(), o => o);
        }

        private static readonly DateTime
            SingleHeadlineGradesPolicyChangeDate = new(2024, 09, 02, 0, 0, 0, DateTimeKind.Utc);

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
}
