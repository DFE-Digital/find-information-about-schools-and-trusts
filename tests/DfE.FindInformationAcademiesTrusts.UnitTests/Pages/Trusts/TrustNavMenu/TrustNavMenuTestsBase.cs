using DfE.FindInformationAcademiesTrusts.Data.Repositories;
using DfE.FindInformationAcademiesTrusts.Pages.Trusts;
using DfE.FindInformationAcademiesTrusts.Pages.Trusts.Academies;
using DfE.FindInformationAcademiesTrusts.Pages.Trusts.Academies.InTrust;
using DfE.FindInformationAcademiesTrusts.Pages.Trusts.Academies.Pipeline;
using DfE.FindInformationAcademiesTrusts.Pages.Trusts.Contacts;
using DfE.FindInformationAcademiesTrusts.Pages.Trusts.FinancialDocuments;
using DfE.FindInformationAcademiesTrusts.Pages.Trusts.Governance;
using DfE.FindInformationAcademiesTrusts.Pages.Trusts.Ofsted;
using DfE.FindInformationAcademiesTrusts.Pages.Trusts.Overview;
using DfE.FindInformationAcademiesTrusts.Services.Academy;
using DfE.FindInformationAcademiesTrusts.Services.Trust;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Pages.Trusts.TrustNavMenu;

public abstract class TrustNavMenuTestsBase
{
    public static TheoryData<Type> SubPageTypes =>
    [
        //Overview
        typeof(TrustDetailsModel),
        typeof(TrustSummaryModel),
        typeof(ReferenceNumbersModel),
        //Contacts
        typeof(InDfeModel),
        typeof(InTrustModel),
        //Academies
        typeof(AcademiesInTrustDetailsModel),
        typeof(PupilNumbersModel),
        typeof(FreeSchoolMealsModel),
        typeof(PreAdvisoryBoardModel),
        typeof(PostAdvisoryBoardModel),
        typeof(FreeSchoolsModel),
        //Ofsted
        typeof(SingleHeadlineGradesModel),
        typeof(CurrentRatingsModel),
        typeof(PreviousRatingsModel),
        typeof(SafeguardingAndConcernsModel),
        //Financial documents
        typeof(FinancialStatementsModel),
        typeof(ManagementLettersModel),
        typeof(InternalScrutinyReportsModel),
        typeof(SelfAssessmentChecklistsModel),
        //Governance
        typeof(TrustLeadershipModel),
        typeof(TrusteesModel),
        typeof(MembersModel),
        typeof(HistoricMembersModel)
    ];

    protected static TrustsAreaModel GetMockTrustPage(Type pageType, string uid = "1234", int numberOfAcademies = 3,
        AcademyPipelineSummaryServiceModel? academyPipelineSummarySummaryServiceModel = null,
        TrustGovernanceServiceModel? trustGovernanceServiceModel = null)
    {
        //Create a mock page
        var parameters = pageType.GetConstructors()[0].GetParameters();
        var arguments = parameters.Select(p => p.ParameterType.Name switch
        {
            _ => Substitute.For([p.ParameterType], [])
        }).ToArray();

        var mockPage = Activator.CreateInstance(pageType, arguments) as TrustsAreaModel ??
                       throw new ArgumentException("Couldn't create mock for given page type", nameof(pageType));

        //Set properties applicable to all types
        mockPage.Uid = uid;
        mockPage.TrustSummary = new TrustSummaryServiceModel(uid, "MY TRUST", "Multi-academy trust", numberOfAcademies);

        //Add extra info for some pages
        switch (mockPage)
        {
            case AcademiesAreaModel academies:
                academies.PipelineSummary = academyPipelineSummarySummaryServiceModel ??
                                            new AcademyPipelineSummaryServiceModel(1, 2, 3);
                break;
            case GovernanceAreaModel governance:
                governance.TrustGovernance = trustGovernanceServiceModel ??
                                             GetTrustGovernanceServiceModel(uid, 1, 2, 3, 4);
                break;
        }

        return mockPage;
    }

    protected static TrustGovernanceServiceModel GetTrustGovernanceServiceModel(string uid, int numTrustLeadership,
        int numCurrentMembers, int numCurrentTrustees, int numHistoricMembers)
    {
        var governor = new Governor("", uid, "", "", "", null, null, "");
        var currentTrustLeadership = Enumerable.Repeat(governor, numTrustLeadership).ToArray();
        var currentMembers = Enumerable.Repeat(governor, numCurrentMembers).ToArray();
        var currentTrustees = Enumerable.Repeat(governor, numCurrentTrustees).ToArray();
        var historicMembers = Enumerable.Repeat(governor, numHistoricMembers).ToArray();
        var trustGovernanceServiceModel = new TrustGovernanceServiceModel(currentTrustLeadership, currentMembers,
            currentTrustees, historicMembers, (decimal)0.5);
        return trustGovernanceServiceModel;
    }
}
