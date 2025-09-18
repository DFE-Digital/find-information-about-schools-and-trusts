using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.PupilCensus;

namespace DfE.FindInformationAcademiesTrusts.Services.Academy;

public record AcademyPupilNumbersServiceModel(
    string Urn,
    string? EstablishmentName,
    string? PhaseOfEducation,
    AgeRange AgeRange,
    Statistic<int> NumberOfPupils,
    int? SchoolCapacity
)
{
    public Statistic<float> PercentageFull
    {
        get
        {
            var schoolCapacity = SchoolCapacity is not null and not 0
                ? new Statistic<int>.WithValue((int)SchoolCapacity)
                : Statistic<int>.NotAvailable;

            return NumberOfPupils.Compute(schoolCapacity, (nop, sc) => (float)Math.Round(nop / (float)sc * 100.0f));
        }
    }
}
