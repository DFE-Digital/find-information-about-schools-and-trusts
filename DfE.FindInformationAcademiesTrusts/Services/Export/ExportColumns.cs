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
            HasRecentShortInspection = 3,
            CurrentSingleHeadlineGrade = 4,
            CurrentBeforeAfterJoining = 5,
            DateOfCurrentInspection = 6,
            PreviousSingleHeadlineGrade = 7,
            PreviousBeforeAfterJoining = 8,
            DateOfPreviousInspection = 9,
            CurrentQualityOfEducation = 10,
            CurrentBehaviourAndAttitudes = 11,
            CurrentPersonalDevelopment = 12,
            CurrentLeadershipAndManagement = 13,
            CurrentEarlyYearsProvision = 14,
            CurrentSixthFormProvision = 15,
            PreviousQualityOfEducation = 16,
            PreviousBehaviourAndAttitudes = 17,
            PreviousPersonalDevelopment = 18,
            PreviousLeadershipAndManagement = 19,
            PreviousEarlyYearsProvision = 20,
            PreviousSixthFormProvision = 21,
            EffectiveSafeguarding = 22,
            CategoryOfConcern = 23
        }

        public enum OfstedSchoolColumns
        {
            InspectionType = 1,
            DateOfInspection = 2,
            Grade = 3,
            QualityOfEducation = 4,
            BehaviourAndAttitudes = 5,
            PersonalDevelopment = 6,
            LeadershipAndManagement = 7,
            EarlyYearsProvision = 8,
            SixthFormProvision = 9,
            EffectiveSafeguarding = 10,
            CategoryOfConcern = 11,
            BeforeOrAfterJoiningTrust = 12,
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
