using DfE.FindInformationAcademiesTrusts.Application.ContactCommands.Commands;
using DfE.FindInformationAcademiesTrusts.Application.ContactCommands.Models;
using DfE.FindInformationAcademiesTrusts.Application.ContactCommands.Queries;
using DfE.FindInformationAcademiesTrusts.Domain.Entities;
using DfE.FindInformationAcademiesTrusts.Domain.Enums;
using DfE.FindInformationAcademiesTrusts.Pages.Trusts.Contacts;
using DfE.FindInformationAcademiesTrusts.Services.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.Trust;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Pages.Trusts.Contacts;

public class EditModelTests
{
    private readonly EditModel _sut;
    private readonly IMediator _mockMediator = Substitute.For<IMediator>();
    private readonly ITrustService _mockTrustService = Substitute.For<ITrustService>();

    private readonly TrustSummaryServiceModel _fakeTrust = new("1234", "TR1234", "My Trust", "Multi-academy trust", 3);

    public EditModelTests()
    {
        _mockTrustService.GetTrustSummaryAsync("TR1234").Returns(_fakeTrust);

        _sut = new EditModel(_mockMediator, Substitute.For<IDataSourceService>(), _mockTrustService)
        {
            Uid = "1234", ReferenceNumber = "TR1234", Role = TrustContactRole.SfsoLead,
            Name = "New Name", Email = "new.name@education.gov.uk"
        };
    }

    [Theory]
    [InlineData(TrustContactRole.TrustRelationshipManager)]
    [InlineData(TrustContactRole.SfsoLead)]
    public async Task OnGetAsync_loads_the_contact_for_the_role(TrustContactRole role)
    {
        _sut.Role = role;
        _mockMediator.Send(new GetTrustContactQuery(1234, role)).Returns(new TrustContact
            { Uid = 1234, Role = role, Name = "Existing Name", Email = "existing@education.gov.uk" });

        var result = await _sut.OnGetAsync();

        result.Should().BeOfType<PageResult>();
        _sut.Name.Should().Be("Existing Name");
        _sut.Email.Should().Be("existing@education.gov.uk");
    }

    [Fact]
    public async Task OnGetAsync_returns_not_found_when_trust_does_not_exist()
    {
        _sut.ReferenceNumber = "TR9999";

        var result = await _sut.OnGetAsync();

        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task OnGetAsync_returns_not_found_for_unknown_role()
    {
        _sut.Role = (TrustContactRole)999;

        var result = await _sut.OnGetAsync();

        result.Should().BeOfType<NotFoundResult>();
    }

    [Theory]
    [InlineData(true, true,
        "Changes made to the SFSO (Schools financial support and oversight) lead name and email were updated.")]
    [InlineData(true, false,
        "Changes made to the SFSO (Schools financial support and oversight) lead name were updated.")]
    [InlineData(false, true,
        "Changes made to the SFSO (Schools financial support and oversight) lead email were updated.")]
    [InlineData(false, false, "")]
    public async Task OnPostAsync_updates_contact_and_redirects(bool nameUpdated, bool emailUpdated,
        string expectedMessage)
    {
        _mockMediator.Send(new UpdateTrustContactCommand(1234, TrustContactRole.SfsoLead, "New Name",
                "new.name@education.gov.uk"))
            .Returns(new ContactUpdated(nameUpdated, emailUpdated));

        var result = await _sut.OnPostAsync();

        _sut.ContactUpdatedMessage.Should().Be(expectedMessage);
        var redirect = result.Should().BeOfType<RedirectToPageResult>().Subject;
        redirect.PageName.Should().Be("/Trusts/Contacts/InDfe");
        redirect.RouteValues!["Uid"].Should().Be("1234");
        redirect.RouteValues!["ReferenceNumber"].Should().Be("TR1234");
    }

    [Fact]
    public async Task OnPostAsync_returns_page_without_updating_when_model_is_invalid()
    {
        _sut.ModelState.AddModelError("Email", "Invalid");

        var result = await _sut.OnPostAsync();

        result.Should().BeOfType<PageResult>();
        await _mockMediator.DidNotReceive().Send(Arg.Any<UpdateTrustContactCommand>());
        _sut.PageMetadata.ModelStateIsValid.Should().BeFalse();
    }

    [Fact]
    public async Task OnGetAsync_should_configure_PageMetadata()
    {
        await _sut.OnGetAsync();

        _sut.PageMetadata.EntityName.Should().Be("My Trust");
        _sut.PageMetadata.PageName.Should().Be("Contacts");
        _sut.PageMetadata.SubPageName.Should()
            .Be("Edit SFSO (Schools financial support and oversight) lead details");
    }
}
