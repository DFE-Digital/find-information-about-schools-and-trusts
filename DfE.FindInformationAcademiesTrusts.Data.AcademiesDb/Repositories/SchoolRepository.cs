using System.Globalization;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Contexts;
using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.School;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Repositories;

public class SchoolRepository(
    IAcademiesDbContext academiesDbContext,
    IStringFormattingUtilities stringFormattingUtilities,
    ILogger<SchoolRepository> logger) : ISchoolRepository
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
        return await academiesDbContext.GiasEstablishments
            .Where(e => e.Urn == urn)
            .Select(establishment => new SchoolDetails(establishment.EstablishmentName!,
                stringFormattingUtilities.BuildAddressString(
                    establishment.Street,
                    null,
                    establishment.Town,
                    establishment.Postcode
                ),
                establishment.GorName!,
                establishment.LaName!,
                establishment.PhaseOfEducationName!,
                new AgeRange(establishment.StatutoryLowAge!, establishment.StatutoryHighAge!),
                establishment.NurseryProvisionName))
            .SingleAsync();
    }

    public async Task<DateOnly?> GetDateJoinedTrustAsync(int urn)
    {
        return await academiesDbContext.GiasGroupLinks.Where(gl => gl.Urn == urn.ToString())
            .Select(gl =>
                DateOnly.ParseExact(gl.JoinedDate!, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None))
            .Cast<DateOnly?>()
            .FirstOrDefaultAsync();
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
        return await academiesDbContext.GiasEstablishments
            .Where(e => e.Urn == urn)
            .Select(establishment => new SenProvision(
                establishment.ResourcedProvisionOnRoll!,
                establishment.ResourcedProvisionCapacity!,
                establishment.SenUnitOnRoll!,
                establishment.SenUnitCapacity!,
                establishment.TypeOfResourcedProvisionName!,
                new List<string>
                {
                    establishment.Sen1Name!,
                    establishment.Sen2Name!,
                    establishment.Sen3Name!,
                    establishment.Sen4Name!,
                    establishment.Sen5Name!,
                    establishment.Sen6Name!,
                    establishment.Sen7Name!,
                    establishment.Sen8Name!,
                    establishment.Sen9Name!,
                    establishment.Sen10Name!,
                    establishment.Sen11Name!,
                    establishment.Sen12Name!,
                    establishment.Sen13Name!
                }
            ))
            .SingleAsync();
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
        return await academiesDbContext.GiasEstablishments.Where(e => e.Urn == urn)
            .Select(e => new SchoolReferenceNumbers(e.LaCode, e.EstablishmentNumber, e.Ukprn)).SingleOrDefaultAsync();
    }
}
