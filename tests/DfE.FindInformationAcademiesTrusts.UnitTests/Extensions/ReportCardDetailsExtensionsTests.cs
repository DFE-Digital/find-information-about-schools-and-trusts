using DfE.FindInformationAcademiesTrusts.Extensions;
using DfE.FindInformationAcademiesTrusts.Services.Ofsted;
using Xunit;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Extensions
{
    public class ReportCardDetailsExtensionsTests
    {
        [Fact]
        public void ToDictionary_NullInput_ReturnsEmptyDictionary()
        {
            ReportCardDetails? details = null;

            var result = details.ToDictionary();

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void ToDictionary_AllPropertiesSet_ReturnsCorrectDictionary()
        {
            var details = new ReportCardDetails(new DateOnly(2023, 1, 1), "http://example.com",
                LeadershipAndGovernance: "1", 
                PersonalDevelopmentAndWellBeing: "2", 
                CurriculumAndTeaching: "3", 
                Inclusion: "4", 
                Achievement: "5", 
                AttendanceAndBehaviour: "6", 
                EarlyYearsProvision: "7",
                Safeguarding: "8",
                Post16Provision: "9", 
                CategoryOfConcern: "10");

            var result = details.ToDictionary();

            result.Count.Should().Be(10);

            result["Leadership and Governance"].Should().Be("1");
            result["Personal Development and Well Being"].Should().Be("2");
            result["Curriculum and Teaching"].Should().Be("3");
            result["Inclusion"].Should().Be("4");
            result["Achievement"].Should().Be("5");
            result["Attendance and Behaviour"].Should().Be("6");
            result["Early Years Provision"].Should().Be("7");
            result["Safeguarding"].Should().Be("8");
            result["Post 16 Provision"].Should().Be("9");
            result["Category of Concern"].Should().Be("10");
        }

        [Fact]
        public void ToDictionary_SomePropertiesNull_ReturnsEmptyStringsForNulls()
        {
            var details = new ReportCardDetails(new DateOnly(2023, 1, 1), "http://example.com",
                LeadershipAndGovernance: null,
                PersonalDevelopmentAndWellBeing: null,
                CurriculumAndTeaching: null,
                Inclusion: null,
                Achievement: null,
                AttendanceAndBehaviour: null,
                EarlyYearsProvision: null,
                Safeguarding: string.Empty,
                Post16Provision: null,
                CategoryOfConcern: string.Empty);

            var result = details.ToDictionary();

            result["Leadership and Governance"].Should().BeEmpty();
            result["Personal Development and Well Being"].Should().BeEmpty();
            result["Curriculum and Teaching"].Should().BeEmpty();
            result["Inclusion"].Should().BeEmpty();
            result["Achievement"].Should().BeEmpty();
            result["Attendance and Behaviour"].Should().BeEmpty();
            result["Early Years Provision"].Should().BeEmpty();
            result["Safeguarding"].Should().BeEmpty();
            result["Post 16 Provision"].Should().BeEmpty();
            result["Category of Concern"].Should().BeEmpty();
        }
    }
}
