using System.Net;
using DfE.FindInformationAcademiesTrusts.Http;
using DfE.FindInformationAcademiesTrusts.HttpServices;
using GovUK.Dfe.CoreLibs.Contracts.Academies.V4.Trusts;
using Moq;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.HttpServices;

public class GetTrustsTempTests
{
    private readonly Mock<IDfeHttpClientFactoryTemp> _httpClientFactoryMock = new();
    private readonly Mock<IHttpClientServiceTemp> _httpClientServiceMock = new();

    private readonly HttpClient _httpClient = new()
    {
        BaseAddress = new Uri("https://fakeapi.com")
    };

    private GetTrustsTemp CreateSut()
    {
        _httpClientFactoryMock
            .Setup(f => f.CreateAcademiesClient())
            .Returns(_httpClient);

        return new GetTrustsTemp(
            _httpClientFactoryMock.Object,
            _httpClientServiceMock.Object);
    }

    [Fact]
    public async Task SearchTrusts_ReturnsBody_WhenApiCallIsSuccessful()
    {
        // Arrange
        string? capturedPath = null;

        var body = new TrustListResponseTemp<TrustDto>
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
            .Setup(x => x.Get<TrustListResponseTemp<TrustDto>>(It.IsAny<HttpClient>(), It.IsAny<string>()))
            .Callback<HttpClient, string>((_, path) => capturedPath = path)
            .ReturnsAsync(new ApiResponseTemp<TrustListResponseTemp<TrustDto>>(HttpStatusCode.OK, body));

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
    public async Task SearchTrusts_AddsCorrelationIdHeader()
    {
        // Arrange
        _httpClientServiceMock
            .Setup(x => x.Get<TrustListResponseTemp<TrustDto>>(It.IsAny<HttpClient>(), It.IsAny<string>()))
            .ReturnsAsync(new ApiResponseTemp<TrustListResponseTemp<TrustDto>>(
                HttpStatusCode.OK,
                new TrustListResponseTemp<TrustDto>()));

        var sut = CreateSut();

        // Act
        await sut.SearchTrusts("Acme");

        // Assert
        Assert.True(_httpClient.DefaultRequestHeaders.TryGetValues("x-correlationId", out var values));
        Assert.True(Guid.TryParse(Assert.Single(values), out _));
    }

    [Fact]
    public async Task SearchTrusts_Throws_WhenApiCallFails()
    {
        // Arrange
        _httpClientServiceMock
            .Setup(x => x.Get<TrustListResponseTemp<TrustDto>>(It.IsAny<HttpClient>(), It.IsAny<string>()))
            .ReturnsAsync(new ApiResponseTemp<TrustListResponseTemp<TrustDto>>(HttpStatusCode.InternalServerError, null!));

        var sut = CreateSut();

        // Act & Assert
        await Assert.ThrowsAsync<ApiResponseTempException>(
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
            .ReturnsAsync(new ApiResponseTemp<TrustDto>(HttpStatusCode.OK, trust));

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
            .ReturnsAsync(new ApiResponseTemp<TrustDto>(HttpStatusCode.NotFound, null!));

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
            .ReturnsAsync(new ApiResponseTemp<TrustDto>(HttpStatusCode.InternalServerError, null!));

        var sut = CreateSut();

        // Act & Assert
        await Assert.ThrowsAsync<ApiResponseTempException>(
            () => sut.GetTrustByReferenceNumber("TR001"));
    }

    [Fact]
    public async Task GetTrustsByReferenceNumbers_ReturnsTrusts_WhenApiCallIsSuccessful()
    {
        // Arrange
        string? capturedPath = null;

        TrustDto[] trusts =
        [
            new() { GroupUid = "1", Name = "Test Trust", ReferenceNumber = "TR001" },
            new() { GroupUid = "2", Name = "Other Trust", ReferenceNumber = "TR002" }
        ];

        _httpClientServiceMock
            .Setup(x => x.Get<TrustDto[]>(It.IsAny<HttpClient>(), It.IsAny<string>()))
            .Callback<HttpClient, string>((_, path) => capturedPath = path)
            .ReturnsAsync(new ApiResponseTemp<TrustDto[]>(HttpStatusCode.OK, trusts));

        var sut = CreateSut();

        // Act
        var result = await sut.GetTrustsByReferenceNumbers(["TR001", "TR002"]);

        // Assert
        Assert.Same(trusts, result);
        Assert.Equal("v4/trusts/trustReferenceNumber/bulk?trns=TR001&trns=TR002", capturedPath);
    }

    [Fact]
    public async Task GetTrustsByReferenceNumbers_Throws_WhenApiCallFails()
    {
        // Arrange
        _httpClientServiceMock
            .Setup(x => x.Get<TrustDto[]>(It.IsAny<HttpClient>(), It.IsAny<string>()))
            .ReturnsAsync(new ApiResponseTemp<TrustDto[]>(HttpStatusCode.InternalServerError, null!));

        var sut = CreateSut();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiResponseTempException>(
            () => sut.GetTrustsByReferenceNumbers(["TR001"]));

        Assert.Contains("InternalServerError", exception.Message);
    }

    [Fact]
    public async Task GetEstablishmentTrust_ReturnsTrust_WhenApiCallIsSuccessful()
    {
        // Arrange
        string? capturedPath = null;
        object? capturedRequest = null;

        var trust = new TrustDto
        {
            GroupUid = "1",
            Name = "Test Trust",
            ReferenceNumber = "TR001"
        };

        _httpClientServiceMock
            .Setup(x => x.Post<object, Dictionary<int, TrustDto>>(
                It.IsAny<HttpClient>(),
                It.IsAny<string>(),
                It.IsAny<object>()))
            .Callback<HttpClient, string, object>((_, path, request) =>
            {
                capturedPath = path;
                capturedRequest = request;
            })
            .ReturnsAsync(new ApiResponseTemp<Dictionary<int, TrustDto>>(
                HttpStatusCode.OK,
                new Dictionary<int, TrustDto> { [123456] = trust }));

        var sut = CreateSut();

        // Act
        var result = await sut.GetEstablishmentTrust(123456);

        // Assert
        Assert.Same(trust, result);
        Assert.Equal("/v4/trusts/establishments/urns", capturedPath);
        Assert.NotNull(capturedRequest);
        var urns = capturedRequest.GetType().GetProperty("urns")!.GetValue(capturedRequest);
        Assert.Equal(new[] { 123456 }, urns);
    }

    [Fact]
    public async Task GetEstablishmentTrust_ReturnsNull_WhenResponseHasNoTrusts()
    {
        // Arrange
        _httpClientServiceMock
            .Setup(x => x.Post<object, Dictionary<int, TrustDto>>(
                It.IsAny<HttpClient>(),
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync(new ApiResponseTemp<Dictionary<int, TrustDto>>(
                HttpStatusCode.OK,
                new Dictionary<int, TrustDto>()));

        var sut = CreateSut();

        // Act
        var result = await sut.GetEstablishmentTrust(123456);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetEstablishmentTrust_ReturnsNull_WhenTrustIsNotFound()
    {
        // Arrange
        _httpClientServiceMock
            .Setup(x => x.Post<object, Dictionary<int, TrustDto>>(
                It.IsAny<HttpClient>(),
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync(new ApiResponseTemp<Dictionary<int, TrustDto>>(HttpStatusCode.NotFound, null!));

        var sut = CreateSut();

        // Act
        var result = await sut.GetEstablishmentTrust(123456);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetEstablishmentTrust_Throws_WhenApiCallFails()
    {
        // Arrange
        _httpClientServiceMock
            .Setup(x => x.Post<object, Dictionary<int, TrustDto>>(
                It.IsAny<HttpClient>(),
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync(new ApiResponseTemp<Dictionary<int, TrustDto>>(HttpStatusCode.InternalServerError, null!));

        var sut = CreateSut();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiResponseTempException>(
            () => sut.GetEstablishmentTrust(123456));

        Assert.Contains("InternalServerError", exception.Message);
    }
}
