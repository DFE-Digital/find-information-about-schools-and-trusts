using DfE.FindInformationAcademiesTrusts.Pages.Trusts.Academies.InTrust;
using DfE.FindInformationAcademiesTrusts.Services.Academy;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Pages.Trusts.Academies.InTrust;

public class FreeSchoolMealsModelTests : AcademiesInTrustAreaModelTests<FreeSchoolMealsModel>
{
    public FreeSchoolMealsModelTests()
    {
        Sut = new FreeSchoolMealsModel(MockDataSourceService,
                MockTrustService,
                MockAcademyService,
                MockAcademiesExportService,
                MockDateTimeProvider)
            { Uid = TrustUid };
    }

    [Fact]
    public override async Task OnGetAsync_sets_academies_from_academyService()
    {
        var academies = new[]
        {
            new AcademyFreeSchoolMealsServiceModel("1", "Academy 1", 12.5, 13.5, 14.5),
            new AcademyFreeSchoolMealsServiceModel("2", "Academy 2", null, 70.1, 64.1),
            new AcademyFreeSchoolMealsServiceModel("3", "Academy 3", 8.2, 4, 10)
        };
        MockAcademyService.GetAcademiesInTrustFreeSchoolMealsAsync(Sut.ReferenceNumber).Returns(Task.FromResult(academies));

        _ = await Sut.OnGetAsync();

        Sut.Academies.Should().BeEquivalentTo(academies);
    }

    [Fact]
    public override async Task OnGetAsync_should_configure_TrustPageMetadata_TabPageName()
    {
        _ = await Sut.OnGetAsync();

        Sut.PageMetadata.TabName.Should().Be("Free school meals");
    }

    [Fact]
    public override async Task OnGetAsync_should_set_active_TabList_to_current_tab()
    {
        _ = await Sut.OnGetAsync();

        Sut.TabList.Should().ContainSingle(l => l.LinkIsActive)
            .Which.AspPage.Should().Be("./FreeSchoolMeals");
    }
}
