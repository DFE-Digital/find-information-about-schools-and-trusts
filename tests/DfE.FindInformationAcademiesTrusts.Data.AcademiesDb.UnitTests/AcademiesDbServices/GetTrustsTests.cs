using System.Net;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.AcademiesDbServices;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Http;
using GovUK.Dfe.CoreLibs.Contracts.Academies.V4.Trusts;
using Moq;

namespace DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.UnitTests.AcademiesDbServices;

public class GetTrustsTests
{
    private readonly Mock<IDfeHttpClientFactory> _httpClientFactoryMock = new();
    private readonly Mock<IHttpClientService> _httpClientServiceMock = new();

    private readonly HttpClient _httpClient = new()
    {
        BaseAddress = new Uri("https://fakeapi.com")
    };

    private GetTrusts CreateSut()
    {
        _httpClientFactoryMock
            .Setup(f => f.CreateAcademiesClient())
            .Returns(_httpClient);

        return new GetTrusts(
            _httpClientFactoryMock.Object,
            _httpClientServiceMock.Object);
    }

    [Fact]
    public async Task SearchTrusts_ReturnsBody_WhenApiCallIsSuccessful()
    {
        // Arrange
        string? capturedPath = null;

        var body = new TrustListResponse<TrustDto>
        {
            Data =
            [
                new TrustDto
                {
                    GroupUid = "1",
                    Name = "Test Trust"
                }
            ]
        };

        _httpClientServiceMock
            .Setup(x => x.Get<TrustListResponse<TrustDto>>(It.IsAny<HttpClient>(), It.IsAny<string>()))
            .Callback<HttpClient, string>((_, path) => capturedPath = path)
            .ReturnsAsync(new ApiResponse<TrustListResponse<TrustDto>>(HttpStatusCode.OK, body));

        var sut = CreateSut();

        // Act
        var result = await sut.SearchTrusts("Acme");

        // Assert
        Assert.Same(body, result);
        Assert.Equal(
            "v4/trusts?groupName=Acme&urn=Acme&page=1&count=10&status=Open",
            capturedPath);
    }
    
     [Fact]
    public async Task SearchTrusts_Throws_WhenApiCallFails()
    {
        // Arrange
        _httpClientServiceMock
            .Setup(x => x.Get<TrustListResponse<TrustDto>>(It.IsAny<HttpClient>(), It.IsAny<string>()))
            .ReturnsAsync(new ApiResponse<TrustListResponse<TrustDto>>(HttpStatusCode.InternalServerError, null!));

        var sut = CreateSut();

        // Act & Assert
        await Assert.ThrowsAsync<ApiResponseException>(
            () => sut.SearchTrusts("Acme"));
    }

    [Fact]
    public async Task GetTrustByReferenceNumber_ReturnsTrust_WhenApiCallIsSuccessful()
    {
        // Arrange
        string? capturedPath = null;

        var trust = new TrustDto
        {
            GroupUid = "1",
            Name = "Test Trust",
            ReferenceNumber = "TR001"
        };

        _httpClientServiceMock
            .Setup(x => x.Get<TrustDto>(It.IsAny<HttpClient>(), It.IsAny<string>()))
            .Callback<HttpClient, string>((_, path) => capturedPath = path)
            .ReturnsAsync(new ApiResponse<TrustDto>(HttpStatusCode.OK, trust));

        var sut = CreateSut();

        // Act
        var result = await sut.GetTrustByReferenceNumber("TR001");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Trust", result.Name);
        Assert.Equal("v4/trust/trustReferenceNumber/TR001", capturedPath);
    }

    [Fact]
    public async Task GetTrustByReferenceNumber_ReturnsNull_WhenTrustIsNotFound()
    {
        // Arrange
        _httpClientServiceMock
            .Setup(x => x.Get<TrustDto>(It.IsAny<HttpClient>(), It.IsAny<string>()))
            .ReturnsAsync(new ApiResponse<TrustDto>(HttpStatusCode.NotFound, null!));

        var sut = CreateSut();

        // Act
        var result = await sut.GetTrustByReferenceNumber("TR001");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetTrustByReferenceNumber_Throws_WhenApiCallFails()
    {
        // Arrange
        _httpClientServiceMock
            .Setup(x => x.Get<TrustDto>(It.IsAny<HttpClient>(), It.IsAny<string>()))
            .ReturnsAsync(new ApiResponse<TrustDto>(HttpStatusCode.InternalServerError, null!));

        var sut = CreateSut();

        // Act & Assert
        await Assert.ThrowsAsync<ApiResponseException>(
            () => sut.GetTrustByReferenceNumber("TR001"));
    }

}