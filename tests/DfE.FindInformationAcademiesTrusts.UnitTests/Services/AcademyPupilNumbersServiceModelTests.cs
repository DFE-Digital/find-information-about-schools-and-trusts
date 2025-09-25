using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.PupilCensus;
using DfE.FindInformationAcademiesTrusts.Services.Academy;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Services;

public class AcademyPupilNumbersServiceModelTests
{
    [Theory]
    [InlineData(25, 100, 25F)]
    [InlineData(1, 4, 25F)]
    [InlineData(816, 1074, 76F)]
    [InlineData(100, 100, 100F)]
    [InlineData(101, 100, 101F)]
    [InlineData(0, 100, 0F)]
    public void PercentageFull_should_return_the_correct_percentage_calculation(int numberOfPupils,
        int capacity, float expected)
    {
        var sut =
            BuildDummyAcademyPupilNumbersServiceModel("111",
                numberOfPupils: new Statistic<int>.WithValue(numberOfPupils), schoolCapacity: capacity);
        var result = sut.PercentageFull;
        result.Should().BeOfType<Statistic<float>.WithValue>()
            .Which.Value.Should().BeApproximately(expected, 0.01F);
    }

    [Theory]
    [InlineData(StatisticKind.Suppressed)]
    [InlineData(StatisticKind.NotPublished)]
    [InlineData(StatisticKind.NotApplicable)]
    [InlineData(StatisticKind.NotAvailable)]
    [InlineData(StatisticKind.NotYetSubmitted)]
    public void PercentageFull_should_return_null_string_if_number_of_pupils_has_no_value(StatisticKind kind)
    {
        var sut =
            BuildDummyAcademyPupilNumbersServiceModel("111", numberOfPupils: Statistic<int>.FromKind(kind),
                schoolCapacity: 12);
        var result = sut.PercentageFull;
        result.Should().BeEquivalentTo(Statistic<float>.FromKind(kind));
    }

    [Theory]
    [InlineData(null)]
    [InlineData(0)]
    public void PercentageFull_should_return_NotAvailable_if_school_capacity_has_no_value(int? schoolCapacity)
    {
        var sut = BuildDummyAcademyPupilNumbersServiceModel("111",
            numberOfPupils: new Statistic<int>.WithValue(100), schoolCapacity: schoolCapacity);
        var result = sut.PercentageFull;
        result.Should().Be(Statistic<float>.NotAvailable);
    }

    private static AcademyPupilNumbersServiceModel BuildDummyAcademyPupilNumbersServiceModel(string urn,
        Statistic<int> numberOfPupils,
        string phaseOfEducation = "test",
        int? schoolCapacity = 400,
        AgeRange? ageRange = null)
    {
        return new AcademyPupilNumbersServiceModel(urn,
            $"Academy {urn}",
            phaseOfEducation,
            ageRange ?? new AgeRange(1, 11),
            numberOfPupils,
            schoolCapacity);
    }
}
