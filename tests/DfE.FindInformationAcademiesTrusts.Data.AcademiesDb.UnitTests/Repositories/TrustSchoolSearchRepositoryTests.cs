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
}
