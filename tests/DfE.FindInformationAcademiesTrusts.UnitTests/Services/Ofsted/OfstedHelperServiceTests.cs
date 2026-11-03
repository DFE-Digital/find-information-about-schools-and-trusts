using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.Ofsted;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.School;
using DfE.FindInformationAcademiesTrusts.HttpServices;
using DfE.FindInformationAcademiesTrusts.Services.Ofsted;
using DfE.FindInformationAcademiesTrusts.UnitTests.Mocks;
using FluentAssertions.Execution;
using GovUK.Dfe.CoreLibs.Contracts.Academies.V4.Establishments;
using Microsoft.Extensions.Logging;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Services.Ofsted;

public class OfstedHelperServiceTests
{
    private const string TrustReferenceNumber = "TR98765";
    private static readonly int[] UrnFoundInOfstedData = [700001];
    private static readonly DateTime DateJoined = new(2020, 1, 1);

    private readonly OfstedHelperService _sut;
    private readonly IGetEstablishmentsTemp _mockGetEstablishments;
    private readonly ISchoolRepository _mockSchoolRepository = Substitute.For<ISchoolRepository>();
    private readonly ILogger<OfstedHelperService> _mockLogger = MockLogger.CreateLogger<OfstedHelperService>();
    private readonly Dictionary<string, List<string>> _predecessorUrnsByCurrentUrn = new();
    private readonly Dictionary<int, EstablishmentDto> _ofstedEstablishmentsByUrn = new();

    public OfstedHelperServiceTests()
    {
        _mockGetEstablishments = Substitute.For<IGetEstablishmentsTemp>();
        _mockGetEstablishments.GetEstablishmentsByTrustReferenceNumber(Arg.Any<string>()).Returns([]);
        _mockGetEstablishments.GetEstablishmentsByUrns(Arg.Any<List<int>>())
            .Returns(callInfo => BuildEstablishmentsByUrns(callInfo.Arg<List<int>>()));
        _mockGetEstablishments.GetEstablishment(Arg.Any<int>()).Returns(callInfo => new EstablishmentDto
        {
            Urn = callInfo.Arg<int>().ToString(),
            Name = $"School {callInfo.Arg<int>()}"
        });

        _sut = new OfstedHelperService(_mockGetEstablishments, _mockSchoolRepository, _mockLogger);
    }

    [Fact]
    public async Task GetAcademiesOfstedAsync_should_return_empty_array_when_no_academies_linked_to_trust()
    {
        var result = await _sut.GetAcademiesOfstedAsync(TrustReferenceNumber);

        result.Should().BeEmpty();
        await _mockGetEstablishments.Received(1).GetEstablishmentsByTrustReferenceNumber(TrustReferenceNumber);
        await _mockGetEstablishments.DidNotReceive().GetEstablishmentsByUrns(Arg.Any<List<int>>());
    }

    [Fact]
    public async Task GetAcademiesOfstedAsync_should_set_EstablishmentName_And_DateJoinedTrust()
    {
        const string name = "A school";
        const string anotherName = "An academy";
        _mockGetEstablishments.GetEstablishmentsByTrustReferenceNumber(TrustReferenceNumber)
            .Returns([
                new EstablishmentDto
                {
                    Urn = "987654",
                    Name = name,
                    DateJoinedTrust = "01/01/2022"
                },
                new EstablishmentDto
                {
                    Urn = "123456",
                    Name = anotherName,
                    DateJoinedTrust = "02/02/2024"
                }
            ]);

        var result = await _sut.GetAcademiesOfstedAsync(TrustReferenceNumber);

        result.Select(a => a.EstablishmentName).Should().BeEquivalentTo(name, anotherName);
        result.Select(a => a.DateEstablishmentJoinedGroup)
            .Should()
            .BeEquivalentTo([
                new DateTime(2022, 1, 1),
                new DateTime(2024, 2, 2)
            ]);
    }

    [Fact]
    public async Task GetAcademiesOfstedAsync_should_combine_establishment_and_ofsted_data_for_each_academy_in_the_trust()
    {
        const string misUrn = "111111";
        const string furtherEdUrn = "222222";
        const string unknownUrn = "333333";

        SetupAcademiesInTrust(misUrn, furtherEdUrn, unknownUrn);

        AddSchoolOfsted(111111, new MisEstablishmentDto
        {
            OverallEffectiveness = "1",
            InspectionStartDate = "15/05/2023",
            DateOfLatestSection8Inspection = "20/06/2024",
            Section8InspectionOverallOutcome = "School remains Good"
        });
        AddFurtherEducationOfsted(222222, new MisFurtherEducationEstablishmentDto
        {
            OverallEffectiveness = "2",
            LastDayOfInspection = "10/03/2023",
            DateOfLatestSection8Inspection = "01/07/2024"
        });

        var result = await _sut.GetAcademiesOfstedAsync(TrustReferenceNumber);

        using (new AssertionScope())
        {
            result.Select(a => a.Urn).Should().Equal(misUrn, furtherEdUrn, unknownUrn);

            var fromMis = result.Should().ContainSingle(a => a.Urn == misUrn).Subject;
            fromMis.IsFurtherEducationalEstablishment.Should().BeFalse();
            fromMis.ShortInspection.InspectionDate.Should().Be(new DateTime(2024, 6, 20));
            fromMis.ShortInspection.InspectionOutcome.Should().Be("School remains Good");

            var fromFurtherEd = result.Should().ContainSingle(a => a.Urn == furtherEdUrn).Subject;
            fromFurtherEd.IsFurtherEducationalEstablishment.Should().BeTrue();
            fromFurtherEd.ShortInspection.InspectionDate.Should().Be(new DateTime(2024, 7, 1));
            fromFurtherEd.ShortInspection.InspectionOutcome.Should().BeNull();

            var unknown = result.Should().ContainSingle(a => a.Urn == unknownUrn).Subject;
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
    public async Task GetAcademiesOfstedAsync_should_map_current_and_previous_judgements_from_mis_establishments()
    {
        SetupAcademiesInTrust("987654");
        AddSchoolOfsted(987654, new MisEstablishmentDto
        {
            OverallEffectiveness = "1",
            QualityOfEducation = "1",
            BehaviourAndAttitudes = "2",
            PersonalDevelopment = "3",
            EffectivenessOfLeadershipAndManagement = "4",
            EarlyYearsProvision = "1",
            SixthFormProvision = "2",
            CategoryOfConcern = "SM",
            SafeguardingIsEffective = "Yes",
            InspectionStartDate = "15/05/2023",
            PreviousFullInspectionOverallEffectiveness = "2",
            PreviousQualityOfEducation = "3",
            PreviousBehaviourAndAttitudes = "4",
            PreviousPersonalDevelopment = "1",
            PreviousEffectivenessOfLeadershipAndManagement = "2",
            PreviousEarlyYearsProvision = "3",
            PreviousSixthFormProvision = "4",
            PreviousCategoryOfConcern = "NTI",
            PreviousSafeguardingIsEffective = "No",
            PreviousInspectionStartDate = "01/02/2013"
        });

        var result = await _sut.GetAcademiesOfstedAsync(TrustReferenceNumber);

        var actual = result.Should().ContainSingle().Subject;
        actual.CurrentOfstedRating.Should().Be(new OfstedRating(
            OfstedRatingScore.Outstanding,
            OfstedRatingScore.Outstanding,
            OfstedRatingScore.Good,
            OfstedRatingScore.RequiresImprovement,
            OfstedRatingScore.Inadequate,
            OfstedRatingScore.Outstanding,
            OfstedRatingScore.Good,
            CategoriesOfConcern.SpecialMeasures,
            SafeguardingScore.Yes,
            new DateTime(2023, 5, 15)));
        actual.PreviousOfstedRating.Should().Be(new OfstedRating(
            OfstedRatingScore.Good,
            OfstedRatingScore.RequiresImprovement,
            OfstedRatingScore.Inadequate,
            OfstedRatingScore.Outstanding,
            OfstedRatingScore.Good,
            OfstedRatingScore.RequiresImprovement,
            OfstedRatingScore.Inadequate,
            CategoriesOfConcern.NoticeToImprove,
            SafeguardingScore.No,
            new DateTime(2013, 2, 1)));
    }

    [Fact]
    public async Task GetAcademiesOfstedAsync_should_map_further_education_judgements_when_urn_is_not_in_mis_establishments()
    {
        SetupAcademiesInTrust("987654");
        AddFurtherEducationOfsted(987654, new MisFurtherEducationEstablishmentDto
        {
            OverallEffectiveness = "1",
            QualityOfEducation = "2",
            BehaviourAndAttitudes = "3",
            PersonalDevelopment = "4",
            EffectivenessOfLeadershipAndManagement = "1",
            SafeguardingIsEffective = "Yes",
            LastDayOfInspection = "15/05/2023",
            PreviousOverallEffectiveness = "2",
            PreviousQualityOfEducation = "3",
            PreviousBehaviourAndAttitudes = "4",
            PreviousPersonalDevelopment = "1",
            PreviousEffectivenessOfLeadershipAndManagement = "2",
            PreviousSafeguardingIsEffective = "No",
            PreviousLastDayOfInspection = "01/02/2013"
        });

        var result = await _sut.GetAcademiesOfstedAsync(TrustReferenceNumber);

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
            SafeguardingScore.Yes,
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
            SafeguardingScore.No,
            new DateTime(2013, 2, 1)));
    }

    [Fact]
    public async Task GetAcademiesOfstedAsync_should_only_use_further_education_data_for_urns_missing_from_mis_establishments()
    {
        SetupAcademiesInTrust("900001", "900002");
        AddSchoolOfsted(900001, new MisEstablishmentDto
        {
            EarlyYearsProvision = "1"
        });
        AddFurtherEducationOfsted(900001, new MisFurtherEducationEstablishmentDto());
        AddFurtherEducationOfsted(900002, new MisFurtherEducationEstablishmentDto());

        var result = await _sut.GetAcademiesOfstedAsync(TrustReferenceNumber);

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
    public async Task GetAcademiesOfstedAsync_should_return_not_inspected_when_ofsted_row_exists_without_ratings()
    {
        SetupAcademiesInTrust("500001", "500002");
        AddSchoolOfsted(500001, new MisEstablishmentDto());
        AddFurtherEducationOfsted(500002, new MisFurtherEducationEstablishmentDto());

        var result = await _sut.GetAcademiesOfstedAsync(TrustReferenceNumber);

        result.Should().ContainSingle(a => a.Urn == "500001").Which.CurrentOfstedRating
            .Should().Be(OfstedRating.NotInspected);
        result.Should().ContainSingle(a => a.Urn == "500002").Which.CurrentOfstedRating
            .Should().Be(OfstedRating.NotInspected with { CategoryOfConcern = CategoriesOfConcern.DoesNotApply });
    }

    [Fact]
    public async Task GetAcademiesOfstedAsync_should_use_predecessor_ratings_when_current_urn_has_no_inspection_dates()
    {
        SetupAcademiesInTrust("800002");
        AddSchoolOfsted(800002, new MisEstablishmentDto());
        AddPredecessorLink("800002", "899998");
        AddSchoolOfsted(899998, new MisEstablishmentDto
        {
            QualityOfEducation = "1",
            InspectionStartDate = "15/05/2023",
            PreviousQualityOfEducation = "3",
            PreviousInspectionStartDate = "01/02/2013",
            DateOfLatestSection8Inspection = "20/06/2024"
        });

        var result = await _sut.GetAcademiesOfstedAsync(TrustReferenceNumber);

        var actual = result.Should().ContainSingle().Subject;
        actual.Urn.Should().Be("800002");
        actual.CurrentOfstedRating.QualityOfEducation.Should().Be(OfstedRatingScore.Outstanding);
        actual.CurrentOfstedRating.InspectionDate.Should().Be(new DateTime(2023, 5, 15));
        actual.PreviousOfstedRating.QualityOfEducation.Should().Be(OfstedRatingScore.RequiresImprovement);
        actual.PreviousOfstedRating.InspectionDate.Should().Be(new DateTime(2013, 2, 1));
        actual.ShortInspection.InspectionDate.Should().Be(new DateTime(2024, 6, 20));
    }

    [Fact]
    public async Task GetAcademiesOfstedAsync_should_use_predecessor_ratings_when_current_urn_has_no_ofsted_record()
    {
        SetupAcademiesInTrust("800001");
        AddPredecessorLink("800001", "899999");
        AddSchoolOfsted(899999, new MisEstablishmentDto
        {
            QualityOfEducation = "1",
            PreviousQualityOfEducation = "3"
        });

        var result = await _sut.GetAcademiesOfstedAsync(TrustReferenceNumber);

        var actual = result.Should().ContainSingle().Subject;
        actual.Urn.Should().Be("800001");
        actual.CurrentOfstedRating.QualityOfEducation.Should().Be(OfstedRatingScore.Outstanding);
    }

    [Fact]
    public async Task GetAcademiesOfstedAsync_should_use_predecessor_further_education_ratings_when_current_urn_has_no_ofsted_record()
    {
        SetupAcademiesInTrust("800003");
        AddPredecessorLink("800003", "899997");
        AddFurtherEducationOfsted(899997, new MisFurtherEducationEstablishmentDto
        {
            OverallEffectiveness = "1",
            LastDayOfInspection = "15/05/2023"
        });

        var result = await _sut.GetAcademiesOfstedAsync(TrustReferenceNumber);

        var actual = result.Should().ContainSingle().Subject;
        actual.Urn.Should().Be("800003");
        actual.IsFurtherEducationalEstablishment.Should().BeTrue();
    }

    [Fact]
    public async Task GetAcademiesOfstedAsync_should_return_unknown_when_predecessor_also_has_no_ofsted_record()
    {
        const string currentUrn = "800004";
        SetupAcademiesInTrust(currentUrn);
        AddPredecessorLink(currentUrn, "899996");

        var result = await _sut.GetAcademiesOfstedAsync(TrustReferenceNumber);

        var academy = result.Should().ContainSingle().Subject;
        academy.CurrentOfstedRating.Should().Be(OfstedRating.Unknown);
        academy.PreviousOfstedRating.Should().Be(OfstedRating.Unknown);
        academy.ShortInspection.Should().Be(OfstedShortInspection.Unknown);
    }

    [Fact]
    public async Task GetAcademiesOfstedAsync_should_return_unknown_when_a_missing_urn_cannot_be_resolved_to_a_single_predecessor()
    {
        const string multiplePredecessorsUrn = "300003";

        SetupAcademiesInTrust(multiplePredecessorsUrn);
        AddPredecessorLink(multiplePredecessorsUrn, "388888");
        AddPredecessorLink(multiplePredecessorsUrn, "377777");

        var result = await _sut.GetAcademiesOfstedAsync(TrustReferenceNumber);

        var academy = result.Should().ContainSingle().Subject;
        academy.CurrentOfstedRating.Should().Be(OfstedRating.Unknown);
        academy.PreviousOfstedRating.Should().Be(OfstedRating.Unknown);
        academy.ShortInspection.Should().Be(OfstedShortInspection.Unknown);
    }

    [Fact]
    public async Task GetAcademiesOfstedAsync_should_not_look_up_predecessor_when_urn_is_found_in_ofsted_data()
    {
        SetupAcademiesInTrust("700001");
        AddSchoolOfsted(700001, new MisEstablishmentDto
        {
            QualityOfEducation = "1",
            InspectionStartDate = "15/05/2023"
        });

        await _sut.GetAcademiesOfstedAsync(TrustReferenceNumber);

        await _mockGetEstablishments.Received(1).GetEstablishmentsByUrns(
            Arg.Is<List<int>>(urns => urns.SequenceEqual(UrnFoundInOfstedData)));
    }

    [Fact]
    public async Task GetAcademiesOfstedAsync_should_log_error_when_ofsted_ratings_are_unrecognised()
    {
        SetupAcademiesInTrust("600001", "600002");
        AddSchoolOfsted(600001, new MisEstablishmentDto { OverallEffectiveness = "not a valid score" });
        AddSchoolOfsted(600002, new MisEstablishmentDto
        {
            OverallEffectiveness = "1",
            InspectionStartDate = "01/01/2022"
        });

        var result = await _sut.GetAcademiesOfstedAsync(TrustReferenceNumber);

        result.Should().ContainSingle(a => a.Urn == "600001").Which.CurrentOfstedRating.OverallEffectiveness
            .Should().Be(OfstedRatingScore.Unknown);
        _mockLogger.VerifyLogError(
            "URN 600001 has some unrecognised ofsted ratings. This could be a data integrity issue with the Ofsted data in Academies Db.");
        _mockLogger.VerifyDidNotReceive("URN 600002 has some unrecognised ofsted ratings");
    }

    [Fact]
    public async Task
        GetAcademiesOfstedAsync_should_replace_non_further_education_single_headline_grades_issued_on_or_after_2_september_2024()
    {
        // Single headline grades stopped being issued on 2 September 2024 for non-further education.
        SetupAcademiesInTrust("400001", "400002", "400003", "400005", "400006");
        AddSchoolOfsted(400001, new MisEstablishmentDto
        {
            OverallEffectiveness = "1",
            InspectionStartDate = "01/01/2025",
            PreviousFullInspectionOverallEffectiveness = "3",
            PreviousInspectionStartDate = "01/01/2021"
        });
        AddSchoolOfsted(400002, new MisEstablishmentDto
        {
            OverallEffectiveness = "2",
            InspectionStartDate = "02/09/2024"
        });
        AddSchoolOfsted(400003, new MisEstablishmentDto
        {
            OverallEffectiveness = "Not judged",
            InspectionStartDate = "01/01/2025",
            PreviousFullInspectionOverallEffectiveness = "3",
            PreviousInspectionStartDate = "12/12/2024"
        });
        AddSchoolOfsted(400005, new MisEstablishmentDto
        {
            OverallEffectiveness = "1",
            InspectionStartDate = "01/09/2024",
            PreviousFullInspectionOverallEffectiveness = "2",
            PreviousInspectionStartDate = "01/01/2021"
        });
        AddSchoolOfsted(400006, new MisEstablishmentDto
        {
            OverallEffectiveness = "1",
            QualityOfEducation = "3",
            InspectionStartDate = "01/01/2025",
            PreviousFullInspectionOverallEffectiveness = "2",
            PreviousQualityOfEducation = "4",
            PreviousInspectionStartDate = "03/09/2024"
        });

        var result = await _sut.GetAcademiesOfstedAsync(TrustReferenceNumber);

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
                academy.CurrentOfstedRating.OverallEffectiveness.Should().Be(OfstedRatingScore.Outstanding);
                academy.PreviousOfstedRating.OverallEffectiveness.Should().Be(OfstedRatingScore.Good);
            },
            academy =>
            {
                academy.CurrentOfstedRating.OverallEffectiveness.Should()
                    .Be(OfstedRatingScore.SingleHeadlineGradeNotAvailable);
                academy.CurrentOfstedRating.QualityOfEducation.Should().Be(OfstedRatingScore.RequiresImprovement);
                academy.PreviousOfstedRating.OverallEffectiveness.Should()
                    .Be(OfstedRatingScore.SingleHeadlineGradeNotAvailable);
                academy.PreviousOfstedRating.QualityOfEducation.Should().Be(OfstedRatingScore.Inadequate);
            });

        _mockLogger.VerifyLogErrors(
            "URN 400001 has a current Ofsted single headline grade of Outstanding issued",
            "URN 400002 has a current Ofsted single headline grade of Good issued",
            "URN 400003 has a previous Ofsted single headline grade of RequiresImprovement issued",
            "URN 400006 has a current Ofsted single headline grade of Outstanding issued",
            "URN 400006 has a previous Ofsted single headline grade of Good issued");
        _mockLogger.VerifyDidNotReceive("URN 400005");
    }

    [Fact]
    public async Task
        GetAcademiesOfstedAsync_should_keep_further_education_single_headline_grades_issued_after_2_september_2024()
    {
        SetupAcademiesInTrust("410001");
        AddFurtherEducationOfsted(410001, new MisFurtherEducationEstablishmentDto
        {
            OverallEffectiveness = "1",
            PreviousOverallEffectiveness = "3",
            LastDayOfInspection = "01/01/2025",
            PreviousLastDayOfInspection = "12/12/2024"
        });

        var result = await _sut.GetAcademiesOfstedAsync(TrustReferenceNumber);

        var actual = result.Should().ContainSingle().Subject;
        actual.CurrentOfstedRating.OverallEffectiveness.Should().Be(OfstedRatingScore.Outstanding);
        actual.PreviousOfstedRating.OverallEffectiveness.Should().Be(OfstedRatingScore.RequiresImprovement);
        _mockLogger.VerifyDidNotReceive();
    }

    [Fact]
    public async Task GetSchoolOfstedAsync_should_set_establishment_name_and_joined_date()
    {
        const int urn = 987654;
        _mockGetEstablishments.GetEstablishment(urn).Returns(new EstablishmentDto
        {
            Urn = urn.ToString(),
            Name = "Test School"
        });
        _mockSchoolRepository.GetJoinedDate(urn.ToString()).Returns("13/06/2023");

        var result = await _sut.GetSchoolOfstedAsync(urn);

        result.Urn.Should().Be(urn.ToString());
        result.EstablishmentName.Should().Be("Test School");
        result.DateEstablishmentJoinedGroup.Should().Be(new DateTime(2023, 6, 13));
        await _mockGetEstablishments.Received(1).GetEstablishment(urn);
        await _mockSchoolRepository.Received(1).GetJoinedDate(urn.ToString());
    }

    [Fact]
    public void GetLatestSafeGuardingInspection_should_use_the_previous_report_card_when_it_is_more_recent()
    {
        var reportCards = new ReportCardServiceModel
        {
            LatestReportCard = ReportCard(new DateOnly(2023, 1, 1), "Yes", "None"),
            PreviousReportCard = ReportCard(new DateOnly(2024, 5, 1), "No", "Special measures")
        };

        var result = _sut.GetLatestSafeGuardingInspection(reportCards, SchoolWithOlderInspections());

        result.InspectionDate.Should().Be(new DateOnly(2024, 5, 1));
        result.SafeGuarding.Should().Be("No");
        result.Concerns.Should().Be("Special measures");
        result.DateJoinedTrust.Should().Be(DateJoined);
    }

    [Fact]
    public void GetLatestSafeGuardingInspection_should_keep_the_latest_report_card_when_inspection_dates_are_equal()
    {
        var inspectionDate = new DateOnly(2024, 5, 1);
        var reportCards = new ReportCardServiceModel
        {
            LatestReportCard = ReportCard(inspectionDate, "Yes", "None"),
            PreviousReportCard = ReportCard(inspectionDate, "No", "Notice to improve")
        };

        var result = _sut.GetLatestSafeGuardingInspection(reportCards, SchoolWithOlderInspections());

        result.InspectionDate.Should().Be(inspectionDate);
        result.SafeGuarding.Should().Be("Yes");
        result.Concerns.Should().Be("None");
    }

    [Fact]
    public void GetLatestSafeGuardingInspection_should_prefer_a_report_card_over_older_inspection_data()
    {
        var school = SchoolWithOlderInspections(
            current: Rating(CategoriesOfConcern.SeriousWeakness, SafeguardingScore.No, new DateTime(2025, 1, 1)));
        var reportCards = new ReportCardServiceModel
        {
            LatestReportCard = ReportCard(new DateOnly(2024, 1, 1), "Yes", "None")
        };

        var result = _sut.GetLatestSafeGuardingInspection(reportCards, school);

        result.InspectionDate.Should().Be(new DateOnly(2024, 1, 1));
        result.SafeGuarding.Should().Be("Yes");
        result.Concerns.Should().Be("None");
    }

    [Fact]
    public void GetLatestSafeGuardingInspection_should_use_the_previous_ofsted_inspection_when_it_is_more_recent()
    {
        var school = SchoolWithOlderInspections(
            current: Rating(CategoriesOfConcern.NoConcerns, SafeguardingScore.Yes, new DateTime(2021, 5, 1)),
            previous: Rating(CategoriesOfConcern.NoticeToImprove, SafeguardingScore.No, new DateTime(2023, 5, 1)));

        var result = _sut.GetLatestSafeGuardingInspection(new ReportCardServiceModel(), school);

        result.InspectionDate.Should().Be(new DateOnly(2023, 5, 1));
        result.SafeGuarding.Should().Be("No");
        result.Concerns.Should().Be("Notice to improve");
    }

    [Fact]
    public void GetLatestSafeGuardingInspection_should_keep_the_current_ofsted_inspection_when_dates_are_equal()
    {
        var inspectionDate = new DateTime(2022, 5, 1);
        var school = SchoolWithOlderInspections(
            current: Rating(CategoriesOfConcern.NoConcerns, SafeguardingScore.Yes, inspectionDate),
            previous: Rating(CategoriesOfConcern.SpecialMeasures, SafeguardingScore.No, inspectionDate));

        var result = _sut.GetLatestSafeGuardingInspection(new ReportCardServiceModel(), school);

        result.InspectionDate.Should().Be(DateOnly.FromDateTime(inspectionDate));
        result.SafeGuarding.Should().Be("Yes");
        result.Concerns.Should().Be("None");
    }

    [Fact]
    public void GetLatestSafeGuardingInspection_should_not_use_previous_ofsted_data_when_the_current_inspection_is_undated()
    {
        var school = SchoolWithOlderInspections(
            current: Rating(CategoriesOfConcern.SpecialMeasures, SafeguardingScore.No, null),
            previous: Rating(CategoriesOfConcern.NoConcerns, SafeguardingScore.Yes, new DateTime(2023, 5, 1)));

        var result = _sut.GetLatestSafeGuardingInspection(new ReportCardServiceModel(), school);

        result.InspectionDate.Should().BeNull();
        result.SafeGuarding.Should().BeEmpty();
        result.Concerns.Should().BeEmpty();
    }

    [Fact]
    public void GetLatestSafeGuardingInspection_should_return_empty_details_when_there_are_no_dated_inspections()
    {
        var result = _sut.GetLatestSafeGuardingInspection(
            new ReportCardServiceModel(),
            SchoolWithOlderInspections());

        result.Should().BeEquivalentTo(new SafeGuardingAndConcernsServiceModel(DateJoined)
        {
            InspectionDate = null,
            SafeGuarding = string.Empty,
            Concerns = string.Empty
        });
        result.WhenDidCurrentInspectionHappen.Should().Be(Data.Enums.BeforeOrAfterJoining.NotYetInspected);
    }

    [Fact]
    public void GetLatestSafeGuardingInspection_should_ignore_a_previous_report_card_when_the_latest_is_missing()
    {
        var school = SchoolWithOlderInspections(
            current: Rating(CategoriesOfConcern.SeriousWeakness, SafeguardingScore.Yes, new DateTime(2022, 3, 4)));
        var reportCards = new ReportCardServiceModel
        {
            PreviousReportCard = ReportCard(new DateOnly(2024, 1, 1), "No", "Special measures")
        };

        var result = _sut.GetLatestSafeGuardingInspection(reportCards, school);

        result.InspectionDate.Should().Be(new DateOnly(2022, 3, 4));
        result.SafeGuarding.Should().Be("Yes");
        result.Concerns.Should().Be("Serious weakness");
    }

    private List<EstablishmentDto> BuildEstablishmentsByUrns(List<int> requestedUrns)
    {
        var results = new List<EstablishmentDto>();

        foreach (var urn in requestedUrns)
        {
            var urnString = urn.ToString();
            _ofstedEstablishmentsByUrn.TryGetValue(urn, out var ofsted);

            if (_predecessorUrnsByCurrentUrn.TryGetValue(urnString, out var predecessors) && predecessors.Count > 0)
            {
                results.AddRange(predecessors.Select(predecessor =>
                    CreateEstablishment(urnString, predecessor, ofsted)));
                continue;
            }

            if (ofsted is not null)
            {
                results.Add(CreateEstablishment(urnString, null, ofsted));
            }
        }

        return results;
    }

    private static EstablishmentDto CreateEstablishment(string urn, string? predecessorUrn, EstablishmentDto? ofsted)
    {
        var establishment = new EstablishmentDto
        {
            Urn = urn,
            PreviousEstablishment = predecessorUrn is null
                ? null
                : new PreviousEstablishmentDto { Urn = predecessorUrn }
        };

        if (ofsted?.MISEstablishment is not null)
        {
            establishment.MISEstablishment = ofsted.MISEstablishment;
        }

        if (ofsted?.MisFurtherEducationEstablishment is not null)
        {
            establishment.MisFurtherEducationEstablishment = ofsted.MisFurtherEducationEstablishment;
        }

        return establishment;
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
        if (!_predecessorUrnsByCurrentUrn.TryGetValue(currentUrn, out var predecessorUrns))
        {
            predecessorUrns = [];
            _predecessorUrnsByCurrentUrn[currentUrn] = predecessorUrns;
        }

        predecessorUrns.Add(predecessorUrn);
    }

    private void AddSchoolOfsted(int urn, MisEstablishmentDto ofstedData)
    {
        var establishment = GetOrCreateOfstedEstablishment(urn);
        establishment.MISEstablishment = ofstedData;
    }

    private void AddFurtherEducationOfsted(int urn, MisFurtherEducationEstablishmentDto ofstedData)
    {
        var establishment = GetOrCreateOfstedEstablishment(urn);
        establishment.MisFurtherEducationEstablishment = ofstedData;
    }

    private EstablishmentDto GetOrCreateOfstedEstablishment(int urn)
    {
        if (!_ofstedEstablishmentsByUrn.TryGetValue(urn, out var establishment))
        {
            establishment = new EstablishmentDto { Urn = urn.ToString() };
            _ofstedEstablishmentsByUrn[urn] = establishment;
        }

        return establishment;
    }

    private static SchoolOfsted SchoolWithOlderInspections(OfstedRating? current = null, OfstedRating? previous = null)
    {
        return new SchoolOfsted(
            "123456",
            "Test School",
            DateJoined,
            OfstedShortInspection.Unknown,
            previous ?? OfstedRating.NotInspected,
            current ?? OfstedRating.NotInspected,
            false);
    }

    private static OfstedRating Rating(
        CategoriesOfConcern categoryOfConcern,
        SafeguardingScore safeguarding,
        DateTime? inspectionDate)
    {
        return new OfstedRating(
            OfstedRatingScore.Good,
            OfstedRatingScore.Good,
            OfstedRatingScore.Good,
            OfstedRatingScore.Good,
            OfstedRatingScore.Good,
            OfstedRatingScore.NotInspected,
            OfstedRatingScore.NotInspected,
            categoryOfConcern,
            safeguarding,
            inspectionDate);
    }

    private static ReportCardDetails ReportCard(DateOnly inspectionDate, string safeguarding, string concerns)
    {
        return new ReportCardDetails(
            inspectionDate,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            safeguarding,
            null,
            concerns);
    }
}
