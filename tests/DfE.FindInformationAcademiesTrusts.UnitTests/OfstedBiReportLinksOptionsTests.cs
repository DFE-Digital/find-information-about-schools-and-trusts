using DfE.FindInformationAcademiesTrusts.Options;

namespace DfE.FindInformationAcademiesTrusts.UnitTests;

public class OfstedBiReportLinksOptionsTests
{
    [Fact]
    public void Configuration_section_should_be_OfstedBiReportLinks()
    {
        OfstedBiReportLinksOptions.ConfigurationSection.Should().Be("OfstedBiReportLinks");
    }

    [Fact]
    public void BaseUrl_should_default_to_empty_string()
    {
        OfstedBiReportLinksOptions sut = new();
        sut.OfstedPublishedBaseUrl.Should().BeNull();
        sut.ReportCardsBaseUrl.Should().BeNull();
    }
}
