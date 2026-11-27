using System.Net;
using DfE.FindInformationAcademiesTrusts.Http;
using DfE.FindInformationAcademiesTrusts.HttpServices;
using GovUK.Dfe.AcademiesApi.Client.Contracts;
using Moq;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.HttpServices;

public class GetLocalAuthoritiesTests
{
    private readonly Mock<IDfeHttpClientFactoryTemp> _httpClientFactoryMock = new();
    private readonly Mock<IHttpClientServiceTemp> _httpClientServiceMock = new();

    private readonly HttpClient _httpClient = new()
    {
        BaseAddress = new Uri("https://fakeapi.com")
    };

    private GetLocalAuthorities CreateSut()
    {
        _httpClientFactoryMock
            .Setup(f => f.CreateAcademiesClient())
            .Returns(_httpClient);

        return new GetLocalAuthorities(
            _httpClientFactoryMock.Object,
            _httpClientServiceMock.Object);
    }

    [Fact]
    public async Task SearchLocalAuthorities_ReturnsResults_WhenApiCallIsSuccessful()
    {
        var expectedResponse = new List<NameAndCodeDto>
        {
            new() { Code = "383", Name = "Leeds" }
        };

        _httpClientServiceMock
            .Setup(x => x.Get<List<NameAndCodeDto>>(_httpClient, It.IsAny<string>()))
            .ReturnsAsync(new ApiResponseTemp<List<NameAndCodeDto>>(HttpStatusCode.OK, expectedResponse));

        var service = CreateSut();

        var result = await service.SearchLocalAuthorities("Leeds", "383");

        Assert.Same(expectedResponse, result);
    }

    [Fact]
    public async Task SearchLocalAuthorities_ReturnsEmptyResults_WhenApiReturnsNoMatches()
    {
        _httpClientServiceMock
            .Setup(x => x.Get<List<NameAndCodeDto>>(_httpClient, It.IsAny<string>()))
            .ReturnsAsync(new ApiResponseTemp<List<NameAndCodeDto>>(HttpStatusCode.OK, []));

        var service = CreateSut();

        var result = await service.SearchLocalAuthorities("No matching authorities", null);

        Assert.Empty(result);
    }

    [Theory]
    [InlineData(HttpStatusCode.InternalServerError)]
    [InlineData(HttpStatusCode.BadRequest)]
    [InlineData(HttpStatusCode.NotFound)]
    public async Task SearchLocalAuthorities_ThrowsApiResponseTempException_WhenApiReturnsError(
        HttpStatusCode statusCode)
    {
        _httpClientServiceMock
            .Setup(x => x.Get<List<NameAndCodeDto>>(_httpClient, It.IsAny<string>()))
            .ReturnsAsync(new ApiResponseTemp<List<NameAndCodeDto>>(statusCode, null!));

        var service = CreateSut();

        var exception = await Assert.ThrowsAsync<ApiResponseTempException>(
            () => service.SearchLocalAuthorities("Leeds", null));

        Assert.Equal($"Request to Api failed | StatusCode - {statusCode}", exception.Message);
    }

    [Theory]
    [InlineData("Leeds", "383", "v4/local-authorities?name=Leeds&code=383")]
    [InlineData("Leeds", null, "v4/local-authorities?name=Leeds")]
    [InlineData(null, "383", "v4/local-authorities?code=383")]
    [InlineData(null, null, "v4/local-authorities")]
    [InlineData("", "", "v4/local-authorities")]
    [InlineData("   ", "\t", "v4/local-authorities")]
    [InlineData("Leeds", " ", "v4/local-authorities?name=Leeds")]
    [InlineData(" ", "383", "v4/local-authorities?code=383")]
    [InlineData("Kingston upon Hull, City of", null,
        "v4/local-authorities?name=Kingston%20upon%20Hull%2C%20City%20of")]
    [InlineData("Bath & North East Somerset", "3&83",
        "v4/local-authorities?name=Bath%20%26%20North%20East%20Somerset&code=3%2683")]
    public async Task SearchLocalAuthorities_CallsApiWithCorrectPath(
        string? name, string? code, string expectedPath)
    {
        string? capturedPath = null;

        _httpClientServiceMock
            .Setup(x => x.Get<List<NameAndCodeDto>>(_httpClient, It.IsAny<string>()))
            .Callback<HttpClient, string>((_, path) => capturedPath = path)
            .ReturnsAsync(new ApiResponseTemp<List<NameAndCodeDto>>(HttpStatusCode.OK, []));

        var service = CreateSut();

        await service.SearchLocalAuthorities(name, code);

        Assert.Equal(expectedPath, capturedPath);
        _httpClientServiceMock.Verify(
            x => x.Get<List<NameAndCodeDto>>(_httpClient, expectedPath), Times.Once);
    }
}
