namespace DfE.FindInformationAcademiesTrusts.Services.Export
{
    public class ExportColumns
    {
        public static class CommonColumnNames
        {
            internal const string AgeRange = "Age range";
            internal const string BeforeOrAfterJoiningHeader = "Before/After Joining";
            internal const string DateJoined = "Date joined";
            internal const string LocalAuthority = "Local authority";
            internal const string SchoolName = "School Name";
            internal const string Urn = "URN";
        }


        public enum OfstedTrustColumns
        {
            SchoolName = 1,
            DateJoined = 2,
            CurrentSingleHeadlineGrade = 3,
            CurrentBeforeAfterJoining = 4,
            DateOfCurrentInspection = 5,
            PreviousSingleHeadlineGrade = 6,
            PreviousBeforeAfterJoining = 7,
            DateOfPreviousInspection = 8,
            CurrentQualityOfEducation = 9,
            CurrentBehaviourAndAttitudes = 10,
            CurrentPersonalDevelopment = 11,
            CurrentLeadershipAndManagement = 12,
            CurrentEarlyYearsProvision = 13,
            CurrentSixthFormProvision = 14,
            PreviousQualityOfEducation = 15,
            PreviousBehaviourAndAttitudes = 16,
            PreviousPersonalDevelopment = 17,
            PreviousLeadershipAndManagement = 18,
            PreviousEarlyYearsProvision = 19,
            PreviousSixthFormProvision = 20,
            EffectiveSafeguarding = 21,
            CategoryOfConcern = 22
        }

        public enum OfstedSchoolColumns
        {
            InspectionType = 1,
            DateOfInspection = 2,
            Grade = 3,
        }

        public enum AcademyColumns
        {
            SchoolName = 1,
            Urn = 2,
            LocalAuthority = 3,
            Type = 4,
            RuralOrUrban = 5,
            DateJoined = 6,
            CurrentOfstedRating = 7,
            CurrentBeforeAfterJoining = 8,
            DateOfCurrentInspection = 9,
            PreviousOfstedRating = 10,
            PreviousBeforeAfterJoining = 11,
            DateOfPreviousInspection = 12,
            PhaseOfEducation = 13,
            AgeRange = 14,
            PupilNumbers = 15,
            Capacity = 16,
            PercentFull = 17,
            PupilsEligibleFreeSchoolMeals = 18,
            LaPupilsEligibleFreeSchoolMeals = 19,
            NationalPupilsEligibleFreeSchoolMeals = 20,
        }

        public enum PipelineAcademiesColumns
        {
            SchoolName = 1,
            Urn = 2,
            AgeRange = 3,
            LocalAuthority = 4,
            ProjectType = 5,
            ChangeDate = 6
        }
    }
}
