using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.AcademiesDbServices;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Models.Gias;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Repositories;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.Academy;
using GovUK.Dfe.CoreLibs.Contracts.Academies.V4.Establishments;
using EstablishmentDto = GovUK.Dfe.CoreLibs.Contracts.Academies.V5.Establishments.EstablishmentDto;

namespace DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.UnitTests.Repositories;

public class AcademyRepositoryTests
{
    private const string GroupUid = "1234";
    private const string ReferenceNumber = "TRN1234";
    private readonly AcademyRepository _sut;
    private readonly MockAcademiesDbContext _mockAcademiesDbContext = new();
    private readonly IGetEstablishments _mockGetEstablishments;
    

    public AcademyRepositoryTests()
    {
        _mockGetEstablishments = Substitute.For<IGetEstablishments>();
        _sut = new AcademyRepository(_mockAcademiesDbContext.Object, _mockGetEstablishments);
    }

    [Fact]
    public async Task GetAcademiesInTrustDetailsAsync_should_return_academies_linked_to_trust()
    {
        var giasGroup = _mockAcademiesDbContext.AddGiasGroupForTrust(GroupUid);
        var giasEstablishments = Enumerable.Range(1000, 6).Select(n => new GiasEstablishment
        {
            Urn = n,
            EstablishmentName = $"Academy {n}",
            TypeOfEstablishmentName = $"Academy type {n}",
            LaName = $"Local authority {n}",
            UrbanRuralName = $"UrbanRuralName {n}",
            EstablishmentTypeGroupName = "Academies",
            EstablishmentStatusName = "Open"
        }).ToArray();
        _mockAcademiesDbContext.GiasEstablishments.AddRange(giasEstablishments);
        _mockAcademiesDbContext.AddGiasGroupLinks(giasGroup, giasEstablishments);

        var result = await _sut.GetAcademiesInTrustDetailsAsync(GroupUid);

        result.Should()
            .BeEquivalentTo(giasEstablishments,
                options => options
                    .WithAutoConversion()
                    .ExcludingMissingMembers()
                    .WithMapping<AcademyDetails>(e => e.TypeOfEstablishmentName, a => a.TypeOfEstablishment)
                    .WithMapping<AcademyDetails>(e => e.LaName, a => a.LocalAuthority)
                    .WithMapping<AcademyDetails>(e => e.UrbanRuralName, a => a.UrbanRural)
            );
    }

    [Fact]
    public async Task GetAcademiesInTrustDetailsAsync_should_return_empty_array_when_no_academies_linked_to_trust()
    {
        var result = await _sut.GetAcademiesInTrustDetailsAsync(GroupUid);
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetNumberOfAcademiesInTrustAsync_should_return_zero_when_no_academies()
    {
        var result = await _sut.GetNumberOfAcademiesInTrustAsync(ReferenceNumber);
        result.Should().Be(0);
    }

    [Fact]
    public async Task GetNumberOfAcademiesInTrustAsync_should_return_number_of_academies()
    {
        _mockGetEstablishments.GetEstablishmentsByTrustReferenceNumber(ReferenceNumber).Returns(new []
        {
            new EstablishmentDto
            {
                Urn = "1234",
                Name = "Academy1",
            },
            new EstablishmentDto()
            {
                Urn = "1235",
                Name = "Academy2",
            }

        });

        var result = await _sut.GetNumberOfAcademiesInTrustAsync(ReferenceNumber);
        result.Should().Be(2);
    }

    [Fact]
    public async Task
        GetUrnForSingleAcademyTrustAsync_should_set_singleAcademyTrustAcademyUrn_to_null_when_multi_academy_trust()
    {
        var mat = _mockAcademiesDbContext.AddGiasGroupForTrust("2806", groupType: "Multi-academy trust");
        var academy = _mockAcademiesDbContext.AddGiasEstablishment(1234);
        _mockAcademiesDbContext.AddGiasGroupLinks(mat, academy);

        var result = await _sut.GetSingleAcademyTrustAcademyUrnAsync("2806");

        result.Should().BeNull();
    }

    [Fact]
    public async Task
        GetUrnForSingleAcademyTrustAsync_should_set_singleAcademyTrustAcademyUrn_to_null_when_Federation()
    {
        var mat = _mockAcademiesDbContext.AddGiasGroupForFederation("2806");
        var academy = _mockAcademiesDbContext.AddGiasEstablishment(1234);
        _mockAcademiesDbContext.AddGiasGroupLinks(mat, academy);

        var result = await _sut.GetSingleAcademyTrustAcademyUrnAsync("2806");

        result.Should().BeNull();
    }

    [Fact]
    public async Task
        GetUrnForSingleAcademyTrustAsync_should_set_singleAcademyTrustAcademyUrn_to_null_when_SAT_with_no_academies()
    {
        _ = _mockAcademiesDbContext.AddGiasGroupForTrust("2806", groupType: "Single-academy trust");

        var result = await _sut.GetSingleAcademyTrustAcademyUrnAsync("2806");

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetUrnForSingleAcademyTrustAsync_should_set_singleAcademyTrustAcademyUrn_to_urn_of_SAT_academy()
    {
        var sat = _mockAcademiesDbContext.AddGiasGroupForTrust("2806", groupType: "Single-academy trust");
        var academy = _mockAcademiesDbContext.AddGiasEstablishment(123456);
        _mockAcademiesDbContext.AddGiasGroupLinks(sat, academy);

        var result = await _sut.GetSingleAcademyTrustAcademyUrnAsync("2806");

        result.Should().Be("123456");
    }

    [Fact]
    public async Task GetAcademiesInTrustPupilNumbersByTrnAsync_should_return_academies_linked_to_trust()
    {

        _mockGetEstablishments.GetEstablishmentsByTrustReferenceNumber(ReferenceNumber).Returns(new EstablishmentDto[]
        {
            new EstablishmentDto
            {
                Urn = "1234",
                Name = "Academy1",
                PhaseOfEducation = new NameAndCodeDto()
                {
                    Name = "Test",
                    Code = "1234"
                },
                Census = new CensusDto()
                {
                    NumberOfPupils = "1234",
                },
                StatutoryHighAge = "3",
                StatutoryLowAge = "2",
                SchoolCapacity = "332"
            },
            new EstablishmentDto()
            {
                Urn = "1235",
                Name = "Academy2",
                PhaseOfEducation = new NameAndCodeDto()
                {
                    Name = "Test2",
                    Code = "1235"
                },
                Census = new CensusDto()
                {
                    NumberOfPupils = "1235",
                },
                StatutoryHighAge = "4",
                StatutoryLowAge = "3",
                SchoolCapacity = "333"
            }

        });

        var result = await _sut.GetAcademiesInTrustPupilNumbersByTrnAsync(ReferenceNumber);
        result.Should().HaveCount(2);
        result.Should().BeEquivalentTo(new[]
        {
            new AcademyPupilNumbers(
                "1234",
                "Academy1",
                "Test",
                new AgeRange(2, 3),
                1234,
                332),
            new AcademyPupilNumbers(
                "1235",
                "Academy2",
                "Test2",
                new AgeRange(3, 4),
                1235,
                333)
        });
    }

    [Fact]
    public async Task GetAcademiesInTrustPupilNumbersByTrnAsync_should_return_empty_array_when_no_academies_linked_to_trust()
    {
        _mockGetEstablishments.GetEstablishmentsByTrustReferenceNumber(ReferenceNumber).Returns([]);

        var result = await _sut.GetAcademiesInTrustPupilNumbersByTrnAsync(ReferenceNumber);
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task
        GetAcademiesInTrustFreeSchoolMealsAsync_should_return_empty_array_when_no_academies_linked_to_trust()
    {
        var result = await _sut.GetAcademiesInTrustFreeSchoolMealsAsync(GroupUid);
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetOverviewOfAcademiesInTrustAsync_should_return_academies_linked_to_trust()
    {
        // Arrange
       var establishments = Enumerable.Range(1000, 3).Select(n => new EstablishmentDto
        {
            Urn = n.ToString(),
            Name = $"Academy {n}",
            LocalAuthorityName = $"Local authority {n}",
            EstablishmentGroupType = new NameAndCodeDto()
            {
                Name = "Academies",
            },
            Census = new CensusDto()
            {
                NumberOfPupils = (n * 10).ToString(),
            },
            SchoolCapacity = (n * 15).ToString()
        }).ToArray();
        
        _mockGetEstablishments.GetEstablishmentsByTrustReferenceNumber(ReferenceNumber).Returns(establishments);

        // Act
        var result = await _sut.GetOverviewOfAcademiesInTrustAsync(ReferenceNumber);

        // Assert
        result.Should().BeEquivalentTo(establishments,
            options => options
                .WithAutoConversion()
                .ExcludingMissingMembers()
                .WithMapping<AcademyOverview>(e => e.LocalAuthorityName, a => a.LocalAuthority)
        );
    }

    [Fact]
    public async Task GetOverviewOfAcademiesInTrustAsync_should_return_empty_array_when_no_academies_linked_to_trust()
    {
        var result = await _sut.GetOverviewOfAcademiesInTrustAsync(GroupUid);
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetOverviewOfAcademiesInTrustAsync_should_return_empty_array_when_trust_does_not_exist()
    {
        var result = await _sut.GetOverviewOfAcademiesInTrustAsync("non-existent-uid");
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }
}
