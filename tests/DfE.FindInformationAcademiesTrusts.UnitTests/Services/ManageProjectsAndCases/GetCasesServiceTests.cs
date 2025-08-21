using Dfe.CaseAggregationService.Client.Contracts;
using DfE.FindInformationAcademiesTrusts.Services.ManageProjectsAndCases;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Services.ManageProjectsAndCases;

public class GetCasesServiceTests
{
    private readonly GetCasesService _sut;
    private readonly ICasesClient _mockCasesClient = Substitute.For<ICasesClient>();

    public GetCasesServiceTests()
    {
        _sut = new GetCasesService(_mockCasesClient);
    }

    [Fact]
    public async Task GetCasesReturnsCases()
    {
        _mockCasesClient.GetCasesByUserAsync(Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Is<bool>(x => x == false),
                Arg.Any<bool>(),
                Arg.Any<bool>(),
                Arg.Any<bool>(),
                Arg.Any<bool>(),
                Arg.Is<bool>(x => x == false),
                Arg.Any<IEnumerable<string>>(),
                Arg.Is<string>(x => x == ""),
                Arg.Any<SortCriteria>(),
                Arg.Any<int>(),
                Arg.Any<int>(),
                Arg.Is<string>(x => x == "1"))
            .Returns(new GetCasesByUserResponseModel { CaseInfos = new List<UserCaseInfo>(), TotalRecordCount = 0 });

        var result = await _sut.GetCasesAsync(new GetCasesParameters("n", "m", false, false, false, false, 1, 25, [],
            SortCriteria.CreatedDateAscending));

        await _mockCasesClient.Received(1).GetCasesByUserAsync(Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<bool>(),
            Arg.Any<bool>(),
            Arg.Any<bool>(),
            Arg.Any<bool>(),
            Arg.Any<bool>(),
            Arg.Any<bool>(),
            Arg.Any<IEnumerable<string>>(),
            Arg.Any<string>(),
            Arg.Any<SortCriteria>(),
            Arg.Any<int>(),
            Arg.Any<int>(),
            Arg.Any<string>());

        result.Should().NotBeNull();
        result.Count.Should().Be(0);
        result.PageStatus.TotalResults.Should().Be(0);
    }
}
