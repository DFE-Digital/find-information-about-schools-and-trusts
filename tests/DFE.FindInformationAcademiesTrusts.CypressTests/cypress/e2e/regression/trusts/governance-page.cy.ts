import commonPage from "../../../pages/commonPage";
import navigation from "../../../pages/navigation";
import governancePage from "../../../pages/trusts/governancePage";
import { trustsWithGovernanceData, trustsWithNoGovernanceData } from "../../../support/test-data-store";

describe("Testing the components of the Governance page", () => {

    trustsWithGovernanceData.forEach(({ typeOfTrust, uid }) => {
        describe(`On the Governance pages for a ${typeOfTrust}`, () => {

            it("The trust leadership page loads with the correct headings and data", () => {
                cy.visit(`/trusts/governance/trust-leadership?uid=${uid}&referencenumber=tr04032`);

                commonPage
                    .checkThatBrowserTitleForTrustPageMatches('Trust leadership - Governance - {trustName} - Find information about schools and trusts');

                // Trust leadership table is visible and working
                governancePage
                    .checkTrustLeadershipSubHeaderPresent()
                    .checkTrustLeadershipTableHeadersAreVisible()
                    .checkTrustLeadershipAppointmentDatesAreCurrent()
                    .checkOnlyTrustLeadershipRolesArePresent();

                // "No Trust Leadership" message is hidden
                governancePage
                    .checkNoTrustLeadershipMessageIsHidden();

                navigation
                    .checkPageNameBreadcrumbPresent("Governance");
            });

            it("The trustees page loads with the correct headings and data", () => {
                cy.visit(`/trusts/governance/trustees?uid=${uid}&referencenumber=tr04032`);

                commonPage
                    .checkThatBrowserTitleForTrustPageMatches('Trustees - Governance - {trustName} - Find information about schools and trusts');

                // Trustee table is visible and working
                governancePage
                    .checkTrusteesSubHeaderPresent()
                    .checkTrusteesTableHeadersAreVisible()
                    .checkTrusteesAppointmentDatesAreCurrent();

                // "No Trustees" message is hidden
                governancePage
                    .checkNoTrusteesMessageIsHidden();

                navigation
                    .checkPageNameBreadcrumbPresent("Governance");
            });

            it("The members page loads with the correct headings and data", () => {
                cy.visit(`/trusts/governance/members?uid=${uid}&referencenumber=tr04032`);

                commonPage
                    .checkThatBrowserTitleForTrustPageMatches('Members - Governance - {trustName} - Find information about schools and trusts');

                // Members table is visible and working
                governancePage
                    .checkMembersSubHeaderPresent()
                    .checkMembersTableHeadersAreVisible()
                    .checkMembersAppointmentDatesAreCurrent();

                // "No Members" message is hidden
                governancePage
                    .checkNoMembersMessageIsHidden();

                navigation
                    .checkPageNameBreadcrumbPresent("Governance");
            });

            it("The historic members page loads with the correct headings and data", () => {
                cy.visit(`/trusts/governance/historic-members?uid=${uid}&referencenumber=tr04032`);

                commonPage
                    .checkThatBrowserTitleForTrustPageMatches('Historic members - Governance - {trustName} - Find information about schools and trusts');

                // Historic members table is visible and working
                governancePage
                    .checkHistoricMembersSubHeaderPresent()
                    .checkHistoricMembersTableHeadersAreVisible()
                    .checkHistoricMembersAppointmentDatesAreInThePast();

                // "No Historic Members" message is hidden
                governancePage
                    .checkNoHistoricMembersMessageIsHidden();

                navigation
                    .checkPageNameBreadcrumbPresent("Governance");
            });

            it("Table sorting is working", () => {
                cy.visit(`/trusts/governance/trust-leadership?uid=${uid}&referencenumber=tr04032`);
                governancePage.checkTrustLeadershipColumnsSortCorrectly();

                cy.visit(`/trusts/governance/trustees?uid=${uid}&referencenumber=tr04032`);
                governancePage.checkTrusteesColumnsSortCorrectly();

                cy.visit(`/trusts/governance/members?uid=${uid}&referencenumber=tr04032`);
                governancePage.checkMembersColumnsSortCorrectly();

                cy.visit(`/trusts/governance/historic-members?uid=${uid}&referencenumber=tr04032`);
                governancePage.checkHistoricMembersColumnsSortCorrectly();
            });

            it("Sub navigation links contain correct number of governors", () => {
                cy.visit(`/trusts/governance/trust-leadership?uid=${uid}&referencenumber=tr04032`);
                governancePage.checkTrustLeadershipLinkValueMatchesNumberOfTrustLeaders();

                cy.visit(`/trusts/governance/trustees?uid=${uid}&referencenumber=tr04032`);
                governancePage.checkTrusteesLinkValueMatchesNumberOfTrustees();

                cy.visit(`/trusts/governance/members?uid=${uid}&referencenumber=tr04032`);
                governancePage.checkMembersLinkValueMatchesNumberOfMembers();

                cy.visit(`/trusts/governance/historic-members?uid=${uid}&referencenumber=tr04032`);
                governancePage.checkHistoricMembersLinkValueMatchesNumberOfHistoricMembers();
            });
            
            it("Governor turnover rate information is displayed", () => {
                cy.visit(`/trusts/governance/trust-leadership?uid=${uid}&referencenumber=tr04032`);
                governancePage
                    .checkTurnoverRateIsPresent()
                    .checkTurnoverRateSummaryIsPresent()
                    .checkTurnoverCalculationExplanationIsPresent()
                    .checkTurnoverCalculationExplanationDetails();

                cy.visit(`/trusts/governance/trustees?uid=${uid}&referencenumber=tr04032`);
                governancePage
                    .checkTurnoverRateIsPresent()
                    .checkTurnoverRateSummaryIsPresent()
                    .checkTurnoverCalculationExplanationIsPresent()
                    .checkTurnoverCalculationExplanationDetails();

                cy.visit(`/trusts/governance/members?uid=${uid}&referencenumber=tr04032`);
                governancePage
                    .checkTurnoverRateIsPresent()
                    .checkTurnoverRateSummaryIsPresent()
                    .checkTurnoverCalculationExplanationIsPresent()
                    .checkTurnoverCalculationExplanationDetails();

                cy.visit(`/trusts/governance/historic-members?uid=${uid}&referencenumber=tr04032`);
                governancePage
                    .checkTurnoverRateIsPresent()
                    .checkTurnoverRateSummaryIsPresent()
                    .checkTurnoverCalculationExplanationIsPresent()
                    .checkTurnoverCalculationExplanationDetails();
            });
        });
    });


    trustsWithNoGovernanceData.forEach(({ typeOfTrust, uid }) => {
        describe(`On the Governance pages for a ${typeOfTrust}`, () => {

            it("The tables should be replaced with no data messages", () => {
                cy.visit(`/trusts/governance/trust-leadership?uid=${uid}&referencenumber=tr04032`);
                governancePage.checkNoTrustLeadershipMessageIsVisible();

                cy.visit(`/trusts/governance/trustees?uid=${uid}&referencenumber=tr04032`);
                governancePage.checkNoTrusteesMessageIsVisible();

                cy.visit(`/trusts/governance/members?uid=${uid}&referencenumber=tr04032`);
                governancePage.checkNotMembersMessageIsVisible();
            });

            //Skipping below test case until no data governance page issue sorted (Bug raised 179544)
            it.skip("The historic members table should be replaced with no data message", () => {
                cy.visit(`/trusts/governance/historic-members?uid=${uid}&referencenumber=tr04032`);
                governancePage.checkNoHistoricMembersMessageIsVisible();
            });

            it("The number of governors in each sub nav title should be 0", () => {
                cy.visit(`/trusts/governance/trust-leadership?uid=${uid}&referencenumber=tr04032`);
                governancePage.checkTrustLeadershipSubnavButtonHasZeroInBrackets();

                cy.visit(`/trusts/governance/trustees?uid=${uid}&referencenumber=tr04032`);
                governancePage.checkTrusteesSubnavButtonHasZeroInBrackets();

                cy.visit(`/trusts/governance/members?uid=${uid}&referencenumber=tr04032`);
                governancePage.checkMembersSubnavButtonHasZeroInBrackets();

                cy.visit(`/trusts/governance/historic-members?uid=${uid}&referencenumber=tr04032`);
                governancePage.checkHistoricMembersSubnavButtonHasZeroInBrackets();
            });

            it("Governor turnover rate information is displayed", () => {
                cy.visit(`/trusts/governance/trust-leadership?uid=${uid}&referencenumber=tr04032`);
                governancePage
                    .checkTurnoverRateIsPresent()
                    .checkTurnoverRateSummaryIsPresent()
                    .checkTurnoverCalculationExplanationIsPresent()
                    .checkTurnoverCalculationExplanationDetails();

                cy.visit(`/trusts/governance/trustees?uid=${uid}&referencenumber=tr04032`);
                governancePage
                    .checkTurnoverRateIsPresent()
                    .checkTurnoverRateSummaryIsPresent()
                    .checkTurnoverCalculationExplanationIsPresent()
                    .checkTurnoverCalculationExplanationDetails();

                cy.visit(`/trusts/governance/members?uid=${uid}&referencenumber=tr04032`);
                governancePage
                    .checkTurnoverRateIsPresent()
                    .checkTurnoverRateSummaryIsPresent()
                    .checkTurnoverCalculationExplanationIsPresent()
                    .checkTurnoverCalculationExplanationDetails();

                cy.visit(`/trusts/governance/historic-members?uid=${uid}&referencenumber=tr04032`);
                governancePage
                    .checkTurnoverRateIsPresent()
                    .checkTurnoverRateSummaryIsPresent()
                    .checkTurnoverCalculationExplanationIsPresent()
                    .checkTurnoverCalculationExplanationDetails();
            });
        });
    });

    describe("Testing the governance sub navigation", () => {

        it('Should check that the trust leadership navigation button takes me to the correct page', () => {
            cy.visit('/trusts/governance/historic-members?uid=5527&referencenumber=tr04032');

            governancePage
                .clickTrustLeadershipSubnavButton();

            navigation
                .checkCurrentURLIsCorrect('/trusts/governance/trust-leadership?uid=5527');

            governancePage
                .checkAllSubNavItemsPresent()
                .checkTrustLeadershipSubHeaderPresent();
        });

        it('Should check that the trustees navigation button takes me to the correct page', () => {
            cy.visit('/trusts/governance/trust-leadership?uid=5527&referencenumber=tr04032');

            governancePage
                .clickTrusteesSubnavButton();

            navigation
                .checkCurrentURLIsCorrect('/trusts/governance/trustees?uid=5527');

            governancePage
                .checkAllSubNavItemsPresent()
                .checkTrusteesSubHeaderPresent();
        });

        it('Should check that the members navigation button takes me to the correct page', () => {
            cy.visit('/trusts/governance/trustees?uid=5527&referencenumber=tr04032');

            governancePage
                .clickMembersSubnavButton();

            navigation
                .checkCurrentURLIsCorrect('/trusts/governance/members?uid=5527&referencenumber=tr04032');

            governancePage
                .checkAllSubNavItemsPresent()
                .checkMembersSubHeaderPresent();
        });

        it('Should check that the historic members navigation button takes me to the correct page', () => {
            cy.visit('/trusts/governance/members?uid=5527&referencenumber=tr04032');

            governancePage
                .clickHistoricMembersSubnavButton();

            navigation
                .checkCurrentURLIsCorrect('/trusts/governance/historic-members?uid=5527&referencenumber=tr04032');

            governancePage
                .checkAllSubNavItemsPresent()
                .checkHistoricMembersSubHeaderPresent();
        });

        it('Should check that the governance sub nav items are not present when I am not on the governance page', () => {
            cy.visit('/trusts/overview/trust-details?uid=5527&referencenumber=tr04032');
            governancePage
                .checkSubNavNotPresent();
        });

        describe("Testing that no unknown entries are found for an academies governance tables/pages", () => {
            trustsWithGovernanceData.forEach(({ typeOfTrust, uid }) => {

                [`/trusts/governance/trust-leadership?uid=${uid}&referencenumber=tr04032`, `/trusts/governance/trustees?uid=${uid}&referencenumber=tr04032`, `/trusts/governance/members?uid=${uid}&referencenumber=tr04032`, `/trusts/governance/historic-members?uid=${uid}&referencenumber=tr04032`].forEach((url) => {
                    it(`Should have no unknown entries on ${url} for a ${typeOfTrust}`, () => {
                        cy.visit(url);
                        commonPage
                            .checkNoUnknownEntries();
                    });
                });
            });
        });
    });
});
