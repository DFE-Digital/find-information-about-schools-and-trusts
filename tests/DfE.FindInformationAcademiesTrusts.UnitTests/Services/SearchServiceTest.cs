using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb;
using DfE.FindInformationAcademiesTrusts.Http;
using DfE.FindInformationAcademiesTrusts.HttpServices;
using DfE.FindInformationAcademiesTrusts.Services.Search;
using GovUK.Dfe.CoreLibs.Contracts.Academies.V4;
using GovUK.Dfe.CoreLibs.Contracts.Academies.V4.Establishments;
using GovUK.Dfe.CoreLibs.Contracts.Academies.V4.Trusts;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Services;

public class SearchServiceTest
{
    private readonly SearchService _sut;
    private readonly IGetEstablishmentsTemp _mockGetEstablishments = Substitute.For<IGetEstablishmentsTemp>();
    private readonly IGetTrustsTemp _mockGetTrusts = Substitute.For<IGetTrustsTemp>();

    public SearchServiceTest()
    {
        _sut = new SearchService(_mockGetEstablishments, _mockGetTrusts, new StringFormattingUtilities());
    }

    private static EstablishmentDto CreateEstablishment(string urn, string name, string groupTypeCode = "10") =>
        new()
        {
            Urn = urn,
            Name = name,
            EstablishmentNumber = urn,
            EstablishmentType = new NameAndCodeDto { Name = "Academy" },
            EstablishmentGroupType = new NameAndCodeDto { Code = groupTypeCode },
            Address = new AddressDto()
        };

    private static TrustDto CreateTrust(string groupUid, string name, string referenceNumber) =>
        new()
        {
            GroupUid = groupUid,
            Name = name,
            ReferenceNumber = referenceNumber,
            Type = new NameAndCodeDto { Name = "Trust" },
            Address = new AddressDto()
        };

    private void SetupTrustSearch(string text, IEnumerable<TrustDto>? trusts)
    {
        _mockGetTrusts.SearchTrusts(text).Returns(new TrustListResponseTemp<TrustDto> { Data = trusts });
    }

    // Search results page

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task GetSearchResultsForPageAsync_if_keywords_are_null_or_empty_should_return_empty_results(
        string? text)
    {
        var result = await _sut.GetSearchResultsForPageAsync(text, 1);

        result.ResultsList.Should().BeEmpty();
        result.ResultsOverview.Should().BeEquivalentTo(new SearchResultsOverview());
        await _mockGetTrusts.DidNotReceive().SearchTrusts(Arg.Any<string>());
        await _mockGetEstablishments.DidNotReceive().SearchEstablishments(Arg.Any<string>());
    }

    [Fact]
    public async Task GetSearchResultsForPageAsync_should_return_empty_results_when_text_is_a_number_below_100000()
    {
        var result = await _sut.GetSearchResultsForPageAsync("99999", 1);

        result.ResultsList.Should().BeEmpty();
        result.ResultsOverview.Should().BeEquivalentTo(new SearchResultsOverview());
        await _mockGetTrusts.DidNotReceive().SearchTrusts(Arg.Any<string>());
        await _mockGetEstablishments.DidNotReceive().SearchEstablishments(Arg.Any<string>());
    }

    [Fact]
    public async Task GetSearchResultsForPageAsync_should_return_combined_mapped_trust_and_school_results()
    {
        SetupTrustSearch("Test",
        [
            new TrustDto
            {
                GroupUid = "1223",
                Name = "Test Trust",
                ReferenceNumber = "TR001",
                Type = new NameAndCodeDto { Name = "Multi-academy trust" },
                Address = new AddressDto { Street = "1 Trust Street", Town = "Testville", Postcode = "AB1 2CD" }
            }
        ]);

        _mockGetEstablishments.SearchEstablishments("Test").Returns(
        [
            new EstablishmentDto
            {
                Urn = "100001",
                Name = "Test School",
                EstablishmentNumber = "2001",
                EstablishmentType = new NameAndCodeDto { Name = "Academy" },
                EstablishmentGroupType = new NameAndCodeDto { Code = "1", Name = "test Group" },
                Address = new AddressDto { Street = "1 School Street", Town = "Testville", Postcode = "AB1 2CD" }
            }
        ]);

        var result = await _sut.GetSearchResultsForPageAsync("Test", 1);

        result.ResultsList.Should().BeEquivalentTo([
            new SearchResultServiceModel("100001", null, "Test School", "1 School Street, Testville, AB1 2CD", "2001",
                "Academy", ResultType.School),
            new SearchResultServiceModel("1223", "TR001", "Test Trust", "1 Trust Street, Testville, AB1 2CD", "TR001",
                "Multi-academy trust", ResultType.Trust)
        ]);
        result.ResultsList.PageStatus.TotalResults.Should().Be(2);
        result.ResultsOverview.NumberOfTrusts.Should().Be(1);
        result.ResultsOverview.NumberOfSchools.Should().Be(1);
    }

    [Fact]
    public async Task GetSearchResultsForPageAsync_should_filter_out_invalid_establishment_group_types()
    {
        SetupTrustSearch("Test", []);
        _mockGetEstablishments.SearchEstablishments("Test").Returns(
        [
            CreateEstablishment("1", "Included"),
            CreateEstablishment("2", "Filtered", "999")
        ]);

        var result = await _sut.GetSearchResultsForPageAsync("Test", 1);

        result.ResultsList.Should().ContainSingle(r => r.Name == "Included");
        result.ResultsList.Should().NotContain(r => r.Name == "Filtered");
        result.ResultsOverview.NumberOfSchools.Should().Be(1);
    }

    [Fact]
    public async Task GetSearchResultsForPageAsync_should_search_by_reference_number_when_no_establishments_found()
    {
        _mockGetEstablishments.SearchEstablishments("TR123").Returns([]);
        SetupTrustSearch("TR123", []);
        _mockGetTrusts.GetTrustByReferenceNumber("TR123").Returns(CreateTrust("1", "Reference Trust", "TR123"));

        var result = await _sut.GetSearchResultsForPageAsync("TR123", 1);

        result.ResultsList.Should().ContainSingle().Which.Name.Should().Be("Reference Trust");
        result.ResultsOverview.NumberOfTrusts.Should().Be(1);
    }

    [Fact]
    public async Task GetSearchResultsForPageAsync_should_return_the_correct_results_page_when_there_are_more_than_20_matches()
    {
        SetupTrustSearch("Test", Enumerable.Range(1, 30)
            .Select(i => CreateTrust(i.ToString(), $"Trust {i:00}", $"TR{i:000}")));

        _mockGetEstablishments.SearchEstablishments("Test").Returns(Enumerable.Range(1, 30)
            .Select(i => CreateEstablishment(i.ToString(), $"School {i:00}",
                ((int)NameAndCodeEnumsTemp.AllowedEstablishmentGroupTypeCodes.Academies).ToString()))
            .ToList());

        var result = await _sut.GetSearchResultsForPageAsync("Test", 2);

        result.ResultsList.Should().HaveCount(20);
        result.ResultsList.PageStatus.TotalResults.Should().Be(60);
        result.ResultsOverview.NumberOfTrusts.Should().Be(30);
        result.ResultsOverview.NumberOfSchools.Should().Be(30);
    }

    // Combined autocomplete

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task GetSearchResultsForAutocompleteAsync_if_keywords_are_null_or_empty_should_return_empty_results(
        string? text)
    {
        var result = await _sut.GetSearchResultsForAutocompleteAsync(text);

        result.Should().BeEmpty();
        await _mockGetTrusts.DidNotReceive().SearchTrusts(Arg.Any<string>());
        await _mockGetEstablishments.DidNotReceive().SearchEstablishments(Arg.Any<string>());
    }

    [Fact]
    public async Task GetSearchResultsForAutocompleteAsync_should_return_a_maximum_of_5_results()
    {
        SetupTrustSearch("Test", Enumerable.Range(1, 10)
            .Select(i => CreateTrust(i.ToString(), $"Test Trust {i:00}", $"TR{i:000}")));
        _mockGetEstablishments.SearchEstablishments("Test").Returns(Enumerable.Range(1, 10)
            .Select(i => CreateEstablishment(i.ToString(), $"Test School {i:00}"))
            .ToList());

        var result = await _sut.GetSearchResultsForAutocompleteAsync("Test");

        result.Should().HaveCount(5);
    }

    // School autocomplete

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("99999")]
    public async Task GetSchoolSearchResultsForAutocompleteAsync_should_return_empty_when_text_is_blank_or_a_number_below_100000(
        string? text)
    {
        var result = await _sut.GetSchoolSearchResultsForAutocompleteAsync(text);

        result.Should().BeEmpty();
        await _mockGetEstablishments.DidNotReceive().SearchEstablishments(Arg.Any<string>());
    }

    [Fact]
    public async Task GetSchoolSearchResultsForAutocompleteAsync_should_return_correct_mapped_models()
    {
        _mockGetEstablishments.SearchEstablishments("Test").Returns(
        [
            new EstablishmentDto
            {
                Urn = "100001",
                Name = "Test School",
                EstablishmentNumber = "2001",
                EstablishmentType = new NameAndCodeDto { Name = "Academy" },
                EstablishmentGroupType = new NameAndCodeDto { Code = "10" },
                Address = new AddressDto { Street = "1 Test Street", Town = "Testville", Postcode = "AB1 2CD" }
            }
        ]);

        var result = await _sut.GetSchoolSearchResultsForAutocompleteAsync("Test");

        result.Should().ContainSingle().Which.Should().Be(new SearchResultServiceModel(
            "100001", null, "Test School", "1 Test Street, Testville, AB1 2CD", "2001", "Academy",
            ResultType.School));
        await _mockGetTrusts.DidNotReceive().SearchTrusts(Arg.Any<string>());
    }

    [Fact]
    public async Task GetSchoolSearchResultsForAutocompleteAsync_should_filter_out_invalid_establishment_group_types()
    {
        _mockGetEstablishments.SearchEstablishments("Test").Returns(
        [
            CreateEstablishment("1", "Included"),
            CreateEstablishment("2", "Filtered", "999"),
            CreateEstablishment("3", "No Group Type Code", "not-a-number")
        ]);

        var result = await _sut.GetSchoolSearchResultsForAutocompleteAsync("Test");

        result.Should().ContainSingle(r => r.Name == "Included");
        result.Should().NotContain(r => r.Name == "Filtered");
        result.Should().NotContain(r => r.Name == "No Group Type Code");
    }

    [Fact]
    public async Task GetSchoolSearchResultsForAutocompleteAsync_should_order_names_starting_with_text_first()
    {
        _mockGetEstablishments.SearchEstablishments("Oak").Returns(
        [
            CreateEstablishment("1", "The Oak School"),
            CreateEstablishment("2", "Oakwood Academy"),
            CreateEstablishment("3", "Big Oak Primary"),
            CreateEstablishment("4", "oak Hill School")
        ]);

        var result = await _sut.GetSchoolSearchResultsForAutocompleteAsync("Oak");

        result.Take(2).Select(r => r.Name).Should().BeEquivalentTo("Oakwood Academy", "oak Hill School");
        result.Skip(2).Select(r => r.Name).Should().BeEquivalentTo("The Oak School", "Big Oak Primary");
    }

    [Fact]
    public async Task GetSchoolSearchResultsForAutocompleteAsync_should_return_a_maximum_of_5_results()
    {
        _mockGetEstablishments.SearchEstablishments("School").Returns(Enumerable.Range(1, 10)
            .Select(i => CreateEstablishment(i.ToString(), $"School {i:00}"))
            .ToList());

        var result = await _sut.GetSchoolSearchResultsForAutocompleteAsync("School");

        result.Should().HaveCount(5);
    }

    [Fact]
    public async Task GetSchoolSearchResultsForAutocompleteAsync_should_return_empty_when_no_establishments_found()
    {
        _mockGetEstablishments.SearchEstablishments("Nothing").Returns([]);

        var result = await _sut.GetSchoolSearchResultsForAutocompleteAsync("Nothing");

        result.Should().BeEmpty();
    }

    // Trust autocomplete

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("99999")]
    public async Task GetTrustSearchResultsForAutocompleteAsync_should_return_empty_when_text_is_blank_or_a_number_below_100000(
        string? text)
    {
        var result = await _sut.GetTrustSearchResultsForAutocompleteAsync(text);

        result.Should().BeEmpty();
        await _mockGetTrusts.DidNotReceive().SearchTrusts(Arg.Any<string>());
        await _mockGetTrusts.DidNotReceive().GetTrustByReferenceNumber(Arg.Any<string>());
    }

    [Fact]
    public async Task GetTrustSearchResultsForAutocompleteAsync_should_return_correct_mapped_models()
    {
        SetupTrustSearch("Test",
        [
            new TrustDto
            {
                GroupUid = "1223",
                Name = "Test Trust",
                ReferenceNumber = "TR001",
                Type = new NameAndCodeDto { Name = "Multi-academy trust" },
                Address = new AddressDto { Street = "1 Test Street", Town = "Testville", Postcode = "AB1 2CD" }
            }
        ]);

        var result = await _sut.GetTrustSearchResultsForAutocompleteAsync("Test");

        result.Should().ContainSingle().Which.Should().Be(new SearchResultServiceModel(
            "1223", "TR001", "Test Trust", "1 Test Street, Testville, AB1 2CD", "TR001", "Multi-academy trust",
            ResultType.Trust));
        await _mockGetTrusts.DidNotReceive().GetTrustByReferenceNumber(Arg.Any<string>());
        await _mockGetEstablishments.DidNotReceive().SearchEstablishments(Arg.Any<string>());
    }

    [Fact]
    public async Task GetTrustSearchResultsForAutocompleteAsync_should_search_by_reference_number_when_no_trusts_found()
    {
        SetupTrustSearch("TR123", []);
        _mockGetTrusts.GetTrustByReferenceNumber("TR123").Returns(CreateTrust("1", "Reference Trust", "TR123"));

        var result = await _sut.GetTrustSearchResultsForAutocompleteAsync("TR123");

        result.Should().ContainSingle();
        result[0].Name.Should().Be("Reference Trust");
        result[0].ResultType.Should().Be(ResultType.Trust);
    }

    [Fact]
    public async Task GetTrustSearchResultsForAutocompleteAsync_should_search_by_reference_number_when_search_data_is_null()
    {
        SetupTrustSearch("TR123", null);
        _mockGetTrusts.GetTrustByReferenceNumber("TR123").Returns(CreateTrust("1", "Reference Trust", "TR123"));

        var result = await _sut.GetTrustSearchResultsForAutocompleteAsync("TR123");

        result.Should().ContainSingle().Which.Name.Should().Be("Reference Trust");
    }

    [Fact]
    public async Task GetTrustSearchResultsForAutocompleteAsync_should_return_empty_when_no_trusts_and_no_reference_number_match()
    {
        SetupTrustSearch("TR123", []);
        _mockGetTrusts.GetTrustByReferenceNumber("TR123").Returns((TrustDto?)null);

        var result = await _sut.GetTrustSearchResultsForAutocompleteAsync("TR123");

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetTrustSearchResultsForAutocompleteAsync_should_return_a_maximum_of_5_results()
    {
        SetupTrustSearch("Trust", Enumerable.Range(1, 10)
            .Select(i => CreateTrust(i.ToString(), $"Trust {i:00}", $"TR{i:000}"))
            .ToList());

        var result = await _sut.GetTrustSearchResultsForAutocompleteAsync("Trust");

        result.Should().HaveCount(5);
    }

    [Fact]
    public async Task GetTrustSearchResultsForAutocompleteAsync_should_order_names_not_starting_with_text_before_names_starting_with_text()
    {
        SetupTrustSearch("Oak",
        [
            CreateTrust("1", "Oakwood Trust", "TR001"),
            CreateTrust("2", "The Oak Trust", "TR002")
        ]);

        var result = await _sut.GetTrustSearchResultsForAutocompleteAsync("Oak");

        result.Select(r => r.Name).Should().Equal("The Oak Trust", "Oakwood Trust");
    }

    [Fact]
    public async Task GetTrustSearchResultsForAutocompleteAsync_should_use_empty_strings_when_trust_has_no_group_uid_or_name()
    {
        SetupTrustSearch("Test",
        [
            new TrustDto
            {
                GroupUid = null,
                Name = null!,
                ReferenceNumber = "TR001",
                Type = new NameAndCodeDto { Name = "Trust" },
                Address = new AddressDto()
            }
        ]);

        var result = await _sut.GetTrustSearchResultsForAutocompleteAsync("Test");

        result.Should().ContainSingle();
        result[0].Id.Should().BeEmpty();
        result[0].Name.Should().BeEmpty();
    }
}
