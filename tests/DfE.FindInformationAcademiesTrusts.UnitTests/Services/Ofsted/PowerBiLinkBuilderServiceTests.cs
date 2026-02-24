using DfE.FindInformationAcademiesTrusts.Options;
using DfE.FindInformationAcademiesTrusts.Services.Ofsted;
using Microsoft.Extensions.Options;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Services.Ofsted;

public class PowerBiLinkBuilderServiceTests
{
    private static PowerBiLinkBuilderService CreateService(
        string? reportCardsBaseUrl = "https://example.com/reportcards?param=1",
        string? ofstedPublishedBaseUrl = "https://example.com/ofstedpublished?param=2")
    {
        var options = Substitute.For<IOptions<OfstedBiReportLinksOptions>>();
        options.Value.Returns(new OfstedBiReportLinksOptions
        {
            ReportCardsBaseUrl = reportCardsBaseUrl,
            OfstedPublishedBaseUrl = ofstedPublishedBaseUrl
        });
        return new PowerBiLinkBuilderService(options);
    }

    [Fact]
    public void BuildReportCardsLink_should_return_null_when_base_url_is_null()
    {
        var service = CreateService(reportCardsBaseUrl: null);

        var result = service.BuildReportCardsLink(123456);

        result.Should().BeNull();
    }

    [Fact]
    public void BuildReportCardsLink_should_return_null_when_base_url_is_empty()
    {
        var service = CreateService(reportCardsBaseUrl: "");

        var result = service.BuildReportCardsLink(123456);

        result.Should().BeNull();
    }

    [Fact]
    public void BuildReportCardsLink_should_return_expected_url()
    {
        var baseUrl = "https://example.com/reportcards?param=1";
        var urn = 123456;
        var service = CreateService(reportCardsBaseUrl: baseUrl);

        var result = service.BuildReportCardsLink(urn);

        result.Should().Be($"{baseUrl}&rp:param_URN={urn}");
    }

    [Fact]
    public void BuildOfstedPublishedLink_should_return_null_when_base_url_is_null()
    {
        var service = CreateService(ofstedPublishedBaseUrl: null);

        var result = service.BuildOfstedPublishedLink(654321);

        result.Should().BeNull();
    }

    [Fact]
    public void BuildOfstedPublishedLink_should_return_null_when_base_url_is_empty()
    {
        var service = CreateService(ofstedPublishedBaseUrl: "");

        var result = service.BuildOfstedPublishedLink(654321);

        result.Should().BeNull();
    }

    [Fact]
    public void BuildOfstedPublishedLink_should_return_expected_url()
    {
        var baseUrl = "https://example.com/ofstedpublished?param=2";
        var urn = 654321;
        var service = CreateService(ofstedPublishedBaseUrl: baseUrl);

        var result = service.BuildOfstedPublishedLink(urn);

        result.Should().Be($"{baseUrl}&rp:URN={urn}");
    }

    [Fact]
    public void BuildReportCardsLinkForTrust_should_return_null_when_base_url_is_null()
    {
        var service = CreateService(reportCardsBaseUrl: null);

        var result = service.BuildReportCardsLinkForTrust("TRUST123");

        result.Should().BeNull();
    }

    [Fact]
    public void BuildReportCardsLinkForTrust_should_return_null_when_base_url_is_empty()
    {
        var service = CreateService(reportCardsBaseUrl: "");

        var result = service.BuildReportCardsLinkForTrust("TRUST123");

        result.Should().BeNull();
    }

    [Fact]
    public void BuildReportCardsLinkForTrust_should_return_expected_url()
    {
        var baseUrl = "https://example.com/reportcards?param=1";
        var trustRef = "TRUST123";
        var service = CreateService(reportCardsBaseUrl: baseUrl);

        var result = service.BuildReportCardsLinkForTrust(trustRef);

        result.Should().Be($"{baseUrl}&rp:param_TrustRef={trustRef}");
    }

    [Fact]
    public void BuildOfstedPublishedLinkForTrust_should_return_null_when_base_url_is_null()
    {
        var service = CreateService(ofstedPublishedBaseUrl: null);

        var result = service.BuildOfstedPublishedLinkForTrust("TRUST456");

        result.Should().BeNull();
    }

    [Fact]
    public void BuildOfstedPublishedLinkForTrust_should_return_null_when_base_url_is_empty()
    {
        var service = CreateService(ofstedPublishedBaseUrl: "");

        var result = service.BuildOfstedPublishedLinkForTrust("TRUST456");

        result.Should().BeNull();
    }

    [Fact]
    public void BuildOfstedPublishedLinkForTrust_should_return_expected_url()
    {
        var baseUrl = "https://example.com/ofstedpublished?param=2";
        var trustRef = "TRUST456";
        var service = CreateService(ofstedPublishedBaseUrl: baseUrl);

        var result = service.BuildOfstedPublishedLinkForTrust(trustRef);

        result.Should().Be($"{baseUrl}&rp:TrustRef={trustRef}");
    }
}