using System.Diagnostics.CodeAnalysis;

namespace DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Models.Edperf_Mstr;

[ExcludeFromCodeCoverage(Justification = "Database model POCO")]
public class EdperfFiat
{
    public required int Urn { get; set; }
    public required string DownloadYear { get; set; }
    public string? CensusNor { get; set; }
    public string? CensusTsenelse { get; set; }
    public string? CensusPsenelse { get; set; }
    public string? CensusTsenelk { get; set; }
    public string? CensusPsenelk { get; set; }
    public string? CensusNumeal { get; set; }
    public string? CensusPnumeal { get; set; }
    public string? CensusNumfsm { get; set; }
    public string? AbsencePerctot { get; set; }
    public string? AbsencePpersabs10 { get; set; }
    public DateTime? MetaCensusIngestionDatetime { get; set; }
    public DateTime? MetaAbsenceIngestionDatetime { get; set; }
}
