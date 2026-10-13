using DfE.FindInformationAcademiesTrusts.Application.ContactCommands.Commands;
using DfE.FindInformationAcademiesTrusts.Application.ContactCommands.Models;
using DfE.FindInformationAcademiesTrusts.Application.ContactCommands.Queries;
using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Domain.Entities;
using DfE.FindInformationAcademiesTrusts.Domain.Enums;
using DfE.FindInformationAcademiesTrusts.Pages.Schools.Contacts;
using DfE.FindInformationAcademiesTrusts.Services.School;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Pages.Schools.Contacts;

public class EditModelTests
{
    private const int Urn = 123456;

    private readonly EditModel _sut;
    private readonly IMediator _mockMediator = Substitute.For<IMediator>();
    private readonly ISchoolService _mockSchoolService = Substitute.For<ISchoolService>();

    public EditModelTests()
    {
        _mockSchoolService.GetSchoolSummaryAsync(Urn)
            .Returns(new SchoolSummaryServiceModel(Urn, "Cool school", "Community school",
                SchoolCategory.LaMaintainedSchool));

        _sut = new EditModel(_mockMediator, _mockSchoolService)
        {
            Urn = Urn, Role = SchoolContactRole.RegionsGroupLocalAuthorityLead,
            Name = "New Name", Email = "new.name@education.gov.uk"
        };
    }

    [Fact]
    public async Task OnGetAsync_loads_the_contact_for_the_role()
    {
        _mockMediator.Send(new GetSchoolContactQuery(Urn, SchoolContactRole.RegionsGroupLocalAuthorityLead))
            .Returns(new SchoolContact
            {
                Urn = Urn, Role = SchoolContactRole.RegionsGroupLocalAuthorityLead, Name = "Existing Name",
                Email = "existing@education.gov.uk"
            });

        var result = await _sut.OnGetAsync();

        result.Should().BeOfType<PageResult>();
        _sut.Name.Should().Be("Existing Name");
        _sut.Email.Should().Be("existing@education.gov.uk");
        _sut.SchoolName.Should().Be("Cool school");
    }

    [Fact]
    public async Task OnGetAsync_returns_not_found_when_school_does_not_exist()
    {
        _sut.Urn = 999999;

        var result = await _sut.OnGetAsync();

        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task OnGetAsync_returns_not_found_for_unknown_role()
    {
        _sut.Role = (SchoolContactRole)999;

        var result = await _sut.OnGetAsync();

        result.Should().BeOfType<NotFoundResult>();
    }

    [Theory]
    [InlineData(true, true, "Changes made to the Regions group local authority lead name and email were updated.")]
    [InlineData(true, false, "Changes made to the Regions group local authority lead name were updated.")]
    [InlineData(false, true, "Changes made to the Regions group local authority lead email were updated.")]
    [InlineData(false, false, "")]
    public async Task OnPostAsync_updates_contact_and_redirects(bool nameUpdated, bool emailUpdated,
        string expectedMessage)
    {
        _mockMediator.Send(new UpdateSchoolContactCommand(Urn, SchoolContactRole.RegionsGroupLocalAuthorityLead,
                "New Name", "new.name@education.gov.uk"))
            .Returns(new ContactUpdated(nameUpdated, emailUpdated));

        var result = await _sut.OnPostAsync();

        _sut.ContactUpdatedMessage.Should().Be(expectedMessage);
        var redirect = result.Should().BeOfType<RedirectToPageResult>().Subject;
        redirect.PageName.Should().Be("/Schools/Contacts/InDfe");
        redirect.RouteValues!["Urn"].Should().Be(Urn);
    }

    [Fact]
    public async Task OnPostAsync_returns_page_without_updating_when_model_is_invalid()
    {
        _sut.ModelState.AddModelError("Email", "Invalid");

        var result = await _sut.OnPostAsync();

        result.Should().BeOfType<PageResult>();
        await _mockMediator.DidNotReceive().Send(Arg.Any<UpdateSchoolContactCommand>());
        _sut.PageMetadata.ModelStateIsValid.Should().BeFalse();
    }

    [Fact]
    public async Task OnGetAsync_should_configure_PageMetadata()
    {
        await _sut.OnGetAsync();

        _sut.PageMetadata.EntityName.Should().Be("Cool school");
        _sut.PageMetadata.PageName.Should().Be("Contacts");
        _sut.PageMetadata.SubPageName.Should().Be("Edit Regions group local authority lead details");
    }
}
