using DfE.FindInformationAcademiesTrusts.Pages.Schools.Overview;
using DfE.FindInformationAcademiesTrusts.Services.School;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Pages.Schools.Overview;

public class SenModelTests : BaseOverviewAreaModelTests<SenModel>
{
    private readonly ISchoolOverviewSenService _mockSchoolOverviewSenService =
        Substitute.For<ISchoolOverviewSenService>();

    private SchoolOverviewSenServiceModel _dummySenDetails = new(
        "20",
        "25",
        "16",
        "22",
        "Resourced provision",
        ["ASD - Autistic Spectrum Disorder", "SpLD - Specific Learning Difficulty"]);

    public SenModelTests()
    {
        _mockSchoolOverviewSenService.GetSchoolOverviewSenAsync(Arg.Any<int>())
            .Returns(_dummySenDetails);

        Sut = new SenModel(MockSchoolService, MockTrustService, _mockSchoolOverviewSenService, MockDataSourceService,
            MockSchoolNavMenu);
        Sut.Urn = SchoolUrn;
    }

    [Fact]
    public override async Task OnGetAsync_should_configure_PageMetadata_SubPageName()
    {
        await Sut.OnGetAsync();

        Sut.PageMetadata.SubPageName.Should().Be("SEN (special educational needs)");
    }

    [Fact]
    public async Task OnGetAsync_should_set_SchoolOverviewSenServiceModel()
    {
        await Sut.OnGetAsync();

        Sut.SchoolOverviewSenServiceModel.Should().Be(_dummySenDetails);
    }

    [Theory]
    [InlineData(null, null, null, null, null)]
    [InlineData("2", null, null, null, null)]
    [InlineData(null, "32", null, null, null)]
    [InlineData(null, null, "77", null, null)]
    [InlineData(null, null, null, "8", null)]
    [InlineData(null, null, null, null, "Provision")]
    public async Task OnGetAsync_should_set_null_values_correctly(
        string? resourcedProvisionOnRoll,
        string? resourcedProvisionCapacity,
        string? senOnRoll,
        string? senCapacity,
        string? resourcedProvisionTypes)
    {
        _dummySenDetails = _dummySenDetails with
        {
            ResourcedProvisionOnRoll = resourcedProvisionOnRoll,
            ResourcedProvisionCapacity = resourcedProvisionCapacity,
            SenOnRoll = senOnRoll,
            SenCapacity = senCapacity,
            ResourcedProvisionTypes = resourcedProvisionTypes
        };

        var resourcedProvisionOnRollValue = resourcedProvisionOnRoll ?? "Not available";
        var resourcedProvisionCapacityValue = resourcedProvisionCapacity ?? "Not available";
        var senOnRollValue = senOnRoll ?? "Not available";
        var senCapacityValue = senCapacity ?? "Not available";
        var resourcedProvisionTypesValue = resourcedProvisionTypes ?? "Not available";

        _mockSchoolOverviewSenService.GetSchoolOverviewSenAsync(Arg.Any<int>())
            .Returns(_dummySenDetails);
        await Sut.OnGetAsync();

        Sut.ResourcedProvisionOnRoll.Should().Be(resourcedProvisionOnRollValue);
        Sut.ResourcedProvisionCapacity.Should().Be(resourcedProvisionCapacityValue);
        Sut.SenOnRoll.Should().Be(senOnRollValue);
        Sut.SenCapacity.Should().Be(senCapacityValue);
        Sut.ResourcedProvisionType.Should().Be(resourcedProvisionTypesValue);
    }

    [Fact]
    public async Task OnGetAsync_should_set_empty_sen_provision_types_correctly()
    {
        _dummySenDetails = _dummySenDetails with { SenProvisionTypes = [null!] };

        _mockSchoolOverviewSenService.GetSchoolOverviewSenAsync(Arg.Any<int>())
            .Returns(_dummySenDetails);
        await Sut.OnGetAsync();

        Sut.SenProvisionTypes.Should().Contain(["Not available"]);
    }
}
