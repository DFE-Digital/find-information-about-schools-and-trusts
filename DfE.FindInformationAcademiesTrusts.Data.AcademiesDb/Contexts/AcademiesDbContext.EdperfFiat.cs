using System.Diagnostics.CodeAnalysis;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Models.Edperf_Mstr;
using Microsoft.EntityFrameworkCore;

namespace DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Contexts;

public partial class AcademiesDbContext
{
    public DbSet<EdperfFiat> EdperfFiats { get; set; }
    
    [ExcludeFromCodeCoverage]
    protected static void OnModelCreatingEdperfFiats(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EdperfFiat>(entity =>
        {
            entity.HasNoKey().ToTable("edperf_fiat", "edperf_mstr");
            entity.Property(e => e.Urn).HasColumnName("URN");
            entity.Property(e => e.DownloadYear).HasColumnName("DownloadYear");
            entity.Property(e => e.CensusNor).HasColumnName("census_NOR");
            entity.Property(e => e.CensusTsenelse).HasColumnName("census_TSENELSE");
            entity.Property(e => e.CensusPsenelse).HasColumnName("census_PSENELSE");
            entity.Property(e => e.CensusTsenelk).HasColumnName("census_TSENELK");
            entity.Property(e => e.CensusPsenelk).HasColumnName("census_PSENELK");
            entity.Property(e => e.CensusNumeal).HasColumnName("census_NUMEAL");
            entity.Property(e => e.CensusPnumeal).HasColumnName("census_PNUMEAL");
            entity.Property(e => e.CensusNumfsm).HasColumnName("census_NUMFSM");
            entity.Property(e => e.AbsencePerctot).HasColumnName("absence_PERCTOT");
            entity.Property(e => e.AbsencePpersabs10).HasColumnName("absence_PPERSABS10");
            entity.Property(e => e.MetaCensusIngestionDatetime).HasColumnName("meta_census_ingestion_datetime");
            entity.Property(e => e.MetaAbsenceIngestionDatetime).HasColumnName("meta_absence_ingestion_datetime");
        });
    }
}
