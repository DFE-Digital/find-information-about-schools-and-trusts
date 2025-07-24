import commonPage from "../../../pages/commonPage";
import navigation from "../../../pages/navigation";
import financialDocumentsPage from "../../../pages/trusts/financialDocumentsPage";
import { testFinanceData } from "../../../support/test-data-store";
testFinanceData.forEach(({ uid }) => {
    describe("Testing the Financial documents pages", () => {
        describe(`On the Finance statements page for a trust`, () => {

            beforeEach(() => {
                cy.visit(`/trusts/financial-documents/financial-statements?uid=${uid}`);
            });

            it("Checks the browser title is correct", () => {
                commonPage
                    .checkThatBrowserTitleForTrustPageMatches('Financial statements - Financial documents - {trustName} - Find information about schools and trusts');
            });

            it("Checks the Finance statements page subpage header is present", () => {
                financialDocumentsPage
                    .checkFinancialStatementsPageHeaderPresent();
            });

            it("Checks the Finance statements page financial status and year", () => {
                financialDocumentsPage
                    .checkFinancialStatementsCorrectStatusTypePresent()
                    .checkFinancialDocumentsCorrectYearRangePresent();
            });

            it("Checks that the internal use only message is not showing", () => {
                financialDocumentsPage
                    .checkForNoInternalUseMessage();
            });

        });

        describe(`On the Management letters page for a trust`, () => {

            beforeEach(() => {
                cy.visit(`/trusts/financial-documents/management-letters?uid=${uid}`);
            });

            it("Checks the browser title is correct", () => {
                commonPage
                    .checkThatBrowserTitleForTrustPageMatches('Management letters - Financial documents - {trustName} - Find information about schools and trusts');
            });

            it("Checks the Management letters page subpage header is present", () => {
                financialDocumentsPage
                    .checkManagementLettersPageHeaderPresent();
            });

            it("Checks the Management letters page financial status and year", () => {
                financialDocumentsPage
                    .checkManagementLettersCorrectStatusTypePresent()
                    .checkFinancialDocumentsCorrectYearRangePresent();
            });

            it("Checks that the correct internal use only message is showing", () => {
                financialDocumentsPage
                    .checkForCorrectInternalUseMessage('Management letters are for internal use only.');
            });

        });

        describe(`On the Internal scrutiny reports page for a trust`, () => {

            beforeEach(() => {
                cy.visit(`/trusts/financial-documents/internal-scrutiny-reports?uid=${uid}`);
            });

            it("Checks the browser title is correct", () => {
                commonPage
                    .checkThatBrowserTitleForTrustPageMatches('Internal scrutiny reports - Financial documents - {trustName} - Find information about schools and trusts');
            });

            it("Checks the Internal scrutiny reports page subpage header is present", () => {
                financialDocumentsPage
                    .checkInternalScrutinyReportsPageHeaderPresent();
            });

            it("Checks the Internal scrutiny reports page financial status and year", () => {
                financialDocumentsPage
                    .checkInternalScrutinyReportCorrectStatusTypePresent()
                    .checkFinancialDocumentsCorrectYearRangePresent();
            });

            it("Checks that the correct internal use only message is showing", () => {
                financialDocumentsPage
                    .checkForCorrectInternalUseMessage('Internal scrutiny reports are for internal use only.');
            });

        });

        describe(`On the Self-assessment checklist page for a trust`, () => {

            beforeEach(() => {
                cy.visit(`/trusts/financial-documents/self-assessment-checklists?uid=${uid}`);
            });

            it("Checks the browser title is correct", () => {
                commonPage
                    .checkThatBrowserTitleForTrustPageMatches('Self-assessment checklists - Financial documents - {trustName} - Find information about schools and trusts');
            });

            it("Checks the Self assessment checklists page subpage header is present", () => {
                financialDocumentsPage
                    .checkSelfAssessmentChecklistsPageHeaderPresent();
            });

            it("Checks the Self assessment checklists page financial status and year", () => {
                financialDocumentsPage
                    .checkSelfAssessmentCorrectStatusTypePresent()
                    .checkFinancialDocumentsCorrectYearRangePresent();
            });

            it("Checks that the correct internal use only message is showing", () => {
                financialDocumentsPage
                    .checkForCorrectInternalUseMessage('Self-assessment checklists are for internal use only.');
            });

        });

        describe(`On every financial documents page`, () => {
            [`/trusts/financial-documents/financial-statements?uid=${uid}`, `/trusts/financial-documents/management-letters?uid=${uid}`, `/trusts/financial-documents/internal-scrutiny-reports?uid=${uid}`, `/trusts/financial-documents/self-assessment-checklists?uid=${uid}`].forEach((url) => {
                beforeEach(() => {
                    cy.visit(url);
                });

                it("Checks the breadcrumb shows the correct page name", () => {
                    navigation
                        .checkPageNameBreadcrumbPresent("Financial documents");
                });

                it(`Should have an about these documents component and the correct information on ${url}`, () => {
                    financialDocumentsPage
                        .checkHasAboutTheseDocumentsComponent()
                        .checkAboutTheseDocumentsComponentDetails();
                });

                it('Should have the correct permission message', () => {
                    financialDocumentsPage
                        .checkForCorrectPermissionMessage();
                });

            });
        });
    });

});
