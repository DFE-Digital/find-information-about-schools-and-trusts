using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Pages.Trusts.Academies.Pipeline;
using DfE.FindInformationAcademiesTrusts.Services.Academy;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Pages.Trusts.Academies.Pipeline;

public class PostDecisionModelTests : BasePipelineAcademiesAreaModelTests<PostDecisionModel>
{
    public PostDecisionModelTests()
    {
        Sut = new PostDecisionModel(
                MockDataSourceService,
                MockTrustService, MockAcademyService, MockPipelineAcademiesExportService,
                MockDateTimeProvider)
            { Uid = TrustUid };
    }

    [Fact]
    public override async Task OnGetAsync_sets_academies_from_academyService()
    {
        AcademyPipelineServiceModel[] academies =
        [
            new("1234", "Baking academy", new AgeRange(4, 16), "Bristol", "Conversion", new DateTime(2025, 3, 3)),
            new("1234", "Chocolate academy", new AgeRange(11, 18), "Birmingham", "Conversion",
                new DateTime(2025, 5, 3)),
            new("1234", "Fruity academy", new AgeRange(9, 16), "Sheffield", "Transfer", new DateTime(2025, 9, 3)),
            new(null, null, null, null, null, null)
        ];

        MockAcademyService.GetAcademiesPipelinePostAdvisoryAsync(TrustReferenceNumber)
            .Returns(Task.FromResult(academies));

        _ = await Sut.OnGetAsync();

        Sut.PostAdvisoryPipelineEstablishments.Should().BeEquivalentTo(academies);
    }

    [Fact]
    public override async Task OnGetAsync_should_configure_TrustPageMetadata_TabPageName()
    {
        _ = await Sut.OnGetAsync();

        Sut.PageMetadata.TabName.Should().Be("Post decision");
    }

    [Fact]
    public override async Task OnGetAsync_should_set_active_TabList_to_current_tab()
    {
        _ = await Sut.OnGetAsync();

        Sut.TabList.Should().ContainSingle(l => l.LinkIsActive)
            .Which.AspPage.Should().Be("./PostDecision");
    }
}
