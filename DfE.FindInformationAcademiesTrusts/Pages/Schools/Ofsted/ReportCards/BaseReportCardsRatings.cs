using DfE.FindInformationAcademiesTrusts.Services.Ofsted;

namespace DfE.FindInformationAcademiesTrusts.Pages.Schools.Ofsted.ReportCards;

using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Data.Enums;

public record BaseReportCardsRatings(string PageName, ReportCardDetails EstablishmentReportCard, BeforeOrAfterJoining WhenDidInspectionHappen);
