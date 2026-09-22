using System.Net;
using Dfe.AcademiesApi.Client.Contracts;
using DfE.FindInformationAcademiesTrusts.Http;
using DfE.FindInformationAcademiesTrusts.HttpServices;
using Moq;
using EstablishmentDto = GovUK.Dfe.CoreLibs.Contracts.Academies.V4.Establishments.EstablishmentDto;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.HttpServices;

public class GetEstablishmentsTempTests
{
    private readonly Mock<IDfeHttpClientFactoryTemp> _httpClientFactoryMock = new();
    private readonly Mock<IHttpClientServiceTemp> _httpClientServiceMock = new();

    private readonly HttpClient _httpClient = new()
    {
        BaseAddress = new Uri("https://fakeapi.com")
    };

    private GetEstablishmentsTemp CreateSut()
    {
        _httpClientFactoryMock
            .Setup(f => f.CreateAcademiesClient())
            .Returns(_httpClient);

        return new GetEstablishmentsTemp(
            _httpClientFactoryMock.Object,
            _httpClientServiceMock.Object);
    }

    [Fact]
    public async Task SearchEstablishments_ReturnsResults_WhenApiCallIsSuccessful()
    {
        // Arrange
        var searchQuery = "Test School";

        var expectedResponse = new List<EstablishmentDto>
        {
            new()
            {
                Urn = "123456",
                Name = "Test School"
            }
        };

        _httpClientServiceMock
            .Setup(x => x.Get<List<EstablishmentDto>>(It.IsAny<HttpClient>(), It.IsAny<string>()))
            .ReturnsAsync(new ApiResponseTemp<List<EstablishmentDto>>(HttpStatusCode.OK, expectedResponse));

        var service = CreateSut();

        // Act
        var result = await service.SearchEstablishments(searchQuery);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(expectedResponse[0].Urn, result[0].Urn);
        Assert.Equal(expectedResponse[0].Name, result[0].Name);
    }

    [Fact]
    public async Task SearchEstablishments_ThrowsApiResponseTempException_WhenApiReturnsError()
    {
        // Arrange
        _httpClientServiceMock
            .Setup(x => x.Get<List<EstablishmentDto>>(It.IsAny<HttpClient>(), It.IsAny<string>()))
            .ReturnsAsync(new ApiResponseTemp<List<EstablishmentDto>>(HttpStatusCode.InternalServerError, null!));

        var service = CreateSut();

        // Act & Assert
        var exception =
            await Assert.ThrowsAsync<ApiResponseTempException>(() => service.SearchEstablishments("Test School"));

        Assert.Contains("InternalServerError", exception.Message);
    }

    [Fact]
    public async Task SearchEstablishments_CallsApiWithCorrectPath()
    {
        // Arrange
        var searchQuery = "Test School";
        string? capturedPath = null;

        _httpClientServiceMock
            .Setup(x => x.Get<List<EstablishmentDto>>(It.IsAny<HttpClient>(), It.IsAny<string>()))
            .Callback<HttpClient, string>((_, path) => capturedPath = path)
            .ReturnsAsync(new ApiResponseTemp<List<EstablishmentDto>>(HttpStatusCode.OK, []));

        var service = CreateSut();

        // Act
        await service.SearchEstablishments(searchQuery);

        // Assert
        Assert.Equal(
            $"v4/establishments?name={searchQuery}&urn={searchQuery}&excludeClosed=true&matchAny=true",
            capturedPath);
    }

    [Fact]
    public async Task GetEstablishment_ReturnsResult_WhenApiCallIsSuccessful()
    {
        // Arrange
        var expectedResponse = new EstablishmentDto
        {
            Urn = "123456",
            Name = "Test School"
        };

        _httpClientServiceMock
            .Setup(x => x.Get<EstablishmentDto>(It.IsAny<HttpClient>(), It.IsAny<string>()))
            .ReturnsAsync(new ApiResponseTemp<EstablishmentDto>(HttpStatusCode.OK, expectedResponse));

        var service = CreateSut();

        // Act
        var result = await service.GetEstablishment(123456);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResponse.Urn, result.Urn);
        Assert.Equal(expectedResponse.Name, result.Name);
    }

    [Fact]
    public async Task GetEstablishment_ThrowsApiResponseTempException_WhenApiReturnsError()
    {
        // Arrange
        _httpClientServiceMock
            .Setup(x => x.Get<EstablishmentDto>(It.IsAny<HttpClient>(), It.IsAny<string>()))
            .ReturnsAsync(new ApiResponseTemp<EstablishmentDto>(HttpStatusCode.InternalServerError, null!));

        var service = CreateSut();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiResponseTempException>(() => service.GetEstablishment(123456));

        Assert.Contains("InternalServerError", exception.Message);
    }

    [Fact]
    public async Task GetEstablishment_CallsApiWithCorrectPath()
    {
        // Arrange
        var urn = 123456;
        string? capturedPath = null;

        _httpClientServiceMock
            .Setup(x => x.Get<EstablishmentDto>(It.IsAny<HttpClient>(), It.IsAny<string>()))
            .Callback<HttpClient, string>((_, path) => capturedPath = path)
            .ReturnsAsync(new ApiResponseTemp<EstablishmentDto>(HttpStatusCode.OK, new EstablishmentDto()));

        var service = CreateSut();

        // Act
        await service.GetEstablishment(urn);

        // Assert
        Assert.Equal($"v4/establishment/urn/{urn}", capturedPath);
    }

    [Fact]
    public async Task GetEstablishmentWithSenData_ReturnsResult_WhenApiCallIsSuccessful()
    {
        // Arrange
        var expectedResponse = new EstablishmentResponse
        {
            Urn = "123456",
            EstablishmentName = "Test School",
            ResourcedProvisionOnRoll = "2",
            ResourcedProvisionOnCapacity = "3",
            SenUnitOnRoll = "22",
            SenUnitCapacity = "4",
            TypeOfResourcedProvision = "Resourced",
            SeN1 = "Sen1",
            SeN2 = "Sen2"
        };

        _httpClientServiceMock
            .Setup(x => x.Get<EstablishmentResponse>(It.IsAny<HttpClient>(), It.IsAny<string>()))
            .ReturnsAsync(new ApiResponseTemp<EstablishmentResponse>(HttpStatusCode.OK, expectedResponse));

        var service = CreateSut();

        // Act
        var result = await service.GetEstablishmentWithSenData(123456);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResponse.Urn, result.Urn);
        Assert.Equal(expectedResponse.EstablishmentName, result.EstablishmentName);
        Assert.Equal(expectedResponse.ResourcedProvisionOnRoll, result.ResourcedProvisionOnRoll);
        Assert.Equal(expectedResponse.SenUnitOnRoll, result.SenUnitOnRoll);
        Assert.Equal(expectedResponse.TypeOfResourcedProvision, result.TypeOfResourcedProvision);
        Assert.Equal(expectedResponse.SeN1, result.SeN1);
        Assert.Equal(expectedResponse.SeN2, result.SeN2);
    }

    [Fact]
    public async Task GetEstablishmentWithSenData_ThrowsApiResponseTempException_WhenApiReturnsError()
    {
        // Arrange
        _httpClientServiceMock
            .Setup(x => x.Get<EstablishmentResponse>(It.IsAny<HttpClient>(), It.IsAny<string>()))
            .ReturnsAsync(new ApiResponseTemp<EstablishmentResponse>(HttpStatusCode.InternalServerError, null!));

        var service = CreateSut();

        // Act & Assert
        var exception =
            await Assert.ThrowsAsync<ApiResponseTempException>(() => service.GetEstablishmentWithSenData(123456));

        Assert.Contains("InternalServerError", exception.Message);
    }

    [Fact]
    public async Task GetEstablishmentWithSenData_CallsApiWithCorrectPath()
    {
        // Arrange
        var urn = 123456;
        string? capturedPath = null;

        _httpClientServiceMock
            .Setup(x => x.Get<EstablishmentResponse>(It.IsAny<HttpClient>(), It.IsAny<string>()))
            .Callback<HttpClient, string>((_, path) => capturedPath = path)
            .ReturnsAsync(new ApiResponseTemp<EstablishmentResponse>(HttpStatusCode.OK, new EstablishmentResponse()));

        var service = CreateSut();

        // Act
        await service.GetEstablishmentWithSenData(urn);

        // Assert
        Assert.Equal($"establishment/urn/{urn}", capturedPath);
    }

    [Fact]
    public async Task GetEstablishmentsByTrustReferenceNumber_ReturnsResults_WhenApiCallIsSuccessful()
    {
        // Arrange
        EstablishmentDto[] expectedResponse =
        [
            new() { Urn = "123456", Name = "Test School" },
            new() { Urn = "654321", Name = "Other School" }
        ];

        _httpClientServiceMock
            .Setup(x => x.Get<EstablishmentDto[]>(It.IsAny<HttpClient>(), It.IsAny<string>()))
            .ReturnsAsync(new ApiResponseTemp<EstablishmentDto[]>(HttpStatusCode.OK, expectedResponse));

        var service = CreateSut();

        // Act
        var result = await service.GetEstablishmentsByTrustReferenceNumber("TR123456");

        // Assert
        Assert.Same(expectedResponse, result);
    }

    [Fact]
    public async Task GetEstablishmentsByTrustReferenceNumber_ThrowsApiResponseTempException_WhenApiReturnsError()
    {
        // Arrange
        _httpClientServiceMock
            .Setup(x => x.Get<EstablishmentDto[]>(It.IsAny<HttpClient>(), It.IsAny<string>()))
            .ReturnsAsync(new ApiResponseTemp<EstablishmentDto[]>(HttpStatusCode.InternalServerError, null!));

        var service = CreateSut();

        // Act & Assert
        var exception =
            await Assert.ThrowsAsync<ApiResponseTempException>(() =>
                service.GetEstablishmentsByTrustReferenceNumber("TRN123456"));

        Assert.Contains("InternalServerError", exception.Message);
    }

    [Fact]
    public async Task GetEstablishmentsByTrustReferenceNumber_CallsApiWithCorrectPath()
    {
        // Arrange
        var referenceNumber = "TR123456";
        string? capturedPath = null;

        _httpClientServiceMock
            .Setup(x => x.Get<EstablishmentDto[]>(It.IsAny<HttpClient>(), It.IsAny<string>()))
            .Callback<HttpClient, string>((_, path) => capturedPath = path)
            .ReturnsAsync(new ApiResponseTemp<EstablishmentDto[]>(HttpStatusCode.OK, []));

        var service = CreateSut();

        // Act
        await service.GetEstablishmentsByTrustReferenceNumber(referenceNumber);

        // Assert
        Assert.Equal($"/v4/establishments/trustReferenceNumber?trustReferenceNumber={referenceNumber}", capturedPath);
    }

    [Fact]
    public async Task GetEstablishmentsByUrns_ReturnsResults_WhenApiCallIsSuccessful()
    {
        // Arrange
        var expectedResponse = new List<EstablishmentDto>
        {
            new() { Urn = "123456", Name = "Test School" },
            new() { Urn = "654321", Name = "Other School" }
        };

        _httpClientServiceMock
            .Setup(x => x.Post<object, List<EstablishmentDto>>(
                It.IsAny<HttpClient>(),
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync(new ApiResponseTemp<List<EstablishmentDto>>(HttpStatusCode.OK, expectedResponse));

        var service = CreateSut();

        // Act
        var result = await service.GetEstablishmentsByUrns([123456, 654321]);

        // Assert
        Assert.Same(expectedResponse, result);
    }

    [Fact]
    public async Task GetEstablishmentsByUrns_ThrowsApiResponseTempException_WhenApiReturnsError()
    {
        // Arrange
        _httpClientServiceMock
            .Setup(x => x.Post<object, List<EstablishmentDto>>(
                It.IsAny<HttpClient>(),
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync(new ApiResponseTemp<List<EstablishmentDto>>(HttpStatusCode.InternalServerError, null!));

        var service = CreateSut();

        // Act & Assert
        var exception =
            await Assert.ThrowsAsync<ApiResponseTempException>(() => service.GetEstablishmentsByUrns([123456]));

        Assert.Contains("InternalServerError", exception.Message);
    }

    [Fact]
    public async Task GetEstablishmentsByUrns_CallsApiWithCorrectPathAndRequestBody()
    {
        // Arrange
        string? capturedPath = null;
        object? capturedRequest = null;

        _httpClientServiceMock
            .Setup(x => x.Post<object, List<EstablishmentDto>>(
                It.IsAny<HttpClient>(),
                It.IsAny<string>(),
                It.IsAny<object>()))
            .Callback<HttpClient, string, object>((_, path, request) =>
            {
                capturedPath = path;
                capturedRequest = request;
            })
            .ReturnsAsync(new ApiResponseTemp<List<EstablishmentDto>>(HttpStatusCode.OK, []));

        var service = CreateSut();

        // Act
        await service.GetEstablishmentsByUrns([123456, 654321]);

        // Assert
        Assert.Equal("v4/establishments/bulk/urns", capturedPath);
        Assert.NotNull(capturedRequest);

        var urns = capturedRequest.GetType().GetProperty("urns")!.GetValue(capturedRequest);

        // Fix: Extracted inline array to a local variable to prevent repeated allocations
        var expectedUrns = new[] { 123456, 654321 };
        Assert.Equal(expectedUrns, urns);
    }
}
