namespace DfE.FindInformationAcademiesTrusts.Pages.Shared;

public interface IEstablishmentSearchFormModel
{
    string? KeyWords { get; set; }
    string PageSearchFormInputId { get; }
    string AutocompletePagePath { get; }
    string Heading { get;  }
    string Hint { get;  }
   
}
