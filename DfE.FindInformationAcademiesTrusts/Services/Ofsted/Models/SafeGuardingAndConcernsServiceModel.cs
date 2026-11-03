
using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Extensions;

namespace DfE.FindInformationAcademiesTrusts.Services.Ofsted
{
    public class SafeGuardingAndConcernsServiceModel(DateTime dateJoinedTrust) : IOfstedInspection
    {
        public DateOnly? InspectionDate { get; set; }

        public DateTime DateJoinedTrust { get; } = dateJoinedTrust;

        public BeforeOrAfterJoining WhenDidCurrentInspectionHappen => BeforeOrAfterJoiningExtensions.GetBeforeOrAfterJoiningTrust(DateJoinedTrust, InspectionDate);

        public string SafeGuarding { get; set; } = string.Empty;

        public string Concerns { get; set; } = string.Empty;
    }
}
