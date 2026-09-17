namespace Dfe.FindInformationAcademiesTrusts.Models;

public class Error
{
   public string Key { get; set; } = string.Empty;
   public string Message { get; set; } = string.Empty;
   public List<string> InvalidInputs { get; set; } = [];
}
