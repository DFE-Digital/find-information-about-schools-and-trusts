import schoolsPage from '../../../../pages/schools/schoolsPage';
import navigation from '../../../../pages/navigation';
import { schoolsWithGovernanceData, schoolsWithNoGovernanceData } from '../../../../support/test-data-store';

describe('Testing the School Governance pages', () => {

    describe('Core Governance page functionality', () => {
        schoolsWithGovernanceData.forEach(({ typeOfSchool, urn }) => {
            beforeEach(() => {
                cy.visit(`/schools/governance/current?urn=${urn}`);
            });

            it(`Checks the page name is correct for ${typeOfSchool}`, () => {
                schoolsPage
                    .checkGovernancePageNamePresent();
            });

            it(`Checks all service navigation items are present for ${typeOfSchool}`, () => {
                navigation
                    .checkAllSchoolServiceNavItemsPresent();
            });

            it(`Checks all governance sub-navigation items are present for ${typeOfSchool}`, () => {
                navigation
                    .checkSchoolsGovernanceSubNavItemsPresent();
            });
        });
    });

    describe('Current governors page functionality', () => {
        schoolsWithGovernanceData.forEach(({ typeOfSchool, urn }) => {
            beforeEach(() => {
                cy.visit(`/schools/governance/current?urn=${urn}`);
            });

            it(`Checks current governors header and data are present for ${typeOfSchool}`, () => {
                schoolsPage
                    .checkCurrentGovernorsHeaderPresent()
                    .checkCurrentGovernorsSectionPresent()
                    .checkCurrentGovernorsTablePresent()
                    .checkCurrentGovernorsDataPresent();
            });

            it(`Checks current governors tab is highlighted and count is accurate for ${typeOfSchool}`, () => {
                navigation
                    .checkCurrentGovernorsSubnavButtonIsHighlighted();
                schoolsPage
                    .checkCurrentGovernorsTabCountMatches();
            });
        });
    });

    describe('Historic governors page functionality', () => {
        schoolsWithGovernanceData.forEach(({ typeOfSchool, urn }) => {
            beforeEach(() => {
                cy.visit(`/schools/governance/historic?urn=${urn}`);
            });

            it(`Checks historic governors header and data are present for ${typeOfSchool}`, () => {
                schoolsPage
                    .checkHistoricGovernorsHeaderPresent()
                    .checkHistoricGovernorsSectionPresent()
                    .checkHistoricGovernorsTablePresent()
                    .checkHistoricGovernorsDataPresent();
            });

            it(`Checks historic governors tab is highlighted and count is accurate for ${typeOfSchool}`, () => {
                navigation
                    .checkHistoricGovernorsSubnavButtonIsHighlighted();
                schoolsPage
                    .checkHistoricGovernorsTabCountMatches();
            });
        });
    });

    describe('Governance data validation', () => {
        schoolsWithGovernanceData.forEach(({ typeOfSchool, urn }) => {
            it(`Validates table structure and data integrity for ${typeOfSchool}`, () => {
                cy.visit(`/schools/governance/current?urn=${urn}`);
                schoolsPage
                    .verifyGovernorsTableStructure('current')
                    .verifyGovernorsDataIntegrity('current');

                cy.visit(`/schools/governance/historic?urn=${urn}`);
                schoolsPage
                    .verifyGovernorsTableStructure('historic')
                    .verifyGovernorsDataIntegrity('historic');
            });

            it(`Validates tab counts are displayed in brackets for ${typeOfSchool}`, () => {
                cy.visit(`/schools/governance/current?urn=${urn}`);
                schoolsPage
                    .checkGovernanceTabCountsDisplayed();
            });
        });
    });

    describe('Empty state testing', () => {
        schoolsWithNoGovernanceData.forEach(({ typeOfSchool, urn }) => {
            it(`Shows appropriate message when no current governors exist for ${typeOfSchool}`, () => {
                cy.visit(`/schools/governance/current?urn=${urn}`, { failOnStatusCode: false });

                // Check if page loads successfully or shows 404
                cy.url().should('include', '/schools/governance/current');
                schoolsPage.checkNoCurrentGovernorsMessagePresent();
            });

            it(`Shows appropriate message when no historic governors exist for ${typeOfSchool}`, () => {
                cy.visit(`/schools/governance/historic?urn=${urn}`, { failOnStatusCode: false });

                // Check if page loads successfully or shows 404  
                cy.url().should('include', '/schools/governance/historic');
                schoolsPage.checkNoHistoricGovernorsMessagePresent();
            });
        });
    });
});
