import commonPage from "../commonPage";
import { TableUtility } from "../tableUtility";

class OfstedPage {
    // #region Common stuffs
    elements = {
        subpageHeader: () => cy.get('[data-testid="subpage-header"]'),
        downloadButton: () => cy.get('[data-testid="download-all-ofsted-data-button"]'),
        singleHeadlineGrades: {
            section: () => cy.get('[data-testid="ofsted-overview-table"]'),
            table: () => this.elements.singleHeadlineGrades.section().find('[aria-describedby="ofsted-caption"]'),
            tableRows: () => this.elements.singleHeadlineGrades.table().find('tbody tr'),
            schoolName: () => this.elements.singleHeadlineGrades.section().find('[data-testid="school-name"]'),
            schoolNameHeader: () => this.elements.singleHeadlineGrades.section().find('[data-testid="ofsted-overview-grades-school-name-header"]'),
            dateJoined: () => this.elements.singleHeadlineGrades.section().find('[data-testid="academy-date-joined"]'),
            dateJoinedHeader: () => this.elements.singleHeadlineGrades.section().find('[data-testid="ofsted-overview-grades-school-date-joined-header"]'),
            hasRecentShortInspection: () => this.elements.singleHeadlineGrades.section().find('td[data-testid="has-recent-short-inspection"]'),
            hasRecentShortInspectionHeader: () => cy.get('th[data-testid="ofsted-overview-grades-school-short-inspection-header"]'),
            currentSHG: () => this.elements.singleHeadlineGrades.section().find('[data-testid="current-inspection-type"]'),
            currentSHGBeforeOrAfter: () => this.elements.singleHeadlineGrades.section().find('[data-testid="ofsted-single-headline-grades-current-before-after-joining"]'),
            currentSHGHeader: () => this.elements.singleHeadlineGrades.section().find('[data-testid="ofsted-overview-grades-school-current-inspection-type-header"]'),
            dateOfCurrentInspection: () => this.elements.singleHeadlineGrades.section().find('[data-testid="date-of-current-inspection"]'),
            dateOfCurrentInspectionHeader: () => this.elements.singleHeadlineGrades.section().find('[data-testid="ofsted-overview-grades-school-date-current-inspection-header"]'),
            previousSHG: () => this.elements.singleHeadlineGrades.section().find('[data-testid="previous-inspection-type"]'),
            previousSHGBeforeOrAfter: () => this.elements.singleHeadlineGrades.section().find('[data-sort-value="3"] > [data-testid="ofsted-single-headline-grades-previous-before-after-joining"]'),
            previousSHGHeader: () => this.elements.singleHeadlineGrades.section().find('[data-testid="ofsted-overview-grades-school-previous-inspection-type-header"]'),
            dateOfPreviousInspection: () => this.elements.singleHeadlineGrades.section().find('[data-testid="date-of-previous-inspection"]'),
            dateOfPreviousInspectionHeader: () => this.elements.singleHeadlineGrades.section().find('[data-testid="ofsted-overview-grades-school-date-previous-inspection-header"]'),
            whereToFindShortInspectionDataDetails: () => cy.get('[data-testid="where-to-find-short-inspection-data"]').closest('details'),
        },
        reportCard: {
            section: () => cy.get('[data-testid="ofsted-report-cards-overview"]'),
            schoolName: () => this.elements.reportCard.section().find('[data-testid="school-name"]'),
            schoolNameHeader: () => this.elements.reportCard.section().find('[data-testid="school-name-header"]'),
            noDataMessage: () => this.elements.reportCard.section().contains('No data available'),
            inclusionHeader: () => this.elements.reportCard.section().find('[data-testid="inclusion-header"]'),
            inclusion: () => this.elements.reportCard.section().find('[data-testid="inclusion"]'),
            curriculumAndTeachingHeader: () => this.elements.reportCard.section().find('[data-testid="curriculum-and-teaching-header"]'),
            curriculumAndTeaching: () => this.elements.reportCard.section().find('[data-testid="curriculum-and-teaching"]'),
            personalDevelopmentHeader: () => this.elements.reportCard.section().find('[data-testid="personal-development-and-well-being-header"]'),
            personalDevelopment: () => this.elements.reportCard.section().find('[data-testid="personal-development"]'),
            leadershipAndGoveranceHeader: () => this.elements.reportCard.section().find('[data-testid="leadership-and-governance-header"]'),
            leadershipAndGoverance: () => this.elements.reportCard.section().find('[data-testid="leadership-and-governance"]'),
            achievementHeader: () => this.elements.reportCard.section().find('[data-testid="achievement-header"]'),
            achievement: () => this.elements.reportCard.section().find('[data-testid="achievement"]'),
            attendanceAndBehaviourHeader: () => this.elements.reportCard.section().find('[data-testid="attendance-and-behaviour-header"]'),
            attendanceAndBehaviour: () => this.elements.reportCard.section().find('[data-testid="attendance-and-behaviour"]'),
        },

        safeguardingAndConcerns: {
            section: () => cy.get('[data-testid="ofsted-safeguarding-and-concerns-name-table"]'),
            schoolNameHeader: () => this.elements.safeguardingAndConcerns.section().find('[data-testid="ofsted-safeguarding-and-concerns-name-header"]'),
            schoolName: () => this.elements.safeguardingAndConcerns.section().find('[data-testid="ofsted-safeguarding-and-concerns-school-name"]'),
            effectiveSafeguardingHeader: () => this.elements.safeguardingAndConcerns.section().find('[data-testid="ofsted-safeguarding-and-concerns-effective-safeguarding-header"]'),
            effectiveSafeguarding: () => this.elements.safeguardingAndConcerns.section().find('[data-testid="ofsted-safeguarding-and-concerns-effective-safeguarding"]'),
            categoryOfConcernHeader: () => this.elements.safeguardingAndConcerns.section().find('[data-testid="ofsted-safeguarding-and-concerns-category-of-concern-header"]'),
            categoryOfConcern: () => this.elements.safeguardingAndConcerns.section().find('[data-testid="ofsted-safeguarding-and-concerns-category-of-concern"]'),
            beforeOrAfterJoiningHeader: () => this.elements.safeguardingAndConcerns.section().find('[data-testid="ofsted-safeguarding-and-concerns-before-or-after-joining-header"]'),
            beforeOrAfterJoining: () => this.elements.safeguardingAndConcerns.section().find('[data-testid="ofsted-safeguarding-before-or-after-joining"]'),
        },
        importantDates: {
            section: () => cy.get('[data-testid="ofsted-important-dates-school-name-table"]'),
            schoolName: () => this.elements.importantDates.section().find('[data-testid="ofsted-important-dates-school-name"]'),
            schoolNameHeader: () => this.elements.importantDates.section().find('[data-testid="ofsted-important-dates-school-name-header"]'),
            dateJoined: () => this.elements.importantDates.section().find('[data-testid="ofsted-important-dates-date-joined"]'),
            dateJoinedHeader: () => this.elements.importantDates.section().find('[data-testid="ofsted-important-dates-date-joined-header"]'),
            dateOfCurrentInspection: () => this.elements.importantDates.section().find('[data-testid="ofsted-important-dates-date-of-current-inspection"]'),
            dateOfCurrentInspectionHeader: () => this.elements.importantDates.section().find('[data-testid="ofsted-important-dates-date-of-current-inspection-header"]'),
            dateOfPreviousInspection: () => this.elements.importantDates.section().find('[data-testid="ofsted-important-dates-date-of-previous-inspection"]'),
            dateOfPreviousInspectionHeader: () => this.elements.importantDates.section().find('[data-testid="ofsted-important-dates-date-of-previous-inspection-header"]'),
        }
    };

    private readonly checkElementMatches = (element: JQuery<HTMLElement>, expected: RegExp) => {
        const text = element.text().trim();
        expect(text).to.match(expected);
    };

    private readonly checkValueIsValidCurrentOfstedRating = (element: JQuery<HTMLElement>) =>
        this.checkElementMatches(element, /^(Good|Not available|Outstanding|Requires improvement|Inadequate|Not yet inspected|Insufficient evidence|Does not apply)$/);

    private readonly checkValueIsValidPreviousOfstedRating = (element: JQuery<HTMLElement>) => {
        const text = element.text().trim();
        expect(text).to.match(/^(Good|Not available|Outstanding|Requires improvement|Inadequate|Not inspected|Insufficient evidence|Does not apply)$/);
    };

    private readonly checkValueIsValidBeforeOrAfterJoiningTag = (element: JQuery<HTMLElement>) => {
        const text = element.text().replace(/\s+/g, ' ').trim();
        expect(text).to.match(/^(Before|After|Not yet inspected)$/);
    };

    // #endregion

    // #region All sub pages

    public clickDownloadButton(): this {
        this.elements.downloadButton().click();
        return this;
    }

    // #endregion

    // #region Single headline grades
    public checkOfstedSHGPageHeaderPresent(): this {
        this.elements.subpageHeader().should('contain', 'Overview');
        return this;
    }

    public checkOfstedSHGTableHeadersPresent(): this {
        this.elements.singleHeadlineGrades.schoolNameHeader().should('be.visible');
        this.elements.singleHeadlineGrades.dateJoinedHeader().should('be.visible');
        this.elements.singleHeadlineGrades.hasRecentShortInspectionHeader().should('be.visible');
        this.elements.singleHeadlineGrades.currentSHGHeader().should('be.visible');
        this.elements.singleHeadlineGrades.dateOfCurrentInspectionHeader().should('be.visible');
        this.elements.singleHeadlineGrades.previousSHGHeader().should('be.visible');
        this.elements.singleHeadlineGrades.dateOfPreviousInspectionHeader().should('be.visible');
        return this;
    }

    public checkOfstedSHGSorting(): this {
        TableUtility.checkStringSorting(
            this.elements.singleHeadlineGrades.schoolName,
            this.elements.singleHeadlineGrades.schoolNameHeader
        );
        TableUtility.checkStringSorting(
            this.elements.singleHeadlineGrades.dateJoined,
            this.elements.singleHeadlineGrades.dateJoinedHeader
        );
        TableUtility.checkStringSorting(
            this.elements.singleHeadlineGrades.hasRecentShortInspection,
            this.elements.singleHeadlineGrades.hasRecentShortInspectionHeader
        );
        TableUtility.checkStringSorting(
            this.elements.singleHeadlineGrades.currentSHG,
            this.elements.singleHeadlineGrades.currentSHGHeader
        );
        TableUtility.checkStringSorting(
            this.elements.singleHeadlineGrades.dateOfCurrentInspection,
            this.elements.singleHeadlineGrades.dateOfCurrentInspectionHeader
        );
        TableUtility.checkStringSorting(
            this.elements.singleHeadlineGrades.previousSHG,
            this.elements.singleHeadlineGrades.previousSHGHeader
        );
        TableUtility.checkStringSorting(
            this.elements.singleHeadlineGrades.dateOfPreviousInspection,
            this.elements.singleHeadlineGrades.dateOfPreviousInspectionHeader
        );
        return this;
    }

    public checkSHGDateJoinedPresent(): this {
        this.elements.singleHeadlineGrades.dateJoined().each(commonPage.checkValueIsValidDate);
        return this;
    }

    public checkSHGDateOfCurrentInspectionPresent(): this {
        this.elements.singleHeadlineGrades.dateOfCurrentInspection().each(commonPage.checkValueIsValidDate);
        return this;
    }

    public checkSHGDateOfPreviousInspectionPresent(): this {
        this.elements.singleHeadlineGrades.dateOfPreviousInspection().each(commonPage.checkValueIsValidDate);
        return this;
    }

    public checkSHGCurrentSHGJudgementsPresent(): this {
        this.elements.singleHeadlineGrades.currentSHG().each(this.checkValueIsValidCurrentOfstedRating);
        return this;
    }

    public checkSHGPreviousSHGJudgementsPresent(): this {
        this.elements.singleHeadlineGrades.previousSHG().each(this.checkValueIsValidPreviousOfstedRating);
        return this;
    }

    public checkSHGCurrentSHGBeforeOrAfterPresent(): this {
        this.elements.singleHeadlineGrades.currentSHGBeforeOrAfter().each((element: JQuery<HTMLElement>) => {
            const text = element.text().replace(/\s+/g, ' ').trim();
            expect(text).to.match(/^(Not yet inspected|After joining|Before joining)$/i);
        });
        return this;
    }

    public checkSHGPreviousSHGBeforeOrAfterPresent(): this {
        this.elements.singleHeadlineGrades.previousSHGBeforeOrAfter().each((element: JQuery<HTMLElement>) => {
            const text = element.text().replace(/\s+/g, ' ').trim();
            expect(text).to.match(/^(Not inspected|After joining|Before joining)$/i);
        });
        return this;
    }

    public checkSHGHasRecentShortInspectionPresent(): this {
        this.elements.singleHeadlineGrades.hasRecentShortInspection().each((element: JQuery<HTMLElement>) => {
            this.checkElementMatches(element, /^(Yes|No|Not available)$/);
        });
        return this;
    }

    // Details sections functionality - following school pattern
    public checkWhereToFindShortInspectionDataDetailsPresent(): this {
        this.elements.singleHeadlineGrades.whereToFindShortInspectionDataDetails().should('be.visible');
        return this;
    }

    public clickWhereToFindShortInspectionDataDetails(): this {
        this.elements.singleHeadlineGrades.whereToFindShortInspectionDataDetails().find('summary').click();
        return this;
    }

    public checkWhereToFindShortInspectionDataDetailsIsOpen(): this {
        this.elements.singleHeadlineGrades.whereToFindShortInspectionDataDetails().should('have.attr', 'open');
        return this;
    }

    public checkSchoolNamesAreCorrectLinksOnSingleHeadlineGradesPage(): this {
        TableUtility.checkSchoolNamesAreCorrectLinksOnPage(this.elements.singleHeadlineGrades, 'ofsted-single-headline-grades-school-name', /^\/schools\/ofsted\/singleheadlinegrades\?urn=\d+$/);
        return this;
    }

    // #endregion

    // #region Current ratings

    public checkOfstedCurrentRatingsPageHeaderPresent(): this {
        this.elements.subpageHeader().should('contain', 'Report cards');
        return this;
    }

    public checkOfstedCurrentRatingsTableHeadersPresent(): this {
        this.elements.reportCard.schoolNameHeader().should('be.visible');
        this.elements.reportCard.inclusionHeader().should('be.visible');
        this.elements.reportCard.curriculumAndTeachingHeader().should('be.visible');
        this.elements.reportCard.personalDevelopmentHeader().should('be.visible');
        this.elements.reportCard.leadershipAndGoveranceHeader().should('be.visible');
        this.elements.reportCard.achievementHeader().should('be.visible');
        this.elements.reportCard.attendanceAndBehaviourHeader().scrollIntoView();
        this.elements.reportCard.attendanceAndBehaviourHeader().should('be.visible');
        return this;
    }

    public checkOfstedCurrentRatingsSorting(): this {
        TableUtility.checkStringSorting(
            this.elements.reportCard.schoolName,
            this.elements.reportCard.schoolNameHeader
        );
        TableUtility.checkStringSorting(
            this.elements.reportCard.inclusion,
            this.elements.reportCard.inclusionHeader
        );
        TableUtility.checkStringSorting(
            this.elements.reportCard.curriculumAndTeaching,
            this.elements.reportCard.curriculumAndTeachingHeader
        );
        TableUtility.checkStringSorting(
            this.elements.reportCard.personalDevelopment,
            this.elements.reportCard.personalDevelopmentHeader
        );
        TableUtility.checkStringSorting(
            this.elements.reportCard.leadershipAndGoverance,
            this.elements.reportCard.leadershipAndGoveranceHeader
        );
        TableUtility.checkStringSorting(
            this.elements.reportCard.achievement,
            this.elements.reportCard.achievementHeader
        );
        TableUtility.checkStringSorting(
            this.elements.reportCard.attendanceAndBehaviour,
            this.elements.reportCard.attendanceAndBehaviourHeader
        );
        return this;
    }

    public checkCurrentRatingsInclusionJudgementsPresent(): this {
        this.elements.reportCard.inclusion().each(this.checkValueIsValidCurrentOfstedRating);
        return this;
    }

    public checkCurrentRatingsCurriculumAndTeachingJudgementsPresent(): this {
        this.elements.reportCard.curriculumAndTeaching().each(this.checkValueIsValidCurrentOfstedRating);
        return this;
    }

    public checkCurrentRatingsPersonalDevelopmentJudgementsPresent(): this {
        this.elements.reportCard.personalDevelopment().each(this.checkValueIsValidCurrentOfstedRating);
        return this;
    }

    public checkCurrentRatingsLeadershipAndGoveranceudgementsPresent(): this {
        this.elements.reportCard.leadershipAndGoverance().each(this.checkValueIsValidCurrentOfstedRating);
        return this;
    }

    public checkCurrentRatingsAchievementJudgementsPresent(): this {
        this.elements.reportCard.achievement().each(this.checkValueIsValidCurrentOfstedRating);
        return this;
    }

    public checkCurrentRatingsAttendanceAndBehaviourJudgementsPresent(): this {
        this.elements.reportCard.attendanceAndBehaviour().each(this.checkValueIsValidCurrentOfstedRating);
        return this;
    }

    public checkNoDataMessageIsVisible(): this {
        this.elements.reportCard.noDataMessage().should('be.visible');
        return this;
    }

    // #endregion
    // #region previous ratings

    public checkOfstedPreviousRatingsPageHeaderPresent(): this {
        this.elements.subpageHeader().should('contain', 'Report cards');
        return this;
    }

    public checkOfstedPreviousRatingsTableHeadersPresent(): this {
        this.elements.reportCard.schoolNameHeader().should('be.visible');
        this.elements.reportCard.inclusionHeader().should('be.visible');
        this.elements.reportCard.curriculumAndTeachingHeader().should('be.visible');
        this.elements.reportCard.personalDevelopmentHeader().should('be.visible');
        this.elements.reportCard.leadershipAndGoveranceHeader().should('be.visible');
        this.elements.reportCard.achievementHeader().should('be.visible');
        this.elements.reportCard.attendanceAndBehaviourHeader().scrollIntoView();
        this.elements.reportCard.attendanceAndBehaviourHeader().should('be.visible');
        return this;
    }

    public checkOfstedPreviousRatingsSorting(): this {
        TableUtility.checkStringSorting(
            this.elements.reportCard.schoolName,
            this.elements.reportCard.schoolNameHeader
        );
        TableUtility.checkStringSorting(
            this.elements.reportCard.inclusion,
            this.elements.reportCard.inclusionHeader
        );
        TableUtility.checkStringSorting(
            this.elements.reportCard.curriculumAndTeaching,
            this.elements.reportCard.curriculumAndTeachingHeader
        );
        TableUtility.checkStringSorting(
            this.elements.reportCard.personalDevelopment,
            this.elements.reportCard.personalDevelopmentHeader
        );
        TableUtility.checkStringSorting(
            this.elements.reportCard.leadershipAndGoverance,
            this.elements.reportCard.leadershipAndGoveranceHeader
        );
        TableUtility.checkStringSorting(
            this.elements.reportCard.achievement,
            this.elements.reportCard.achievementHeader
        );
        TableUtility.checkStringSorting(
            this.elements.reportCard.attendanceAndBehaviour,
            this.elements.reportCard.attendanceAndBehaviourHeader
        );
        return this;
    }

    public checkPreviousRatingsInclusionJudgementsPresent(): this {
        this.elements.reportCard.inclusion().each(this.checkValueIsValidPreviousOfstedRating);
        return this;
    }

    public checkPreviousRatingsCurriculumAndTeachingJudgementsPresent(): this {
        this.elements.reportCard.curriculumAndTeaching().each(this.checkValueIsValidPreviousOfstedRating);
        return this;
    }

    public checkPreviousRatingsPersonalDevelopmentJudgementsPresent(): this {
        this.elements.reportCard.personalDevelopment().each(this.checkValueIsValidPreviousOfstedRating);
        return this;
    }

    public checkPreviousRatingsLeadershipAndGoveranceJudgementsPresent(): this {
        this.elements.reportCard.leadershipAndGoverance().each(this.checkValueIsValidPreviousOfstedRating);
        return this;
    }

    public checkPreviousRatingsAchievementJudgementsPresent(): this {
        this.elements.reportCard.achievement().each(this.checkValueIsValidPreviousOfstedRating);
        return this;
    }

    public checkPreviousRatingsAttendanceAndBehaviourJudgementsPresent(): this {
        this.elements.reportCard.attendanceAndBehaviour().each(this.checkValueIsValidPreviousOfstedRating);
        return this;
    }

    // #endregion
    // #region Safeguarding and Concerns

    public checkOfstedSafeguardingConcernsPageHeaderPresent(): this {
        this.elements.subpageHeader().should('contain', 'Safeguarding and concerns');
        return this;
    }

    public checkOfstedSafeguardingConcernsTableHeadersPresent(): this {
        this.elements.safeguardingAndConcerns.schoolNameHeader().should('be.visible');
        this.elements.safeguardingAndConcerns.effectiveSafeguardingHeader().should('be.visible');
        this.elements.safeguardingAndConcerns.categoryOfConcern().should('be.visible');
        this.elements.safeguardingAndConcerns.beforeOrAfterJoining().should('be.visible');
        return this;
    }

    public checkOfstedSafeguardingConcernsSorting(): this {
        TableUtility.checkStringSorting(
            this.elements.safeguardingAndConcerns.schoolName,
            this.elements.safeguardingAndConcerns.schoolNameHeader
        );
        TableUtility.checkStringSorting(
            this.elements.safeguardingAndConcerns.effectiveSafeguarding,
            this.elements.safeguardingAndConcerns.effectiveSafeguardingHeader
        );
        TableUtility.checkStringSorting(
            this.elements.safeguardingAndConcerns.categoryOfConcern,
            this.elements.safeguardingAndConcerns.categoryOfConcernHeader
        );
        TableUtility.checkStringSorting(
            this.elements.safeguardingAndConcerns.beforeOrAfterJoining,
            this.elements.safeguardingAndConcerns.beforeOrAfterJoiningHeader
        );
        return this;
    }

    public checkSafeguardingConcernsEffectiveSafeguardingJudgementsPresent(): this {
        this.elements.safeguardingAndConcerns.effectiveSafeguarding().each((element) => {
            const text = element.text().trim();
            expect(text).to.match(/^(Yes|No|Not recorded|Not yet inspected)$/);
        });
        return this;
    }

    public checkSafeguardingConcernsCategoryOfConcernJudgementsPresent(): this {
        this.elements.safeguardingAndConcerns.categoryOfConcern().each((element) => {
            const text = element.text().trim();
            expect(text).to.match(/^(None|Special measures|Serious weakness|Notice to improve|Not yet inspected|Does not apply)$/);
        });
        return this;
    }

    public checkSafeguardingConcernsBeforeOrAfterJoiningJudgementsPresent(): this {
        this.elements.safeguardingAndConcerns.beforeOrAfterJoining().each(this.checkValueIsValidBeforeOrAfterJoiningTag);
        return this;
    }

    // #endregion
}

const ofstedPage = new OfstedPage();
export default ofstedPage;
