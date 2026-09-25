using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.AcademiesDbServices;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Models.Mis_Mstr;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Repositories;
using FluentAssertions.Execution;
using GovUK.Dfe.CoreLibs.Contracts.Academies.V4.Establishments;
using Microsoft.Extensions.Logging;
using EstablishmentResponse = GovUK.Dfe.AcademiesApi.Client.Contracts.EstablishmentResponse;
using MISEstablishmentResponse = GovUK.Dfe.AcademiesApi.Client.Contracts.MISEstablishmentResponse;
using MISFEAResponse = GovUK.Dfe.AcademiesApi.Client.Contracts.MISFEAResponse;

namespace DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.UnitTests.Repositories;

public class OfstedRepositoryTests
{
    private const string TrustReferenceNumber = "TR98765";
    private readonly OfstedRepository _sut;
    private readonly MockAcademiesDbContext _mockAcademiesDbContext = new();
    private readonly IGetEstablishments _mockGetEstablishments;
    private readonly ILogger<OfstedRepository> _mockLogger = MockLogger.CreateLogger<OfstedRepository>();
    private readonly Dictionary<string, List<string>> _predecessorUrnsByCurrentUrn = new();
    private readonly Dictionary<int, EstablishmentResponse> _ofstedEstablishmentsByUrn = new();

    public OfstedRepositoryTests()
    {
        _mockGetEstablishments = Substitute.For<IGetEstablishments>();
        _mockGetEstablishments.GetEstablishmentsByTrustReferenceNumber(Arg.Any<string>()).Returns([]);
        _mockGetEstablishments.GetEstablishmentsByUrns(Arg.Any<List<int>>()).Returns([]);
        _mockGetEstablishments.GetEstablishmentsWithOfstedData(Arg.Any<int[]>())
            .Returns(callInfo => callInfo.Arg<int[]>()
                .Where(_ofstedEstablishmentsByUrn.ContainsKey)
                .Select(urn => _ofstedEstablishmentsByUrn[urn])
                .ToList());
        _sut = new OfstedRepository(_mockAcademiesDbContext.Object, _mockGetEstablishments, _mockLogger);
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

        _mockGetEstablishments.GetEstablishmentsByUrns(Arg.Any<List<int>>())
            .Returns(callInfo =>
            {
                var requestedUrns = callInfo.Arg<List<int>>();
                return requestedUrns.SelectMany(urn =>
                {
                    if (!_predecessorUrnsByCurrentUrn.TryGetValue(urn.ToString(), out var predecessors) ||
                        predecessors.Count == 0)
                    {
                        return [];
                    }

                    return predecessors.Select(predecessor => new EstablishmentDto
                    {
                        Urn = urn.ToString(),
                        PreviousEstablishment = new PreviousEstablishmentDto { Urn = predecessor }
                    });
                }).ToList();
            });
    }

    private void SetupEstablishment(int urn, string name = "Test School", string? dateJoinedTrust = "01/01/2022")
    {
        _mockGetEstablishments.GetEstablishment(urn).Returns(new EstablishmentDto
        {
            Urn = urn.ToString(),
            Name = name,
            DateJoinedTrust = dateJoinedTrust
        });
    }

    private void AddSchoolOfsted(int urn, MISEstablishmentResponse ofstedData)
    {
        var establishment = GetOrCreateOfstedEstablishment(urn);
        establishment.MisEstablishment = ofstedData;
    }

    private void AddFurtherEducationOfsted(int urn, MISFEAResponse ofstedData)
    {
        var establishment = GetOrCreateOfstedEstablishment(urn);
        establishment.MisFurtherEducationEstablishment = ofstedData;
    }

    private EstablishmentResponse GetOrCreateOfstedEstablishment(int urn)
    {
        if (!_ofstedEstablishmentsByUrn.TryGetValue(urn, out var establishment))
        {
            establishment = new EstablishmentResponse { Urn = urn.ToString() };
            _ofstedEstablishmentsByUrn[urn] = establishment;
        }

        return establishment;
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

        AddSchoolOfsted(111111, new MISEstablishmentResponse
        {
            OverallEffectiveness = "1",
            InspectionStartDate = "15/05/2023",
            DateOfLatestSection8Inspection = "20/06/2024",
            Section8InspectionOverallOutcome = "School remains Good"
        });
        AddFurtherEducationOfsted(222222, new MISFEAResponse
        {
            OverallEffectiveness = "2",
            LastDayOfInspection = "10/03/2023",
            DateOfLatestShortInspection = "01/07/2024"
        });

        var result = await _sut.GetAcademiesInTrustOfstedAsync(TrustReferenceNumber);

        using (new AssertionScope())
        {
            result.Select(a => a.Urn).Should().Equal(misUrn, furtherEdUrn, unknownUrn);

            var fromMis = result.Should().ContainSingle(a => a.Urn == misUrn).Subject;
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
        AddSchoolOfsted(987654, new MISEstablishmentResponse
        {
            OverallEffectiveness = "1",
            QualityOfEducation = "1",
            BehaviourAndAttitudes = "2",
            PersonalDevelopment = "3",
            EffectivenessOfLeadershipAndManagement = "4",
            EarlyYearsProvision = "1",
            SixthFormProvision = "2",
            InspectionStartDate = "15/05/2023",
            PreviousFullInspectionOverallEffectiveness = "2",
            PreviousQualityOfEducation = "3",
            PreviousBehaviourAndAttitudes = "4",
            PreviousPersonalDevelopment = "1",
            PreviousEffectivenessOfLeadershipAndManagement = "2",
            PreviousEarlyYearsProvision = "3",
            PreviousSixthFormProvision = "4",
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
        AddFurtherEducationOfsted(987654, new MISFEAResponse
        {
            OverallEffectiveness = "1",
            QualityOfEducation = "2",
            BehaviourAndAttitudes = "3",
            PersonalDevelopment = "4",
            EffectivenessOfLeadershipAndManagement = "1",
            LastDayOfInspection = "15/05/2023",
            PreviousOverallEffectiveness = "2",
            PreviousQualityOfEducation = "3",
            PreviousBehaviourAndAttitudes = "4",
            PreviousPersonalDevelopment = "1",
            PreviousEffectivenessOfLeadershipAndManagement = "2",
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
        AddSchoolOfsted(900001, new MISEstablishmentResponse
        {
            EarlyYearsProvision = "1"
        });
        AddFurtherEducationOfsted(900001, new MISFEAResponse());
        AddFurtherEducationOfsted(900002, new MISFEAResponse());

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
        AddSchoolOfsted(500001, new MISEstablishmentResponse());
        AddFurtherEducationOfsted(500002, new MISFEAResponse());

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
        AddSchoolOfsted(899999, new MISEstablishmentResponse
        {
            QualityOfEducation = "1",
            PreviousQualityOfEducation = "3"
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
        const string multiplePredecessorsUrn = "300003";

        SetupAcademiesInTrust(multiplePredecessorsUrn);
        AddPredecessorLink(multiplePredecessorsUrn, "388888");
        AddPredecessorLink(multiplePredecessorsUrn, "377777");

        var result = await _sut.GetAcademiesInTrustOfstedAsync(TrustReferenceNumber);

        var academy = result.Should().ContainSingle().Subject;
        academy.CurrentOfstedRating.Should().Be(OfstedRating.Unknown);
        academy.PreviousOfstedRating.Should().Be(OfstedRating.Unknown);
        academy.ShortInspection.Should().Be(OfstedShortInspection.Unknown);
    }

    [Fact]
    public async Task GetAcademiesInTrustOfstedAsync_should_not_look_up_predecessor_when_urn_is_found_in_ofsted_data()
    {
        SetupAcademiesInTrust("700001");
        AddSchoolOfsted(700001, new MISEstablishmentResponse
        {
            QualityOfEducation = "1"
        });

        await _sut.GetAcademiesInTrustOfstedAsync(TrustReferenceNumber);

        await _mockGetEstablishments.DidNotReceive().GetEstablishmentsByUrns(Arg.Any<List<int>>());
    }

    [Fact]
    public async Task GetAcademiesInTrustOfstedAsync_should_log_error_when_ofsted_ratings_are_unrecognised()
    {
        SetupAcademiesInTrust("600001", "600002");
        AddSchoolOfsted(600001, new MISEstablishmentResponse { OverallEffectiveness = "not a valid score" });
        AddSchoolOfsted(600002, new MISEstablishmentResponse
        {
            OverallEffectiveness = "1",
            InspectionStartDate = "01/01/2022"
        });

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
        SetupAcademiesInTrust("400001", "400002", "400003", "400005");
        AddSchoolOfsted(400001, new MISEstablishmentResponse
        {
            OverallEffectiveness = "1",
            InspectionStartDate = "01/01/2025",
            PreviousFullInspectionOverallEffectiveness = "3",
            PreviousInspectionStartDate = "01/01/2021"
        });
        AddSchoolOfsted(400002, new MISEstablishmentResponse
        {
            OverallEffectiveness = "2",
            InspectionStartDate = "02/09/2024"
        });
        AddSchoolOfsted(400003, new MISEstablishmentResponse
        {
            OverallEffectiveness = "Not judged",
            InspectionStartDate = "01/01/2025",
            PreviousFullInspectionOverallEffectiveness = "3",
            PreviousInspectionStartDate = "12/12/2024"
        });
        AddSchoolOfsted(400005, new MISEstablishmentResponse
        {
            OverallEffectiveness = "1",
            InspectionStartDate = "01/09/2024",
            PreviousFullInspectionOverallEffectiveness = "2",
            PreviousInspectionStartDate = "01/01/2021"
        });

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
                academy.CurrentOfstedRating.OverallEffectiveness.Should().Be(OfstedRatingScore.Outstanding);
                academy.PreviousOfstedRating.OverallEffectiveness.Should().Be(OfstedRatingScore.Good);
            });

        _mockLogger.VerifyLogErrors(
            "URN 400001 has a current Ofsted single headline grade of Outstanding issued",
            "URN 400002 has a current Ofsted single headline grade of Good issued",
            "URN 400003 has a previous Ofsted single headline grade of RequiresImprovement issued");
        _mockLogger.VerifyDidNotReceive("URN 400005");
    }

    [Fact]
    public async Task
        GetAcademiesInTrustOfstedAsync_should_keep_further_education_single_headline_grades_issued_after_2_september_2024()
    {
        SetupAcademiesInTrust("410001");
        AddFurtherEducationOfsted(410001, new MISFEAResponse
        {
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

    [Fact]
    public async Task
        GetOfstedInspectionHistorySummaryAsync_when_current_inspection_date_is_missing_then_CurrentInspection_has_null_InspectionDate()
    {
        AddSchoolOfsted(123456, new MISEstablishmentResponse
        {
            InspectionStartDate = null,
            OverallEffectiveness = "1",
            PreviousInspectionStartDate = "01/01/2012",
            PreviousFullInspectionOverallEffectiveness = "1"
        });

        var result = await _sut.GetOfstedInspectionHistorySummaryAsync(123456);

        result.CurrentInspection.InspectionDate.Should().BeNull();
        result.PreviousInspection.InspectionDate.Should().NotBeNull();
    }

    [Fact]
    public async Task
        GetOfstedInspectionHistorySummaryAsync_when_previous_inspection_date_is_missing_then_PreviousInspection_has_null_InspectionDate()
    {
        AddSchoolOfsted(123456, new MISEstablishmentResponse
        {
            InspectionStartDate = "01/01/2022",
            OverallEffectiveness = "1",
            PreviousInspectionStartDate = null,
            PreviousFullInspectionOverallEffectiveness = "1"
        });

        var result = await _sut.GetOfstedInspectionHistorySummaryAsync(123456);

        result.CurrentInspection.InspectionDate.Should().NotBeNull();
        result.PreviousInspection.InspectionDate.Should().BeNull();
    }

    [Fact]
    public async Task
        GetOfstedInspectionHistorySummaryAsync_when_previous_overall_effectiveness_is_missing_then_PreviousInspection_has_NotInspected_InspectionOutcome()
    {
        AddSchoolOfsted(123456, new MISEstablishmentResponse
        {
            InspectionStartDate = "01/01/2022",
            OverallEffectiveness = "1",
            PreviousInspectionStartDate = "01/01/2012",
            PreviousFullInspectionOverallEffectiveness = null
        });

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

    [Fact]
    public async Task
        GetOfstedShortInspectionAsync_when_establishment_exists_then_returns_ShortInspection_with_correct_data()
    {
        _mockAcademiesDbContext.MisMstrEstablishmentFiat.AddRange([
            new MisMstrEstablishmentFiat
            {
                Urn = 123456,
                DateOfLatestSection8Inspection = "01/01/2025",
                Section8InspectionOverallOutcome = "School remains Good"
            },
            new MisMstrEstablishmentFiat
            {
                Urn = 987654,
                DateOfLatestSection8Inspection = "06/06/2023",
                Section8InspectionOverallOutcome = "School remains Outstanding"
            }
        ]);

        var result = await _sut.GetOfstedShortInspectionAsync(123456);

        result.InspectionDate.Should().Be(new DateTime(2025, 1, 1));
        result.InspectionOutcome.Should().Be("School remains Good");
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
    public async Task GetSchoolOfstedRatingsAsync_should_set_establishment_name_and_date_joined_trust()
    {
        const int urn = 987654;
        SetupEstablishment(urn, "Test School", "13/06/2023");

        var result = await _sut.GetSchoolOfstedRatingsAsync(urn);

        result.Urn.Should().Be(urn.ToString());
        result.EstablishmentName.Should().Be("Test School");
        result.DateAcademyJoinedTrust.Should().Be(new DateTime(2023, 6, 13));
        await _mockGetEstablishments.Received(1).GetEstablishment(urn);
    }

    [Fact]
    public async Task
        GetSchoolOfstedRatingsAsync_should_leave_DateAcademyJoinedTrust_null_when_establishment_has_no_joined_date()
    {
        const int urn = 987654;
        SetupEstablishment(urn, "Independent school", null);

        var result = await _sut.GetSchoolOfstedRatingsAsync(urn);

        result.DateAcademyJoinedTrust.Should().BeNull();
    }
}
