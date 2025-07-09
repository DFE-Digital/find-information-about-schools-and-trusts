using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Pages.Schools.Contacts;
using DfE.FindInformationAcademiesTrusts.Services.School;
using DfE.FindInformationAcademiesTrusts.Services.Trust;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Pages.Schools.Contacts;

public class EditRegionsGroupLocalAuthorityLeadModelTests
{
    private readonly EditRegionsGroupLocalAuthorityLeadModel _sut;

    private readonly ISchoolService _mockSchoolService = Substitute.For<ISchoolService>();
    private readonly ISchoolContactsService _mockSchoolContactsService = Substitute.For<ISchoolContactsService>();

    private readonly SchoolSummaryServiceModel _fakeSchool = new(123456, "My School", "Community school",
        SchoolCategory.LaMaintainedSchool);

    private readonly InternalContact _regionsGroupLocalAuthorityLead = new("Regions Group Local Authority Lead",
        "regions.group.local.authority.lead@test.com", DateTime.Today, "test@email.com");

    public EditRegionsGroupLocalAuthorityLeadModelTests()
    {
        _mockSchoolContactsService.GetInternalContactsAsync(123456)
            .Returns(Task.FromResult(new SchoolInternalContactsServiceModel(_regionsGroupLocalAuthorityLead)));
        _mockSchoolService.GetSchoolSummaryAsync(_fakeSchool.Urn)!.Returns(Task.FromResult(_fakeSchool));

        _sut = new EditRegionsGroupLocalAuthorityLeadModel(_mockSchoolService, _mockSchoolContactsService)
            { Urn = 123456 };
    }

    [Fact]
    public async Task OnGetAsync_returns_NotFoundResult_if_School_is_not_found()
    {
        _mockSchoolService.GetSchoolSummaryAsync(123456).Returns(Task.FromResult<SchoolSummaryServiceModel?>(null));
        var result = await _sut.OnGetAsync();
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task OnGetAsync_loads_the_correct_name_and_email()
    {
        var result = await _sut.OnGetAsync();
        result.Should().BeOfType<PageResult>();
        _sut.Name.Should().Be(_regionsGroupLocalAuthorityLead.FullName);
        _sut.Email.Should().Be(_regionsGroupLocalAuthorityLead.Email);
    }

    [Theory]
    [InlineData(true, true,
        "Changes made to the Regions group local authority lead name and email were updated.")]
    [InlineData(true, false,
        "Changes made to the Regions group local authority lead name were updated.")]
    [InlineData(false, true,
        "Changes made to the Regions group local authority lead email were updated.")]
    [InlineData(false, false, "")]
    public async Task OnPostAsync_sets_ContactUpdated_to_true_when_validation_is_correct(bool nameUpdated,
        bool emailUpdated, string expectedMessage)
    {
        _mockSchoolContactsService
            .UpdateContactAsync(123456, Arg.Any<string>(), Arg.Any<string>(),
                SchoolContactRole.RegionsGroupLocalAuthorityLead)
            .Returns(Task.FromResult(new InternalContactUpdatedServiceModel(emailUpdated, nameUpdated)));

        var result = await _sut.OnPostAsync();

        _sut.ContactUpdatedMessage.Should().Be(expectedMessage);

        result.Should().BeOfType<RedirectToPageResult>().Which.PageName.Should().Be("/Schools/Contacts/InDfe");
    }

    [Fact]
    public async Task OnPostAsync_sets_ContactUpdated_to_false_when_validation_is_incorrect()
    {
        _sut.ModelState.AddModelError("Test", "Test");

        var result = await _sut.OnPostAsync();

        result.Should().BeOfType<PageResult>();
        _sut.ContactUpdatedMessage.Should().Be(string.Empty);
    }

    [Fact]
    public async Task OnPostAsync_should_configure_SchoolPageMetadata_when_model_is_valid()
    {
        _sut.SchoolName = _fakeSchool.Name;
        _mockSchoolContactsService
            .UpdateContactAsync(123456, Arg.Any<string>(), Arg.Any<string>(),
                SchoolContactRole.RegionsGroupLocalAuthorityLead)
            .Returns(Task.FromResult(new InternalContactUpdatedServiceModel(true, true)));
        _ = await _sut.OnPostAsync();

        _sut.PageMetadata.SubPageName.Should().Be("Edit Regions group local authority lead details");
        _sut.PageMetadata.PageName.Should().Be("Contacts");
        _sut.PageMetadata.EntityName.Should().Be("My School");
        _sut.PageMetadata.ModelStateIsValid.Should().BeTrue();
    }

    [Fact]
    public async Task OnPostAsync_should_configure_SchoolPageMetadata_when_model_is_not_valid()
    {
        _sut.ModelState.AddModelError("Test", "Test");
        _ = await _sut.OnPostAsync();

        _sut.PageMetadata.SubPageName.Should().Be("Edit Regions group local authority lead details");
        _sut.PageMetadata.PageName.Should().Be("Contacts");
        _sut.PageMetadata.EntityName.Should().Be("My School");
        _sut.PageMetadata.ModelStateIsValid.Should().BeFalse();
    }
}
