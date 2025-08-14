using System.Diagnostics.CodeAnalysis;

namespace DfE.FindInformationAcademiesTrusts.Pages.Trusts;

[ExcludeFromCodeCoverage(Justification = "Record with no behaviour and only used within Razor pages")]
public record AcademyLinkModel(string? EstablishmentName, string AspPage, string? Urn);
