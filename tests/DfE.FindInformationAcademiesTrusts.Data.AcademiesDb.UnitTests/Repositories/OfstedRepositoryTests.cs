using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Models.Gias;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Models.Mis_Mstr;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Repositories;
using FluentAssertions.Execution;
using Microsoft.Extensions.Logging;

namespace DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.UnitTests.Repositories;

public class OfstedRepositoryTests
{
    private const string GroupUid = "1234";
    private readonly OfstedRepository _sut;
    private readonly MockAcademiesDbContext _mockAcademiesDbContext = new();
    private readonly ILogger<AcademyRepository> _mockLogger = MockLogger.CreateLogger<AcademyRepository>();

    public OfstedRepositoryTests()
    {
        _sut = new OfstedRepository(_mockAcademiesDbContext.Object, _mockLogger);

        _mockAcademiesDbContext.AddGiasGroupForTrust(GroupUid);
    }

    [Fact]
    public async Task GetAcademiesInTrustOfstedAsync_should_return_empty_array_when_no_academies_linked_to_trust()
    {
        var result = await _sut.GetAcademiesInTrustOfstedAsync(GroupUid);
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAcademiesInTrustOfstedAsync_should_only_return_academies_linked_to_trust()
    {
        _mockAcademiesDbContext.AddGiasGroupLinks("some other trust", "some other academy");

        var giasGroupLinks = _mockAcademiesDbContext.AddGiasGroupLinks(GroupUid, 6);

        var result = await _sut.GetAcademiesInTrustOfstedAsync(GroupUid);

        result.Select(a => a.Urn).Should().BeEquivalentTo(giasGroupLinks.Select(g => g.Urn));
        result.Select(a => a.EstablishmentName).Should()
            .BeEquivalentTo(giasGroupLinks.Select(g => g.EstablishmentName));
    }

    [Fact]
    public async Task GetAcademiesInTrustOfstedAsync_should_set_EstablishmentName_from_giasGroupLink()
    {
        var giasGroupLinks = _mockAcademiesDbContext.AddGiasGroupLinks(GroupUid, 6);

        var result = await _sut.GetAcademiesInTrustOfstedAsync(GroupUid);

        result.Select(a => a.EstablishmentName).Should()
            .BeEquivalentTo(giasGroupLinks.Select(g => g.EstablishmentName));
    }

    [Fact]
    public async Task GetAcademiesInTrustOfstedAsync_should_set_DateAcademyJoinedTrust_from_giasGroupLink()
    {
        var giasGroupLinks = _mockAcademiesDbContext.AddGiasGroupLinks(GroupUid, 3);
        giasGroupLinks[0].JoinedDate = "01/01/2022";
        giasGroupLinks[1].JoinedDate = "29/02/2024";
        giasGroupLinks[2].JoinedDate = "31/12/1999";

        var result = await _sut.GetAcademiesInTrustOfstedAsync(GroupUid);

        result.Select(a => a.DateAcademyJoinedTrust)
            .Should()
            .BeEquivalentTo([
                new DateTime(2022, 01, 01),
                new DateTime(2024, 02, 29),
                new DateTime(1999, 12, 31)
            ]);
    }

    [Fact]
    public async Task GetAcademiesInTrustOfstedAsync_should_set_InspectionDate_when_not_further_ed()
    {
        _mockAcademiesDbContext.AddGiasGroupLinks(GroupUid, "987654");
        _mockAcademiesDbContext.AddEstablishmentFiat(987654, "15/05/2023");

        var result = await _sut.GetAcademiesInTrustOfstedAsync(GroupUid);

        result.Should().ContainSingle()
            .Which.CurrentOfstedRating.InspectionDate
            .Should().HaveDay(15).And.HaveMonth(5).And.HaveYear(2023);
    }

    [Fact]
    public async Task GetAcademiesInTrustOfstedAsync_should_set_InspectionDate_when_further_ed()
    {
        _mockAcademiesDbContext.AddGiasGroupLinks(GroupUid, "987654");
        _mockAcademiesDbContext.MisMstrFurtherEducationEstablishmentFiat.Add(
            new MisMstrFurtherEducationEstablishmentFiat
            {
                ProviderUrn = 987654, LastDayOfInspection = "15/05/2023", PreviousLastDayOfInspection = "01/02/2013"
            });

        var result = await _sut.GetAcademiesInTrustOfstedAsync(GroupUid);

        var actual = result.Should().ContainSingle().Subject;
        actual.CurrentOfstedRating.InspectionDate.Should().HaveDay(15).And.HaveMonth(5).And.HaveYear(2023);
        actual.PreviousOfstedRating.InspectionDate.Should().HaveDay(1).And.HaveMonth(2).And.HaveYear(2013);
    }

    [Fact]
    public async Task GetAcademiesInTrustOfstedAsync_should_log_error_for_each_urn_with_unknown_judgement()
    {
        //---Arrange---
        // Add each type of invalid current rating
        var invalidEstablishmentsFiat = new List<MisMstrEstablishmentFiat>
        {
            new() { Urn = 111111, OverallEffectiveness = "some unknown value" },
            new() { Urn = 222222, QualityOfEducation = 212 },
            new() { Urn = 333333, BehaviourAndAttitudes = 212 },
            new() { Urn = 444444, PersonalDevelopment = 212 },
            new() { Urn = 555555, EffectivenessOfLeadershipAndManagement = 212 },
            new() { Urn = 666666, EarlyYearsProvisionWhereApplicable = 212 },
            new() { Urn = 777777, SixthFormProvisionWhereApplicable = 212 },
            new() { Urn = 888888, CategoryOfConcern = "some unknown value" },
            new() { Urn = 999999, SafeguardingIsEffective = "some unknown value" }
        };
        var invalidFurtherEducationEstablishmentsFiat = new List<MisMstrFurtherEducationEstablishmentFiat>
        {
            new() { ProviderUrn = 101111, OverallEffectiveness = "212" },
            new() { ProviderUrn = 102222, QualityOfEducation = 212 },
            new() { ProviderUrn = 103333, BehaviourAndAttitudes = 212 },
            new() { ProviderUrn = 104444, PersonalDevelopment = 212 },
            new() { ProviderUrn = 105555, EffectivenessOfLeadershipAndManagement = 212 },
            new() { ProviderUrn = 106666, IsSafeguardingEffective = "some unknown value" }
        };

        // Add each type of invalid previous rating
        invalidEstablishmentsFiat.AddRange([
            new MisMstrEstablishmentFiat
                { Urn = 111109, PreviousFullInspectionOverallEffectiveness = "some unknown value" },
            new MisMstrEstablishmentFiat { Urn = 222209, PreviousQualityOfEducation = 212 },
            new MisMstrEstablishmentFiat { Urn = 333309, PreviousBehaviourAndAttitudes = 212 },
            new MisMstrEstablishmentFiat { Urn = 444409, PreviousPersonalDevelopment = 212 },
            new MisMstrEstablishmentFiat { Urn = 555509, PreviousEffectivenessOfLeadershipAndManagement = 212 },
            new MisMstrEstablishmentFiat { Urn = 666609, PreviousEarlyYearsProvisionWhereApplicable = 212 },
            new MisMstrEstablishmentFiat
                { Urn = 777709, PreviousSixthFormProvisionWhereApplicable = "some unknown value" },
            new MisMstrEstablishmentFiat { Urn = 888809, PreviousCategoryOfConcern = "some unknown value" },
            new MisMstrEstablishmentFiat { Urn = 999909, PreviousSafeguardingIsEffective = "some unknown value" }
        ]);
        invalidFurtherEducationEstablishmentsFiat.AddRange([
            new MisMstrFurtherEducationEstablishmentFiat { ProviderUrn = 101109, PreviousOverallEffectiveness = "212" },
            new MisMstrFurtherEducationEstablishmentFiat { ProviderUrn = 102209, PreviousQualityOfEducation = 212 },
            new MisMstrFurtherEducationEstablishmentFiat { ProviderUrn = 103309, PreviousBehaviourAndAttitudes = 212 },
            new MisMstrFurtherEducationEstablishmentFiat { ProviderUrn = 104409, PreviousPersonalDevelopment = 212 },
            new MisMstrFurtherEducationEstablishmentFiat
                { ProviderUrn = 105509, PreviousEffectivenessOfLeadershipAndManagement = 212 },
            new MisMstrFurtherEducationEstablishmentFiat
                { ProviderUrn = 106609, PreviousSafeguarding = "some unknown value" }
        ]);

        //Add invalid establishments to mock db
        _mockAcademiesDbContext.MisMstrEstablishmentFiat.AddRange(invalidEstablishmentsFiat);
        _mockAcademiesDbContext.MisMstrFurtherEducationEstablishmentFiat.AddRange(
            invalidFurtherEducationEstablishmentsFiat);

        //Get urns of invalid entries added
        var invalidEstablishmentUrns = invalidEstablishmentsFiat.Select(e => e.Urn)
            .Concat(invalidFurtherEducationEstablishmentsFiat.Select(e => e.ProviderUrn))
            .ToArray();

        //Add some valid establishments to ensure we're not just logging everything
        int[] validEstablishmentUrns = [123, 456];
        _mockAcademiesDbContext.MisMstrEstablishmentFiat.Add(new MisMstrEstablishmentFiat
            { Urn = validEstablishmentUrns[0] });
        _mockAcademiesDbContext.MisMstrFurtherEducationEstablishmentFiat.Add(
            new MisMstrFurtherEducationEstablishmentFiat
                { ProviderUrn = validEstablishmentUrns[1] });

        //Create group links
        foreach (var urn in validEstablishmentUrns.Concat(invalidEstablishmentUrns))
        {
            _mockAcademiesDbContext.AddGiasGroupLinks(GroupUid, urn.ToString());
        }

        //---Act---
        _ = await _sut.GetAcademiesInTrustOfstedAsync(GroupUid);

        //---Assert--
        //Check we got a log error for each invalid establishment
        VerifyLogs(invalidEstablishmentUrns, true);
        VerifyLogs(validEstablishmentUrns, false);
    }

    [Fact]
    public async Task GetAcademiesInTrustOfstedAsync_should_set_CategoryOfConcern_to_DoesNotApply_when_further_ed()
    {
        _mockAcademiesDbContext.AddGiasGroupLinks(GroupUid, "987654");
        _mockAcademiesDbContext.MisMstrFurtherEducationEstablishmentFiat.Add(
            new MisMstrFurtherEducationEstablishmentFiat
                { ProviderUrn = 987654 });

        var result = await _sut.GetAcademiesInTrustOfstedAsync(GroupUid);

        var actual = result.Should().ContainSingle().Subject;
        actual.CurrentOfstedRating.CategoryOfConcern.Should().Be(CategoriesOfConcern.DoesNotApply);
        actual.PreviousOfstedRating.CategoryOfConcern.Should().Be(CategoriesOfConcern.DoesNotApply);
    }

    [Fact]
    public async Task GetAcademiesInTrustOfstedAsync_should_handle_not_inspected_when_not_further_ed()
    {
        _mockAcademiesDbContext.AddGiasGroupLinks(GroupUid, "987654");
        _mockAcademiesDbContext.MisMstrEstablishmentFiat.Add(new MisMstrEstablishmentFiat { Urn = 987654 });

        var result = await _sut.GetAcademiesInTrustOfstedAsync(GroupUid);

        var actual = result.Should().ContainSingle().Subject;
        actual.CurrentOfstedRating.Should().Be(OfstedRating.NotInspected);
        actual.PreviousOfstedRating.Should().Be(OfstedRating.NotInspected);
    }

    [Fact]
    public async Task GetAcademiesInTrustOfstedAsync_should_handle_not_inspected_when_further_ed()
    {
        _mockAcademiesDbContext.AddGiasGroupLinks(GroupUid, "987654");
        _mockAcademiesDbContext.MisMstrFurtherEducationEstablishmentFiat.Add(
            new MisMstrFurtherEducationEstablishmentFiat { ProviderUrn = 987654 });

        var result = await _sut.GetAcademiesInTrustOfstedAsync(GroupUid);

        var actual = result.Should().ContainSingle().Subject;
        actual.CurrentOfstedRating.Should()
            .Be(OfstedRating.NotInspected with { CategoryOfConcern = CategoriesOfConcern.DoesNotApply });
        actual.PreviousOfstedRating.Should()
            .Be(OfstedRating.NotInspected with { CategoryOfConcern = CategoriesOfConcern.DoesNotApply });
    }

    [Fact]
    public async Task GetAcademiesInTrustOfstedAsync_should_set_ofsted_sub_judgements_when_not_further_ed()
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

        var result = await _sut.GetAcademiesInTrustOfstedAsync(GroupUid);

        var actual = result.Should().ContainSingle().Subject;

        actual.CurrentOfstedRating.OverallEffectiveness.Should().Be(OfstedRatingScore.Outstanding);
        actual.CurrentOfstedRating.QualityOfEducation.Should().Be(OfstedRatingScore.Outstanding);
        actual.CurrentOfstedRating.BehaviourAndAttitudes.Should().Be(OfstedRatingScore.Good);
        actual.CurrentOfstedRating.PersonalDevelopment.Should().Be(OfstedRatingScore.RequiresImprovement);
        actual.CurrentOfstedRating.EffectivenessOfLeadershipAndManagement.Should().Be(OfstedRatingScore.Inadequate);
        actual.CurrentOfstedRating.EarlyYearsProvision.Should().Be(OfstedRatingScore.Outstanding);
        actual.CurrentOfstedRating.SixthFormProvision.Should().Be(OfstedRatingScore.Good);

        actual.PreviousOfstedRating.OverallEffectiveness.Should().Be(OfstedRatingScore.Good);
        actual.PreviousOfstedRating.QualityOfEducation.Should().Be(OfstedRatingScore.RequiresImprovement);
        actual.PreviousOfstedRating.BehaviourAndAttitudes.Should().Be(OfstedRatingScore.Inadequate);
        actual.PreviousOfstedRating.PersonalDevelopment.Should().Be(OfstedRatingScore.Outstanding);
        actual.PreviousOfstedRating.EffectivenessOfLeadershipAndManagement.Should().Be(OfstedRatingScore.Good);
        actual.PreviousOfstedRating.EarlyYearsProvision.Should().Be(OfstedRatingScore.RequiresImprovement);
        actual.PreviousOfstedRating.SixthFormProvision.Should().Be(OfstedRatingScore.Inadequate);
    }

    [Fact]
    public async Task GetAcademiesInTrustOfstedAsync_should_set_ofsted_judgements_when_further_ed()
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

        var result = await _sut.GetAcademiesInTrustOfstedAsync(GroupUid);

        var actual = result.Should().ContainSingle().Subject;
        actual.CurrentOfstedRating.OverallEffectiveness.Should().Be(OfstedRatingScore.Outstanding);
        actual.CurrentOfstedRating.QualityOfEducation.Should().Be(OfstedRatingScore.Good);
        actual.CurrentOfstedRating.BehaviourAndAttitudes.Should().Be(OfstedRatingScore.RequiresImprovement);
        actual.CurrentOfstedRating.PersonalDevelopment.Should().Be(OfstedRatingScore.Inadequate);
        actual.CurrentOfstedRating.EffectivenessOfLeadershipAndManagement.Should().Be(OfstedRatingScore.Outstanding);

        actual.PreviousOfstedRating.OverallEffectiveness.Should().Be(OfstedRatingScore.Good);
        actual.PreviousOfstedRating.QualityOfEducation.Should().Be(OfstedRatingScore.RequiresImprovement);
        actual.PreviousOfstedRating.BehaviourAndAttitudes.Should().Be(OfstedRatingScore.Inadequate);
        actual.PreviousOfstedRating.PersonalDevelopment.Should().Be(OfstedRatingScore.Outstanding);
        actual.PreviousOfstedRating.EffectivenessOfLeadershipAndManagement.Should().Be(OfstedRatingScore.Good);
    }

    [Fact]
    public async Task GetAcademiesInTrustOfstedAsync_should_only_query_further_ed_for_urns_not_in_mis_establishments()
    {
        //--Arrange--
        var giasGroupLink = _mockAcademiesDbContext.AddGiasGroupLinks(GroupUid, 7);
        var allUrns = giasGroupLink.Select(gl => int.Parse(gl.Urn!)).ToArray();

        // The first three urns are set up in non-further establishments with a non-further only property
        var nonFurtherUrns = allUrns.Take(3).ToArray();
        _mockAcademiesDbContext.MisMstrEstablishmentFiat.AddRange(nonFurtherUrns.Select(urn =>
            new MisMstrEstablishmentFiat
                { Urn = urn, EarlyYearsProvisionWhereApplicable = 1 }));

        // All urns are set up in further (note that this wouldn't occur in the actual db)
        _mockAcademiesDbContext.MisMstrFurtherEducationEstablishmentFiat.AddRange(allUrns.Select(urn =>
            new MisMstrFurtherEducationEstablishmentFiat { ProviderUrn = urn }));

        //--Act--
        var results = await _sut.GetAcademiesInTrustOfstedAsync(GroupUid);

        //--Assert--
        using var scope = new AssertionScope();
        // We expect the 3 urns that went to the non-further table to have early years provision
        var fromNonFurther = results.Where(ofsted => nonFurtherUrns.Contains(int.Parse(ofsted.Urn))).ToArray();
        fromNonFurther.Should().HaveCount(3);
        fromNonFurther.Should().AllSatisfy(o =>
            o.CurrentOfstedRating.EarlyYearsProvision.Should().Be(OfstedRatingScore.Outstanding));
        // We expect the other 4 urns to have come from further ed table and so to not have early years provision
        var fromFurther = results.Except(fromNonFurther).ToArray();
        fromFurther.Should().HaveCount(4);
        fromFurther.Should().AllSatisfy(o =>
            o.CurrentOfstedRating.EarlyYearsProvision.Should().Be(OfstedRatingScore.NotInspected));
    }

    [Fact]
    public async Task
        GetAcademiesInTrustOfstedAsync_should_set_all_ofsted_judgements_for_previous_urn_when_urn_has_changed()
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

        var result = await _sut.GetAcademiesInTrustOfstedAsync(GroupUid);

        var actual = result.Should().ContainSingle().Subject;

        actual.CurrentOfstedRating.QualityOfEducation.Should().Be(OfstedRatingScore.Outstanding);
        actual.CurrentOfstedRating.BehaviourAndAttitudes.Should().Be(OfstedRatingScore.Good);
        actual.CurrentOfstedRating.PersonalDevelopment.Should().Be(OfstedRatingScore.RequiresImprovement);
        actual.CurrentOfstedRating.EffectivenessOfLeadershipAndManagement.Should().Be(OfstedRatingScore.Inadequate);
        actual.CurrentOfstedRating.EarlyYearsProvision.Should().Be(OfstedRatingScore.Outstanding);
        actual.CurrentOfstedRating.SixthFormProvision.Should().Be(OfstedRatingScore.Good);

        actual.PreviousOfstedRating.QualityOfEducation.Should().Be(OfstedRatingScore.RequiresImprovement);
        actual.PreviousOfstedRating.BehaviourAndAttitudes.Should().Be(OfstedRatingScore.Inadequate);
        actual.PreviousOfstedRating.PersonalDevelopment.Should().Be(OfstedRatingScore.Outstanding);
        actual.PreviousOfstedRating.EffectivenessOfLeadershipAndManagement.Should().Be(OfstedRatingScore.Good);
        actual.PreviousOfstedRating.EarlyYearsProvision.Should().Be(OfstedRatingScore.RequiresImprovement);
        actual.PreviousOfstedRating.SixthFormProvision.Should().Be(OfstedRatingScore.Inadequate);
    }

    [Fact]
    public async Task GetAcademiesInTrustOfstedAsync_should_return_unknown_when_urn_doesnt_have_predecessor()
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

        var result = await _sut.GetAcademiesInTrustOfstedAsync(GroupUid);

        var actual = result.Should().ContainSingle().Subject;

        actual.CurrentOfstedRating.Should().Be(OfstedRating.Unknown);
        actual.PreviousOfstedRating.Should().Be(OfstedRating.Unknown);
    }

    [Fact]
    public async Task GetAcademiesInTrustOfstedAsync_should_return_unknown_when_urn_has_multiple_predecessors()
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

        var result = await _sut.GetAcademiesInTrustOfstedAsync(GroupUid);

        var actual = result.Should().ContainSingle().Subject;

        actual.CurrentOfstedRating.Should().Be(OfstedRating.Unknown);
        actual.PreviousOfstedRating.Should().Be(OfstedRating.Unknown);
    }

    [Fact]
    public async Task
        GetAcademiesInTrustOfstedAsync_should_only_query_further_ed_for_urns_not_in_mis_establishments_when_urn_has_changed()
    {
        //--Arrange--
        var giasGroupLink = _mockAcademiesDbContext.AddGiasGroupLinks(GroupUid, 7);
        var allUrns = giasGroupLink.Select(gl => int.Parse(gl.Urn!)).ToArray();
        var linkUrns = allUrns.Select(urn => urn + 100).ToArray();

        var nonFurtherUrns = allUrns.Take(3).ToArray();

        var nonFurtherLinkUrns = linkUrns.Take(3).ToArray();
        var furtherLinkUrns = linkUrns.Skip(3).Take(4).ToArray();

        var giasEstablishmentLinks = allUrns.Select(urn => new GiasEstablishmentLink
        {
            Urn = urn.ToString(),
            LinkUrn = $"{urn + 100}",
            LinkType = "Predecessor"
        });

        _mockAcademiesDbContext.GiasEstablishmentLinks.AddRange(giasEstablishmentLinks);

        _mockAcademiesDbContext.MisMstrEstablishmentFiat.AddRange(nonFurtherLinkUrns.Select(urn =>
            new MisMstrEstablishmentFiat
                { Urn = urn, EarlyYearsProvisionWhereApplicable = 1 }));

        _mockAcademiesDbContext.MisMstrFurtherEducationEstablishmentFiat.AddRange(
            furtherLinkUrns.Select(urn => new MisMstrFurtherEducationEstablishmentFiat { ProviderUrn = urn }));

        //--Act--
        var results = await _sut.GetAcademiesInTrustOfstedAsync(GroupUid);

        //--Assert--
        using var scope = new AssertionScope();
        var fromNonFurther = results.Where(ofsted => nonFurtherUrns.Contains(int.Parse(ofsted.Urn))).ToArray();
        fromNonFurther.Should().HaveCount(3);
        fromNonFurther.Should().AllSatisfy(o =>
            o.CurrentOfstedRating.EarlyYearsProvision.Should().Be(OfstedRatingScore.Outstanding));
        var fromFurther = results.Except(fromNonFurther).ToArray();
        fromFurther.Should().HaveCount(4);
        fromFurther.Should().AllSatisfy(o =>
            o.CurrentOfstedRating.EarlyYearsProvision.Should().Be(OfstedRatingScore.NotInspected));
    }

    [Fact]
    public async Task GetAcademiesInTrustOfstedAsync_should_not_query_gias_establishment_link_when_urn_is_found_in_mis()
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

        await _sut.GetAcademiesInTrustOfstedAsync(GroupUid);

        _ = _mockAcademiesDbContext.Object.DidNotReceive().GiasEstablishmentLink;
    }

    [Fact]
    public async Task
        GetAcademiesInTrustOfstedAsync_should_log_error_and_return_ofsted_unknown_when_urn_not_found_in_mis()
    {
        var giasGroupLink = _mockAcademiesDbContext.AddGiasGroupLinks(GroupUid, 1).Single();

        var result = await _sut.GetAcademiesInTrustOfstedAsync(GroupUid);

        var academyOfsted = result.Should().ContainSingle().Which;
        academyOfsted.Urn.Should().Be(giasGroupLink.Urn);
        academyOfsted.CurrentOfstedRating.Should().Be(OfstedRating.Unknown);
        academyOfsted.PreviousOfstedRating.Should().Be(OfstedRating.Unknown);

        _mockLogger.VerifyLogError(
            $"URN {giasGroupLink.Urn} was not found in Mis.Establishments or Mis.FurtherEducationEstablishments. This indicates a data integrity issue with the Ofsted data in Academies Db.");
    }

    [Fact]
    public async Task GetAcademiesInTrustOfstedAsync_should_not_log_error_when_all_establishments_exist_and_are_valid()
    {
        var giasGroupLinks = _mockAcademiesDbContext.AddGiasGroupLinks(GroupUid, 2);
        var urns = giasGroupLinks.Select(gl => int.Parse(gl.Urn!)).ToArray();

        _mockAcademiesDbContext.MisMstrEstablishmentFiat.Add(
            new MisMstrEstablishmentFiat
            {
                Urn = urns[0],
                PreviousFullInspectionOverallEffectiveness = "1",
                PreviousInspectionStartDate = "01/01/2022"
            }
        );
        _mockAcademiesDbContext.MisMstrFurtherEducationEstablishmentFiat.Add(
            new MisMstrFurtherEducationEstablishmentFiat
            {
                ProviderUrn = urns[1],
                PreviousOverallEffectiveness = "1",
                PreviousLastDayOfInspection = "01/01/2022"
            }
        );

        await _sut.GetAcademiesInTrustOfstedAsync(GroupUid);

        _mockLogger.VerifyDidNotReceive();
    }

    [Fact]
    public async Task
        GetAcademiesInTrustOfstedAsync_should_remove_single_headline_grades_for_non_further_ed_issued_after_2nd_sept_2024()
    {
        // Arrange
        var giasGroupLinks = _mockAcademiesDbContext.AddGiasGroupLinks(GroupUid, 5);
        var urns = giasGroupLinks.Select(gl => int.Parse(gl.Urn!)).ToArray();

        _mockAcademiesDbContext.MisMstrEstablishmentFiat.AddRange([
            new MisMstrEstablishmentFiat // OverallEffectiveness set after policy change date
            {
                Urn = urns[0],
                OverallEffectiveness = "1",
                PreviousFullInspectionOverallEffectiveness = "3",
                InspectionStartDate = "01/01/2025",
                PreviousInspectionStartDate = "01/01/2021"
            },
            new MisMstrEstablishmentFiat // OverallEffectiveness set on policy change date
            {
                Urn = urns[1],
                OverallEffectiveness = "2",
                InspectionStartDate = "02/09/2024"
            },
            new MisMstrEstablishmentFiat // PreviousFullInspectionOverallEffectiveness set after policy change date
            {
                Urn = urns[2],
                OverallEffectiveness = "Not judged",
                PreviousFullInspectionOverallEffectiveness = "3",
                InspectionStartDate = "01/01/2025",
                PreviousInspectionStartDate = "12/12/2024"
            },
            new MisMstrEstablishmentFiat // PreviousFullInspectionOverallEffectiveness set on policy change date
            {
                Urn = urns[3],
                OverallEffectiveness = "Not judged",
                PreviousFullInspectionOverallEffectiveness = "4",
                InspectionStartDate = "01/01/2025",
                PreviousInspectionStartDate = "02/09/2024"
            },
            new MisMstrEstablishmentFiat // OverallEffectiveness before policy change date
            {
                Urn = urns[4],
                OverallEffectiveness = "1",
                PreviousFullInspectionOverallEffectiveness = "2",
                InspectionStartDate = "01/09/2024",
                PreviousInspectionStartDate = "01/01/2021"
            }
        ]);

        // Act
        var result = await _sut.GetAcademiesInTrustOfstedAsync(GroupUid);

        // Assert
        result.Should().SatisfyRespectively(
            rating => // OverallEffectiveness set after policy change date
            {
                rating.CurrentOfstedRating.OverallEffectiveness.Should()
                    .Be(OfstedRatingScore.SingleHeadlineGradeNotAvailable);
                rating.PreviousOfstedRating.OverallEffectiveness.Should()
                    .NotBe(OfstedRatingScore.SingleHeadlineGradeNotAvailable);
            },
            rating => // OverallEffectiveness set on policy change date
            {
                rating.CurrentOfstedRating.OverallEffectiveness.Should()
                    .Be(OfstedRatingScore.SingleHeadlineGradeNotAvailable);
                rating.PreviousOfstedRating.OverallEffectiveness.Should()
                    .NotBe(OfstedRatingScore.SingleHeadlineGradeNotAvailable);
            },
            rating => // PreviousFullInspectionOverallEffectiveness set after policy change date
            {
                rating.CurrentOfstedRating.OverallEffectiveness.Should()
                    .Be(OfstedRatingScore.SingleHeadlineGradeNotAvailable);
                rating.PreviousOfstedRating.OverallEffectiveness.Should()
                    .Be(OfstedRatingScore.SingleHeadlineGradeNotAvailable);
            },
            rating => // PreviousFullInspectionOverallEffectiveness set on policy change date
            {
                rating.CurrentOfstedRating.OverallEffectiveness.Should()
                    .Be(OfstedRatingScore.SingleHeadlineGradeNotAvailable);
                rating.PreviousOfstedRating.OverallEffectiveness.Should()
                    .Be(OfstedRatingScore.SingleHeadlineGradeNotAvailable);
            },
            rating => // OverallEffectiveness before policy change date
            {
                rating.CurrentOfstedRating.OverallEffectiveness.Should()
                    .Be(OfstedRatingScore.Outstanding);
                rating.PreviousOfstedRating.OverallEffectiveness.Should()
                    .Be(OfstedRatingScore.Good);
            }
        );

        //Assert error logging occurred
        _mockLogger.VerifyLogErrors(
            $"URN {urns[0]} has a current Ofsted single headline grade of Outstanding issued",
            $"URN {urns[1]} has a current Ofsted single headline grade of Good issued",
            $"URN {urns[2]} has a previous Ofsted single headline grade of RequiresImprovement issued",
            $"URN {urns[3]} has a previous Ofsted single headline grade of Inadequate issued"
        );
        _mockLogger.VerifyDidNotReceive(urns[4]
            .ToString()); // giasGroupLinks[4] - OverallEffectiveness before policy change date so no error log expected
    }

    [Fact]
    public async Task
        GetAcademiesInTrustOfstedAsync_should_not_remove_single_headline_grades_for_further_ed_issued_after_2nd_sept_2024()
    {
        // Arrange
        var giasGroupLinks = _mockAcademiesDbContext.AddGiasGroupLinks(GroupUid, 5);
        var urns = giasGroupLinks.Select(gl => int.Parse(gl.Urn!)).ToArray();

        _mockAcademiesDbContext.MisMstrFurtherEducationEstablishmentFiat.AddRange([
            new MisMstrFurtherEducationEstablishmentFiat // OverallEffectiveness set after policy change date
            {
                ProviderUrn = urns[0],
                OverallEffectiveness = "1",
                PreviousOverallEffectiveness = "3",
                LastDayOfInspection = "01/01/2025",
                PreviousLastDayOfInspection = "01/01/2021"
            },
            new MisMstrFurtherEducationEstablishmentFiat // OverallEffectiveness set on policy change date
            {
                ProviderUrn = urns[1],
                OverallEffectiveness = "2",
                LastDayOfInspection = "02/09/2024"
            },
            new
                MisMstrFurtherEducationEstablishmentFiat // PreviousOverallEffectiveness set after policy change date
                {
                    ProviderUrn = urns[2],
                    OverallEffectiveness = "1",
                    PreviousOverallEffectiveness = "3",
                    LastDayOfInspection = "01/01/2025",
                    PreviousLastDayOfInspection = "12/12/2024"
                },
            new
                MisMstrFurtherEducationEstablishmentFiat // PreviousOverallEffectiveness set on policy change date
                {
                    ProviderUrn = urns[3],
                    OverallEffectiveness = "2",
                    PreviousOverallEffectiveness = "4",
                    LastDayOfInspection = "01/01/2025",
                    PreviousLastDayOfInspection = "02/09/2024"
                },
            new MisMstrFurtherEducationEstablishmentFiat // OverallEffectiveness before policy change date
            {
                ProviderUrn = urns[4],
                OverallEffectiveness = "1",
                PreviousOverallEffectiveness = "2",
                LastDayOfInspection = "01/09/2024",
                PreviousLastDayOfInspection = "01/01/2021"
            }
        ]);

        // Act
        var result = await _sut.GetAcademiesInTrustOfstedAsync(GroupUid);

        // Assert
        result.Should().AllSatisfy(o =>
        {
            o.CurrentOfstedRating.OverallEffectiveness.Should()
                .NotBe(OfstedRatingScore.SingleHeadlineGradeNotAvailable);
            o.PreviousOfstedRating.OverallEffectiveness.Should()
                .NotBe(OfstedRatingScore.SingleHeadlineGradeNotAvailable);
        });

        _mockLogger.VerifyDidNotReceive();
    }

    [Fact]
    public async Task GetAcademiesInTrustOfstedAsync_should_not_log_error_for_valid_overall_effectiveness()
    {
        // Arrange
        var giasGroupLinks = _mockAcademiesDbContext.AddGiasGroupLinks(GroupUid, 8);
        var urns = giasGroupLinks.Select(gl => int.Parse(gl.Urn!)).ToArray();

        // - Add non-further eds
        _mockAcademiesDbContext.MisMstrEstablishmentFiat.AddRange([
            new MisMstrEstablishmentFiat // OverallEffectiveness before policy change date
            {
                Urn = urns[0],
                OverallEffectiveness = "1",
                PreviousFullInspectionOverallEffectiveness = "2",
                InspectionStartDate = "01/09/2024",
                PreviousInspectionStartDate = "01/01/2021"
            },
            new MisMstrEstablishmentFiat // Not judged on policy change date
            {
                Urn = urns[1],
                OverallEffectiveness = "Not judged",
                PreviousFullInspectionOverallEffectiveness = "2",
                InspectionStartDate = "02/09/2024",
                PreviousInspectionStartDate = "01/01/2021"
            },
            new MisMstrEstablishmentFiat // Not judged after policy change date
            {
                Urn = urns[2],
                OverallEffectiveness = "Not judged",
                PreviousFullInspectionOverallEffectiveness = "Not judged",
                InspectionStartDate = "20/05/2025",
                PreviousInspectionStartDate = "02/09/2024"
            },
            new MisMstrEstablishmentFiat // Not inspected
            {
                Urn = urns[3]
            }
        ]);

        // - Add further eds
        _mockAcademiesDbContext.MisMstrFurtherEducationEstablishmentFiat.AddRange([
            new MisMstrFurtherEducationEstablishmentFiat // OverallEffectiveness before policy change date
            {
                ProviderUrn = urns[4],
                OverallEffectiveness = "1",
                PreviousOverallEffectiveness = "2",
                LastDayOfInspection = "01/09/2024",
                PreviousLastDayOfInspection = "01/01/2021"
            },
            new MisMstrFurtherEducationEstablishmentFiat // Not judged on policy change date
            {
                ProviderUrn = urns[5],
                OverallEffectiveness = "Not judged",
                PreviousOverallEffectiveness = "2",
                LastDayOfInspection = "02/09/2024",
                PreviousLastDayOfInspection = "01/01/2021"
            },
            new MisMstrFurtherEducationEstablishmentFiat // Not judged after policy change date
            {
                ProviderUrn = urns[6],
                OverallEffectiveness = "Not judged",
                PreviousOverallEffectiveness = "Not judged",
                LastDayOfInspection = "20/05/2025",
                PreviousLastDayOfInspection = "02/09/2024"
            },
            new MisMstrFurtherEducationEstablishmentFiat // Not inspected
            {
                ProviderUrn = urns[7]
            }
        ]);

        // Act
        _ = await _sut.GetAcademiesInTrustOfstedAsync(GroupUid);

        _mockLogger.VerifyDidNotReceive();
    }

    [Fact]
    public async Task GetAcademiesInTrustOfstedAsync_should_include_short_inspection_data_when_not_further_education()
    {
        _mockAcademiesDbContext.AddGiasGroupLinks(GroupUid, "987654");
        _mockAcademiesDbContext.MisMstrEstablishmentFiat.Add(
            new MisMstrEstablishmentFiat()
            {
                Urn = 987654, DateOfLatestSection8Inspection = "15/05/2023", Section8InspectionOverallOutcome = "School remains Good",
            });

        var result = await _sut.GetAcademiesInTrustOfstedAsync(GroupUid);

        var actual = result.Should().ContainSingle().Subject;
        actual.ShortInspection.InspectionDate.Should().HaveDay(15).And.HaveMonth(5).And.HaveYear(2023);
        actual.ShortInspection.InspectionOutcome.Should().Be("School remains Good");
    }

    [Fact]
    public async Task GetAcademiesInTrustOfstedAsync_should_include_short_inspection_data_when_further_education()
    {
        _mockAcademiesDbContext.AddGiasGroupLinks(GroupUid, "987654");
        _mockAcademiesDbContext.MisMstrFurtherEducationEstablishmentFiat.Add(new MisMstrFurtherEducationEstablishmentFiat()
        {
            ProviderUrn = 987654,
            DateOfLatestShortInspection = "01/07/2025"
        });

        var result = await _sut.GetAcademiesInTrustOfstedAsync(GroupUid);

        var actual = result.Should().ContainSingle().Subject;
        actual.ShortInspection.InspectionDate
            .Should().HaveDay(1).And.HaveMonth(7).And.HaveYear(2025);
        actual.ShortInspection.InspectionOutcome.Should().BeNull();
    }

    [Fact]
    public async Task GetAcademiesInTrustOfstedAsync_should_include_unknown_short_inspection_when_urn_is_unknown()
    {
        _mockAcademiesDbContext.AddGiasGroupLinks(GroupUid, "987654");
        
        var result = await _sut.GetAcademiesInTrustOfstedAsync(GroupUid);
        
        result.Should().ContainSingle()
            .Which.ShortInspection.Should().BeEquivalentTo(OfstedShortInspection.Unknown);
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
    public async Task GetSchoolOfstedRatingsAsync_should_set_DateAcademyJoinedTrust_from_giasGroupLink_when_linked_to_trust()
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
            new MisMstrEstablishmentFiat()
            {
                Urn = 987654, DateOfLatestSection8Inspection = "15/05/2023", Section8InspectionOverallOutcome = "School remains Good",
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
        _mockAcademiesDbContext.MisMstrFurtherEducationEstablishmentFiat.Add(new MisMstrFurtherEducationEstablishmentFiat()
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
