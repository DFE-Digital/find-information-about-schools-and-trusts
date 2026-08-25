using System.Net;
using Dfe.AcademiesApi.Client.Contracts;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.AcademiesDbServices;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Http;
using Moq;
using EstablishmentDto = GovUK.Dfe.CoreLibs.Contracts.Academies.V4.Establishments.EstablishmentDto;

namespace DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.UnitTests.AcademiesDbServices;

public class GetEstablishmentsTests
{
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

        var apiResponse = new ApiResponse<List<EstablishmentDto>>(HttpStatusCode.OK, expectedResponse);

        var httpClientServiceMock = new Mock<IHttpClientService>();
        httpClientServiceMock
            .Setup(x => x.Get<List<EstablishmentDto>>(
                It.IsAny<HttpClient>(),
                It.IsAny<string>()))
            .ReturnsAsync(apiResponse);

        var httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://fakeapi.com")
        };

        var httpClientFactoryMock = new Mock<IDfeHttpClientFactory>();
        httpClientFactoryMock
            .Setup(x => x.CreateAcademiesClient())
            .Returns(httpClient);

        var service = new GetEstablishments(
            httpClientFactoryMock.Object,
            httpClientServiceMock.Object);

        // Act
        var result = await service.SearchEstablishments(searchQuery);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(expectedResponse[0].Urn, result[0].Urn);
        Assert.Equal(expectedResponse[0].Name, result[0].Name);
    }
    
    [Fact]
    public async Task SearchEstablishments_ThrowsApiResponseException_WhenApiReturnsError()
    {
        // Arrange
        var searchQuery = "Test School";

        var apiResponse = new ApiResponse<List<EstablishmentDto>>(
            HttpStatusCode.InternalServerError,
            null!);

        var httpClientServiceMock = new Mock<IHttpClientService>();
        httpClientServiceMock
            .Setup(x => x.Get<List<EstablishmentDto>>(
                It.IsAny<HttpClient>(),
                It.IsAny<string>()))
            .ReturnsAsync(apiResponse);

        var httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://fakeapi.com")
        };

        var httpClientFactoryMock = new Mock<IDfeHttpClientFactory>();
        httpClientFactoryMock
            .Setup(x => x.CreateAcademiesClient())
            .Returns(httpClient);

        var service = new GetEstablishments(
            httpClientFactoryMock.Object,
            httpClientServiceMock.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiResponseException>(
            () => service.SearchEstablishments(searchQuery));

        Assert.Contains("InternalServerError", exception.Message);
    }

    [Fact]
    public async Task SearchEstablishments_CallsApiWithCorrectPath()
    {
        // Arrange
        var searchQuery = "Test School";

        string? capturedPath = null;

        var apiResponse = new ApiResponse<List<EstablishmentDto>>(
            HttpStatusCode.OK,
            new List<EstablishmentDto>());

        var httpClientServiceMock = new Mock<IHttpClientService>();
        httpClientServiceMock
            .Setup(x => x.Get<List<EstablishmentDto>>(
                It.IsAny<HttpClient>(),
                It.IsAny<string>()))
            .Callback<HttpClient, string>((_, path) => capturedPath = path)
            .ReturnsAsync(apiResponse);

        var httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://fakeapi.com")
        };

        var httpClientFactoryMock = new Mock<IDfeHttpClientFactory>();
        httpClientFactoryMock
            .Setup(x => x.CreateAcademiesClient())
            .Returns(httpClient);

        var service = new GetEstablishments(
            httpClientFactoryMock.Object,
            httpClientServiceMock.Object);

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
        var urn = 123456;

        var expectedResponse = new EstablishmentDto
        {
            Urn = "123456",
            Name = "Test School"
        };

        var apiResponse = new ApiResponse<EstablishmentDto>(HttpStatusCode.OK, expectedResponse);

        var httpClientServiceMock = new Mock<IHttpClientService>();
        httpClientServiceMock
            .Setup(x => x.Get<EstablishmentDto>(
                It.IsAny<HttpClient>(),
                It.IsAny<string>()))
            .ReturnsAsync(apiResponse);

        var httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://fakeapi.com")
        };

        var httpClientFactoryMock = new Mock<IDfeHttpClientFactory>();
        httpClientFactoryMock
            .Setup(x => x.CreateAcademiesClient())
            .Returns(httpClient);

        var service = new GetEstablishments(
            httpClientFactoryMock.Object,
            httpClientServiceMock.Object);

        // Act
        var result = await service.GetEstablishment(urn);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResponse.Urn, result.Urn);
        Assert.Equal(expectedResponse.Name, result.Name);
    }

    [Fact]
    public async Task GetEstablishment_ThrowsApiResponseException_WhenApiReturnsError()
    {
        // Arrange
        var urn = 123456;

        var apiResponse = new ApiResponse<EstablishmentDto>(
            HttpStatusCode.InternalServerError,
            null!);

        var httpClientServiceMock = new Mock<IHttpClientService>();
        httpClientServiceMock
            .Setup(x => x.Get<EstablishmentDto>(
                It.IsAny<HttpClient>(),
                It.IsAny<string>()))
            .ReturnsAsync(apiResponse);

        var httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://fakeapi.com")
        };

        var httpClientFactoryMock = new Mock<IDfeHttpClientFactory>();
        httpClientFactoryMock
            .Setup(x => x.CreateAcademiesClient())
            .Returns(httpClient);

        var service = new GetEstablishments(
            httpClientFactoryMock.Object,
            httpClientServiceMock.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiResponseException>(
            () => service.GetEstablishment(urn));

        Assert.Contains("InternalServerError", exception.Message);
    }

    [Fact]
    public async Task GetEstablishment_CallsApiWithCorrectPath()
    {
        // Arrange
        var urn = 123456;

        string? capturedPath = null;

        var apiResponse = new ApiResponse<EstablishmentDto>(
            HttpStatusCode.OK,
            new EstablishmentDto());

        var httpClientServiceMock = new Mock<IHttpClientService>();
        httpClientServiceMock
            .Setup(x => x.Get<EstablishmentDto>(
                It.IsAny<HttpClient>(),
                It.IsAny<string>()))
            .Callback<HttpClient, string>((_, path) => capturedPath = path)
            .ReturnsAsync(apiResponse);

        var httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://fakeapi.com")
        };

        var httpClientFactoryMock = new Mock<IDfeHttpClientFactory>();
        httpClientFactoryMock
            .Setup(x => x.CreateAcademiesClient())
            .Returns(httpClient);

        var service = new GetEstablishments(
            httpClientFactoryMock.Object,
            httpClientServiceMock.Object);

        // Act
        await service.GetEstablishment(urn);

        // Assert
        Assert.Equal($"v4/establishment/urn/{urn}", capturedPath);
    }

    [Fact]
    public async Task GetEstablishmentWithSenData_ReturnsResult_WhenApiCallIsSuccessful()
    {
        // Arrange
        var urn = 123456;

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

        var apiResponse = new ApiResponse<EstablishmentResponse>(HttpStatusCode.OK, expectedResponse);

        var httpClientServiceMock = new Mock<IHttpClientService>();
        httpClientServiceMock
            .Setup(x => x.Get<EstablishmentResponse>(
                It.IsAny<HttpClient>(),
                It.IsAny<string>()))
            .ReturnsAsync(apiResponse);

        var httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://fakeapi.com")
        };

        var httpClientFactoryMock = new Mock<IDfeHttpClientFactory>();
        httpClientFactoryMock
            .Setup(x => x.CreateAcademiesClient())
            .Returns(httpClient);

        var service = new GetEstablishments(
            httpClientFactoryMock.Object,
            httpClientServiceMock.Object);

        // Act
        var result = await service.GetEstablishmentWithSenData(urn);

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
    public async Task GetEstablishmentWithSenData_ThrowsApiResponseException_WhenApiReturnsError()
    {
        // Arrange
        var urn = 123456;

        var apiResponse = new ApiResponse<EstablishmentResponse>(
            HttpStatusCode.InternalServerError,
            null!);

        var httpClientServiceMock = new Mock<IHttpClientService>();
        httpClientServiceMock
            .Setup(x => x.Get<EstablishmentResponse>(
                It.IsAny<HttpClient>(),
                It.IsAny<string>()))
            .ReturnsAsync(apiResponse);

        var httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://fakeapi.com")
        };

        var httpClientFactoryMock = new Mock<IDfeHttpClientFactory>();
        httpClientFactoryMock
            .Setup(x => x.CreateAcademiesClient())
            .Returns(httpClient);

        var service = new GetEstablishments(
            httpClientFactoryMock.Object,
            httpClientServiceMock.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiResponseException>(
            () => service.GetEstablishmentWithSenData(urn));

        Assert.Contains("InternalServerError", exception.Message);
    }

    [Fact]
    public async Task GetEstablishmentWithSenData_CallsApiWithCorrectPath()
    {
        // Arrange
        var urn = 123456;

        string? capturedPath = null;

        var apiResponse = new ApiResponse<EstablishmentResponse>(
            HttpStatusCode.OK,
            new EstablishmentResponse());

        var httpClientServiceMock = new Mock<IHttpClientService>();
        httpClientServiceMock
            .Setup(x => x.Get<EstablishmentResponse>(
                It.IsAny<HttpClient>(),
                It.IsAny<string>()))
            .Callback<HttpClient, string>((_, path) => capturedPath = path)
            .ReturnsAsync(apiResponse);

        var httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://fakeapi.com")
        };

        var httpClientFactoryMock = new Mock<IDfeHttpClientFactory>();
        httpClientFactoryMock
            .Setup(x => x.CreateAcademiesClient())
            .Returns(httpClient);

        var service = new GetEstablishments(
            httpClientFactoryMock.Object,
            httpClientServiceMock.Object);

        // Act
        await service.GetEstablishmentWithSenData(urn);

        // Assert
        Assert.Equal($"establishment/urn/{urn}", capturedPath);
    }
    
    
    [Fact]
    public async Task GetEstablishmentsByTrustReferenceNumber_ThrowsApiResponseException_WhenApiReturnsError()
    {
        // Arrange
        var referenceNumber = "TRN123456";

        var apiResponse = new ApiResponse<EstablishmentDto[]>(
            HttpStatusCode.InternalServerError,
            null!);

        var httpClientServiceMock = new Mock<IHttpClientService>();
        httpClientServiceMock
            .Setup(x => x.Get<EstablishmentDto[]>(
                It.IsAny<HttpClient>(),
                It.IsAny<string>()))
            .ReturnsAsync(apiResponse);

        var httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://fakeapi.com")
        };

        var httpClientFactoryMock = new Mock<IDfeHttpClientFactory>();
        httpClientFactoryMock
            .Setup(x => x.CreateAcademiesClient())
            .Returns(httpClient);

        var service = new GetEstablishments(
            httpClientFactoryMock.Object,
            httpClientServiceMock.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiResponseException>(
            () => service.GetEstablishmentsByTrustReferenceNumber(referenceNumber));

        Assert.Contains("InternalServerError", exception.Message);
    }

    [Fact]
    public async Task GetEstablishmentsByTrustReferenceNumber_CallsApiWithCorrectPath()
    {
        // Arrange
        var referenceNumber = "TR123456";

        string? capturedPath = null;

        var apiResponse = new ApiResponse<EstablishmentDto[]>(
            HttpStatusCode.OK,
            []);

        var httpClientServiceMock = new Mock<IHttpClientService>();
        httpClientServiceMock
            .Setup(x => x.Get<EstablishmentDto[]>(
                It.IsAny<HttpClient>(),
                It.IsAny<string>()))
            .Callback<HttpClient, string>((_, path) => capturedPath = path)
            .ReturnsAsync(apiResponse);

        var httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://fakeapi.com")
        };

        var httpClientFactoryMock = new Mock<IDfeHttpClientFactory>();
        httpClientFactoryMock
            .Setup(x => x.CreateAcademiesClient())
            .Returns(httpClient);

        var service = new GetEstablishments(
            httpClientFactoryMock.Object,
            httpClientServiceMock.Object);

        // Act
        await service.GetEstablishmentsByTrustReferenceNumber(referenceNumber);

        // Assert
        Assert.Equal($"/v4/establishments/trustReferenceNumber?trustReferenceNumber={referenceNumber}", capturedPath);
    }
}
