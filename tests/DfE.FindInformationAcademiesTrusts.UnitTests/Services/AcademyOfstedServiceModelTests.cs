using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Services.Academy;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Services;

public class AcademyOfstedServiceModelTests
{
    private readonly AcademyOfstedServiceModel _sut;
    private readonly DateTime _joinDate = new(2024, 11, 29);

    public AcademyOfstedServiceModelTests()
    {
        _sut = new AcademyOfstedServiceModel("1234", "test", _joinDate, OfstedShortInspection.Unknown, OfstedRating.NotInspected,
            OfstedRating.NotInspected);
    }

    [Fact]
    public void WhenDidCurrentInspectionHappen_should_be_Before_when_CurrentInspectionDate_is_Before_joining_date()
    {
        var sut = _sut with { CurrentOfstedRating = new OfstedRating(1, _joinDate.AddYears(-1)) };

        sut.WhenDidCurrentInspectionHappen.Should().Be(BeforeOrAfterJoining.Before);
    }

    [Fact]
    public void WhenDidCurrentInspectionHappen_should_be_After_when_CurrentInspectionDate_is_After_joining_date()
    {
        var sut = _sut with { CurrentOfstedRating = new OfstedRating(1, _joinDate.AddYears(1)) };

        sut.WhenDidCurrentInspectionHappen.Should().Be(BeforeOrAfterJoining.After);
    }

    [Fact]
    public void WhenDidCurrentInspectionHappen_should_be_After_when_CurrentInspectionDate_is_EqualTo_joining_date()
    {
        var sut = _sut with { CurrentOfstedRating = new OfstedRating(1, _joinDate) };

        sut.WhenDidCurrentInspectionHappen.Should().Be(BeforeOrAfterJoining.After);
    }

    [Fact]
    public void WhenDidCurrentInspectionHappen_should_be_NotYetInspected_when_CurrentInspectionDate_is_null()
    {
        _sut.WhenDidCurrentInspectionHappen.Should().Be(BeforeOrAfterJoining.NotYetInspected);
    }

    [Fact]
    public void WhenDidPreviousInspectionHappen_should_be_Before_when_PreviousInspectionDate_is_Before_joining_date()
    {
        var sut = _sut with { PreviousOfstedRating = new OfstedRating(1, _joinDate.AddYears(-1)) };

        sut.WhenDidPreviousInspectionHappen.Should().Be(BeforeOrAfterJoining.Before);
    }

    [Fact]
    public void WhenDidPreviousInspectionHappen_should_be_After_when_PreviousInspectionDate_is_After_joining_date()
    {
        var sut = _sut with { PreviousOfstedRating = new OfstedRating(1, _joinDate.AddYears(1)) };

        sut.WhenDidPreviousInspectionHappen.Should().Be(BeforeOrAfterJoining.After);
    }

    [Fact]
    public void WhenDidPreviousInspectionHappen_should_be_After_when_PreviousInspectionDate_is_EqualTo_joining_date()
    {
        var sut = _sut with { PreviousOfstedRating = new OfstedRating(1, _joinDate) };

        sut.WhenDidPreviousInspectionHappen.Should().Be(BeforeOrAfterJoining.After);
    }

    [Fact]
    public void WhenDidPreviousInspectionHappen_should_be_NotYetInspected_when_PreviousInspectionDate_is_null()
    {
        _sut.WhenDidPreviousInspectionHappen.Should().Be(BeforeOrAfterJoining.NotYetInspected);
    }
}
