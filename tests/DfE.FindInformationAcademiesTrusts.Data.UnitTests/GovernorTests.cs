using DfE.FindInformationAcademiesTrusts.Data.Repositories;

namespace DfE.FindInformationAcademiesTrusts.Data.UnitTests;

public class GovernorTests
{
    [Theory]
    [InlineData("Accounting Officer", true)]
    [InlineData("Chief Financial Officer", true)]
    [InlineData("Chair of Trustees", true)]
    [InlineData("Member", false)]
    [InlineData("Trustee", false)]
    public void HasRoleLeadership_should_be_calculated_with_given_role(string role, bool hasLeadership)
    {
        var governor = new Governor(string.Empty, string.Empty, "test name", role, string.Empty, null, null, null);

        governor.HasRoleLeadership.Should().Be(hasLeadership);
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData(1, true)]
    [InlineData(0, true)]
    [InlineData(-1, false)]
    public void IsCurrentOrFutureGovernor_should_be_calculated_from_date(int? daysToAdd, bool expected)
    {
        var dateOfTermEnd = daysToAdd.HasValue ? DateTime.Today.AddDays(daysToAdd.Value) : (DateTime?)null;

        var governor = new Governor(string.Empty, string.Empty, "test name", "Member", string.Empty, null,
            dateOfTermEnd, null);

        governor.IsCurrentOrFutureGovernor.Should().Be(expected);
    }
}
