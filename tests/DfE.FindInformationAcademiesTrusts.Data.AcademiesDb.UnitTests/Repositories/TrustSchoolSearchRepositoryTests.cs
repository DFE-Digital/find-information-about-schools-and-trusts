using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Models.Gias;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Repositories;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.Search;

namespace DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.UnitTests.Repositories;

public class TrustSchoolSearchRepositoryTests
{
    private readonly TrustSchoolSearchRepository _sut;
    private readonly MockAcademiesDbContext _mockAcademiesDbContext = new();

    private const int PageSize = 20;

    public TrustSchoolSearchRepositoryTests()
    {
        _sut = new TrustSchoolSearchRepository(_mockAcademiesDbContext.Object, new StringFormattingUtilities());
    }

    [Theory]
    [InlineData(10, 10)]
    [InlineData(20, 1)]
    [InlineData(20, 10)]
    public async Task GetSearchResultsAsync_should_only_return_20_results_when_there_are_more_than_20_matches(
        int numberOfTrusts, int numberOfSchools)
    {
        AddGiasGroupsForSearchTerm("inspire", numberOfTrusts);
        AddGiasEstablishmentForSearchTerm("inspire", numberOfSchools);

        var searchResults = await _sut.GetSearchResultsAsync("inspire", PageSize);
        searchResults.Results.Should().HaveCount(PageSize);
        searchResults.NumberOfResults.Should()
            .BeEquivalentTo(new SearchResultCount(numberOfSchools + numberOfTrusts, numberOfTrusts, numberOfSchools));
    }

    [Theory]
    [InlineData(3, 2)]
    [InlineData(3, 3)]
    [InlineData(15, 15)]
    public async Task GetAutoCompleteSearchResultsAsync_should_only_return_5_results_when_there_are_more_than_5_matches(
        int numberOfTrusts, int numberOfSchools)
    {
        AddGiasGroupsForSearchTerm("inspire", numberOfTrusts);
        AddGiasEstablishmentForSearchTerm("inspire", numberOfSchools);

        var result = await _sut.GetAutoCompleteSearchResultsAsync("inspire");
        result.Should().HaveCount(5);
    }

    [Fact]
    public async Task GetSearchResultsAsync_should_return_the_correct_results_page_when_there_are_more_than_20_matches()
    {
        for (var page = 1; page <= 3; page++)
        {
            for (var i = 0; i < 10; i++)
            {
                _mockAcademiesDbContext.AddGiasGroupForTrust(name: $"Page {page}");
            }

            for (var i = 0; i < 10; i++)
            {
                _mockAcademiesDbContext.AddGiasEstablishment(establishmentName: $"Page {page}",
                    establishmentType: "Local authority maintained schools");
            }
        }

        var searchResults = await _sut.GetSearchResultsAsync("Page", PageSize);
        searchResults.Results.All(entry => entry.Name == "Page 1").Should().Be(true);

        searchResults = await _sut.GetSearchResultsAsync("Page", PageSize, 2);
        searchResults.Results.All(entry => entry.Name == "Page 2").Should().Be(true);

        searchResults = await _sut.GetSearchResultsAsync("Page", PageSize, 3);
        searchResults.Results.All(entry => entry.Name == "Page 3").Should().Be(true);
    }

    [Fact]
    public async Task GetSearchResultsAsync_should_return_all_results_when_there_are_less_than_20_matches()
    {
        AddGiasGroupsForSearchTerm("inspire", 9);
        AddGiasEstablishmentForSearchTerm("inspire", 10);

        var searchResults = await _sut.GetSearchResultsAsync("inspire", PageSize);
        searchResults.Results.Should().HaveCount(19);
        searchResults.NumberOfResults.Should().Be(new SearchResultCount(19, 9, 10));
    }

    [Fact]
    public async Task GetAutoCompleteSearchResultsAsync_should_return_all_results_when_there_are_less_than_5_matches()
    {
        AddGiasGroupsForSearchTerm("inspire", 3);
        AddGiasEstablishmentForSearchTerm("inspire", 1);

        var result = await _sut.GetAutoCompleteSearchResultsAsync("inspire");
        result.Should().HaveCount(4);
    }

    [Fact]
    public async Task GetSearchResultsAsync_should_return_empty_if_there_is_no_matching_result()
    {
        var searchResults = await _sut.GetSearchResultsAsync("non existent trust", PageSize);
        searchResults.Results.Should().BeEmpty();
        searchResults.NumberOfResults.Should().Be(new SearchResultCount(0, 0, 0));
    }

    [Fact]
    public async Task GetAutoCompleteSearchResultsAsync_should_return_empty_if_there_is_no_matching_result()
    {
        var result = await _sut.GetAutoCompleteSearchResultsAsync("non existent trust");
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetSearchResultsAsync_should_return_a_single_item_if_there_is_one_matching_term_for_trusts()
    {
        AddGiasGroupsForSearchTerm("inspire", 1);

        var searchResults = await _sut.GetSearchResultsAsync("inspire", PageSize);
        searchResults.Results.Should().ContainSingle().Which.Name.Should().Contain("inspire");
        searchResults.NumberOfResults.Should().Be(new SearchResultCount(1, 1, 0));
    }

    [Fact]
    public async Task GetSearchResultsAsync_should_return_a_single_item_if_there_is_one_matching_term_for_schools()
    {
        AddGiasEstablishmentForSearchTerm("inspire", 1);

        var searchResults = await _sut.GetSearchResultsAsync("inspire", PageSize);
        searchResults.Results.Should().ContainSingle().Which.Name.Should().Contain("inspire");
        searchResults.NumberOfResults.Should().Be(new SearchResultCount(1, 0, 1));
    }

    [Fact]
    public async Task
        GetAutoCompleteSearchResultsAsync_should_return_a_single_item_if_there_is_one_matching_term_for_trusts()
    {
        AddGiasGroupsForSearchTerm("inspire", 1);

        var result = await _sut.GetAutoCompleteSearchResultsAsync("inspire");
        result.Should().ContainSingle().Which.Name.Should().Contain("inspire");
    }

    [Fact]
    public async Task
        GetAutoCompleteSearchResultsAsync_should_return_a_single_item_if_there_is_one_matching_term_for_schools()
    {
        AddGiasEstablishmentForSearchTerm("inspire", 1);

        var result = await _sut.GetAutoCompleteSearchResultsAsync("inspire");
        result.Should().ContainSingle().Which.Name.Should().Contain("inspire");
    }

    [Fact]
    public async Task GetSearchResultsAsync_should_map_properties_for_trusts()
    {
        _mockAcademiesDbContext.GiasGroups.Add(new GiasGroup
        {
            GroupUid = "1234",
            GroupType = "Multi-academy trust",
            GroupId = "TR01234",
            GroupName = "Inspire 1234",
            GroupStatusCode = "OPEN",
            GroupContactStreet = "A street",
            GroupContactTown = "Town",
            GroupContactLocality = "Station Road",
            GroupContactPostcode = "GH1 8JH"
        });

        var searchResults = await _sut.GetSearchResultsAsync("Inspire 1234", PageSize);

        searchResults.Results.Should().ContainSingle()
            .Which.Should().BeEquivalentTo(new SearchResult("1234", "Inspire 1234", "Multi-academy trust",
                "A street, Station Road, Town, GH1 8JH", true, "TR01234"));
    }

    [Fact]
    public async Task GetSearchResultsAsync_should_map_properties_for_schools()
    {
        _mockAcademiesDbContext.GiasEstablishments.Add(new GiasEstablishment
        {
            EstablishmentName = "Inspire 1234",
            Urn = 99999,
            EstablishmentTypeGroupName = "Local authority maintained schools",
            TypeOfEstablishmentName = "Community school",
            EstablishmentStatusName = "Open",
            Street = "A street",
            Town = "Town",
            Locality = "Station Road",
            Postcode = "GH1 8JH"
        });

        var searchResults = await _sut.GetSearchResultsAsync("Inspire", PageSize);

        searchResults.Results.Should().ContainSingle()
            .Which.Should().BeEquivalentTo(new SearchResult(Id: "99999", Name: "Inspire 1234",
                Type: "Community school", Address: "A street, Station Road, Town, GH1 8JH", IsTrust: false, TrustReferenceNumber: null));
    }

    [Fact]
    public async Task GetAutoCompleteSearchResultsAsync_should_map_properties_for_trusts()
    {
        _mockAcademiesDbContext.GiasGroups.Add(new GiasGroup
        {
            GroupUid = "1234",
            GroupType = "Multi-academy trust",
            GroupId = "TR01234",
            GroupName = "Inspire 1234",
            GroupStatusCode = "OPEN",
            GroupContactStreet = "A street",
            GroupContactTown = "Town",
            GroupContactLocality = "Station Road",
            GroupContactPostcode = "GH1 8JH"
        });

        var result = await _sut.GetAutoCompleteSearchResultsAsync("Inspire 1234");

        result.Should().ContainSingle()
            .Which.Should().BeEquivalentTo(new SearchResult("1234", "Inspire 1234", "Multi-academy trust",
                "A street, Station Road, Town, GH1 8JH", true, "TR01234"));
    }

    [Fact]
    public async Task GetAutoCompleteSearchResultsAsync_should_map_properties_for_schools()
    {
        _mockAcademiesDbContext.GiasEstablishments.Add(new GiasEstablishment
        {
            EstablishmentName = "Inspire 1234",
            Urn = 99999,
            EstablishmentTypeGroupName = "Local authority maintained schools",
            TypeOfEstablishmentName = "Community school",
            EstablishmentStatusName = "Open",
            Street = "A street",
            Town = "Town",
            Locality = "Station Road",
            Postcode = "GH1 8JH"
        });

        var searchResults = await _sut.GetAutoCompleteSearchResultsAsync("Inspire");

        searchResults.Should().ContainSingle()
            .Which.Should().BeEquivalentTo(new SearchResult(Id: "99999", Name: "Inspire 1234",
                Type: "Community school", Address: "A street, Station Road, Town, GH1 8JH", IsTrust: false, TrustReferenceNumber: null));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task SearchAsync_should_return_empty_if_empty_search_term(string? term)
    {
        var searchResults = await _sut.GetSearchResultsAsync(term, PageSize);
        searchResults.Results.Should().BeEmpty();
        searchResults.NumberOfResults.Should().Be(new SearchResultCount(0, 0, 0));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task SearchAutocompleteAsync_should_return_empty_if_empty_search_term(string? term)
    {
        var result = await _sut.GetAutoCompleteSearchResultsAsync(term);
        result.Should().BeEmpty();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task SearchAsync_should_not_call_database_if_empty_search_term(string? term)
    {
        await _sut.GetSearchResultsAsync(term, PageSize);
        _ = _mockAcademiesDbContext.Object.DidNotReceive().Groups;
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task SearchAutocompleteAsync_should_not_call_database_if_empty_search_term(string? term)
    {
        await _sut.GetAutoCompleteSearchResultsAsync(term);
        _ = _mockAcademiesDbContext.Object.DidNotReceive().Groups;
    }

    [Theory]
    [InlineData("a")]
    public async Task
        GetSearchResultsAsync_should_return_first_20_trusts_or_schools_after_sorting_alphabetically_by_name_and_id(
            string searchTerm)
    {
        //Set up 12 unordered trusts with duplicate names (two are "zetta" which we don't want to be returned)
        (string uid, string name)[] unorderedTrusts =
        [
            ("1230", "education trust"),
            ("9231", "beta trust"),
            ("2232", "zetta trust"),
            ("8233", "derbyshire academies"),
            ("3234", "chelsea learning"),
            ("7235", "all stars"),
            ("2340", "education trust"),
            ("8231", "beta trust"),
            ("1232", "zetta trust"),
            ("9233", "derbyshire academies"),
            ("2234", "chelsea learning"),
            ("8235", "all stars")
        ];

        foreach (var (uid, name) in unorderedTrusts)
        {
            _mockAcademiesDbContext.AddGiasGroupForTrust(uid, name);
        }

        //Set up 12 unordered schools with duplicate names (two are "zetta" which we don't want to be returned)
        (int urn, string name)[] schoolNames =
        [
            (100230, "academy"),
            (900231, "beta school"),
            (200232, "zetta school"),
            (800233, "derbyshire academy"),
            (300234, "chelsea learning"),
            (700235, "all stars"),
            (200340, "academy"),
            (800231, "beta school"),
            (100232, "zetta school"),
            (900233, "derbyshire academy"),
            (200234, "chelsea learning"),
            (800235, "all stars")
        ];

        foreach (var (urn, name) in schoolNames)
        {
            _mockAcademiesDbContext.AddGiasEstablishment(urn, name, "Local authority maintained schools");
        }

        var searchResults = await _sut.GetSearchResultsAsync(searchTerm, PageSize);

        searchResults.Results.Should()
            .HaveCount(20)
            .And.BeInAscendingOrder(t => t.Name).And.ThenBeInAscendingOrder(t => t.Id)
            .And.NotContain(t => t.Name == "zetta trust")
            .And.NotContain(t => t.Name == "zetta school");
    }


    [Theory]
    [InlineData("united")]
    public async Task SearchAutocompleteAsync_should_return_trusts_or_schools_ordered_by_starts_with(string searchTerm)
    {
        //Set up 3 unordered trusts with duplicate names (one is "zetta" which we don't want to be returned)
        (string uid, string name)[] unorderedTrusts =
        [
            ("2340", "united trusts"),
            ("2232", "some united trust"),
            ("1230", "all stars")
        ];

        foreach (var (uid, name) in unorderedTrusts)
        {
            _mockAcademiesDbContext.AddGiasGroupForTrust(uid, name);
        }

        //Set up 4 unordered schools with duplicate names (one is "zetta" which we don't want to be returned)
        (int urn, string name)[] schoolNames =
        [
            (900231, "united school"),
            (200232, "some united school"),
            (700235, "united"),
            (800231, "beta school")
        ];

        foreach (var (urn, name) in schoolNames)
        {
            _mockAcademiesDbContext.AddGiasEstablishment(urn, name, "Local authority maintained schools");
        }

        var result = await _sut.GetAutoCompleteSearchResultsAsync(searchTerm);

        result.Should()
            .HaveCount(5)
            .And.OnlyContain(t => t.Name.Contains("united"));

        result[0].Name.Should().Be("united");
        result[1].Name.Should().Be("united trusts");
        result[2].Name.Should().Be("united school");
        result[3].Name.Should().Be("some united trust");
        result[4].Name.Should().Be("some united school");
    }


    [Theory]
    [InlineData("all")]
    public async Task SearchAutocompleteAsync_should_return_trusts_or_schools_ordered_by_exact_match(string searchTerm)
    {
        //Set up 3 unordered trusts with duplicate names (one is "zetta" which we don't want to be returned)
        (string uid, string name)[] unorderedTrusts =
        [
            ("2340", "all stars"),
            ("2232", "zetta trust"),
            ("1230", "all stars")
        ];

        foreach (var (uid, name) in unorderedTrusts)
        {
            _mockAcademiesDbContext.AddGiasGroupForTrust(uid, name);
        }

        //Set up 4 unordered schools with duplicate names (one is "zetta" which we don't want to be returned)
        (int urn, string name)[] schoolNames =
        [
            (900231, "beta school"),
            (200232, "zetta school"),
            (700235, "all stars"),
            (800231, "beta school")
        ];

        foreach (var (urn, name) in schoolNames)
        {
            _mockAcademiesDbContext.AddGiasEstablishment(urn, name, "Local authority maintained schools");
        }

        var result = await _sut.GetAutoCompleteSearchResultsAsync(searchTerm);

        result.Should()
            .HaveCount(3)
            .And.BeInAscendingOrder(t => t.Id)
            .And.OnlyContain(t => t.Name == "all stars");
    }

    [Theory]
    [InlineData("all")]
    public async Task
        SearchAutocompleteAsync_should_return_first_5_trusts_or_schools_after_ordering_by_weighting_and_id(
            string searchTerm)
    {
        //Set up 3 unordered trusts with duplicate names (one is "zetta" which we don't want to be returned)
        (string uid, string name)[] unorderedTrusts =
        [
            ("2340", "all stars"),
            ("2232", "zetta trust"),
            ("1230", "all stars")
        ];

        foreach (var (uid, name) in unorderedTrusts)
        {
            _mockAcademiesDbContext.AddGiasGroupForTrust(uid, name);
        }

        //Set up 4 unordered schools with duplicate names (one is "zetta" which we don't want to be returned)
        (int urn, string name)[] schoolNames =
        [
            (900231, "beta school"),
            (200232, "zetta school"),
            (700235, "all stars"),
            (800231, "beta school")
        ];

        foreach (var (urn, name) in schoolNames)
        {
            _mockAcademiesDbContext.AddGiasEstablishment(urn, name, "Local authority maintained schools");
        }

        var result = await _sut.GetAutoCompleteSearchResultsAsync(searchTerm);

        result.Should()
            .HaveCount(5)
            .And.BeInAscendingOrder(t => t.Id)
            .And.NotContain(t => t.Name == "zetta trust")
            .And.NotContain(t => t.Name == "zetta school");
    }

    [Fact]
    public async Task GetSearchResultsAsync_should_not_return_groups_which_are_not_trusts()
    {
        _mockAcademiesDbContext.AddGiasGroupForFederation(name: "Federation ABC");
        _mockAcademiesDbContext.AddGiasGroupForTrust(name: "Trust ABC");

        var searchResults = await _sut.GetSearchResultsAsync("ABC", PageSize);

        searchResults.Results.Should().ContainSingle()
            .Which.Name.Should().Be("Trust ABC");
    }

    [Fact]
    public async Task GetSearchResultsAsync_should_not_return_schools_which_are_not_supported_types()
    {
        _mockAcademiesDbContext.AddGiasEstablishment(establishmentName: "expected school ABC",
            establishmentType: "Academies");
        _mockAcademiesDbContext.AddGiasEstablishment(establishmentName: "school ABC",
            establishmentType: "Independent schools");
        _mockAcademiesDbContext.AddGiasEstablishment(establishmentName: "school ABC",
            establishmentType: "Online provider");
        _mockAcademiesDbContext.AddGiasEstablishment(establishmentName: "school ABC", establishmentType: "Other types");
        _mockAcademiesDbContext.AddGiasEstablishment(establishmentName: "school ABC",
            establishmentType: "Universities");
        _mockAcademiesDbContext.AddGiasEstablishment(establishmentName: "school ABC",
            establishmentType: "Closed academy");
        _mockAcademiesDbContext.AddGiasEstablishment(establishmentName: "school ABC",
            establishmentType: "Closed school");

        var searchResults = await _sut.GetSearchResultsAsync("ABC", PageSize);

        searchResults.Results.Should().ContainSingle()
            .Which.Name.Should().Be("expected school ABC");
    }

    [Fact]
    public async Task GetAutoCompleteSearchResultsAsync_should_not_return_groups_which_are_not_trusts()
    {
        _mockAcademiesDbContext.AddGiasGroupForFederation(name: "Federation ABC");
        _mockAcademiesDbContext.AddGiasGroupForTrust(name: "Trust ABC");

        var result = await _sut.GetAutoCompleteSearchResultsAsync("ABC");

        result.Should().ContainSingle()
            .Which.Name.Should().Be("Trust ABC");
    }

    [Fact]
    public async Task GetAutoCompleteSearchResultsAsync_should_not_return_schools_which_are_not_supported_types()
    {
        _mockAcademiesDbContext.AddGiasEstablishment(establishmentName: "expected school ABC",
            establishmentType: "Academies");
        _mockAcademiesDbContext.AddGiasEstablishment(establishmentName: "school ABC",
            establishmentType: "Independent schools");
        _mockAcademiesDbContext.AddGiasEstablishment(establishmentName: "school ABC",
            establishmentType: "Online provider");
        _mockAcademiesDbContext.AddGiasEstablishment(establishmentName: "school ABC", establishmentType: "Other types");
        _mockAcademiesDbContext.AddGiasEstablishment(establishmentName: "school ABC",
            establishmentType: "Universities");
        _mockAcademiesDbContext.AddGiasEstablishment(establishmentName: "school ABC",
            establishmentType: "Closed academy");
        _mockAcademiesDbContext.AddGiasEstablishment(establishmentName: "school ABC",
            establishmentType: "Closed school");

        var searchResults = await _sut.GetAutoCompleteSearchResultsAsync("ABC");

        searchResults.Should().ContainSingle()
            .Which.Name.Should().Be("expected school ABC");
    }

    [Fact]
    public async Task GetSearchResultsAsync_Should_Return_Trust_When_Searching_By_TrustId()
    {
        _mockAcademiesDbContext.AddGiasGroupForTrust(trustReferenceNumber: "TR01234");

        var searchResults = await _sut.GetSearchResultsAsync("TR01234", PageSize);

        searchResults.Results.Should().ContainSingle()
            .Which.TrustReferenceNumber.Should().Be("TR01234");
    }

    [Fact]
    public async Task GetSearchResultsAsync_Should_Return_School_When_Searching_By_Urn()
    {
        var establishmentUrn = 1234;

        _mockAcademiesDbContext.AddGiasEstablishment(establishmentUrn, "expected school ABC", "Academies");

        var searchResults = await _sut.GetSearchResultsAsync("34", PageSize);

        searchResults.Results.Should().ContainSingle()
            .Which.Id.Should().Be(establishmentUrn.ToString());
    }

    [Fact]
    public async Task SearchAutocompleteAsync_Should_Return_Trust_When_Searching_By_TrustId()
    {
        _mockAcademiesDbContext.AddGiasGroupForTrust(trustReferenceNumber: "TR01234");

        var result = await _sut.GetAutoCompleteSearchResultsAsync("TR01234");

        result.Should().ContainSingle()
            .Which.TrustReferenceNumber.Should().Be("TR01234");
    }

    [Fact]
    public async Task GetAutoCompleteSearchResultsAsync_Should_Return_School_When_Searching_By_Urn()
    {
        var establishmentUrn = 1234;

        _mockAcademiesDbContext.AddGiasEstablishment(establishmentUrn, "expected school ABC", "Academies");

        var searchResults = await _sut.GetAutoCompleteSearchResultsAsync("34");

        searchResults.Should().ContainSingle()
            .Which.Id.Should().Be(establishmentUrn.ToString());
    }

    [Fact]
    public async Task GetSearchResultsAsync_Should_Return_Empty_When_TrustId_Does_Not_Exist()
    {
        var searchResults = await _sut.GetSearchResultsAsync("TR99999", PageSize);

        searchResults.Results.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAutoCompleteSearchResultsAsync_Should_Return_Empty_When_TrustId_Does_Not_Exist()
    {
        var result = await _sut.GetAutoCompleteSearchResultsAsync("TR99999");

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetSearchResultsAsync_Should_Return_Trusts_When_SearchTerm_Matches_Both_TrustId_And_TrustName()
    {
        _mockAcademiesDbContext.AddGiasGroupForTrust(trustReferenceNumber: "TR01234", name: "A Trust");
        _mockAcademiesDbContext.AddGiasGroupForTrust(trustReferenceNumber: "TR05678", name: "Trust 1234");

        var searchResults = await _sut.GetSearchResultsAsync("1234", PageSize);

        // Assert
        searchResults.Results.Should().HaveCount(2);
        searchResults.Results.Should().Contain(t => t.TrustReferenceNumber == "TR01234");
        searchResults.Results.Should().Contain(t => t.Name == "Trust 1234");
    }

    [Fact]
    public async Task GetSearchResultsAsync_Should_Return_School_When_SearchTerm_Matches_Both_Urn_And_Name()
    {
        _mockAcademiesDbContext.AddGiasEstablishment(1200, "school", "Academies");
        _mockAcademiesDbContext.AddGiasEstablishment(89, "school 8900", "Academies");

        var searchResults = await _sut.GetSearchResultsAsync("00", PageSize);

        searchResults.Results.Should().HaveCount(2);
        searchResults.Results.Should().Contain(t => t.Id == "1200");
        searchResults.Results.Should().Contain(t => t.Name == "school 8900");
    }

    [Fact]
    public async Task
        GetAutoCompleteSearchResultsAsync_Should_Return_Trusts_When_SearchTerm_Matches_Both_TrustId_And_TrustName()
    {
        _mockAcademiesDbContext.AddGiasGroupForTrust(trustReferenceNumber: "TR01234", name: "A Trust");
        _mockAcademiesDbContext.AddGiasGroupForTrust(trustReferenceNumber: "TR05678", name: "Trust 1234");

        var result = await _sut.GetAutoCompleteSearchResultsAsync("1234");

        result.Should().HaveCount(2);
        result.Should().Contain(t => t.TrustReferenceNumber == "TR01234");
        result.Should().Contain(t => t.Name == "Trust 1234");
    }

    [Fact]
    public async Task GetAutoCompleteSearchResultsAsync_Should_Return_School_When_SearchTerm_Matches_Both_Urn_And_Name()
    {
        _mockAcademiesDbContext.AddGiasEstablishment(1200, "school", "Academies");
        _mockAcademiesDbContext.AddGiasEstablishment(89, "school 8900", "Academies");

        var searchResults = await _sut.GetAutoCompleteSearchResultsAsync("00");

        searchResults.Should().HaveCount(2);
        searchResults.Should().Contain(t => t.Id == "1200");
        searchResults.Should().Contain(t => t.Name == "school 8900");
    }

    private void AddGiasGroupsForSearchTerm(string term, int num)
    {
        for (var i = 0; i < num; i++)
        {
            _mockAcademiesDbContext.AddGiasGroupForTrust(name: $"Trust {term} {i}");
        }
    }

    private void AddGiasEstablishmentForSearchTerm(string term, int num)
    {
        for (var i = 0; i < num; i++)
        {
            _mockAcademiesDbContext.AddGiasEstablishment(establishmentName: $"School {term} {i}",
                establishmentType: "Local authority maintained schools");
        }
    }
}
