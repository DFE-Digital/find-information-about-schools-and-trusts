import commonPage from "../commonPage";

class SchoolOfstedRatingsPage {

    elements = {
        subpageHeader: () => cy.get('[data-testid="subpage-header"]'),
        downloadButton: () => cy.get('[data-testid="download-all-ofsted-data-button"]'),
        oftsedRatings: {
            table: () => cy.get('[data-testid="ofsted-inspection-table"]'),
            beforeOrAfterJoiningRow: () => cy.get('[data-testid="before-or-after-joining"]'),
            qualityOfEducationRow: () => cy.get('[data-testid="quality-of-education-row"]'),
            behaviourAndAttitudesRow: () => cy.get('[data-testid="behaviour-and-attitudes-row"]'),
            personalDevelopmentRow: () => cy.get('[data-testid="personal-development-row"]'),
            leadershipAndManagementRow: () => cy.get('[data-testid="leadership-and-management-row"]'),
            earlyYearsProvisionRow: () => cy.get('[data-testid="early-years-provision-row"]'),
            sixthFormProvisionRow: () => cy.get('[data-testid="sixth-form-provision-row"]'),
        }
    };

    public checkOfstedCurrentRatingsPageHeaderPresent(): this {
        this.elements.subpageHeader().should('contain', 'Current ratings');
        return this;
    }

    public checkOfstedPreviousRatingsPageHeaderPresent(): this {
        this.elements.subpageHeader().should('contain', 'Previous ratings');
        return this;
    }

    public checkQualityOfEducationSectionPresent(): this {
        this.elements.oftsedRatings.qualityOfEducationRow().should('be.visible');
        return this;
    }

    public checkBeforeOrAfterJoiningRowPresent(): this {
        this.elements.oftsedRatings.beforeOrAfterJoiningRow().should('be.visible');
        return this;
    }

    public checkBehaviourAndAttitudesSectionPresent(): this {
        this.elements.oftsedRatings.behaviourAndAttitudesRow().should('be.visible');
        return this;
    }

    public checkPersonalDevelopmentSectionPresent(): this {
        this.elements.oftsedRatings.personalDevelopmentRow().should('be.visible');
        return this;
    }

    public checkSixthFormProvisionSectionPresent(): this {
        this.elements.oftsedRatings.sixthFormProvisionRow().should('be.visible');
        return this;
    }
    public checkEarlyYearsProvisionSectionPresent(): this {
        this.elements.oftsedRatings.earlyYearsProvisionRow().should('be.visible');
        return this;
    }

    public checkLeadershipAndManagementSectionPresent(): this {
        this.elements.oftsedRatings.leadershipAndManagementRow().should('be.visible');
        return this;
    }
}



const schoolOfstedRatingsPage = new SchoolOfstedRatingsPage();
export default schoolOfstedRatingsPage; 
