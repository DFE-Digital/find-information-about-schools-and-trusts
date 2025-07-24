using System.Collections;
using Dfe.CaseAggregationService.Client.Contracts;
using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Pages.ManageProjectsAndCases.Overview;
using DfE.FindInformationAcademiesTrusts.Services.ManageProjectsAndCases;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Primitives;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Pages.ManageProjectsAndCases.Overview
{
    public class IndexModelTests
    {
        private readonly IndexModel _indexModel;
        private readonly IGetCasesService _getCasesService;
        private readonly IWebHostEnvironment _environment;

        private readonly IPaginatedList<UserCaseInfo> _emptyList = PaginatedList<UserCaseInfo>.Empty();

        public IndexModelTests()
        {
            _getCasesService = Substitute.For<IGetCasesService>();
            _environment = Substitute.For<IWebHostEnvironment>();

            _indexModel = new IndexModel(_getCasesService, _environment);

            SetupContext();

            _getCasesService.GetCasesAsync(Arg.Any<GetCasesParameters>()).Returns(_emptyList);
        }

        [Fact]
        public async Task OnGetAsync_DefaultPageName()
        {
            // Act
            await _indexModel.OnGetAsync();
            // Assert
            _indexModel.PageName.Should().Be("ManageMyProjectsAndCases/Overview/Index");
        }

        [Fact]
        public async Task OnGetAsync_ShouldInitializeAvailableProjectTypes()
        {
            // Arrange
            var expected = new List<string>
            {
                "Conversion",
                "Form a MAT",
                "Governance capability",
                "Non-compliance",
                "Pre-opening",
                "Pre-opening - not included in figures",
                "Safeguarding non-compliance",
                "Transfer"
            };

            IPaginatedList<UserCaseInfo> emptyList = PaginatedList<UserCaseInfo>.Empty();
            _getCasesService.GetCasesAsync(Arg.Any<GetCasesParameters>()).Returns(emptyList);

            // Act
            await _indexModel.OnGetAsync();
            // Assert
            _indexModel.Filters.AvailableProjectTypes.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task OnGetAsync_ShouldInitializeAvailableSystems()
        {
            // Arrange
            var expected = new List<string>
            {
                "Complete conversions, transfers and changes",
                "Manage free school projects",
                "Prepare conversions and transfers",
                "Record concerns and support for trusts",
            };

            IPaginatedList<UserCaseInfo> emptyList = PaginatedList<UserCaseInfo>.Empty();

            // Act
            await _indexModel.OnGetAsync();
            // Assert
            _indexModel.Filters.AvailableSystems.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task OnGetAsync_DefaultSorting()
        {
            // Act
            await _indexModel.OnGetAsync();
            // Assert
            await _getCasesService.Received(1).GetCasesAsync(Arg.Is<GetCasesParameters>(x => x.Sort == SortCriteria.CreatedDateDescending));
        }


        [Theory]
        [InlineData(ResultSorting.createdAsc, SortCriteria.CreatedDateAscending)]
        [InlineData(ResultSorting.createdDesc, SortCriteria.CreatedDateDescending)]
        [InlineData(ResultSorting.updatedAsc, SortCriteria.UpdatedDateAscending)]
        [InlineData(ResultSorting.updatedDesc, SortCriteria.UpdatedDateDescending)]
        public async Task OnGetAsync_SetSorting(string sortInput, SortCriteria expected)
        {
            // Arrange
            _indexModel.Sorting = sortInput;

            // Act
            await _indexModel.OnGetAsync();
            // Assert
            await _getCasesService.Received(1).GetCasesAsync(Arg.Is<GetCasesParameters>(x => x.Sort == expected));
        }


        [Fact]
        public async Task OnGetAsync_ShowTotal()
        {
            IPaginatedList<UserCaseInfo> paginatedList =
                new PaginatedList<UserCaseInfo>(Enumerable.Range(1, 5).Select(_ => new UserCaseInfo()), 1, 99, 25);

            _getCasesService.GetCasesAsync(Arg.Is<GetCasesParameters>(x => x.Page == 999)).Returns(Task.FromResult(paginatedList));
            
            _indexModel.PageNumber = 999;

            // Act
            await _indexModel.OnGetAsync();

            // Assert
            _indexModel.TotalProjects.Should().Be(5);
        }


        [Fact]
        public async Task OnGetAsync_DefaultSystem()
        {
            // Act
            await _indexModel.OnGetAsync();

            // Assert
            await _getCasesService.Received(1)
                .GetCasesAsync(Arg.Is<GetCasesParameters>(x =>
                    x.IncludePrepare == true &&
                    x.IncludeComplete == true &&
                    x.IncludeConcerns == true &&
                    x.IncludeManageFreeSchools == true));
        }

        [Theory]
        [ClassData(typeof(IncludeSystemData))]
        public async Task OnGetAsync_IncludeSystems(IncludeSystemTestCase testCase)
        {
            SetupContext(testCase.SelectedSystem);
            // Act
            await _indexModel.OnGetAsync();
            // Assert
            await _getCasesService.Received(1)
                .GetCasesAsync(Arg.Is<GetCasesParameters>(x =>
                    x.IncludePrepare == testCase.IncludePrepare &&
                    x.IncludeComplete == testCase.IncludeComplete &&
                    x.IncludeConcerns == testCase.IncludeConcerns &&
                    x.IncludeManageFreeSchools == testCase.IncludeManageFreeSchools));
        }

        [Theory]
        [ClassData(typeof(IncludeProjectTypeData))]
        public async Task OnGetAsync_IncludeProjectType(IncludeProjectTypeTestCase testCase)
        {
            SetupContext(selectedProjectTypes: testCase.SelectedProjectType);
            // Act
            await _indexModel.OnGetAsync();
            // Assert
            await _getCasesService.Received(1)
                .GetCasesAsync(Arg.Is<GetCasesParameters>(x => testCase.SelectedProjectType.SequenceEqual(x.ProjectFilters)));

            _indexModel.Filters.SelectedProjectTypes
                .Should().BeEquivalentTo(testCase.SelectedProjectType.ToArray());
        }

        [Fact]
        public async Task OnGetAsync_ShouldSetPageNumber()
        {
            _indexModel.PageNumber = 2;
            // Act
            await _indexModel.OnGetAsync();
            // Assert
            await _getCasesService.Received(1)
                .GetCasesAsync(Arg.Is<GetCasesParameters>(x => x.Page == 2));
        }

        [Fact]
        public async Task OnGetAsync_ShouldAlwaysBeFixedRecordCount()
        {
            // Act
            await _indexModel.OnGetAsync();
            // Assert
            await _getCasesService.Received(1)
                .GetCasesAsync(Arg.Is<GetCasesParameters>(x => x.RecordCount == 25));
        }

        private void SetupContext(StringValues? selectedSystems = null, StringValues? selectedProjectTypes = null)
        {
            var httpContext = new DefaultHttpContext();
            var query = new Dictionary<string, StringValues>();
            
            if(selectedSystems is { Count: > 0 })
            {
                query.Add("SelectedSystems", selectedSystems.Value);
            }

            if (selectedProjectTypes is { Count: > 0 })
            {
                query.Add("SelectedProjectTypes", selectedProjectTypes.Value);
            }

            httpContext.Request.Query = new QueryCollection(query);

            _indexModel.PageContext = new Microsoft.AspNetCore.Mvc.RazorPages.PageContext
            {
                HttpContext = httpContext
            };
            
            _indexModel.TempData = new TempDataDictionary(new DefaultHttpContext(), Substitute.For<ITempDataProvider>());
        }
    }

    public record IncludeSystemTestCase(
        StringValues SelectedSystem,
        bool IncludePrepare,
        bool IncludeComplete,
        bool IncludeConcerns,
        bool IncludeManageFreeSchools);

    public class IncludeSystemData : IEnumerable<object[]>
    {


        public IEnumerator<object[]> GetEnumerator()
        {
            
            yield return [new IncludeSystemTestCase(new StringValues(), true, true, true, true)];

            for (int i = 1; i < 16; i++)
            {
                yield return [BuildCase(i)];
            }

        }

        private IncludeSystemTestCase BuildCase(int index)
        {
            bool includePrepare = (index & 1) != 0;
            bool includeComplete = (index & 2) != 0;
            bool includeConcerns = (index & 4) != 0;
            bool includeManageFreeSchools = (index & 8) != 0;

            List<string> systems = new();

            if (includePrepare)
            {
                systems.Add("Prepare conversions and transfers");
            }

            if (includeComplete)
            {
                systems.Add("Complete conversions, transfers and changes");
            }

            if (includeConcerns)
            {
                systems.Add("Record concerns and support for trusts");
            }

            if (includeManageFreeSchools)
            {
                systems.Add("Manage free school projects");
            }

            return new IncludeSystemTestCase(new StringValues(systems.ToArray()), includePrepare, includeComplete,
                includeConcerns, includeManageFreeSchools);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }



    public record IncludeProjectTypeTestCase(
        StringValues SelectedProjectType);

    public class IncludeProjectTypeData : IEnumerable<object[]>
    {
        private readonly List<string> Expected = new List<string>
        {
            "Conversion",
            "Form a MAT",
            "Governance capability",
            "Non-compliance",
            "Pre-opening",
            "Pre-opening - not included in figures",
            "Safeguarding non-compliance",
            "Transfer"
        };

        private IEnumerable<IncludeProjectTypeTestCase> GenerateAllProjectTypePermutations()
        {
            int n = Expected.Count;
            for (int i = 1; i < (1 << n); i++)
            {
                var selected = new List<string>();
                for (int bit = 0; bit < n; bit++)
                {
                    if ((i & (1 << bit)) != 0)
                    {
                        selected.Add(Expected[bit]);
                    }
                }
                yield return new IncludeProjectTypeTestCase(new StringValues(selected.ToArray()));
            }
        }

        public IEnumerator<object[]> GetEnumerator()
        {
            yield return [new IncludeProjectTypeTestCase(new StringValues())];

            foreach (var testCase in GenerateAllProjectTypePermutations())
            {
                yield return [testCase];
            }
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

}
