namespace DfE.FindInformationAcademiesTrusts.Services.Ofsted;

public interface IPowerBiLinkBuilderService
{
    string? BuildReportCardsLink(int urn);
    string? BuildOfstedPublishedLink(int urn);
    string? BuildReportCardsLinkForTrust(string trustReference);
    string? BuildOfstedPublishedLinkForTrust(string trustReference);
}
