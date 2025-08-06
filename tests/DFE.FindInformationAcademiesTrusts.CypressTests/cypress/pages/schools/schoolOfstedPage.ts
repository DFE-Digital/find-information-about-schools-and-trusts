import commonPage from "../commonPage";

class SchoolOfstedPage {
    elements = {
        subpageHeader: () => cy.get('[data-testid="subpage-header"]'),
        downloadButton: () => cy.get('[data-testid="download-all-ofsted-data-button"]'),
        singleHeadlineGrades: {
            table: () => cy.get('[data-testid="ofsted-inspection-table"]'),
            joinedTrustText: () => cy.get('[data-testid="academy-joined-trust-text"]'),
            dateOfInspectionHeader: () => cy.get('[data-testid="date-of-inspection-header"]'),
            gradeHeader: () => cy.get('[data-testid="grade-header"]'),
            // Recent short inspection (conditional)
            recentShortInspectionRow: () => cy.get('[data-testid="recent-short-inspection-row"]'),
            recentShortInspectionDate: () => cy.get('[data-testid="recent-short-inspection-date"]'),
            recentShortInspectionGrade: () => cy.get('[data-testid="recent-short-inspection-grade"]'),
            // Current full inspection
            currentFullInspectionRow: () => cy.get('[data-testid="current-full-inspection-row"]'),
            currentFullInspectionDate: () => cy.get('[data-testid="current-full-inspection-date"]'),
            currentFullInspectionGrade: () => cy.get('[data-testid="current-full-inspection-grade"]'),
            // Previous full inspection
            previousFullInspectionRow: () => cy.get('[data-testid="previous-full-inspection-row"]'),
            previousFullInspectionDate: () => cy.get('[data-testid="previous-full-inspection-date"]'),
            previousFullInspectionGrade: () => cy.get('[data-testid="previous-full-inspection-grade"]'),
            // Details sections
            whySingleHeadlineNotAvailableDetails: () => cy.get('[data-testid="why-single-headline-not-available-details"]'),
            whyShortInspectionNotAvailableDetails: () => cy.get('[data-testid="why-short-inspection-not-available-details"]'),
            // Links
            inspectionReportsLink: () => cy.get('[data-testid="ofsted-inspection-reports-link"]'),
        }
    };

    private readonly checkElementMatches = (element: JQuery<HTMLElement>, expected: RegExp) => {
        const text = element.text().trim();
        expect(text).to.match(expected);
    };

    private readonly checkValueIsValidOfstedGrade = (element: JQuery<HTMLElement>) =>
        this.checkElementMatches(element, /^(School remains Good|Outstanding|Good|Requires improvement|Inadequate|Not yet inspected|Not inspected|Not available)$/);

    // Page header checks
    public checkOfstedSingleHeadlineGradesPageHeaderPresent(): this {
        this.elements.subpageHeader().should('contain', 'Single headline grades');
        return this;
    }

    // Table header checks
    public checkSingleHeadlineGradesTableHeadersPresent(): this {
        this.elements.singleHeadlineGrades.dateOfInspectionHeader().should('be.visible');
        this.elements.singleHeadlineGrades.gradeHeader().should('be.visible');
        return this;
    }

    // Academy joined trust text check
    public checkJoinedTrustTextPresent(): this {
        this.elements.singleHeadlineGrades.joinedTrustText().should('be.visible');
        return this;
    }

    // Recent short inspection checks (conditional)
    public checkRecentShortInspectionPresent(): this {
        this.elements.singleHeadlineGrades.recentShortInspectionRow().should('be.visible');
        return this;
    }

    public checkRecentShortInspectionNotPresent(): this {
        this.elements.singleHeadlineGrades.recentShortInspectionRow().should('not.exist');
        return this;
    }

    public checkRecentShortInspectionData(): this {
        this.elements.singleHeadlineGrades.recentShortInspectionDate().should('be.visible');
        this.elements.singleHeadlineGrades.recentShortInspectionGrade().should('be.visible');
        return this;
    }

    public checkRecentShortInspectionGradeValid(): this {
        this.elements.singleHeadlineGrades.recentShortInspectionGrade().each(this.checkValueIsValidOfstedGrade);
        return this;
    }

    // Current full inspection checks
    public checkCurrentFullInspectionPresent(): this {
        this.elements.singleHeadlineGrades.currentFullInspectionRow().should('be.visible');
        return this;
    }

    public checkCurrentFullInspectionData(): this {
        this.elements.singleHeadlineGrades.currentFullInspectionDate().should('be.visible');
        this.elements.singleHeadlineGrades.currentFullInspectionGrade().should('be.visible');
        return this;
    }

    public checkCurrentFullInspectionGradeValid(): this {
        this.elements.singleHeadlineGrades.currentFullInspectionGrade().each(this.checkValueIsValidOfstedGrade);
        return this;
    }

    // Previous full inspection checks
    public checkPreviousFullInspectionPresent(): this {
        this.elements.singleHeadlineGrades.previousFullInspectionRow().should('be.visible');
        return this;
    }

    public checkPreviousFullInspectionData(): this {
        this.elements.singleHeadlineGrades.previousFullInspectionDate().should('be.visible');
        this.elements.singleHeadlineGrades.previousFullInspectionGrade().should('be.visible');
        return this;
    }

    public checkPreviousFullInspectionGradeValid(): this {
        this.elements.singleHeadlineGrades.previousFullInspectionGrade().each(this.checkValueIsValidOfstedGrade);
        return this;
    }

    // Details sections checks
    public checkWhySingleHeadlineNotAvailableDetailsPresent(): this {
        this.elements.singleHeadlineGrades.whySingleHeadlineNotAvailableDetails().should('be.visible');
        return this;
    }

    public clickWhySingleHeadlineNotAvailableDetails(): this {
        this.elements.singleHeadlineGrades.whySingleHeadlineNotAvailableDetails().click();
        return this;
    }

    public checkWhyShortInspectionNotAvailableDetailsPresent(): this {
        this.elements.singleHeadlineGrades.whyShortInspectionNotAvailableDetails().should('be.visible');
        return this;
    }

    public clickWhyShortInspectionNotAvailableDetails(): this {
        this.elements.singleHeadlineGrades.whyShortInspectionNotAvailableDetails().click();
        return this;
    }

    public checkWhySingleHeadlineDetailsIsOpen(): this {
        this.elements.singleHeadlineGrades.whySingleHeadlineNotAvailableDetails().should('have.attr', 'open');
        return this;
    }

    public checkWhyShortInspectionDetailsIsOpen(): this {
        this.elements.singleHeadlineGrades.whyShortInspectionNotAvailableDetails().should('have.attr', 'open');
        return this;
    }

    // Inspection reports link check
    public checkInspectionReportsLinkPresent(): this {
        this.elements.singleHeadlineGrades.inspectionReportsLink().should('be.visible');
        return this;
    }

    public checkInspectionReportsLinkValid(): this {
        this.elements.singleHeadlineGrades.inspectionReportsLink()
            .should('have.attr', 'href')
            .and('match', /^https:\/\/reports\.ofsted\.gov\.uk/);
        return this;
    }

    // Date validation methods
    public checkCurrentFullInspectionDateValid(): this {
        this.elements.singleHeadlineGrades.currentFullInspectionDate().each(commonPage.checkValueIsValidFullDate);
        return this;
    }

    public checkPreviousFullInspectionDateValid(): this {
        this.elements.singleHeadlineGrades.previousFullInspectionDate().each(commonPage.checkValueIsValidFullDate);
        return this;
    }

    public checkRecentShortInspectionDateValid(): this {
        this.elements.singleHeadlineGrades.recentShortInspectionDate().each(commonPage.checkValueIsValidFullDate);
        return this;
    }

    // Conditional checks for data-dependent elements
    public checkRecentShortInspectionGradeIfExists(): this {
        cy.get('body').then(($body) => {
            if ($body.find('[data-testid="recent-short-inspection-row"]').length > 0) {
                this.checkRecentShortInspectionGradeValid();
            }
        });
        return this;
    }

    public checkRecentShortInspectionDateIfExists(): this {
        cy.get('body').then(($body) => {
            if ($body.find('[data-testid="recent-short-inspection-row"]').length > 0) {
                this.checkRecentShortInspectionDateValid();
            }
        });
        return this;
    }

    // Download button - following same pattern as trust Ofsted page
    public clickDownloadButton(): this {
        this.elements.downloadButton().click();
        return this;
    }
}

const schoolOfstedPage = new SchoolOfstedPage();
export default schoolOfstedPage; 
