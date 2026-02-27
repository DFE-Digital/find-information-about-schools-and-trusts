using DfE.FindInformationAcademiesTrusts.Options;
using Microsoft.Extensions.Options;

namespace DfE.FindInformationAcademiesTrusts.Services.Ofsted
{
    public class PowerBiLinkBuilderService(IOptions<OfstedBiReportLinksOptions> options) : IPowerBiLinkBuilderService
    {
        private readonly OfstedBiReportLinksOptions _options = options.Value;

        public string? BuildReportCardsLink(int urn)
        {
            if (string.IsNullOrEmpty(_options.ReportCardsBaseUrl))
            {
                return null;
            }

            return $"{_options.ReportCardsBaseUrl}&rp:param_URN={urn}";
        }

        public string? BuildOfstedPublishedLink(int urn)
        {
            if (string.IsNullOrEmpty(_options.OfstedPublishedBaseUrl))
            {
                return null;
            }

            return $"{_options.OfstedPublishedBaseUrl}&rp:URN={urn}";
        }

        public string? BuildReportCardsLinkForTrust(string trustReference)
        {
            if (string.IsNullOrEmpty(_options.ReportCardsBaseUrl))
            {
                return null;
            }

            return $"{_options.ReportCardsBaseUrl}&rp:param_TrustRef={trustReference}";
        }

        public string? BuildOfstedPublishedLinkForTrust(string trustReference)
        {
            if (string.IsNullOrEmpty(_options.OfstedPublishedBaseUrl))
            {
                return null;
            }
            
            return $"{_options.OfstedPublishedBaseUrl}&rp:TrustRef={trustReference}";
        }
    }
}
