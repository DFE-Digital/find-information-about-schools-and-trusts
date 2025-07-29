using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Data.Repositories;
using DfE.FindInformationAcademiesTrusts.Pages.Schools.Governance;
using DfE.FindInformationAcademiesTrusts.Pages.Shared.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.School;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Pages.Schools.Governance;

public abstract class BaseGovernanceAreaModelTests<T> : BaseSchoolPageTests<T> where T : GovernanceAreaModel
{
    [Fact]
    public override async Task OnGetAsync_should_configure_PageMetadata_PageName()
    {
        await Sut.OnGetAsync();

        Sut.PageMetadata.PageName.Should().Be("Governance");
    }

    [Fact]
    public override async Task OnGetAsync_sets_correct_data_source_list()
    {
        _ = await Sut.OnGetAsync();
        await MockDataSourceService.Received(1).GetAsync(Source.Gias);

        Sut.DataSourcesPerPage.Should().BeEquivalentTo([
            new DataSourcePageListEntry("Current governors", [
                new DataSourceListEntry(Mocks.MockDataSourceService.Gias)
            ]),
            new DataSourcePageListEntry("Historic governors",
                [new DataSourceListEntry(Mocks.MockDataSourceService.Gias)])
        ]);
    }

    [Theory]
    [InlineData(0, "Historic governors (0)")]
    [InlineData(1, "Historic governors (1)")]
    public void Historic_SubNavTitle_should_be_generated_from_number_of_results(int number, string expected)
    {
        HistoricModel.NavTitle(number).Should().Be(expected);
    }

    [Theory]
    [InlineData(0, "Current governors (0)")]
    [InlineData(1, "Current governors (1)")]
    public void Current_SubNavTitle_should_be_generated_from_number_of_results(int number, string expected)
    {
        CurrentModel.NavTitle(number).Should().Be(expected);
    }

    [Fact]
    public async Task OnGetAsync_sets_SchoolGovernance()
    {
        var schoolGovernanceServiceModel = new SchoolGovernanceServiceModel(
            GenerateGovernors(true, "Member", 2),
            GenerateGovernors(true, "Chair", 3));

        MockSchoolService.GetSchoolGovernanceAsync(Sut.Urn)
            .Returns(schoolGovernanceServiceModel);

        await Sut.OnGetAsync();

        await MockSchoolService.Received(1).GetSchoolGovernanceAsync(Sut.Urn);
        Sut.SchoolGovernance.Should().BeEquivalentTo(schoolGovernanceServiceModel);
    }

    private static Governor[] GenerateGovernors(bool isCurrent, string role, int numberToGenerate)
    {
        return Enumerable.Repeat(new Governor(
            "9999",
            string.Empty,
            Role: role,
            FullName: "First Second Last",
            DateOfAppointment: DateTime.Today.AddYears(-3),
            DateOfTermEnd: isCurrent ? DateTime.Today.AddYears(1) : DateTime.Today.AddYears(-1),
            AppointingBody: "School board",
            Email: null
        ), numberToGenerate).ToArray();
    }
}
