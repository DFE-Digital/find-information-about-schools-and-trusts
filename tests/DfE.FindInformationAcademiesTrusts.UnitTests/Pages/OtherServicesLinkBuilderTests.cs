using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Pages;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Pages;

public class OtherServicesLinkBuilderTests
{
    private readonly OtherServicesLinkBuilder _sut = new();

    [Fact]
    public void CompaniesHouseListingLink_should_return_url_containing_CompaniesHouseNumber()
    {
        var result = _sut.CompaniesHouseListingLink("2345");

        result.Should()
            .Be("https://find-and-update.company-information.service.gov.uk/company/2345");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void CompaniesHouseListingLink_should_return_null_if_trust_has_no_CompaniesHouseNumber(
        string? companiesHouseNumber)
    {
        var result = _sut.CompaniesHouseListingLink(companiesHouseNumber);
        result.Should().BeNull();
    }

    [Fact]
    public void GetInformationAboutSchoolsListingLink_should_return_url_containing_trust_uid()
    {
        var result = _sut.GetInformationAboutSchoolsListingLinkForTrust("1234");
        result.Should().Be("https://www.get-information-schools.service.gov.uk/Groups/Group/Details/1234");
    }

    [Fact]
    public void GetInformationAboutSchoolsListingLink_should_return_school_urn_in_link()
    {
        var result = _sut.GetInformationAboutSchoolsListingLinkForSchool("111");
        result.Should()
            .Be("https://www.get-information-schools.service.gov.uk/Establishments/Establishment/Details/111");
    }

    [Fact]
    public void FinancialBenchmarkingInsightsToolListingLink_should_be_to_the_correct_link()
    {
        var result = _sut.FinancialBenchmarkingInsightsToolListingLink("1111");
        result.Should().Be("https://financial-benchmarking-and-insights-tool.education.gov.uk/trust/1111");
    }

    [Fact]
    public void FinancialBenchmarkingInsightsToolListingLink_should_be_null_companies_house_is_null()
    {
        var result = _sut.FinancialBenchmarkingInsightsToolListingLink(null);
        result.Should().BeNull();
    }

    [Fact]
    public void FindSchoolPerformanceDataListingLink_should_be_to_a_trust_page_if_multiacademy_trust()
    {
        var result =
            _sut.FindSchoolPerformanceDataListingLink("1234", TrustType.MultiAcademyTrust, null);
        result.Should().Be("https://www.find-school-performance-data.service.gov.uk/multi-academy-trust/1234");
    }

    [Fact]
    public void FindSchoolPerformanceDataListingLink_should_be_to_a_school_page_if_single_academy_trust_with_academies()
    {
        var result =
            _sut.FindSchoolPerformanceDataListingLink("1234", TrustType.SingleAcademyTrust, "1111");
        result.Should().Be("https://www.find-school-performance-data.service.gov.uk/school/1111");
    }

    [Fact]
    public void FindSchoolPerformanceDataListingLink_should_be_null_if_trust_is_single_academy_type_with_no_academies()
    {
        var result =
            _sut.FindSchoolPerformanceDataListingLink("1234", TrustType.SingleAcademyTrust, null);
        result.Should().BeNull();
    }

    [Theory]
    [InlineData("TR02345",
        "https://educationgovuk.sharepoint.com/_layouts/15/sharepoint.aspx?oobRefiners=%7B%22FileType%22%3A%5B%22other%22%5D%7D&q=TR02345&v=%2Fsearch")]
    [InlineData("",
        "https://educationgovuk.sharepoint.com/_layouts/15/sharepoint.aspx?oobRefiners=%7B%22FileType%22%3A%5B%22other%22%5D%7D&q=&v=%2Fsearch")]
    public void SharepointFolderLink_should_return_url_containing_GroupId(string trn, string expected)
    {
        var result =
            _sut.SharepointFolderLink(trn);

        result.Should()
            .Be(expected);
    }

    [Fact]
    public void FinancialBenchmarkingLinkForSchool_should_be_to_the_correct_link()
    {
        var result = _sut.FinancialBenchmarkingLinkForSchool(111);
        result.Should()
            .Be("https://financial-benchmarking-and-insights-tool.education.gov.uk/school/111/spending-and-costs");
    }


    [Fact]
    public void FindSchoolPerformanceDataListingLink_should_be_to_the_correct_link()
    {
        var result = _sut.FindSchoolPerformanceDataListingLink(111);
        result.Should()
            .Be("https://www.find-school-performance-data.service.gov.uk/school/111");
    }

    [Theory]
    [InlineData(123456, "https://reports.ofsted.gov.uk/inspection-reports/find-inspection-report/provider/ELS/123456")]
    [InlineData(987654, "https://reports.ofsted.gov.uk/inspection-reports/find-inspection-report/provider/ELS/987654")]
    public void OfstedReportLinkForSchool_should_be_to_the_correct_link(int urn, string expected)
    {
        var result = _sut.OfstedReportLinkForSchool(urn);
        result.Should().Be(expected);
    }
}
