using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.AcademiesDbServices;
using GovUK.Dfe.CoreLibs.Contracts.Academies.V4.Establishments;
using GovUK.Dfe.CoreLibs.Contracts.Academies.V4.Trusts;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Repositories;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Http;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.Search;
using GovUK.Dfe.CoreLibs.Contracts.Academies.V4;

namespace DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.UnitTests.Repositories;

public class TrustSchoolSearchRepositoryTests
{
    private readonly TrustSchoolSearchRepository _sut;
    private readonly IGetEstablishments _mockGetEstablishments;
    private readonly IGetTrusts _mockGetTrusts;

    private const int PageSize = 20;

    public TrustSchoolSearchRepositoryTests()
    {
        _mockGetEstablishments = Substitute.For<IGetEstablishments>();
        _mockGetTrusts = Substitute.For<IGetTrusts>();
        _sut = new TrustSchoolSearchRepository(_mockGetEstablishments , _mockGetTrusts, new StringFormattingUtilities());
    }

    [Fact]
    public async Task GetSearchResultsAsync_should_return_empty_results_when_text_is_null()
    {
        var (results, count) = await _sut.GetSearchResultsAsync(null, PageSize);

        results.Should().BeEmpty();
        count.TotalRecords.Should().Be(0);
        count.NumberOfTrusts.Should().Be(0);
        count.NumberOfSchools.Should().Be(0);

        await _mockGetTrusts.DidNotReceive().SearchTrusts(Arg.Any<string>());
        await _mockGetEstablishments.DidNotReceive().SearchEstablishments(Arg.Any<string>());
    }
    
    [Fact]
    public async Task GetSearchResultsAsync_should_return_combined_trust_and_school_results()
    {
        _mockGetTrusts.SearchTrusts("Test")
            .Returns(new TrustListResponse<TrustDto>()
            {
                Data =
                [
                    new TrustDto
                    {
                        GroupUid = "1223",
                        Name = "Test Trust",
                        ReferenceNumber = "TR001",
                        Type = new NameAndCodeDto { Name = "Trust" },
                        Address = new AddressDto()
                    }
                ]
            });

        _mockGetEstablishments.SearchEstablishments("Test")
            .Returns(
            [
                new EstablishmentDto
                {
                    Urn = "100",
                    Name = "Test School",
                    EstablishmentNumber = "100",
                    EstablishmentType = new NameAndCodeDto
                    {
                        Name = "Academy"
                    },
                    EstablishmentGroupType = new NameAndCodeDto
                    {
                        Code = "1",
                        Name = "test Group"
                        
                    },
                    Address = new AddressDto()
                }
            ]);

        var (results, count) = await _sut.GetSearchResultsAsync("Test", PageSize);

        results.Should().HaveCount(2);
        count.TotalRecords.Should().Be(2);
        count.NumberOfTrusts.Should().Be(1);
        count.NumberOfSchools.Should().Be(1);
    }
    
    [Fact]
    public async Task GetSearchResultsAsync_should_filter_out_invalid_establishment_group_types()
    {
        _mockGetTrusts.SearchTrusts("Test")
            .Returns(new TrustListResponse<TrustDto>() { Data = [] });

        _mockGetEstablishments.SearchEstablishments("Test")
            .Returns(
            [
                new EstablishmentDto
                {
                    Urn = "1",
                    Name = "Included",
                    EstablishmentType = new NameAndCodeDto { Name = "Academy" },
                    EstablishmentGroupType = new NameAndCodeDto
                    {
                        Code = "10"
                    },
                    Address = new AddressDto()
                },
                new EstablishmentDto
                {
                    Urn = "2",
                    Name = "Filtered",
                    EstablishmentType = new NameAndCodeDto { Name = "Not On Allowed Enum" },
                    EstablishmentGroupType = new NameAndCodeDto
                    {
                        Code = "999"
                    },
                    Address = new AddressDto()
                }
            ]);

        var (results, count) = await _sut.GetSearchResultsAsync("Test", PageSize);

        results.Should().ContainSingle(r => r.Name == "Included");
        results.Should().NotContain(r => r.Name == "Filtered");

        count.NumberOfSchools.Should().Be(1);
    }
    
    [Fact]
    public async Task GetSearchResultsAsync_should_search_by_reference_number_when_no_establishments_found()
    {
        _mockGetEstablishments.SearchEstablishments("TR123")
            .Returns([]);

        _mockGetTrusts.SearchTrusts("TR123")
            .Returns(new TrustListResponse<TrustDto>() { Data = [] });

        _mockGetTrusts.GetTrustByReferenceNumber("TR123")
            .Returns(new TrustDto
            {
                GroupUid = "1",
                Name = "Reference Trust",
                ReferenceNumber = "TR123",
                Type = new NameAndCodeDto { Name = "Trust" },
                Address = new AddressDto()
            });

        var (results, count) = await _sut.GetSearchResultsAsync("TR123", PageSize);

        results.Should().ContainSingle();
        results[0].Name.Should().Be("Reference Trust");
        count.NumberOfTrusts.Should().Be(1);
    }
    
    [Fact]
    public async Task GetSearchResultsAsync_should_return_the_correct_results_page_when_there_are_more_than_20_matches()
    {
        var trusts = Enumerable.Range(1, 30)
            .Select(i => new TrustDto
            {
                GroupUid = i.ToString(),
                Name = $"Trust {i:00}",
                ReferenceNumber = $"TR{i:000}",
                Type = new NameAndCodeDto { Name = "Trust" },
                Address = new AddressDto()
            });

        var schools = Enumerable.Range(1, 30)
            .Select(i => new EstablishmentDto
            {
                Urn = i.ToString(),
                Name = $"School {i:00}",
                EstablishmentNumber = i.ToString(),
                EstablishmentType = new NameAndCodeDto { Name = "Academy" },
                EstablishmentGroupType = new NameAndCodeDto
                {
                    Code = ((int)NameAndCodeEnums.AllowedEstablishmentGroupTypeCodes.Academies).ToString()
                },
                Address = new AddressDto()
            });

        _mockGetTrusts.SearchTrusts("Test")
            .Returns(new TrustListResponse<TrustDto>() { Data = trusts });

        _mockGetEstablishments.SearchEstablishments("Test")
            .Returns(schools.ToList());

        var (results, count) = await _sut.GetSearchResultsAsync("Test", 20, 2);

        results.Should().HaveCount(20);
        count.TotalRecords.Should().Be(60);
        count.NumberOfTrusts.Should().Be(30);
        count.NumberOfSchools.Should().Be(30);
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

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("99999")]
    public async Task GetSchoolAutoCompleteSearchResultsAsync_should_return_empty_when_text_is_blank_or_a_number_below_100000(string? text)
    {
        var results = await _sut.GetSchoolAutoCompleteSearchResultsAsync(text);

        results.Should().BeEmpty();

        await _mockGetEstablishments.DidNotReceive().SearchEstablishments(Arg.Any<string>());
    }

    [Fact]
    public async Task GetSchoolAutoCompleteSearchResultsAsync_should_map_establishment_to_search_result()
    {
        _mockGetEstablishments.SearchEstablishments("Test")
            .Returns(
            [
                new EstablishmentDto
                {
                    Urn = "100001",
                    Name = "Test School",
                    EstablishmentNumber = "2001",
                    EstablishmentType = new NameAndCodeDto { Name = "Academy" },
                    EstablishmentGroupType = new NameAndCodeDto { Code = "10" },
                    Address = new AddressDto
                    {
                        Street = "1 Test Street",
                        Town = "Testville",
                        Postcode = "AB1 2CD"
                    }
                }
            ]);

        var results = await _sut.GetSchoolAutoCompleteSearchResultsAsync("Test");

        results.Should().ContainSingle().Which.Should().BeEquivalentTo(new SearchResult(
            "100001",
            null,
            "Test School",
            "Academy",
            "1 Test Street, Testville, AB1 2CD",
            false,
            "2001"));
    }

    [Fact]
    public async Task GetSchoolAutoCompleteSearchResultsAsync_should_filter_out_invalid_establishment_group_types()
    {
        _mockGetEstablishments.SearchEstablishments("Test")
            .Returns(
            [
                CreateEstablishment("1", "Included"),
                CreateEstablishment("2", "Filtered", "999"),
                CreateEstablishment("3", "No Group Type Code", "not-a-number")
            ]);

        var results = await _sut.GetSchoolAutoCompleteSearchResultsAsync("Test");

        results.Should().ContainSingle(r => r.Name == "Included");
        results.Should().NotContain(r => r.Name == "Filtered");
        results.Should().NotContain(r => r.Name == "No Group Type Code");
    }

    [Fact]
    public async Task GetSchoolAutoCompleteSearchResultsAsync_should_order_names_starting_with_text_first()
    {
        _mockGetEstablishments.SearchEstablishments("Oak")
            .Returns(
            [
                CreateEstablishment("1", "The Oak School"),
                CreateEstablishment("2", "Oakwood Academy"),
                CreateEstablishment("3", "Big Oak Primary"),
                CreateEstablishment("4", "oak Hill School")
            ]);

        var results = await _sut.GetSchoolAutoCompleteSearchResultsAsync("Oak");

        results.Take(2).Select(r => r.Name).Should().BeEquivalentTo("Oakwood Academy", "oak Hill School");
        results.Skip(2).Select(r => r.Name).Should().BeEquivalentTo("The Oak School", "Big Oak Primary");
    }

    [Fact]
    public async Task GetSchoolAutoCompleteSearchResultsAsync_should_return_a_maximum_of_5_results()
    {
        _mockGetEstablishments.SearchEstablishments("School")
            .Returns(Enumerable.Range(1, 10)
                .Select(i => CreateEstablishment(i.ToString(), $"School {i:00}"))
                .ToList());

        var results = await _sut.GetSchoolAutoCompleteSearchResultsAsync("School");

        results.Should().HaveCount(5);
    }

    [Fact]
    public async Task GetSchoolAutoCompleteSearchResultsAsync_should_return_empty_when_no_establishments_found()
    {
        _mockGetEstablishments.SearchEstablishments("Nothing").Returns([]);

        var results = await _sut.GetSchoolAutoCompleteSearchResultsAsync("Nothing");

        results.Should().BeEmpty();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("99999")]
    public async Task GetTrustAutoCompleteSearchResultsAsync_should_return_empty_when_text_is_blank_or_a_number_below_100000(string? text)
    {
        var results = await _sut.GetTrustAutoCompleteSearchResultsAsync(text);

        results.Should().BeEmpty();

        await _mockGetTrusts.DidNotReceive().SearchTrusts(Arg.Any<string>());
        await _mockGetTrusts.DidNotReceive().GetTrustByReferenceNumber(Arg.Any<string>());
    }

    [Fact]
    public async Task GetTrustAutoCompleteSearchResultsAsync_should_map_trust_to_search_result()
    {
        _mockGetTrusts.SearchTrusts("Test")
            .Returns(new TrustListResponse<TrustDto>
            {
                Data =
                [
                    new TrustDto
                    {
                        GroupUid = "1223",
                        Name = "Test Trust",
                        ReferenceNumber = "TR001",
                        Type = new NameAndCodeDto { Name = "Multi-academy trust" },
                        Address = new AddressDto
                        {
                            Street = "1 Test Street",
                            Town = "Testville",
                            Postcode = "AB1 2CD"
                        }
                    }
                ]
            });

        var results = await _sut.GetTrustAutoCompleteSearchResultsAsync("Test");

        results.Should().ContainSingle().Which.Should().BeEquivalentTo(new SearchResult(
            "1223",
            "TR001",
            "Test Trust",
            "Multi-academy trust",
            "1 Test Street, Testville, AB1 2CD",
            true,
            "TR001"));

        await _mockGetTrusts.DidNotReceive().GetTrustByReferenceNumber(Arg.Any<string>());
    }

    [Fact]
    public async Task GetTrustAutoCompleteSearchResultsAsync_should_search_by_reference_number_when_no_trusts_found()
    {
        _mockGetTrusts.SearchTrusts("TR123")
            .Returns(new TrustListResponse<TrustDto> { Data = [] });

        _mockGetTrusts.GetTrustByReferenceNumber("TR123")
            .Returns(CreateTrust("1", "Reference Trust", "TR123"));

        var results = await _sut.GetTrustAutoCompleteSearchResultsAsync("TR123");

        results.Should().ContainSingle();
        results[0].Name.Should().Be("Reference Trust");
        results[0].IsTrust.Should().BeTrue();
    }

    [Fact]
    public async Task GetTrustAutoCompleteSearchResultsAsync_should_search_by_reference_number_when_search_data_is_null()
    {
        _mockGetTrusts.SearchTrusts("TR123")
            .Returns(new TrustListResponse<TrustDto> { Data = null });

        _mockGetTrusts.GetTrustByReferenceNumber("TR123")
            .Returns(CreateTrust("1", "Reference Trust", "TR123"));

        var results = await _sut.GetTrustAutoCompleteSearchResultsAsync("TR123");

        results.Should().ContainSingle().Which.Name.Should().Be("Reference Trust");
    }

    [Fact]
    public async Task GetTrustAutoCompleteSearchResultsAsync_should_return_empty_when_no_trusts_and_no_reference_number_match()
    {
        _mockGetTrusts.SearchTrusts("TR123")
            .Returns(new TrustListResponse<TrustDto> { Data = [] });

        _mockGetTrusts.GetTrustByReferenceNumber("TR123").Returns((TrustDto?)null);

        var results = await _sut.GetTrustAutoCompleteSearchResultsAsync("TR123");

        results.Should().BeEmpty();
    }

    [Fact]
    public async Task GetTrustAutoCompleteSearchResultsAsync_should_return_a_maximum_of_5_results()
    {
        _mockGetTrusts.SearchTrusts("Trust")
            .Returns(new TrustListResponse<TrustDto>
            {
                Data = Enumerable.Range(1, 10)
                    .Select(i => CreateTrust(i.ToString(), $"Trust {i:00}", $"TR{i:000}"))
                    .ToList()
            });

        var results = await _sut.GetTrustAutoCompleteSearchResultsAsync("Trust");

        results.Should().HaveCount(5);
    }

    [Fact]
    public async Task GetTrustAutoCompleteSearchResultsAsync_should_order_names_not_starting_with_text_before_names_starting_with_text()
    {
        _mockGetTrusts.SearchTrusts("Oak")
            .Returns(new TrustListResponse<TrustDto>
            {
                Data =
                [
                    CreateTrust("1", "Oakwood Trust", "TR001"),
                    CreateTrust("2", "The Oak Trust", "TR002")
                ]
            });

        var results = await _sut.GetTrustAutoCompleteSearchResultsAsync("Oak");

        results.Select(r => r.Name).Should().Equal("The Oak Trust", "Oakwood Trust");
    }

    [Fact]
    public async Task GetTrustAutoCompleteSearchResultsAsync_should_use_empty_strings_when_trust_has_no_group_uid_or_name()
    {
        _mockGetTrusts.SearchTrusts("Test")
            .Returns(new TrustListResponse<TrustDto>
            {
                Data =
                [
                    new TrustDto
                    {
                        GroupUid = null,
                        Name = null!,
                        ReferenceNumber = "TR001",
                        Type = new NameAndCodeDto { Name = "Trust" },
                        Address = new AddressDto()
                    }
                ]
            });

        var results = await _sut.GetTrustAutoCompleteSearchResultsAsync("Test");

        results.Should().ContainSingle();
        results[0].Id.Should().BeEmpty();
        results[0].Name.Should().BeEmpty();
    }
}
