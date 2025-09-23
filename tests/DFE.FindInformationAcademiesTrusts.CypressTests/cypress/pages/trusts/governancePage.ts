import { TableUtility } from "../tableUtility";
class GovernancePage {

    elements = {
        trustLeadership: {
            section: () => cy.get('[data-testid="trust-leadership"]'),
            names: () => this.elements.trustLeadership.section().find('[data-testid="trust-leadership-name"]'),
            nameHeader: () => this.elements.trustLeadership.section().find("th:contains('Name')"),
            roles: () => this.elements.trustLeadership.section().find('[data-testid="trust-leadership-role"]'),
            roleHeader: () => this.elements.trustLeadership.section().find("th:contains('Role')"),
            from: () => this.elements.trustLeadership.section().find('[data-testid="trust-leadership-from"]'),
            fromHeader: () => this.elements.trustLeadership.section().find("th:contains('From')"),
            to: () => this.elements.trustLeadership.section().find('[data-testid="trust-leadership-to"]'),
            toHeader: () => this.elements.trustLeadership.section().find("th:contains('To')"),
            tableRows: () => this.elements.trustLeadership.section().find('tbody tr'),
            noDataMessage: () => this.elements.trustLeadership.section().contains('No Trust Leadership'),
        },
        trustees: {
            section: () => cy.get('[data-testid="trustees"]'),
            names: () => this.elements.trustees.section().find('[data-testid="trustees-name"]'),
            nameHeader: () => this.elements.trustees.section().find("th:contains('Name')"),
            appointedBy: () => this.elements.trustees.section().find('[data-testid="trustees-appointed"]'),
            appointedHeader: () => this.elements.trustees.section().find("th:contains('Appointed by')"),
            from: () => this.elements.trustees.section().find('[data-testid="trustees-from"]'),
            fromHeader: () => this.elements.trustees.section().find("th:contains('From')"),
            to: () => this.elements.trustees.section().find('[data-testid="trustees-to"]'),
            toHeader: () => this.elements.trustees.section().find("th:contains('To')"),
            tableRows: () => this.elements.trustees.section().find('tbody tr'),
            noDataMessage: () => this.elements.trustees.section().contains('No Trustees'),
        },
        members: {
            section: () => cy.get('[data-testid="members"]'),
            names: () => this.elements.members.section().find('[data-testid="members-name"]'),
            nameHeader: () => this.elements.members.section().find("th:contains('Name')"),
            appointedBy: () => this.elements.members.section().find('[data-testid="members-appointed"]'),
            appointedHeader: () => this.elements.members.section().find("th:contains('Appointed by')"),
            from: () => this.elements.members.section().find('[data-testid="members-from"]'),
            fromHeader: () => this.elements.members.section().find("th:contains('From')"),
            to: () => this.elements.members.section().find('[data-testid="members-to"]'),
            toHeader: () => this.elements.members.section().find("th:contains('To')"),
            tableRows: () => this.elements.members.section().find('tbody tr'),
            noDataMessage: () => this.elements.members.section().contains('No Members'),

        },
        historicMembers: {
            section: () => cy.get('[data-testid="historic-members"]'),
            names: () => this.elements.historicMembers.section().find('[data-testid="historic-members-name"]'),
            nameHeader: () => this.elements.historicMembers.section().find("th:contains('Name')"),
            roles: () => this.elements.historicMembers.section().find('[data-testid="historic-members-role"]'),
            roleHeader: () => this.elements.historicMembers.section().find("th:contains('Role')"),
            appointedBy: () => this.elements.historicMembers.section().find('[data-testid="historic-members-appointed"]'),
            appointedHeader: () => this.elements.historicMembers.section().find("th:contains('Appointed by')"),
            from: () => this.elements.historicMembers.section().find('[data-testid="historic-members-from"]'),
            fromHeader: () => this.elements.historicMembers.section().find("th:contains('From')"),
            to: () => this.elements.historicMembers.section().find('[data-testid="historic-members-to"]'),
            toHeader: () => this.elements.historicMembers.section().find("th:contains('To')"),
            tableRows: () => this.elements.historicMembers.section().find('tbody tr'),
            noDataMessage: () => this.elements.historicMembers.section().contains('No Historic Members'),
        },
        subNav: {
            trustLeadershipSubnavButton: () => cy.get('[data-testid="governance-trust-leadership-subnav"]'),
            trusteesSubnavButton: () => cy.get('[data-testid="governance-trustees-subnav"]'),
            membersSubnavButton: () => cy.get('[data-testid="governance-members-subnav"]'),
            historicMembersSubnavButton: () => cy.get('[data-testid="governance-historic-members-subnav"]')
        },
        subHeaders: {
            subHeader: () => cy.get('[data-testid="subpage-header"]')
        },
        turnover: {
            rateHeadline: () => cy.get('[data-testid="turnover-rate-headline"]'),
            rateSummary: () => cy.get('[data-testid="turnover-rate-summary"]'),
            calculationExplanation: () => cy.get('[data-testid="turnover-calculation-explanation"]'),
        }
    };

    // *************
    //
    // HEADER CHECKS
    //
    // *************

    public checkTrustLeadershipSubHeaderPresent() {
        this.elements.subHeaders.subHeader().should('be.visible');
        this.elements.subHeaders.subHeader().should('contain', 'Trust Leadership');
        return this;
    }

    public checkTrusteesSubHeaderPresent() {
        this.elements.subHeaders.subHeader().should('be.visible');
        this.elements.subHeaders.subHeader().should('contain', 'Trustees');
        return this;
    }

    public checkMembersSubHeaderPresent() {
        this.elements.subHeaders.subHeader().should('be.visible');
        this.elements.subHeaders.subHeader().should('contain', 'Members');
        return this;
    }

    public checkHistoricMembersSubHeaderPresent() {
        this.elements.subHeaders.subHeader().should('be.visible');
        this.elements.subHeaders.subHeader().should('contain', 'Historic members');
        return this;
    }

    public checkTrustLeadershipTableHeadersAreVisible() {
        this.elements.trustLeadership.nameHeader().should('be.visible');
        this.elements.trustLeadership.roleHeader().should('be.visible');
        this.elements.trustLeadership.fromHeader().should('be.visible');
        this.elements.trustLeadership.toHeader().should('be.visible');
        return this;
    }

    public checkTrusteesTableHeadersAreVisible() {
        this.elements.trustees.nameHeader().should('be.visible');
        this.elements.trustees.appointedHeader().should('be.visible');
        this.elements.trustees.fromHeader().should('be.visible');
        this.elements.trustees.toHeader().should('be.visible');
        return this;
    }

    public checkMembersTableHeadersAreVisible() {
        this.elements.members.nameHeader().should('be.visible');
        this.elements.members.appointedHeader().should('be.visible');
        this.elements.members.fromHeader().should('be.visible');
        this.elements.members.toHeader().should('be.visible');
        return this;
    }

    public checkHistoricMembersTableHeadersAreVisible() {
        this.elements.historicMembers.nameHeader().should('be.visible');
        this.elements.historicMembers.roleHeader().should('be.visible');
        this.elements.historicMembers.appointedHeader().should('be.visible');
        this.elements.historicMembers.fromHeader().should('be.visible');
        this.elements.historicMembers.toHeader().should('be.visible');
        return this;
    }

    // **************
    //
    // SORTING CHECKS
    //
    // **************

    public checkTrustLeadershipColumnsSortCorrectly() {
        const { trustLeadership } = this.elements;
        TableUtility.checkStringSorting(trustLeadership.names, trustLeadership.nameHeader);
        TableUtility.checkStringSorting(trustLeadership.roles, trustLeadership.roleHeader);
        TableUtility.checkStringSorting(trustLeadership.from, trustLeadership.fromHeader);
        TableUtility.checkStringSorting(trustLeadership.to, trustLeadership.toHeader);
    }

    public checkTrusteesColumnsSortCorrectly() {
        const { trustees } = this.elements;
        TableUtility.checkStringSorting(trustees.names, trustees.nameHeader);
        TableUtility.checkStringSorting(trustees.appointedBy, trustees.appointedHeader);
        TableUtility.checkStringSorting(trustees.from, trustees.fromHeader);
        TableUtility.checkStringSorting(trustees.to, trustees.toHeader);
    }

    public checkMembersColumnsSortCorrectly() {
        const { members } = this.elements;
        TableUtility.checkStringSorting(members.names, members.nameHeader);
        TableUtility.checkStringSorting(members.appointedBy, members.appointedHeader);
        TableUtility.checkStringSorting(members.from, members.fromHeader);
        TableUtility.checkStringSorting(members.to, members.toHeader);
    }

    public checkHistoricMembersColumnsSortCorrectly() {
        const { historicMembers } = this.elements;
        TableUtility.checkStringSorting(historicMembers.names, historicMembers.nameHeader);
        TableUtility.checkStringSorting(historicMembers.roles, historicMembers.roleHeader);
        TableUtility.checkStringSorting(historicMembers.appointedBy, historicMembers.appointedHeader);
        TableUtility.checkStringSorting(historicMembers.from, historicMembers.fromHeader);
        TableUtility.checkStringSorting(historicMembers.to, historicMembers.toHeader);
    }

    // ***********
    //
    // DATE CHECKS
    //
    // ***********

    public checkTrustLeadershipAppointmentDatesAreCurrent(): this {
        governancePage.elements.trustLeadership.from().each(cell => { TableUtility.checkCellDateIsBeforeTodayOrHasNoData(cell); });
        governancePage.elements.trustLeadership.to().each(cell => { TableUtility.checkCellDateIsOnOrAfterTodayOrHasNoData(cell); });
        return this;
    }

    public checkTrusteesAppointmentDatesAreCurrent(): this {
        governancePage.elements.trustees.from().each(cell => { TableUtility.checkCellDateIsBeforeTodayOrHasNoData(cell); });
        governancePage.elements.trustees.to().each(cell => { TableUtility.checkCellDateIsOnOrAfterTodayOrHasNoData(cell); });
        return this;
    }

    public checkMembersAppointmentDatesAreCurrent(): this {
        governancePage.elements.members.from().each(cell => { TableUtility.checkCellDateIsBeforeTodayOrHasNoData(cell); });
        governancePage.elements.members.to().each(cell => { TableUtility.checkCellDateIsOnOrAfterTodayOrHasNoData(cell); });
        return this;
    }

    public checkHistoricMembersAppointmentDatesAreInThePast(): this {
        governancePage.elements.historicMembers.from().each(cell => { TableUtility.checkCellDateIsBeforeTodayOrHasNoData(cell); });
        governancePage.elements.historicMembers.to().each(cell => { TableUtility.checkCellDateIsBeforeTodayOrHasNoData(cell); });
        return this;
    }

    // **********************
    //
    // NO DATA MESSAGE CHECKS
    //
    // **********************

    public checkNoTrustLeadershipMessageIsVisible(): this {
        this.elements.trustLeadership.noDataMessage().should('be.visible');
        return this;
    }

    public checkNoTrusteesMessageIsVisible(): this {
        this.elements.trustees.noDataMessage().should('be.visible');
        return this;
    }

    public checkNotMembersMessageIsVisible(): this {
        this.elements.members.noDataMessage().should('be.visible');
        return this;
    }

    public checkNoHistoricMembersMessageIsVisible(): this {
        this.elements.historicMembers.noDataMessage().should('be.visible');
        return this;
    }

    public checkNoTrustLeadershipMessageIsHidden(): this {
        this.elements.trustLeadership.noDataMessage().should('not.exist');
        return this;
    }

    public checkNoTrusteesMessageIsHidden(): this {
        this.elements.trustees.noDataMessage().should('not.exist');
        return this;
    }

    public checkNoMembersMessageIsHidden(): this {
        this.elements.members.noDataMessage().should('not.exist');
        return this;
    }

    public checkNoHistoricMembersMessageIsHidden(): this {
        this.elements.historicMembers.noDataMessage().should('not.exist');
        return this;
    }

    // ***********
    //
    // ROLE CHECKS
    //
    // ***********

    public checkOnlyTrustLeadershipRolesArePresent() {
        this.elements.trustLeadership.roles().each(element => {
            expect(element.text().trim()).to.be.oneOf(["Chief Financial Officer", "Accounting Officer", "Chair of Trustees"]);
        });
        return this;
    }
    
    // ***********
    //
    // TURNOVER CHECKS
    //
    // ***********
    
    public checkTurnoverRateIsPresent(): this {
        this.elements.turnover.rateHeadline().should('be.visible');
        return this;
    }
    
    public checkTurnoverRateSummaryIsPresent(): this {
        this.elements.turnover.rateSummary().should('be.visible');
        return this;
    }
    public checkTurnoverCalculationExplanationIsPresent(): this {
        this.elements.turnover.calculationExplanation().should('be.visible');
        return this;
    }

    public checkTurnoverCalculationExplanationDetails(): this {
        // Expand the details element so we can see its contents on any screenshots if this fails
        this.elements.turnover.calculationExplanation().expandDetailsElement();

        // Check for the presence of each line of text
        const expectedTexts = [
            "Governance turnover % is calculated based on the total number of appointments and resignations in the past calendar year, divided by the total number of current governors.",
            "This calculation includes:",
            "Chair of Trustees",
            "Trustees",
            "This calculation does not include:",
            "Accounting Officer",
            "Chief Financial Officer",
            "Members",
        ];

        expectedTexts.forEach(text => {
            this.elements.turnover.calculationExplanation().contains(text).should('exist');
        });

        return this;
    }

    // ***********
    //
    // SUB NAVIGATION MENU
    //
    // ***********

    public clickTrustLeadershipSubnavButton(): this {
        this.elements.subNav.trustLeadershipSubnavButton().click();
        return this;
    }

    public clickTrusteesSubnavButton(): this {
        this.elements.subNav.trusteesSubnavButton().click();
        return this;
    }

    public clickMembersSubnavButton(): this {
        this.elements.subNav.membersSubnavButton().click();
        return this;
    }

    public clickHistoricMembersSubnavButton(): this {
        this.elements.subNav.historicMembersSubnavButton().click();
        return this;
    }

    public checkAllSubNavItemsPresent(): this {
        this.elements.subNav.trustLeadershipSubnavButton().should('be.visible');
        this.elements.subNav.trusteesSubnavButton().should('be.visible');
        this.elements.subNav.membersSubnavButton().should('be.visible');
        this.elements.subNav.historicMembersSubnavButton().should('be.visible');
        return this;
    }

    public checkSubNavNotPresent(): this {
        this.elements.subNav.trustLeadershipSubnavButton().should('not.exist');
        this.elements.subNav.trusteesSubnavButton().should('not.exist');
        this.elements.subNav.membersSubnavButton().should('not.exist');
        this.elements.subNav.historicMembersSubnavButton().should('not.exist');
        return this;
    }

    public checkTrustLeadershipSubnavButtonIsHighlighted(): this {
        this.elements.subNav.trustLeadershipSubnavButton().should('have.prop', 'aria-current', true);
        return this;
    }

    public checkTrusteesSubnavButtonIsHighlighted(): this {
        this.elements.subNav.trusteesSubnavButton().should('have.prop', 'aria-current', true);
        return this;
    }

    public checkMembersSubnavButtonIsHighlighted(): this {
        this.elements.subNav.membersSubnavButton().should('have.prop', 'aria-current', true);
        return this;
    }

    public checkHistoricMembersSubnavButtonIsHighlighted(): this {
        this.elements.subNav.historicMembersSubnavButton().should('have.prop', 'aria-current', true);
        return this;
    }

    private getCountFromSubNavButton(subNavButton: Cypress.Chainable<JQuery<HTMLElement>>): Cypress.Chainable<number> {
        return subNavButton
            .invoke('text')
            .then(text => {
                const matches = /\d+/.exec(text);
                if (!matches)
                    throw new Error("No count found in button text.");
                return parseInt(matches[0]);
            });
    }

    public checkTrustLeadershipSubnavButtonHasZeroInBrackets(): this {
        this.getCountFromSubNavButton(this.elements.subNav.trustLeadershipSubnavButton()).should('eq', 0);
        return this;
    }

    public checkTrusteesSubnavButtonHasZeroInBrackets(): this {
        this.getCountFromSubNavButton(this.elements.subNav.trusteesSubnavButton()).should('eq', 0);
        return this;
    }

    public checkMembersSubnavButtonHasZeroInBrackets(): this {
        this.getCountFromSubNavButton(this.elements.subNav.membersSubnavButton()).should('eq', 0);
        return this;
    }

    public checkHistoricMembersSubnavButtonHasZeroInBrackets(): this {
        this.getCountFromSubNavButton(this.elements.subNav.historicMembersSubnavButton()).should('eq', 0);
        return this;
    }

    private checkButtonCountMatchesActualNumberOfGovernors(subNavButton: Cypress.Chainable<JQuery<HTMLElement>>, tableRows: Cypress.Chainable<JQuery<HTMLElement>>) {
        tableRows.its('length')
            .then((numberOfGovernors) => {
                //Ensure that the table has some data
                expect(numberOfGovernors).to.be.greaterThan(0, 'Check that there are governors because this test requires a trust with full governance data');

                this.getCountFromSubNavButton(subNavButton)
                    .then((buttonCount) => {
                        expect(buttonCount).to.eq(numberOfGovernors, 'Check that number of governors is the same as the number in the sub nav link');
                    });
            });
    }

    public checkTrustLeadershipLinkValueMatchesNumberOfTrustLeaders(): this {
        this.checkButtonCountMatchesActualNumberOfGovernors(this.elements.subNav.trustLeadershipSubnavButton(), this.elements.trustLeadership.tableRows());
        return this;
    }

    public checkTrusteesLinkValueMatchesNumberOfTrustees(): this {
        this.checkButtonCountMatchesActualNumberOfGovernors(this.elements.subNav.trusteesSubnavButton(), this.elements.trustees.tableRows());
        return this;
    }

    public checkMembersLinkValueMatchesNumberOfMembers(): this {
        this.checkButtonCountMatchesActualNumberOfGovernors(this.elements.subNav.membersSubnavButton(), this.elements.members.tableRows());
        return this;
    }

    public checkHistoricMembersLinkValueMatchesNumberOfHistoricMembers(): this {
        this.checkButtonCountMatchesActualNumberOfGovernors(this.elements.subNav.historicMembersSubnavButton(), this.elements.historicMembers.tableRows());
        return this;
    }
}

const governancePage = new GovernancePage();
export default governancePage;
