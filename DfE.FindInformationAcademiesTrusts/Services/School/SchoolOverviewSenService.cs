using DfE.FindInformationAcademiesTrusts.HttpServices;

namespace DfE.FindInformationAcademiesTrusts.Services.School;

public interface ISchoolOverviewSenService
{
    Task<SchoolOverviewSenServiceModel> GetSchoolOverviewSenAsync(int urn);
}

public class SchoolOverviewSenService(IGetEstablishmentsTemp getEstablishments) : ISchoolOverviewSenService
{
    public async Task<SchoolOverviewSenServiceModel> GetSchoolOverviewSenAsync(int urn)
    {
        var result = await getEstablishments.GetEstablishmentWithSenData(urn);

        var senProvision = new List<string>
        {
            result.SeN1!,
            result.SeN2!,
            result.SeN3!,
            result.SeN4!,
            result.SeN5!,
            result.SeN6!,
            result.SeN7!,
            result.SeN8!,
            result.SeN9!,
            result.SeN10!,
            result.SeN11!,
            result.SeN12!,
            result.SeN13!
        };
        
        var senProvisionTypes = new List<string>();

        foreach (var senType in senProvision)
        {
            senProvisionTypes.Add(senType);
        }

        var senModel = new SchoolOverviewSenServiceModel(
            result.ResourcedProvisionOnRoll,
            result.ResourcedProvisionOnCapacity,
            result.SenUnitOnRoll,
            result.SenUnitCapacity,
            result.TypeOfResourcedProvision,
            senProvisionTypes);
        
        return senModel;
    }
}

