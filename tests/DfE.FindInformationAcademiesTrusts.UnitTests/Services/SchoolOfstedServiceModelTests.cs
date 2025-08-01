using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Services.Academy;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Services;

public class SchoolOfstedServiceModelTests
{
    private readonly SchoolOfstedServiceModel _sut;
    private readonly DateTime _joinDate = new(2024, 11, 29);

    public SchoolOfstedServiceModelTests()
    {
        _sut = new SchoolOfstedServiceModel("1234", "test", _joinDate, OfstedShortInspection.Unknown, OfstedRating.NotInspected,
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
    
    [Fact]
    public void
        HasRecentShortInspection_returns_true_when_ShortInspection_InspectionDate_is_after_CurrentInspection_InspectionDate()
    {
        var sut = _sut with
        {
            ShortInspection = new OfstedShortInspection(new DateTime(2025, 1, 1), "School remains Good"),
            CurrentOfstedRating = new OfstedRating(1, new DateTime(2021, 1, 1))
        };
        
        sut.HasRecentShortInspection.Should().BeTrue();
    }

    [Theory]
    [InlineData(2021)]
    [InlineData(2020)]
    public void
        HasRecentShortInspection_returns_false_when_ShortInspection_InspectionDate_is_not_after_CurrentInspection_InspectionDate(int year)
    {
        var sut = _sut with
        {
            ShortInspection = new OfstedShortInspection(new DateTime(year, 1, 1), "School remains Good"),
            CurrentOfstedRating = new OfstedRating(1, new DateTime(2021, 1, 1))
        };
        
        sut.HasRecentShortInspection.Should().BeFalse();
    }

    [Fact]
    public void HasRecentShortInspection_returns_false_when_ShortInspection_InspectionDate_is_null()
    {
        var sut = _sut with
        {
            ShortInspection = OfstedShortInspection.Unknown,
            CurrentOfstedRating = new OfstedRating(1, new DateTime(2021, 1, 1))
        };
        
        sut.HasRecentShortInspection.Should().BeFalse();
    }
}
