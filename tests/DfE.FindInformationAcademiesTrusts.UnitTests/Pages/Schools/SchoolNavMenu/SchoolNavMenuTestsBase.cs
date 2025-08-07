using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Pages.Schools;
using DfE.FindInformationAcademiesTrusts.Pages.Schools.Contacts;
using DfE.FindInformationAcademiesTrusts.Pages.Schools.Governance;
using DfE.FindInformationAcademiesTrusts.Pages.Schools.Overview;
using DfE.FindInformationAcademiesTrusts.Services.School;
using Microsoft.FeatureManagement;
using Sut = DfE.FindInformationAcademiesTrusts.Pages.Schools.SchoolNavMenu;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Pages.Schools.SchoolNavMenu;

public abstract class SchoolNavMenuTestsBase
{
    protected IVariantFeatureManager MockFeatureManager = Substitute.For<IVariantFeatureManager>();
    protected readonly Sut Sut;

    protected SchoolNavMenuTestsBase()
    {
        Sut = new Sut(MockFeatureManager);
    }

    public static TheoryData<Type> ContactsInDfeForSchoolsDisabledSubPageTypes =>
    [
        //Overview
        typeof(DetailsModel),
        typeof(SenModel),
        typeof(FederationModel),
        typeof(ReferenceNumbersModel),
        //Contacts
        typeof(InSchoolModel)
    ];

    public static TheoryData<Type> ContactsInDfeForSchoolsEnabledSubPageTypes =>
    [
        //Overview
        typeof(DetailsModel),
        typeof(SenModel),
        typeof(ReferenceNumbersModel),
        //Contacts
        typeof(InDfeModel),
        typeof(InSchoolModel),
        //Governance
        typeof(CurrentModel)
    ];

    protected static SchoolAreaModel GetMockSchoolPage(Type pageType, int urn = 123456,
        SchoolCategory schoolCategory = SchoolCategory.LaMaintainedSchool, bool isFederation = true)
    {
        //Create a mock page
        var parameters = pageType.GetConstructors()[0].GetParameters();
        var arguments = parameters.Select(p => p.ParameterType.Name switch
        {
            _ => Substitute.For([p.ParameterType], [])
        }).ToArray();

        var mockPage = Activator.CreateInstance(pageType, arguments) as SchoolAreaModel ??
                       throw new ArgumentException("Couldn't create mock for given page type", nameof(pageType));

        //Set properties applicable to all types
        mockPage.Urn = urn;
        mockPage.SchoolSummary = new SchoolSummaryServiceModel(urn, "Chill primary school", "", schoolCategory);
        mockPage.IsPartOfAFederation = isFederation;

        return mockPage;
    }
}
