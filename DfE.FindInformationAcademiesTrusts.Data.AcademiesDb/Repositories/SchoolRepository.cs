using System.Globalization;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.AcademiesDbServices;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Contexts;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Extensions;
using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Data.Repositories;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.School;
using GovUK.Dfe.CoreLibs.Contracts.Academies.V4.Establishments;
using GovUK.Dfe.PersonsApi.Client.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Repositories;

public class SchoolRepository(IAcademiesDbContext academiesDbContext,
    IStringFormattingUtilities stringFormattingUtilities,
    ILogger<SchoolRepository> logger, 
    IGetEstablishments getEstablishments,
    IEstablishmentsClient establishmentsClient) : ISchoolRepository
{
    public async Task<SchoolSummary?> GetSchoolSummaryAsync(int urn)
    {
        return await academiesDbContext.GiasEstablishments
            .Where(e => e.Urn == urn)
            .Select(e => new SchoolSummary(
                e.EstablishmentName!,
                e.TypeOfEstablishmentName!,
                e.EstablishmentTypeGroupName == "Academies"
                    ? SchoolCategory.Academy
                    : SchoolCategory.LaMaintainedSchool))
            .SingleOrDefaultAsync();
    }

    public async Task<SchoolDetails> GetSchoolDetailsAsync(int urn)
    {
        var result = await getEstablishments.GetEstablishment(urn);
        
        DateTime? dateJoinedTrust = null;

        if (!string.IsNullOrEmpty(result.DateJoinedTrust) &&
            DateTime.TryParseExact(
                result.DateJoinedTrust,
                "dd/MM/yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var parsedDate))
        {
            dateJoinedTrust = parsedDate;
        }


        return new SchoolDetails(
            Name:  result.Name,
                Address: stringFormattingUtilities.BuildAddressString(
                    result.Address.Street,
                    result.Address.Locality,
                    result.Address.Town,
                    result.Address.Postcode
                ),
                Region: result.Gor.Name,
                LocalAuthority: result.LocalAuthorityName,
                PhaseOfEducationName: result.PhaseOfEducation.Name,
                AgeRange: new AgeRange(result.StatutoryLowAge, result.StatutoryHighAge),
                NurseryProvision: result.NurseryProvision,
                TrustName: result.TrustName,
                DateJoinedTrust: dateJoinedTrust);
    }

    public async Task<SchoolContact?> GetSchoolContactsAsync(int urn)
    {
        var headteacher = await academiesDbContext.TadHeadTeacherContacts
            .Where(c => c.Urn == urn)
            .Select(contact => new { contact.HeadFirstName, contact.HeadLastName, contact.HeadEmail })
            .SingleOrDefaultAsync();

        if (headteacher is null)
        {
            logger.LogError("Unable to find head teacher contact for school with URN {urn}", urn);

            return null;
        }

        var fullName = stringFormattingUtilities.GetFullName(headteacher.HeadFirstName, headteacher.HeadLastName);
        var email = string.IsNullOrWhiteSpace(headteacher.HeadEmail) ? null : headteacher.HeadEmail;

        return new SchoolContact(fullName, email);
    }

    public async Task<SenProvision> GetSchoolSenProvisionAsync(int urn)
    {
        var result = await getEstablishments.GetEstablishmentWithSenData(urn);
        
        var senProvision = new SenProvision(
            result.ResourcedProvisionOnRoll,
            result.ResourcedProvisionOnCapacity,
            result.SenUnitOnRoll,
            result.SenUnitCapacity,
            result.TypeOfResourcedProvision,
            new List<string>
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
            }
        );
        
        return senProvision;
    }

    public async Task<bool> IsPartOfFederationAsync(int urn)
    {
        var federationsCode = await academiesDbContext.GiasEstablishments
            .Where(e => e.Urn == urn)
            .Select(x => x.FederationsCode)
            .FirstOrDefaultAsync();

        return !string.IsNullOrWhiteSpace(federationsCode);
    }

    public async Task<FederationDetails> GetSchoolFederationDetailsAsync(int urn)
    {
        var schoolFederationDetails = await academiesDbContext.GiasEstablishments
            .Where(e => e.Urn == urn)
            .Select(establishment => new FederationDetails(
                establishment.FederationsName,
                establishment.FederationsCode))
            .SingleAsync();

        if (schoolFederationDetails.FederationUid != null)
        {
            var openedOnDate = await academiesDbContext.GiasGroupLinks
                .Where(gl => gl.GroupUid == schoolFederationDetails.FederationUid)
                .Select(gl =>
                    DateOnly.ParseExact(gl.OpenDate!, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None))
                .FirstAsync();

            var schools = await academiesDbContext.GiasEstablishments
                .Where(e => e.FederationsCode == schoolFederationDetails.FederationUid)
                .ToDictionaryAsync(establishment => establishment.Urn.ToString(),
                    establishment => establishment.EstablishmentName!);

            schoolFederationDetails = schoolFederationDetails with { OpenedOnDate = openedOnDate, Schools = schools };
        }

        return schoolFederationDetails;
    }

    public async Task<SchoolReferenceNumbers?> GetReferenceNumbersAsync(int urn)
    {
        var result = await getEstablishments.GetEstablishment(urn);

        return new SchoolReferenceNumbers(result.LocalAuthorityCode, result.EstablishmentNumber, result.Ukprn);
    }

    public async Task<List<Governor>> GetGovernanceAsync(int urn)
    {
        var result = await establishmentsClient.GetAllPersonsAssociatedWithAcademyByUrnAsync(urn);
        
        var governors = new List<Governor>();

        foreach (var person in result)
        {
            governors.Add(new Governor(
                FullName: person.DisplayName,
                Role: person.Roles[0],
                AppointingBody: person.AppointingBody,
                DateOfAppointment: person.DateOfAppointment.ParseAsNullableDate(),
                DateOfTermEnd: person.DateTermOfOfficeEndsEnded.ParseAsNullableDate(),
                Email: person.Email));
        }
        
        return governors;
    }

    public async Task<ReligiousCharacteristics> GetReligiousCharacteristicsAsync(int urn)
    {
        var result = await getEstablishments.GetEstablishment(urn);
        
        return new ReligiousCharacteristics(result.Diocese.Name, result.ReligiousCharacter.Name, result.ReligousEthos);
    }
}
