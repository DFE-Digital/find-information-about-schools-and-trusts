using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DocumentFormat.OpenXml.Office.PowerPoint.Y2021.M06.Main;

namespace DfE.FindInformationAcademiesTrusts.Extensions
{
    public static class BeforeOrAfterJoiningExtensions
    {
        public static string ToDisplayString(this BeforeOrAfterJoining beforeOrAfterJoining)
        {
            return beforeOrAfterJoining switch
            {
                BeforeOrAfterJoining.Before => "Before joining",
                BeforeOrAfterJoining.After => "After joining",
                _ => "Unknown"
            };
        }

        public static BeforeOrAfterJoining GetBeforeOrAfterJoiningTrust(this DateTime? dateJoined, DateOnly? inspectionDate)
        {
            DateTime? inspectionDateTimeValue = inspectionDate?.ToDateTime(TimeOnly.MinValue);

            return GetBeforeOrAfterJoiningTrust(dateJoined, inspectionDateTimeValue);
        }

        public static BeforeOrAfterJoining GetBeforeOrAfterJoiningTrust(this DateTime? dateJoined, DateTime? inspectionDate)
        {
            if (dateJoined is null)
            {
                return BeforeOrAfterJoining.NotApplicable;
            }

            if (inspectionDate is null)
            {
                return BeforeOrAfterJoining.NotYetInspected;
            }

            if (inspectionDate >= dateJoined)
            {
                return BeforeOrAfterJoining.After;
            }

            // Must be inspectionDate < dateJoined by process of elimination

            return BeforeOrAfterJoining.Before;
        }

    }

}
