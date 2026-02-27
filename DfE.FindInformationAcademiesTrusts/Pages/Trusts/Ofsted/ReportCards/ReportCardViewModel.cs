using DfE.FindInformationAcademiesTrusts.Services.Ofsted;

namespace DfE.FindInformationAcademiesTrusts.Pages.Trusts.Ofsted.ReportCards;

public record ReportCardViewModel(int Urn, string SchoolName, ReportCardDetails? ReportCardDetails);

