namespace DfE.FindInformationAcademiesTrusts.Services.Trust;

public record InternalContactUpdatedServiceModel(
    bool EmailUpdated,
    bool NameUpdated
)
{
    public string ToContactUpdatedMessage(string contactViewString) => this switch
    {
        { NameUpdated: true, EmailUpdated: true } =>
            $"Changes made to the {contactViewString} name and email were updated.",
        { NameUpdated: true, EmailUpdated: false } =>
            $"Changes made to the {contactViewString} name were updated.",
        { NameUpdated: false, EmailUpdated: true } =>
            $"Changes made to the {contactViewString} email were updated.",
        { NameUpdated: false, EmailUpdated: false } => string.Empty
    };
}
