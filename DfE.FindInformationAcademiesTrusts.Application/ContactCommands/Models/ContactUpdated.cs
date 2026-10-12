namespace DfE.FindInformationAcademiesTrusts.Application.ContactCommands.Models;

public record ContactUpdated(bool NameUpdated, bool EmailUpdated)
{
    public string ToMessage(string roleName) => this switch
    {
        { NameUpdated: true, EmailUpdated: true } => $"Changes made to the {roleName} name and email were updated.",
        { NameUpdated: true } => $"Changes made to the {roleName} name were updated.",
        { EmailUpdated: true } => $"Changes made to the {roleName} email were updated.",
        _ => string.Empty
    };
}
