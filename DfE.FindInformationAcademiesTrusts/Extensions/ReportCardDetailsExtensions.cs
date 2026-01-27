namespace DfE.FindInformationAcademiesTrusts.Extensions
{
    using DfE.FindInformationAcademiesTrusts.Services.Ofsted;

    public static class ReportCardDetailsExtensions
    {
        public static Dictionary<string, string> ToDictionary(this ReportCardDetails? reportCardDetails)
        {
           if(reportCardDetails is null)
           {
                return new Dictionary<string, string>();
           }

           return new Dictionary<string, string>
           {
               { "Leadership and Governance", reportCardDetails.LeadershipAndGovernance ?? string.Empty },
               { "Personal Development and Well Being", reportCardDetails.PersonalDevelopmentAndWellBeing ?? string.Empty },
               { "Curriculum and Teaching", reportCardDetails.CurriculumAndTeaching ?? string.Empty },
               { "Inclusion", reportCardDetails.Inclusion ?? string.Empty },
               { "Achievement", reportCardDetails.Achievement ?? string.Empty },
               { "Attendance and Behaviour", reportCardDetails.AttendanceAndBehaviour ?? string.Empty },
               { "Early Years Provision", reportCardDetails.EarlyYearsProvision ?? string.Empty },
               { "Safeguarding", reportCardDetails.Safeguarding ?? string.Empty },
               { "Post 16 Provision", reportCardDetails.Post16Provision ?? string.Empty },
               { "Category of Concern", reportCardDetails.CategoryOfConcern ?? string.Empty }
           };
        }
    }
}
