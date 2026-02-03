namespace DfE.FindInformationAcademiesTrusts.UnitTests.Extensions
{
    using DfE.FindInformationAcademiesTrusts.Extensions;
    using DfE.FindInformationAcademiesTrusts.Services.School;

    public class OfstedOverviewServiceModelExtensionsTests
    {
        [Fact]
        public void AsInspectionTypeString_IfModelIsNull_ReturnNotApplicable()
        {
            OverviewServiceModel? details = null;

            details.AsInspectionTypeString().Should().Be("Not applicable");
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void AsInspectionTypeString_ShouldReturnCorrectString(bool isReportCard)
        {
            OverviewServiceModel details = new OverviewServiceModel
            {
                IsReportCard = isReportCard
            };

            details.AsInspectionTypeString().Should().Be(isReportCard ? "Report card" : "Older inspection");
        }
    }
}
