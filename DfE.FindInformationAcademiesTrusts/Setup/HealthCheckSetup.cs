using System.Diagnostics.CodeAnalysis;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Contexts;
using DfE.FindInformationAcademiesTrusts.Data.FiatDb.Contexts;

namespace DfE.FindInformationAcademiesTrusts.Setup;

[ExcludeFromCodeCoverage]
public static class HealthCheckSetup
{
  public static void AddHealthChecks(WebApplicationBuilder builder) {
    builder.Services.AddHealthChecks();
    AddDbHealthChecks(builder);
  }

  public static void AddDbHealthChecks(WebApplicationBuilder builder) {
     builder.Services.AddHealthChecks()
      .AddDbContextCheck<AcademiesDbContext>()
      .AddDbContextCheck<FindInformationAcademiesTrustsContext>();
  }
}
