using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.AcademiesDbServices;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Models.Gias;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Models.Mis_Mstr;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Repositories;
using FluentAssertions.Execution;
using GovUK.Dfe.CoreLibs.Contracts.Academies.V4.Establishments;
using Microsoft.Extensions.Logging;

namespace DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.UnitTests.Repositories;

public class OfstedRepositoryTests
{
    private const string GroupUid = "1234";
    private const string TrustReferenceNumber = "TR98765";
    private readonly OfstedRepository _sut;
    private readonly MockAcademiesDbContext _mockAcademiesDbContext = new();
    private readonly IGetEstablishments _mockGetEstablishments;
    private readonly ILogger<AcademyRepository> _mockLogger = MockLogger.CreateLogger<AcademyRepository>();

    public OfstedRepositoryTests()
    {
        _mockGetEstablishments = Substitute.For<IGetEstablishments>();
        _mockGetEstablishments.GetEstablishmentsByTrustReferenceNumber(Arg.Any<string>()).Returns([]);
        _sut = new OfstedRepository(_mockAcademiesDbContext.Object, _mockGetEstablishments, _mockLogger);

        _mockAcademiesDbContext.AddGiasGroupForTrust(GroupUid);
    }

    private void SetupAcademiesInTrust(params string[] urns)
    {
        _mockGetEstablishments.GetEstablishmentsByTrustReferenceNumber(TrustReferenceNumber)
            .Returns(urns.Select(urn => new EstablishmentDto
            {
                Urn = urn,
                Name = $"Academy {urn}",
                DateJoinedTrust = "01/01/2022"
            }).ToArray());
    }

    private void AddPredecessorLink(string currentUrn, string predecessorUrn)
    {
        _mockAcademiesDbContext.GiasEstablishmentLinks.Add(new GiasEstablishmentLink
        {
            Urn = currentUrn,
            LinkUrn = predecessorUrn,
            LinkType = "Predecessor"
        });
    }

    [Fact]
    public async Task GetAcademiesInTrustOfstedAsync_should_return_empty_array_when_no_academies_linked_to_trust()
    {
        var result = await _sut.GetAcademiesInTrustOfstedAsync(TrustReferenceNumber);

        result.Should().BeEmpty();
        await _mockGetEstablishments.Received(1).GetEstablishmentsByTrustReferenceNumber(TrustReferenceNumber);
    }

    [Fact]
    public async Task GetAcademiesInTrustOfstedAsync_should_set_EstablishmentName_And_DateJoinedTrust()
    {
        var name = "A school";
        var anotherName = "An academy";
        var dateJoinedTrust = "01/01/2022";
        var anotherDateJoinedTrust = "02/02/2024";
        _mockGetEstablishments.GetEstablishmentsByTrustReferenceNumber(TrustReferenceNumber)
            .Returns([
                new EstablishmentDto
                {
                    Urn = "987654",
                    Name = name,
                    DateJoinedTrust = dateJoinedTrust,
                    EstablishmentType = new NameAndCodeDto
                    {
                        Name = "Local authority maintained schools"
                    }
                },
                new EstablishmentDto
                {
                    Urn = "123456",
                    Name = anotherName,
                    DateJoinedTrust = anotherDateJoinedTrust,
                    EstablishmentType = new NameAndCodeDto
                    {
                        Name = "Local authority maintained schools"
                    }
                }
            ]);

        var result = await _sut.GetAcademiesInTrustOfstedAsync(TrustReferenceNumber);

        result.Select(a => a.EstablishmentName).Should()
            .BeEquivalentTo(name, anotherName);
        result.Select(a => a.DateAcademyJoinedTrust)
            .Should()
            .BeEquivalentTo([
                new DateTime(2022, 01, 01),
                new DateTime(2024, 02, 02)
            ]);
    }

    [Fact]
    public async Task GetAcademiesInTrustOfstedAsync_should_combine_establishment_and_ofsted_data_for_each_academy_in_the_trust()
    {
        const string misUrn = "111111";
        const string furtherEdUrn = "222222";
        const string unknownUrn = "333333";

        SetupAcademiesInTrust(misUrn, furtherEdUrn, unknownUrn);

        _mockAcademiesDbContext.MisMstrEstablishmentFiat.Add(new MisMstrEstablishmentFiat
        {
            Urn = 111111,
            OverallEffectiveness = "1",
            InspectionStartDate = "15/05/2023",
            DateOfLatestSection8Inspection = "20/06/2024",
            Section8InspectionOverallOutcome = "School remains Good"
        });
        _mockAcademiesDbContext.MisMstrFurtherEducationEstablishmentFiat.Add(
            new MisMstrFurtherEducationEstablishmentFiat
            {
                ProviderUrn = 222222,
                OverallEffectiveness = "2",
                LastDayOfInspection = "10/03/2023",
                DateOfLatestShortInspection = "01/07/2024"
            });

        var result = await _sut.GetAcademiesInTrustOfstedAsync(TrustReferenceNumber);

        using (new AssertionScope())
        {
            result.Select(a => a.Urn).Should().Equal(misUrn, furtherEdUrn, unknownUrn);

            var fromMis = result.Should().ContainSingle(a => a.Urn == misUrn).Subject;
            fromMis.EstablishmentName.Should().Be("Academy 111111");
            fromMis.DateAcademyJoinedTrust.Should().Be(new DateTime(2022, 1, 1));
            fromMis.IsFurtherEducationalEstablishment.Should().BeFalse();
            fromMis.CurrentOfstedRating.OverallEffectiveness.Should().Be(OfstedRatingScore.Outstanding);
            fromMis.CurrentOfstedRating.InspectionDate.Should().Be(new DateTime(2023, 5, 15));
            fromMis.ShortInspection.InspectionDate.Should().Be(new DateTime(2024, 6, 20));
            fromMis.ShortInspection.InspectionOutcome.Should().Be("School remains Good");

            var fromFurtherEd = result.Should().ContainSingle(a => a.Urn == furtherEdUrn).Subject;
            fromFurtherEd.IsFurtherEducationalEstablishment.Should().BeTrue();
            fromFurtherEd.CurrentOfstedRating.OverallEffectiveness.Should().Be(OfstedRatingScore.Good);
            fromFurtherEd.CurrentOfstedRating.InspectionDate.Should().Be(new DateTime(2023, 3, 10));
            fromFurtherEd.ShortInspection.InspectionDate.Should().Be(new DateTime(2024, 7, 1));
            fromFurtherEd.ShortInspection.InspectionOutcome.Should().BeNull();

            var unknown = result.Should().ContainSingle(a => a.Urn == unknownUrn).Subject;
            unknown.EstablishmentName.Should().Be("Academy 333333");
            unknown.DateAcademyJoinedTrust.Should().Be(new DateTime(2022, 1, 1));
            unknown.CurrentOfstedRating.Should().Be(OfstedRating.Unknown);
            unknown.PreviousOfstedRating.Should().Be(OfstedRating.Unknown);
            unknown.ShortInspection.Should().Be(OfstedShortInspection.Unknown);
            unknown.IsFurtherEducationalEstablishment.Should().BeFalse();
        }

        _mockLogger.VerifyLogError(
            $"URN {unknownUrn} was not found in Mis.Establishments or Mis.FurtherEducationEstablishments. This indicates a data integrity issue with the Ofsted data in Academies Db.");
        _mockLogger.VerifyDidNotReceive($"URN {misUrn} was not found");
        _mockLogger.VerifyDidNotReceive($"URN {furtherEdUrn} was not found");
    }

    [Fact]
    public async Task GetAcademiesInTrustOfstedAsync_should_map_current_and_previous_judgements_from_mis_establishments()
    {
        SetupAcademiesInTrust("987654");
        _mockAcademiesDbContext.MisMstrEstablishmentFiat.Add(new MisMstrEstablishmentFiat
        {
            Urn = 987654,
            OverallEffectiveness = "1",
            QualityOfEducation = 1,
            BehaviourAndAttitudes = 2,
            PersonalDevelopment = 3,
            EffectivenessOfLeadershipAndManagement = 4,
            EarlyYearsProvisionWhereApplicable = 1,
            SixthFormProvisionWhereApplicable = 2,
            InspectionStartDate = "15/05/2023",
            PreviousFullInspectionOverallEffectiveness = "2",
            PreviousQualityOfEducation = 3,
            PreviousBehaviourAndAttitudes = 4,
            PreviousPersonalDevelopment = 1,
            PreviousEffectivenessOfLeadershipAndManagement = 2,
            PreviousEarlyYearsProvisionWhereApplicable = 3,
            PreviousSixthFormProvisionWhereApplicable = "4",
            PreviousInspectionStartDate = "01/02/2013"
        });

        var result = await _sut.GetAcademiesInTrustOfstedAsync(TrustReferenceNumber);

        var actual = result.Should().ContainSingle().Subject;
        actual.CurrentOfstedRating.Should().Be(new OfstedRating(
            OfstedRatingScore.Outstanding,
            OfstedRatingScore.Outstanding,
            OfstedRatingScore.Good,
            OfstedRatingScore.RequiresImprovement,
            OfstedRatingScore.Inadequate,
            OfstedRatingScore.Outstanding,
            OfstedRatingScore.Good,
            CategoriesOfConcern.NotInspected,
            SafeguardingScore.NotInspected,
            new DateTime(2023, 5, 15)));
        actual.PreviousOfstedRating.Should().Be(new OfstedRating(
            OfstedRatingScore.Good,
            OfstedRatingScore.RequiresImprovement,
            OfstedRatingScore.Inadequate,
            OfstedRatingScore.Outstanding,
            OfstedRatingScore.Good,
            OfstedRatingScore.RequiresImprovement,
            OfstedRatingScore.Inadequate,
            CategoriesOfConcern.NotInspected,
            SafeguardingScore.NotInspected,
            new DateTime(2013, 2, 1)));
    }

    [Fact]
    public async Task GetAcademiesInTrustOfstedAsync_should_map_further_education_judgements_when_urn_is_not_in_mis_establishments()
    {
        SetupAcademiesInTrust("987654");
        _mockAcademiesDbContext.MisMstrFurtherEducationEstablishmentFiat.Add(
            new MisMstrFurtherEducationEstablishmentFiat
            {
                ProviderUrn = 987654,
                OverallEffectiveness = "1",
                QualityOfEducation = 2,
                BehaviourAndAttitudes = 3,
                PersonalDevelopment = 4,
                EffectivenessOfLeadershipAndManagement = 1,
                LastDayOfInspection = "15/05/2023",
                PreviousOverallEffectiveness = "2",
                PreviousQualityOfEducation = 3,
                PreviousBehaviourAndAttitudes = 4,
                PreviousPersonalDevelopment = 1,
                PreviousEffectivenessOfLeadershipAndManagement = 2,
                PreviousLastDayOfInspection = "01/02/2013"
            });

        var result = await _sut.GetAcademiesInTrustOfstedAsync(TrustReferenceNumber);

        var actual = result.Should().ContainSingle().Subject;
        actual.IsFurtherEducationalEstablishment.Should().BeTrue();
        actual.CurrentOfstedRating.Should().Be(new OfstedRating(
            OfstedRatingScore.Outstanding,
            OfstedRatingScore.Good,
            OfstedRatingScore.RequiresImprovement,
            OfstedRatingScore.Inadequate,
            OfstedRatingScore.Outstanding,
            OfstedRatingScore.NotInspected,
            OfstedRatingScore.NotInspected,
            CategoriesOfConcern.DoesNotApply,
            SafeguardingScore.NotInspected,
            new DateTime(2023, 5, 15)));
        actual.PreviousOfstedRating.Should().Be(new OfstedRating(
            OfstedRatingScore.Good,
            OfstedRatingScore.RequiresImprovement,
            OfstedRatingScore.Inadequate,
            OfstedRatingScore.Outstanding,
            OfstedRatingScore.Good,
            OfstedRatingScore.NotInspected,
            OfstedRatingScore.NotInspected,
            CategoriesOfConcern.DoesNotApply,
            SafeguardingScore.NotInspected,
            new DateTime(2013, 2, 1)));
    }

    [Fact]
    public async Task GetAcademiesInTrustOfstedAsync_should_only_use_further_education_data_for_urns_missing_from_mis_establishments()
    {
        SetupAcademiesInTrust("900001", "900002");
        _mockAcademiesDbContext.MisMstrEstablishmentFiat.Add(new MisMstrEstablishmentFiat
        {
            Urn = 900001,
            EarlyYearsProvisionWhereApplicable = 1
        });
        _mockAcademiesDbContext.MisMstrFurtherEducationEstablishmentFiat.AddRange(
        [
            new MisMstrFurtherEducationEstablishmentFiat { ProviderUrn = 900001 },
            new MisMstrFurtherEducationEstablishmentFiat { ProviderUrn = 900002 }
        ]);

        var result = await _sut.GetAcademiesInTrustOfstedAsync(TrustReferenceNumber);

        using (new AssertionScope())
        {
            var fromMis = result.Should().ContainSingle(a => a.Urn == "900001").Subject;
            fromMis.IsFurtherEducationalEstablishment.Should().BeFalse();
            fromMis.CurrentOfstedRating.EarlyYearsProvision.Should().Be(OfstedRatingScore.Outstanding);

            var fromFurtherEd = result.Should().ContainSingle(a => a.Urn == "900002").Subject;
            fromFurtherEd.IsFurtherEducationalEstablishment.Should().BeTrue();
            fromFurtherEd.CurrentOfstedRating.EarlyYearsProvision.Should().Be(OfstedRatingScore.NotInspected);
        }
    }

    [Fact]
    public async Task GetAcademiesInTrustOfstedAsync_should_return_not_inspected_when_ofsted_row_exists_without_ratings()
    {
        SetupAcademiesInTrust("500001", "500002");
        _mockAcademiesDbContext.MisMstrEstablishmentFiat.Add(new MisMstrEstablishmentFiat { Urn = 500001 });
        _mockAcademiesDbContext.MisMstrFurtherEducationEstablishmentFiat.Add(
            new MisMstrFurtherEducationEstablishmentFiat { ProviderUrn = 500002 });

        var result = await _sut.GetAcademiesInTrustOfstedAsync(TrustReferenceNumber);

        result.Should().ContainSingle(a => a.Urn == "500001").Which.CurrentOfstedRating
            .Should().Be(OfstedRating.NotInspected);
        result.Should().ContainSingle(a => a.Urn == "500002").Which.CurrentOfstedRating
            .Should().Be(OfstedRating.NotInspected with { CategoryOfConcern = CategoriesOfConcern.DoesNotApply });
    }

    [Fact]
    public async Task GetAcademiesInTrustOfstedAsync_should_use_predecessor_ratings_when_current_urn_has_no_ofsted_record()
    {
        SetupAcademiesInTrust("800001");
        AddPredecessorLink("800001", "899999");
        _mockAcademiesDbContext.MisMstrEstablishmentFiat.Add(new MisMstrEstablishmentFiat
        {
            Urn = 899999,
            QualityOfEducation = 1,
            PreviousQualityOfEducation = 3
        });

        var result = await _sut.GetAcademiesInTrustOfstedAsync(TrustReferenceNumber);

        var actual = result.Should().ContainSingle().Subject;
        actual.Urn.Should().Be("800001");
        actual.CurrentOfstedRating.QualityOfEducation.Should().Be(OfstedRatingScore.Outstanding);
        actual.PreviousOfstedRating.QualityOfEducation.Should().Be(OfstedRatingScore.RequiresImprovement);
    }

    [Fact]
    public async Task GetAcademiesInTrustOfstedAsync_should_return_unknown_when_a_missing_urn_cannot_be_resolved_to_a_single_predecessor()
    {
        const string noLinkUrn = "300001";
        const string successorOnlyUrn = "300002";
        const string multiplePredecessorsUrn = "300003";

        SetupAcademiesInTrust(noLinkUrn, successorOnlyUrn, multiplePredecessorsUrn);

        _mockAcademiesDbContext.GiasEstablishmentLinks.Add(new GiasEstablishmentLink
        {
            Urn = successorOnlyUrn,
            LinkUrn = "399999",
            LinkType = "Successor"
        });
        AddPredecessorLink(multiplePredecessorsUrn, "388888");
        AddPredecessorLink(multiplePredecessorsUrn, "377777");
        _mockAcademiesDbContext.MisMstrEstablishmentFiat.AddRange(
        [
            new MisMstrEstablishmentFiat { Urn = 399999, QualityOfEducation = 1 },
            new MisMstrEstablishmentFiat { Urn = 388888, QualityOfEducation = 1 },
            new MisMstrEstablishmentFiat { Urn = 377777, QualityOfEducation = 1 }
        ]);

        var result = await _sut.GetAcademiesInTrustOfstedAsync(TrustReferenceNumber);

        result.Should().HaveCount(3).And.AllSatisfy(academy =>
        {
            academy.CurrentOfstedRating.Should().Be(OfstedRating.Unknown);
            academy.PreviousOfstedRating.Should().Be(OfstedRating.Unknown);
            academy.ShortInspection.Should().Be(OfstedShortInspection.Unknown);
        });
    }

    [Fact]
    public async Task GetAcademiesInTrustOfstedAsync_should_not_look_up_predecessor_when_urn_is_found_in_ofsted_data()
    {
        SetupAcademiesInTrust("700001");
        _mockAcademiesDbContext.MisMstrEstablishmentFiat.Add(new MisMstrEstablishmentFiat
        {
            Urn = 700001,
            QualityOfEducation = 1
        });

        await _sut.GetAcademiesInTrustOfstedAsync(TrustReferenceNumber);

        _ = _mockAcademiesDbContext.Object.DidNotReceive().GiasEstablishmentLink;
    }

    [Fact]
    public async Task GetAcademiesInTrustOfstedAsync_should_log_error_when_ofsted_ratings_are_unrecognised()
    {
        SetupAcademiesInTrust("600001", "600002");
        _mockAcademiesDbContext.MisMstrEstablishmentFiat.AddRange(
        [
            new MisMstrEstablishmentFiat { Urn = 600001, OverallEffectiveness = "not a valid score" },
            new MisMstrEstablishmentFiat
            {
                Urn = 600002,
                OverallEffectiveness = "1",
                InspectionStartDate = "01/01/2022"
            }
        ]);

        await _sut.GetAcademiesInTrustOfstedAsync(TrustReferenceNumber);

        _mockLogger.VerifyLogError(
            "URN 600001 has some unrecognised ofsted ratings. This could be a data integrity issue with the Ofsted data in Academies Db.");
        _mockLogger.VerifyDidNotReceive("URN 600002 has some unrecognised ofsted ratings");
    }

    [Fact]
    public async Task
        GetAcademiesInTrustOfstedAsync_should_replace_non_further_education_single_headline_grades_issued_on_or_after_2_september_2024()
    {
        // Single headline grades stopped being issued on 2 September 2024 for non-further education.
        SetupAcademiesInTrust("400001", "400002", "400003", "400004", "400005");
        _mockAcademiesDbContext.MisMstrEstablishmentFiat.AddRange(
        [
            new MisMstrEstablishmentFiat
            {
                Urn = 400001,
                OverallEffectiveness = "1",
                InspectionStartDate = "01/01/2025",
                PreviousFullInspectionOverallEffectiveness = "3",
                PreviousInspectionStartDate = "01/01/2021"
            },
            new MisMstrEstablishmentFiat
            {
                Urn = 400002,
                OverallEffectiveness = "2",
                InspectionStartDate = "02/09/2024"
            },
            new MisMstrEstablishmentFiat
            {
                Urn = 400003,
                OverallEffectiveness = "Not judged",
                InspectionStartDate = "01/01/2025",
                PreviousFullInspectionOverallEffectiveness = "3",
                PreviousInspectionStartDate = "12/12/2024"
            },
            new MisMstrEstablishmentFiat
            {
                Urn = 400004,
                OverallEffectiveness = "Not judged",
                InspectionStartDate = "01/01/2025",
                PreviousFullInspectionOverallEffectiveness = "4",
                PreviousInspectionStartDate = "02/09/2024"
            },
            new MisMstrEstablishmentFiat
            {
                Urn = 400005,
                OverallEffectiveness = "1",
                InspectionStartDate = "01/09/2024",
                PreviousFullInspectionOverallEffectiveness = "2",
                PreviousInspectionStartDate = "01/01/2021"
            }
        ]);

        var result = await _sut.GetAcademiesInTrustOfstedAsync(TrustReferenceNumber);

        result.Should().SatisfyRespectively(
            academy =>
            {
                academy.CurrentOfstedRating.OverallEffectiveness.Should()
                    .Be(OfstedRatingScore.SingleHeadlineGradeNotAvailable);
                academy.PreviousOfstedRating.OverallEffectiveness.Should()
                    .Be(OfstedRatingScore.RequiresImprovement);
            },
            academy =>
            {
                academy.CurrentOfstedRating.OverallEffectiveness.Should()
                    .Be(OfstedRatingScore.SingleHeadlineGradeNotAvailable);
            },
            academy =>
            {
                academy.CurrentOfstedRating.OverallEffectiveness.Should()
                    .Be(OfstedRatingScore.SingleHeadlineGradeNotAvailable);
                academy.PreviousOfstedRating.OverallEffectiveness.Should()
                    .Be(OfstedRatingScore.SingleHeadlineGradeNotAvailable);
            },
            academy =>
            {
                academy.CurrentOfstedRating.OverallEffectiveness.Should()
                    .Be(OfstedRatingScore.SingleHeadlineGradeNotAvailable);
                academy.PreviousOfstedRating.OverallEffectiveness.Should()
                    .Be(OfstedRatingScore.SingleHeadlineGradeNotAvailable);
            },
            academy =>
            {
                academy.CurrentOfstedRating.OverallEffectiveness.Should().Be(OfstedRatingScore.Outstanding);
                academy.PreviousOfstedRating.OverallEffectiveness.Should().Be(OfstedRatingScore.Good);
            });

        _mockLogger.VerifyLogErrors(
            "URN 400001 has a current Ofsted single headline grade of Outstanding issued",
            "URN 400002 has a current Ofsted single headline grade of Good issued",
            "URN 400003 has a previous Ofsted single headline grade of RequiresImprovement issued",
            "URN 400004 has a previous Ofsted single headline grade of Inadequate issued");
        _mockLogger.VerifyDidNotReceive("URN 400005");
    }

    [Fact]
    public async Task
        GetAcademiesInTrustOfstedAsync_should_keep_further_education_single_headline_grades_issued_after_2_september_2024()
    {
        SetupAcademiesInTrust("410001");
        _mockAcademiesDbContext.MisMstrFurtherEducationEstablishmentFiat.Add(
            new MisMstrFurtherEducationEstablishmentFiat
            {
                ProviderUrn = 410001,
                OverallEffectiveness = "1",
                PreviousOverallEffectiveness = "3",
                LastDayOfInspection = "01/01/2025",
                PreviousLastDayOfInspection = "12/12/2024"
            });

        var result = await _sut.GetAcademiesInTrustOfstedAsync(TrustReferenceNumber);

        var actual = result.Should().ContainSingle().Subject;
        actual.CurrentOfstedRating.OverallEffectiveness.Should().Be(OfstedRatingScore.Outstanding);
        actual.PreviousOfstedRating.OverallEffectiveness.Should().Be(OfstedRatingScore.RequiresImprovement);
        _mockLogger.VerifyDidNotReceive();
    }
    

    private void VerifyLogs(int[] urns, bool shouldLogError)
    {
        foreach (var urn in urns)
        {
            var message =
                $"URN {urn} has some unrecognised ofsted ratings. This could be a data integrity issue with the Ofsted data in Academies Db.";

            if (shouldLogError)
            {
                _mockLogger.VerifyLogError(message);
            }
            else
            {
                _mockLogger.VerifyDidNotReceive(message);
            }
        }
    }

    [Fact]
    public async Task
        GetOfstedInspectionHistorySummaryAsync_when_no_establishment_with_urn_exists_then_returns_Unknown_OfsteadInspectionHistorySummary()
    {
        var result = await _sut.GetOfstedInspectionHistorySummaryAsync(123456);

        result.Should().BeEquivalentTo(OfstedInspectionHistorySummary.Unknown);
    }

    [Theory]
    [InlineData(
        "01/01/2024",
        "2",
        OfstedRatingScore.Good,
        "01/01/2014",
        "3",
        OfstedRatingScore.RequiresImprovement)]
    [InlineData(
        "12/12/2023",
        "3",
        OfstedRatingScore.RequiresImprovement,
        "12/12/2013",
        "4",
        OfstedRatingScore.Inadequate)]
    public async Task
        GetOfstedInspectionHistorySummaryAsync_when_establishment_exists_then_returns_OfsteadInspectionHistorySummary_with_correct_data(
            string currentInspectionDate,
            string currentInspectionOutcome,
            OfstedRatingScore expectedCurrentInspectionOutcome,
            string previousInspectionDate,
            string previousInspectionOutcome,
            OfstedRatingScore expectedPreviousInspectionOutcome)
    {
        _mockAcademiesDbContext.MisMstrEstablishmentFiat.AddRange([
            new MisMstrEstablishmentFiat
            {
                Urn = 123456,
                InspectionStartDate = currentInspectionDate,
                OverallEffectiveness = currentInspectionOutcome,
                PreviousInspectionStartDate = previousInspectionDate,
                PreviousFullInspectionOverallEffectiveness = previousInspectionOutcome
            },
            new MisMstrEstablishmentFiat
            {
                Urn = 987654,
                InspectionStartDate = "06/06/2023",
                OverallEffectiveness = "Outstanding",
                PreviousInspectionStartDate = "06/06/2013",
                PreviousFullInspectionOverallEffectiveness = "Outstanding"
            }
        ]);

        var result = await _sut.GetOfstedInspectionHistorySummaryAsync(123456);

        result.CurrentInspection.InspectionDate.Should().NotBeNull();
        result.CurrentInspection.InspectionDate.Should().Be(DateTime.Parse(currentInspectionDate));
        result.CurrentInspection.InspectionOutcome.Should().Be(expectedCurrentInspectionOutcome);

        result.PreviousInspection.InspectionDate.Should().NotBeNull();
        result.PreviousInspection.InspectionDate.Should().Be(DateTime.Parse(previousInspectionDate));
        result.PreviousInspection.InspectionOutcome.Should().Be(expectedPreviousInspectionOutcome);
    }

    [Fact]
    public async Task
        GetOfstedInspectionHistorySummaryAsync_when_current_inspection_date_is_missing_then_CurrentInspection_has_null_InspectionDate()
    {
        _mockAcademiesDbContext.MisMstrEstablishmentFiat.AddRange([
            new MisMstrEstablishmentFiat
            {
                Urn = 123456,
                InspectionStartDate = null,
                OverallEffectiveness = "1",
                PreviousInspectionStartDate = "01/01/2012",
                PreviousFullInspectionOverallEffectiveness = "1"
            }
        ]);

        var result = await _sut.GetOfstedInspectionHistorySummaryAsync(123456);

        result.CurrentInspection.InspectionDate.Should().BeNull();
        result.PreviousInspection.InspectionDate.Should().NotBeNull();
    }

    [Fact]
    public async Task
        GetOfstedInspectionHistorySummaryAsync_when_previous_inspection_date_is_missing_then_PreviousInspection_has_null_InspectionDate()
    {
        _mockAcademiesDbContext.MisMstrEstablishmentFiat.AddRange([
            new MisMstrEstablishmentFiat
            {
                Urn = 123456,
                InspectionStartDate = "01/01/2022",
                OverallEffectiveness = "1",
                PreviousInspectionStartDate = null,
                PreviousFullInspectionOverallEffectiveness = "1"
            }
        ]);

        var result = await _sut.GetOfstedInspectionHistorySummaryAsync(123456);

        result.CurrentInspection.InspectionDate.Should().NotBeNull();
        result.PreviousInspection.InspectionDate.Should().BeNull();
    }

    [Fact]
    public async Task
        GetOfstedInspectionHistorySummaryAsync_when_current_overall_effectiveness_is_missing_then_CurrentInspection_has_NotInspected_InspectionOutcome()
    {
        _mockAcademiesDbContext.MisMstrEstablishmentFiat.AddRange([
            new MisMstrEstablishmentFiat
            {
                Urn = 123456,
                InspectionStartDate = "01/01/2022",
                OverallEffectiveness = null,
                PreviousInspectionStartDate = "01/01/2012",
                PreviousFullInspectionOverallEffectiveness = "1"
            }
        ]);

        var result = await _sut.GetOfstedInspectionHistorySummaryAsync(123456);

        result.CurrentInspection.InspectionOutcome.Should().Be(OfstedRatingScore.NotInspected);
        result.PreviousInspection.InspectionOutcome.Should().NotBe(OfstedRatingScore.NotInspected);
    }

    [Fact]
    public async Task
        GetOfstedInspectionHistorySummaryAsync_when_previous_overall_effectiveness_is_missing_then_PreviousInspection_has_NotInspected_InspectionOutcome()
    {
        _mockAcademiesDbContext.MisMstrEstablishmentFiat.AddRange([
            new MisMstrEstablishmentFiat
            {
                Urn = 123456,
                InspectionStartDate = "01/01/2022",
                OverallEffectiveness = "1",
                PreviousInspectionStartDate = "01/01/2012",
                PreviousFullInspectionOverallEffectiveness = null
            }
        ]);

        var result = await _sut.GetOfstedInspectionHistorySummaryAsync(123456);

        result.CurrentInspection.InspectionOutcome.Should().NotBe(OfstedRatingScore.NotInspected);
        result.PreviousInspection.InspectionOutcome.Should().Be(OfstedRatingScore.NotInspected);
    }

    [Fact]
    public async Task
        GetOfstedShortInspectionAsync_when_no_establishment_with_urn_exists_then_returns_Unknown_ShortInspection()
    {
        var result = await _sut.GetOfstedShortInspectionAsync(123456);

        result.Should().BeEquivalentTo(OfstedShortInspection.Unknown);
    }

    [Theory]
    [InlineData("01/01/2025", "School remains Good")]
    [InlineData("12/12/2024", "Improved significantly")]
    public async Task
        GetOfstedShortInspectionAsync_when_establishment_exists_then_returns_ShortInspection_with_correct_data(
            string inspectionDate, string inspectionOutcome)
    {
        _mockAcademiesDbContext.MisMstrEstablishmentFiat.AddRange([
            new MisMstrEstablishmentFiat
            {
                Urn = 123456,
                DateOfLatestSection8Inspection = inspectionDate,
                Section8InspectionOverallOutcome = inspectionOutcome
            },
            new MisMstrEstablishmentFiat
            {
                Urn = 987654,
                DateOfLatestSection8Inspection = "06/06/2023",
                Section8InspectionOverallOutcome = "School remains Outstanding"
            }
        ]);

        var result = await _sut.GetOfstedShortInspectionAsync(123456);

        result.InspectionDate.Should().NotBeNull();
        result.InspectionDate!.Should().Be(DateTime.Parse(inspectionDate));
        result.InspectionOutcome.Should().NotBeNull();
        result.InspectionOutcome!.Should().Be(inspectionOutcome);
    }

    [Fact]
    public async Task
        GetOfstedShortInspectionAsync_when_inspection_date_is_missing_then_ShortInspection_has_null_InspectionDate()
    {
        _mockAcademiesDbContext.MisMstrEstablishmentFiat.AddRange([
            new MisMstrEstablishmentFiat
            {
                Urn = 123456,
                DateOfLatestSection8Inspection = null,
                Section8InspectionOverallOutcome = "School remains Outstanding"
            }
        ]);

        var result = await _sut.GetOfstedShortInspectionAsync(123456);

        result.InspectionDate.Should().BeNull();
    }

    [Fact]
    public async Task
        GetOfstedShortInspectionAsync_when_inspection_outcome_is_missing_then_ShortInspection_has_null_InspectionOutcome()
    {
        _mockAcademiesDbContext.MisMstrEstablishmentFiat.AddRange([
            new MisMstrEstablishmentFiat
            {
                Urn = 123456,
                DateOfLatestSection8Inspection = "01/01/2025",
                Section8InspectionOverallOutcome = null
            }
        ]);

        var result = await _sut.GetOfstedShortInspectionAsync(123456);

        result.InspectionOutcome.Should().BeNull();
    }

    [Fact]
    public async Task GetSchoolOfstedRatingsAsync_should_return_Unknown_for_unknown_urn()
    {
        var result = await _sut.GetSchoolOfstedRatingsAsync(987654);
        result.ShortInspection.Should().BeEquivalentTo(OfstedShortInspection.Unknown);
        result.CurrentOfstedRating.Should().BeEquivalentTo(OfstedRating.Unknown);
        result.PreviousOfstedRating.Should().BeEquivalentTo(OfstedRating.Unknown);
        result.EstablishmentName.Should().BeNull();
        result.DateAcademyJoinedTrust.Should().BeNull();
    }

    [Fact]
    public async Task GetSchoolOfstedRatingsAsync_returns_academy_information_when_linked_to_trust()
    {
        _mockAcademiesDbContext.AddGiasGroupLinks("some other trust", "some other academy");

        var giasGroupLinks = _mockAcademiesDbContext.AddGiasGroupLinks(GroupUid, "987654");

        var result = await _sut.GetSchoolOfstedRatingsAsync(987654);

        result.Should().NotBeNull();
        result.EstablishmentName.Should().BeEquivalentTo(giasGroupLinks[0].EstablishmentName);
    }

    [Fact]
    public async Task GetSchoolOfstedRatingsAsync_should_set_EstablishmentName_from_giasGroupLink_when_linked_to_trust()
    {
        var giasGroupLinks = _mockAcademiesDbContext.AddGiasGroupLinks(GroupUid, "987654");

        var result = await _sut.GetSchoolOfstedRatingsAsync(987654);

        result.Should().NotBeNull();
        result.EstablishmentName.Should()
            .BeEquivalentTo(giasGroupLinks.Select(g => g.EstablishmentName).First());
    }

    [Fact]
    public async Task
        GetSchoolOfstedRatingsAsync_should_set_DateAcademyJoinedTrust_from_giasGroupLink_when_linked_to_trust()
    {
        var giasGroupLinks = _mockAcademiesDbContext.AddGiasGroupLinks(GroupUid, "987654");
        giasGroupLinks[0].JoinedDate = "01/01/2022";

        var result = await _sut.GetSchoolOfstedRatingsAsync(987654);

        result.Should().NotBeNull();
        result.DateAcademyJoinedTrust.Should().Be(new DateTime(2022, 01, 01));
    }

    [Fact]
    public async Task GetSchoolOfstedRatingsAsync_should_set_InspectionDate_when_not_further_ed()
    {
        _mockAcademiesDbContext.AddGiasGroupLinks(GroupUid, "987654");
        _mockAcademiesDbContext.AddEstablishmentFiat(987654, "15/05/2023");

        var result = await _sut.GetSchoolOfstedRatingsAsync(987654);

        result.Should().NotBeNull();
        result.CurrentOfstedRating.InspectionDate
            .Should().HaveDay(15).And.HaveMonth(5).And.HaveYear(2023);
    }

    [Fact]
    public async Task GetSchoolOfstedRatingsAsync_should_set_InspectionDate_when_further_ed()
    {
        _mockAcademiesDbContext.AddGiasGroupLinks(GroupUid, "987654");
        _mockAcademiesDbContext.MisMstrFurtherEducationEstablishmentFiat.Add(
            new MisMstrFurtherEducationEstablishmentFiat
            {
                ProviderUrn = 987654, LastDayOfInspection = "15/05/2023", PreviousLastDayOfInspection = "01/02/2013"
            });

        var result = await _sut.GetSchoolOfstedRatingsAsync(987654);

        result.Should().NotBeNull();
        result.CurrentOfstedRating.InspectionDate.Should().HaveDay(15).And.HaveMonth(5).And.HaveYear(2023);
        result.PreviousOfstedRating.InspectionDate.Should().HaveDay(1).And.HaveMonth(2).And.HaveYear(2013);
    }

    [Fact]
    public async Task GetSchoolOfstedRatingsAsync_should_set_CategoryOfConcern_to_DoesNotApply_when_further_ed()
    {
        _mockAcademiesDbContext.AddGiasGroupLinks(GroupUid, "987654");
        _mockAcademiesDbContext.MisMstrFurtherEducationEstablishmentFiat.Add(
            new MisMstrFurtherEducationEstablishmentFiat
                { ProviderUrn = 987654 });

        var result = await _sut.GetSchoolOfstedRatingsAsync(987654);

        result.Should().NotBeNull();
        result.CurrentOfstedRating.CategoryOfConcern.Should().Be(CategoriesOfConcern.DoesNotApply);
        result.PreviousOfstedRating.CategoryOfConcern.Should().Be(CategoriesOfConcern.DoesNotApply);
    }

    [Fact]
    public async Task GetSchoolOfstedRatingsAsync_should_handle_not_inspected_when_not_further_ed()
    {
        _mockAcademiesDbContext.AddGiasGroupLinks(GroupUid, "987654");
        _mockAcademiesDbContext.MisMstrEstablishmentFiat.Add(new MisMstrEstablishmentFiat { Urn = 987654 });

        var result = await _sut.GetSchoolOfstedRatingsAsync(987654);

        result.Should().NotBeNull();
        result.CurrentOfstedRating.Should().Be(OfstedRating.NotInspected);
        result.PreviousOfstedRating.Should().Be(OfstedRating.NotInspected);
    }

    [Fact]
    public async Task GetSchoolOfstedRatingsAsync_should_handle_not_inspected_when_further_ed()
    {
        _mockAcademiesDbContext.AddGiasGroupLinks(GroupUid, "987654");
        _mockAcademiesDbContext.MisMstrFurtherEducationEstablishmentFiat.Add(
            new MisMstrFurtherEducationEstablishmentFiat { ProviderUrn = 987654 });

        var result = await _sut.GetSchoolOfstedRatingsAsync(987654);

        result.Should().NotBeNull();
        result.CurrentOfstedRating.Should()
            .Be(OfstedRating.NotInspected with { CategoryOfConcern = CategoriesOfConcern.DoesNotApply });
        result.PreviousOfstedRating.Should()
            .Be(OfstedRating.NotInspected with { CategoryOfConcern = CategoriesOfConcern.DoesNotApply });
    }

    [Fact]
    public async Task GetSchoolOfstedRatingsAsync_should_set_ofsted_sub_judgements_when_not_further_ed()
    {
        _mockAcademiesDbContext.AddGiasGroupLinks(GroupUid, "987654");

        _mockAcademiesDbContext.MisMstrEstablishmentFiat.Add(new MisMstrEstablishmentFiat
        {
            Urn = 987654,

            OverallEffectiveness = "1",
            QualityOfEducation = 1,
            BehaviourAndAttitudes = 2,
            PersonalDevelopment = 3,
            EffectivenessOfLeadershipAndManagement = 4,
            EarlyYearsProvisionWhereApplicable = 1,
            SixthFormProvisionWhereApplicable = 2,

            PreviousFullInspectionOverallEffectiveness = "2",
            PreviousQualityOfEducation = 3,
            PreviousBehaviourAndAttitudes = 4,
            PreviousPersonalDevelopment = 1,
            PreviousEffectivenessOfLeadershipAndManagement = 2,
            PreviousEarlyYearsProvisionWhereApplicable = 3,
            PreviousSixthFormProvisionWhereApplicable = "4"
        });

        var result = await _sut.GetSchoolOfstedRatingsAsync(987654);

        result.Should().NotBeNull();
        result.CurrentOfstedRating.OverallEffectiveness.Should().Be(OfstedRatingScore.Outstanding);
        result.CurrentOfstedRating.QualityOfEducation.Should().Be(OfstedRatingScore.Outstanding);
        result.CurrentOfstedRating.BehaviourAndAttitudes.Should().Be(OfstedRatingScore.Good);
        result.CurrentOfstedRating.PersonalDevelopment.Should().Be(OfstedRatingScore.RequiresImprovement);
        result.CurrentOfstedRating.EffectivenessOfLeadershipAndManagement.Should().Be(OfstedRatingScore.Inadequate);
        result.CurrentOfstedRating.EarlyYearsProvision.Should().Be(OfstedRatingScore.Outstanding);
        result.CurrentOfstedRating.SixthFormProvision.Should().Be(OfstedRatingScore.Good);

        result.PreviousOfstedRating.OverallEffectiveness.Should().Be(OfstedRatingScore.Good);
        result.PreviousOfstedRating.QualityOfEducation.Should().Be(OfstedRatingScore.RequiresImprovement);
        result.PreviousOfstedRating.BehaviourAndAttitudes.Should().Be(OfstedRatingScore.Inadequate);
        result.PreviousOfstedRating.PersonalDevelopment.Should().Be(OfstedRatingScore.Outstanding);
        result.PreviousOfstedRating.EffectivenessOfLeadershipAndManagement.Should().Be(OfstedRatingScore.Good);
        result.PreviousOfstedRating.EarlyYearsProvision.Should().Be(OfstedRatingScore.RequiresImprovement);
        result.PreviousOfstedRating.SixthFormProvision.Should().Be(OfstedRatingScore.Inadequate);
    }

    [Fact]
    public async Task GetSchoolOfstedRatingsAsync_should_set_ofsted_judgements_when_further_ed()
    {
        _mockAcademiesDbContext.AddGiasGroupLinks(GroupUid, "987654");

        _mockAcademiesDbContext.MisMstrFurtherEducationEstablishmentFiat.Add(
            new MisMstrFurtherEducationEstablishmentFiat
            {
                ProviderUrn = 987654,

                OverallEffectiveness = "1",
                QualityOfEducation = 2,
                BehaviourAndAttitudes = 3,
                PersonalDevelopment = 4,
                EffectivenessOfLeadershipAndManagement = 1,

                PreviousOverallEffectiveness = "2",
                PreviousQualityOfEducation = 3,
                PreviousBehaviourAndAttitudes = 4,
                PreviousPersonalDevelopment = 1,
                PreviousEffectivenessOfLeadershipAndManagement = 2
            });

        var result = await _sut.GetSchoolOfstedRatingsAsync(987654);

        result.Should().NotBeNull();
        result.CurrentOfstedRating.OverallEffectiveness.Should().Be(OfstedRatingScore.Outstanding);
        result.CurrentOfstedRating.QualityOfEducation.Should().Be(OfstedRatingScore.Good);
        result.CurrentOfstedRating.BehaviourAndAttitudes.Should().Be(OfstedRatingScore.RequiresImprovement);
        result.CurrentOfstedRating.PersonalDevelopment.Should().Be(OfstedRatingScore.Inadequate);
        result.CurrentOfstedRating.EffectivenessOfLeadershipAndManagement.Should().Be(OfstedRatingScore.Outstanding);

        result.PreviousOfstedRating.OverallEffectiveness.Should().Be(OfstedRatingScore.Good);
        result.PreviousOfstedRating.QualityOfEducation.Should().Be(OfstedRatingScore.RequiresImprovement);
        result.PreviousOfstedRating.BehaviourAndAttitudes.Should().Be(OfstedRatingScore.Inadequate);
        result.PreviousOfstedRating.PersonalDevelopment.Should().Be(OfstedRatingScore.Outstanding);
        result.PreviousOfstedRating.EffectivenessOfLeadershipAndManagement.Should().Be(OfstedRatingScore.Good);
    }

    [Fact]
    public async Task
        GetSchoolOfstedRatingsAsync_should_set_all_ofsted_judgements_for_previous_urn_when_urn_has_changed()
    {
        var giasEstablishmentLink = new GiasEstablishmentLink
        {
            Urn = "123456",
            LinkUrn = "987654",
            LinkType = "Predecessor"
        };

        _mockAcademiesDbContext.AddGiasGroupLinks(GroupUid, "123456");

        _mockAcademiesDbContext.MisMstrEstablishmentFiat.Add(new MisMstrEstablishmentFiat
        {
            Urn = 987654,

            QualityOfEducation = 1,
            BehaviourAndAttitudes = 2,
            PersonalDevelopment = 3,
            EffectivenessOfLeadershipAndManagement = 4,
            EarlyYearsProvisionWhereApplicable = 1,
            SixthFormProvisionWhereApplicable = 2,

            PreviousQualityOfEducation = 3,
            PreviousBehaviourAndAttitudes = 4,
            PreviousPersonalDevelopment = 1,
            PreviousEffectivenessOfLeadershipAndManagement = 2,
            PreviousEarlyYearsProvisionWhereApplicable = 3,
            PreviousSixthFormProvisionWhereApplicable = "4"
        });

        _mockAcademiesDbContext.GiasEstablishmentLinks.Add(giasEstablishmentLink);

        var result = await _sut.GetSchoolOfstedRatingsAsync(987654);

        result.Should().NotBeNull();
        result.CurrentOfstedRating.QualityOfEducation.Should().Be(OfstedRatingScore.Outstanding);
        result.CurrentOfstedRating.BehaviourAndAttitudes.Should().Be(OfstedRatingScore.Good);
        result.CurrentOfstedRating.PersonalDevelopment.Should().Be(OfstedRatingScore.RequiresImprovement);
        result.CurrentOfstedRating.EffectivenessOfLeadershipAndManagement.Should().Be(OfstedRatingScore.Inadequate);
        result.CurrentOfstedRating.EarlyYearsProvision.Should().Be(OfstedRatingScore.Outstanding);
        result.CurrentOfstedRating.SixthFormProvision.Should().Be(OfstedRatingScore.Good);

        result.PreviousOfstedRating.QualityOfEducation.Should().Be(OfstedRatingScore.RequiresImprovement);
        result.PreviousOfstedRating.BehaviourAndAttitudes.Should().Be(OfstedRatingScore.Inadequate);
        result.PreviousOfstedRating.PersonalDevelopment.Should().Be(OfstedRatingScore.Outstanding);
        result.PreviousOfstedRating.EffectivenessOfLeadershipAndManagement.Should().Be(OfstedRatingScore.Good);
        result.PreviousOfstedRating.EarlyYearsProvision.Should().Be(OfstedRatingScore.RequiresImprovement);
        result.PreviousOfstedRating.SixthFormProvision.Should().Be(OfstedRatingScore.Inadequate);
    }

    [Fact]
    public async Task GetSchoolOfstedRatingsAsync_should_return_Unknown_when_urn_doesnt_have_predecessor()
    {
        var giasEstablishmentLink = new GiasEstablishmentLink
        {
            Urn = "123456",
            LinkUrn = "987654",
            LinkType = "Successor"
        };

        _mockAcademiesDbContext.AddGiasGroupLinks(GroupUid, "123456");

        _mockAcademiesDbContext.MisMstrEstablishmentFiat.Add(new MisMstrEstablishmentFiat
            { Urn = 987654, QualityOfEducation = 1 });

        _mockAcademiesDbContext.GiasEstablishmentLinks.Add(giasEstablishmentLink);

        var result = await _sut.GetSchoolOfstedRatingsAsync(123456);

        result.Should().NotBeNull();
        result.CurrentOfstedRating.Should().Be(OfstedRating.Unknown);
        result.PreviousOfstedRating.Should().Be(OfstedRating.Unknown);
    }

    [Fact]
    public async Task GetSchoolOfstedRatingsAsync_should_return_Unknown_when_urn_has_multiple_predecessors()
    {
        const string currentUrn = "123456";
        _mockAcademiesDbContext.AddGiasGroupLinks(GroupUid, currentUrn);
        _mockAcademiesDbContext.GiasEstablishmentLinks.AddRange([
            new GiasEstablishmentLink
            {
                Urn = currentUrn,
                LinkUrn = "987654",
                LinkType = "Predecessor"
            },
            new GiasEstablishmentLink
            {
                Urn = currentUrn,
                LinkUrn = "876543",
                LinkType = "Predecessor"
            }
        ]);
        _mockAcademiesDbContext.MisMstrEstablishmentFiat.AddRange([
            new MisMstrEstablishmentFiat { Urn = 987654, QualityOfEducation = 1 },
            new MisMstrEstablishmentFiat { Urn = 876543, QualityOfEducation = 1 }
        ]);

        var result = await _sut.GetSchoolOfstedRatingsAsync(123456);

        result.Should().NotBeNull();
        result.CurrentOfstedRating.Should().Be(OfstedRating.Unknown);
        result.PreviousOfstedRating.Should().Be(OfstedRating.Unknown);
    }

    [Fact]
    public async Task GetSchoolOfstedRatingsAsync_should_not_query_gias_establishment_link_when_urn_is_found_in_mis()
    {
        _mockAcademiesDbContext.AddGiasGroupLinks(GroupUid, "987654");

        _mockAcademiesDbContext.MisMstrEstablishmentFiat.Add(new MisMstrEstablishmentFiat
        {
            Urn = 987654,

            QualityOfEducation = 1,
            BehaviourAndAttitudes = 2,
            PersonalDevelopment = 3,
            EffectivenessOfLeadershipAndManagement = 4,
            EarlyYearsProvisionWhereApplicable = 1,
            SixthFormProvisionWhereApplicable = 2,

            PreviousQualityOfEducation = 3,
            PreviousBehaviourAndAttitudes = 4,
            PreviousPersonalDevelopment = 1,
            PreviousEffectivenessOfLeadershipAndManagement = 2,
            PreviousEarlyYearsProvisionWhereApplicable = 3,
            PreviousSixthFormProvisionWhereApplicable = "4"
        });

        await _sut.GetSchoolOfstedRatingsAsync(987654);

        _ = _mockAcademiesDbContext.Object.DidNotReceive().GiasEstablishmentLink;
    }

    [Fact]
    public async Task
        GetSchoolOfstedRatingsAsync_should_log_error_and_return_ofsted_unknown_when_urn_not_found_in_mis()
    {
        var giasGroupLink = _mockAcademiesDbContext.AddGiasGroupLinks(GroupUid, "987654").Single();

        var result = await _sut.GetSchoolOfstedRatingsAsync(987654);

        result.Should().NotBeNull();
        result.Urn.Should().Be(giasGroupLink.Urn);
        result.CurrentOfstedRating.Should().Be(OfstedRating.Unknown);
        result.PreviousOfstedRating.Should().Be(OfstedRating.Unknown);

        _mockLogger.VerifyLogError(
            $"URN {giasGroupLink.Urn} was not found in Mis.Establishments or Mis.FurtherEducationEstablishments. This indicates a data integrity issue with the Ofsted data in Academies Db.");
    }

    [Fact]
    public async Task GetSchoolOfstedRatingsAsync_should_include_short_inspection_data_when_not_further_education()
    {
        _mockAcademiesDbContext.AddGiasGroupLinks(GroupUid, "987654");
        _mockAcademiesDbContext.MisMstrEstablishmentFiat.Add(
            new MisMstrEstablishmentFiat
            {
                Urn = 987654, DateOfLatestSection8Inspection = "15/05/2023",
                Section8InspectionOverallOutcome = "School remains Good"
            });

        var result = await _sut.GetSchoolOfstedRatingsAsync(987654);

        result.Should().NotBeNull();
        result.ShortInspection.InspectionDate.Should().HaveDay(15).And.HaveMonth(5).And.HaveYear(2023);
        result.ShortInspection.InspectionOutcome.Should().Be("School remains Good");
    }

    [Fact]
    public async Task GetSchoolOfstedRatingsAsync_should_include_short_inspection_data_when_further_education()
    {
        _mockAcademiesDbContext.AddGiasGroupLinks(GroupUid, "987654");
        _mockAcademiesDbContext.MisMstrFurtherEducationEstablishmentFiat.Add(
            new MisMstrFurtherEducationEstablishmentFiat
            {
                ProviderUrn = 987654,
                DateOfLatestShortInspection = "01/07/2025"
            });

        var result = await _sut.GetSchoolOfstedRatingsAsync(987654);

        result.Should().NotBeNull();
        result.ShortInspection.InspectionDate.Should().HaveDay(1).And.HaveMonth(7).And.HaveYear(2025);
        result.ShortInspection.InspectionOutcome.Should().BeNull();
    }

    [Fact]
    public async Task GetSchoolOfstedRatingsAsync_should_include_unknown_short_inspection_when_urn_is_unknown()
    {
        _mockAcademiesDbContext.AddGiasGroupLinks(GroupUid, "987654");

        var result = await _sut.GetSchoolOfstedRatingsAsync(987654);

        result.Should().NotBeNull();
        result.ShortInspection.Should().BeEquivalentTo(OfstedShortInspection.Unknown);
    }
}
