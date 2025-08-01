using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.Academy;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.Ofsted;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.PipelineAcademy;
using DfE.FindInformationAcademiesTrusts.Services.Academy;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Services;

public class AcademyServiceTests
{
    private readonly AcademyService _sut;
    private readonly IAcademyRepository _mockAcademyRepository = Substitute.For<IAcademyRepository>();
    private readonly IOfstedRepository _mockOfstedRepository = Substitute.For<IOfstedRepository>();

    private readonly IPipelineEstablishmentRepository _mockPipelineEstablishmentRepository =
        Substitute.For<IPipelineEstablishmentRepository>();

    private readonly IFreeSchoolMealsAverageProvider _mockFreeSchoolMealsAverageProvider =
        Substitute.For<IFreeSchoolMealsAverageProvider>();

    public AcademyServiceTests()
    {
        _sut = new AcademyService(_mockAcademyRepository, _mockOfstedRepository, _mockPipelineEstablishmentRepository,
            _mockFreeSchoolMealsAverageProvider);
    }

    [Fact]
    public async Task GetAcademiesInTrustDetailsAsync_should_return_mapped_result_from_repository()
    {
        const string uid = "1234";

        AcademyDetails[] academies =
        [
            new("9876", "Academy 1", "Academy converter", "Oxfordshire",
                "Urban city and town", DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-10))),
            new("9876", "Academy 2", "Academy sponsor led", "Lincolnshire",
                "Rural town and fringe", DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-5)))
        ];
        
        _mockAcademyRepository.GetAcademiesInTrustDetailsAsync(uid).Returns(academies);

        var result = await _sut.GetAcademiesInTrustDetailsAsync(uid);

        result.Should().BeOfType<AcademyDetailsServiceModel[]>();
        result.Should().BeEquivalentTo(academies);
    }

    [Fact]
    public async Task GetAcademiesInTrustOfstedAsync_should_return_mapped_result_from_repository()
    {
        const string uid = "1234";
        var academies = new[]
        {
            new SchoolOfsted("1", "Academy 1", new DateTime(2022, 12, 1),
                new OfstedShortInspection(new DateTime(2025, 7, 1), "School remains Good"),
                new OfstedRating((int)OfstedRatingScore.Good, new DateTime(2023, 1, 1)),
                new OfstedRating((int)OfstedRatingScore.RequiresImprovement, new DateTime(2023, 2, 1))),
            new SchoolOfsted("2", "Academy 2", new DateTime(2022, 11, 2),
                new OfstedShortInspection(new DateTime(2025, 7, 2), "School remains Good"),
                new OfstedRating((int)OfstedRatingScore.Good, new DateTime(2023, 1, 2)),
                new OfstedRating((int)OfstedRatingScore.RequiresImprovement, new DateTime(2023, 3, 1))),
            new SchoolOfsted("3", "Academy 3", new DateTime(2022, 10, 3),
                new OfstedShortInspection(new DateTime(2025, 7, 3), "School remains Good"),
                new OfstedRating((int)OfstedRatingScore.Good, new DateTime(2023, 1, 3)),
                new OfstedRating((int)OfstedRatingScore.RequiresImprovement, new DateTime(2023, 4, 1)))
        };

        _mockOfstedRepository.GetAcademiesInTrustOfstedAsync(uid).Returns(academies);

        var result = await _sut.GetAcademiesInTrustOfstedAsync(uid);

        result.Should().BeOfType<AcademyOfstedServiceModel[]>();
        result.Should().BeEquivalentTo(academies);
    }

    [Fact]
    public async Task GetAcademiesInTrustPupilNumbersAsync_should_return_mapped_result_from_repository()
    {
        const string uid = "1234";
        AcademyPupilNumbers[] academies =
        [
            BuildDummyAcademyPupilNumbers("9876", "phase1", new AgeRange(2, 15), 100, 200),
            BuildDummyAcademyPupilNumbers("8765", "phase2", new AgeRange(7, 12), 2, 5)
        ];
        
        _mockAcademyRepository.GetAcademiesInTrustPupilNumbersAsync(uid).Returns(academies);
        
        var result = await _sut.GetAcademiesInTrustPupilNumbersAsync(uid);

        result.Should().BeOfType<AcademyPupilNumbersServiceModel[]>();
        result.Should().BeEquivalentTo(academies);
    }

    private static AcademyPupilNumbers BuildDummyAcademyPupilNumbers(string urn,
        string phaseOfEducation = "test",
        AgeRange? ageRange = null,
        int? numberOfPupils = 300,
        int? schoolCapacity = 400
    )
    {
        return new AcademyPupilNumbers(urn,
            $"Academy {urn}",
            phaseOfEducation,
            ageRange ?? new AgeRange(11, 18),
            numberOfPupils,
            schoolCapacity);
    }

    [Fact]
    public async Task GetAcademiesInTrustFreeSchoolMealsAsync_should_map_relevant_fields_from_repository()
    {
        const string uid = "1234";
        var academy =
            new AcademyFreeSchoolMeals(string.Empty, null, null, 0, string.Empty, string.Empty);
        AcademyFreeSchoolMeals[] academies =
        [
            academy with { Urn = "1", EstablishmentName = "Academy 1", PercentageFreeSchoolMeals = 12.0 },
            academy with { Urn = "2", EstablishmentName = "Academy 2", PercentageFreeSchoolMeals = 0 },
            academy with { Urn = "3", EstablishmentName = "Academy 3", PercentageFreeSchoolMeals = 24.1 },
            academy with { Urn = "4", EstablishmentName = "Academy 4", PercentageFreeSchoolMeals = null },
            academy with { Urn = "5", EstablishmentName = "Academy 5", PercentageFreeSchoolMeals = 100 }
        ];
        
        _mockAcademyRepository.GetAcademiesInTrustFreeSchoolMealsAsync(uid).Returns(academies);
        
        var result = await _sut.GetAcademiesInTrustFreeSchoolMealsAsync(uid);

        result.Should().BeOfType<AcademyFreeSchoolMealsServiceModel[]>();
        result.Should().BeEquivalentTo(academies, options => options
            .Excluding(a => a.LocalAuthorityCode)
            .Excluding(a => a.PhaseOfEducation)
            .Excluding(a => a.TypeOfEstablishment));
    }

    [Fact]
    public async Task
        GetAcademiesInTrustFreeSchoolMealsAsync_should_get_relevant_fields_from_freeSchoolMealsAveragesProvider()
    {
        const string uid = "1234";
        var academy =
            new AcademyFreeSchoolMeals(string.Empty, null, null, 0, string.Empty, string.Empty);
        AcademyFreeSchoolMeals[] academies =
        [
            academy with
            {
                LocalAuthorityCode = 12, PhaseOfEducation = "Primary", TypeOfEstablishment = "Community school"
            },
            academy with
            {
                LocalAuthorityCode = 22, PhaseOfEducation = "Secondary", TypeOfEstablishment = "Academy converter"
            },
            academy with
            {
                LocalAuthorityCode = 34, TypeOfEstablishment = "Foundation special school"
            }
        ];
        _mockAcademyRepository.GetAcademiesInTrustFreeSchoolMealsAsync(uid).Returns(academies);

        _mockFreeSchoolMealsAverageProvider.GetLaAverage(12, "Primary", "Community school").Returns(22.4);
        _mockFreeSchoolMealsAverageProvider.GetLaAverage(22, "Secondary", "Academy converter").Returns(23.5);
        _mockFreeSchoolMealsAverageProvider.GetLaAverage(34, string.Empty, "Foundation special school").Returns(70);
        _mockFreeSchoolMealsAverageProvider.GetNationalAverage("Primary", "Community school").Returns(12.4);
        _mockFreeSchoolMealsAverageProvider.GetNationalAverage("Secondary", "Academy converter").Returns(13.5);
        _mockFreeSchoolMealsAverageProvider.GetNationalAverage(string.Empty, "Foundation special school").Returns(60);

        var result = await _sut.GetAcademiesInTrustFreeSchoolMealsAsync(uid);

        result[0].LaAveragePercentageFreeSchoolMeals.Should().Be(22.4);
        result[0].NationalAveragePercentageFreeSchoolMeals.Should().Be(12.4);
        result[1].LaAveragePercentageFreeSchoolMeals.Should().Be(23.5);
        result[1].NationalAveragePercentageFreeSchoolMeals.Should().Be(13.5);
        result[2].LaAveragePercentageFreeSchoolMeals.Should().Be(70);
        result[2].NationalAveragePercentageFreeSchoolMeals.Should().Be(60);
    }

    [Fact]
    public async Task GetAcademiesPipelineSummaryAsync_should_return_mapped_result_from_repository()
    {
        // Arrange
        const string trustReferenceNumber = "TRN123";
        var repoSummary = new PipelineSummary(5, 3, 2);
        
        _mockPipelineEstablishmentRepository.GetAcademiesPipelineSummaryAsync(trustReferenceNumber).Returns(
            Task.FromResult(repoSummary));

        // Act
        var result = await _sut.GetAcademiesPipelineSummaryAsync(trustReferenceNumber);

        // Assert
        result.PreAdvisoryCount.Should().Be(repoSummary.PreAdvisoryCount);
        result.PostAdvisoryCount.Should().Be(repoSummary.PostAdvisoryCount);
        result.FreeSchoolsCount.Should().Be(repoSummary.FreeSchoolsCount);
    }

    // Pre Advisory Pipeline tests

    [Fact]
    public async Task GetAcademiesPipelinePreAdvisoryAsync_should_return_empty_array_when_no_establishments_found()
    {
        // Arrange
        const string trustReferenceNumber = "TRN123";
        _mockPipelineEstablishmentRepository
            .GetAdvisoryConversionEstablishmentsAsync(trustReferenceNumber, AdvisoryType.PreAdvisory)
            .Returns(Task.FromResult<PipelineEstablishment[]>([]));
        _mockPipelineEstablishmentRepository
            .GetAdvisoryTransferEstablishmentsAsync(trustReferenceNumber, AdvisoryType.PreAdvisory)
            .Returns(Task.FromResult<PipelineEstablishment[]>([]));

        // Act
        var result = await _sut.GetAcademiesPipelinePreAdvisoryAsync(trustReferenceNumber);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAcademiesPipelinePreAdvisoryAsync_should_return_mapped_establishments_when_data_found()
    {
        // Arrange
        const string trustReferenceNumber = "TRN123";

        var conversionEstablishment = new PipelineEstablishment(
            "1001",
            "Academy Conversion 1",
            new AgeRange(11, 18),
            "Authority A",
            ProjectType.Conversion,
            new DateTime(2023, 1, 1));

        var transferEstablishment = new PipelineEstablishment(
            "1002",
            "Academy Transfer 1",
            new AgeRange(11, 16),
            "Authority B",
            ProjectType.Transfer,
            new DateTime(2023, 2, 1));

        _mockPipelineEstablishmentRepository.GetAdvisoryConversionEstablishmentsAsync(trustReferenceNumber, AdvisoryType.PreAdvisory).Returns(
            Task.FromResult(new[] { conversionEstablishment }));
        _mockPipelineEstablishmentRepository.GetAdvisoryTransferEstablishmentsAsync(trustReferenceNumber, AdvisoryType.PreAdvisory).Returns(
            Task.FromResult(new[] { transferEstablishment }));

        // Act
        var result = await _sut.GetAcademiesPipelinePreAdvisoryAsync(trustReferenceNumber);

        // Assert
        result.Should().HaveCount(2);

        // Verify first establishment (from conversion)
        result[0].Urn.Should().Be("1001");
        result[0].EstablishmentName.Should().Be("Academy Conversion 1");
        result[0].AgeRange.Should().Be(new AgeRange(11, 18));
        result[0].LocalAuthority.Should().Be("Authority A");
        result[0].ProjectType.Should().Be("Conversion");
        result[0].ChangeDate.Should().Be(new DateTime(2023, 1, 1));

        // Verify second establishment (from transfer)
        result[1].Urn.Should().Be("1002");
        result[1].EstablishmentName.Should().Be("Academy Transfer 1");
        result[1].AgeRange.Should().Be(new AgeRange(11, 16));
        result[1].LocalAuthority.Should().Be("Authority B");
        result[1].ProjectType.Should().Be("Transfer");
        result[1].ChangeDate.Should().Be(new DateTime(2023, 2, 1));
    }

    // Post Advisory Pipeline tests

    [Fact]
    public async Task GetAcademiesPipelinePostAdvisoryAsync_should_return_empty_array_when_no_establishments_found()
    {
        // Arrange
        const string trustReferenceNumber = "TRN123";
        _mockPipelineEstablishmentRepository.GetAdvisoryConversionEstablishmentsAsync(trustReferenceNumber, AdvisoryType.PostAdvisory).Returns(
            Task.FromResult(Array.Empty<PipelineEstablishment>()));
        _mockPipelineEstablishmentRepository.GetAdvisoryTransferEstablishmentsAsync(trustReferenceNumber, AdvisoryType.PostAdvisory).Returns(
            Task.FromResult(Array.Empty<PipelineEstablishment>()));

        // Act
        var result = await _sut.GetAcademiesPipelinePostAdvisoryAsync(trustReferenceNumber);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAcademiesPipelinePostAdvisoryAsync_should_return_mapped_establishments_when_data_found()
    {
        // Arrange
        const string trustReferenceNumber = "TRN123";

        var conversionEstablishment = new PipelineEstablishment(
            "2001",
            "Academy Post Advisory Conversion 1",
            new AgeRange(11, 18),
            "Authority X",
            "Conversion",
            new DateTime(2023, 3, 1));

        var transferEstablishment = new PipelineEstablishment(
            "2002",
            "Academy Post Advisory Transfer 1",
            new AgeRange(11, 16),
            "Authority Y",
            "Transfer",
            new DateTime(2023, 4, 1));

        _mockPipelineEstablishmentRepository.GetAdvisoryConversionEstablishmentsAsync(trustReferenceNumber, AdvisoryType.PostAdvisory).Returns(
            Task.FromResult(new[] { conversionEstablishment }));
        _mockPipelineEstablishmentRepository.GetAdvisoryTransferEstablishmentsAsync(trustReferenceNumber, AdvisoryType.PostAdvisory).Returns(
            Task.FromResult(new[] { transferEstablishment }));

        // Act
        var result = await _sut.GetAcademiesPipelinePostAdvisoryAsync(trustReferenceNumber);

        // Assert
        result.Should().HaveCount(2);

        // Verify first establishment (from conversion)
        result[0].Urn.Should().Be("2001");
        result[0].EstablishmentName.Should().Be("Academy Post Advisory Conversion 1");
        result[0].AgeRange.Should().Be(new AgeRange(11, 18));
        result[0].LocalAuthority.Should().Be("Authority X");
        result[0].ProjectType.Should().Be("Conversion");
        result[0].ChangeDate.Should().Be(new DateTime(2023, 3, 1));

        // Verify second establishment (from transfer)
        result[1].Urn.Should().Be("2002");
        result[1].EstablishmentName.Should().Be("Academy Post Advisory Transfer 1");
        result[1].AgeRange.Should().Be(new AgeRange(11, 16));
        result[1].LocalAuthority.Should().Be("Authority Y");
        result[1].ProjectType.Should().Be("Transfer");
        result[1].ChangeDate.Should().Be(new DateTime(2023, 4, 1));
    }

    // Free schools

    [Fact]
    public async Task GetAcademiesPipelineFreeSchoolsAsync_should_return_empty_array_when_repository_returns_null()
    {
        // Arrange
        const string trustReferenceNumber = "TRN123";
        _mockPipelineEstablishmentRepository.GetPipelineFreeSchoolProjectsAsync(trustReferenceNumber)
            .Returns(Task.FromResult<PipelineEstablishment[]>([]));

        // Act
        var result = await _sut.GetAcademiesPipelineFreeSchoolsAsync(trustReferenceNumber);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAcademiesPipelineFreeSchoolsAsync_should_return_empty_array_when_repository_returns_empty_array()
    {
        // Arrange
        const string trustReferenceNumber = "TRN123";
        _mockPipelineEstablishmentRepository.GetPipelineFreeSchoolProjectsAsync(trustReferenceNumber)
            .Returns(Task.FromResult<PipelineEstablishment[]>([]));

        // Act
        var result = await _sut.GetAcademiesPipelineFreeSchoolsAsync(trustReferenceNumber);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAcademiesPipelineFreeSchoolsAsync_should_return_mapped_establishments_when_data_found()
    {
        // Arrange
        const string trustReferenceNumber = "TRN123";

        var freeSchoolEstablishment1 = new PipelineEstablishment(
            "3001",
            "Free School 1",
            new AgeRange(11, 18),
            "Authority FS1",
            "FreeSchool",
            new DateTime(2023, 5, 1));

        var freeSchoolEstablishment2 = new PipelineEstablishment(
            "3002",
            "Free School 2",
            new AgeRange(11, 16),
            "Authority FS2",
            "FreeSchool",
            new DateTime(2023, 6, 1));

        _mockPipelineEstablishmentRepository.GetPipelineFreeSchoolProjectsAsync(trustReferenceNumber).Returns(
            Task.FromResult<PipelineEstablishment[]>(
                [freeSchoolEstablishment1, freeSchoolEstablishment2]));

        // Act
        var result = await _sut.GetAcademiesPipelineFreeSchoolsAsync(trustReferenceNumber);

        // Assert
        result.Should().HaveCount(2);

        // Verify first establishment
        result[0].Urn.Should().Be("3001");
        result[0].EstablishmentName.Should().Be("Free School 1");
        result[0].AgeRange.Should().Be(new AgeRange(11, 18));
        result[0].LocalAuthority.Should().Be("Authority FS1");
        result[0].ProjectType.Should().Be("FreeSchool");
        result[0].ChangeDate.Should().Be(new DateTime(2023, 5, 1));

        // Verify second establishment
        result[1].Urn.Should().Be("3002");
        result[1].EstablishmentName.Should().Be("Free School 2");
        result[1].AgeRange.Should().Be(new AgeRange(11, 16));
        result[1].LocalAuthority.Should().Be("Authority FS2");
        result[1].ProjectType.Should().Be("FreeSchool");
        result[1].ChangeDate.Should().Be(new DateTime(2023, 6, 1));
    }

    [Fact]
    public async Task GetAcademiesInTrustDetailsAsync_ShouldRemoveEnglandWalesFromUrbanRural()
    {
        const string uid = "1234";
        
        AcademyDetails[] academies =
        [
            new("9876", "Academy 1", "Academy converter", "Oxfordshire",
                "(England/Wales) Urban city and town", DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-10))),
            new("9876", "Academy 2", "Academy sponsor led", "Lincolnshire",
                "(England/Wales) Rural town and fringe", DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-10)))
        ];

        _mockAcademyRepository.GetAcademiesInTrustDetailsAsync(uid).Returns(academies);

        AcademyDetailsServiceModel[] result = await _sut.GetAcademiesInTrustDetailsAsync(uid);

        result[0].UrbanRural.Should().Be("Urban city and town");
        result[1].UrbanRural.Should().Be("Rural town and fringe");
    }
}
