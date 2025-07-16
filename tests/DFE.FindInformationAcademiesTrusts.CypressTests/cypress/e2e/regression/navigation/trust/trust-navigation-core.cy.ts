import navigation from "../../../../pages/navigation";
import academiesInTrustPage from "../../../../pages/trusts/academiesInTrustPage";
import governancePage from "../../../../pages/trusts/governancePage";
import trustContactsPage from "../../../../pages/trusts/trustContactsPage";
import overviewPage from "../../../../pages/trusts/overviewPage";
import financialDocumentsPage from "../../../../pages/trusts/financialDocumentsPage";
import ofstedPage from "../../../../pages/trusts/ofstedPage";

describe('Trust Navigation Core Tests', () => {

    describe('Testing trust service navigation and sub navigation', () => {

        describe("Testing the trusts service navigation", () => {
            // Test each link from a different page in round robin
            // Governance -> Overview -> Contacts -> Academies -> Ofsted -> Financial documents -> Governance

            it('Should check that the Overview navigation button takes me to the overview trust details page', () => {
                // Governance -> Overview
                cy.visit('/trusts/governance/trust-leadership?uid=5527');

                navigation
                    .clickOverviewServiceNavButton()
                    .checkOverviewServiceNavButtonIsHighlighted()
                    .checkCurrentURLIsCorrect('/trusts/overview/trust-details?uid=5527')
                    .checkAllServiceNavItemsPresent();
                overviewPage
                    .checkTrustDetailsSubHeaderPresent();
            });

            it('Should check that the contacts navigation button takes me to the contacts in DfE page', () => {
                // Overview -> Contacts
                cy.visit('/trusts/overview/trust-details?uid=5527');

                navigation
                    .clickContactsServiceNavButton()
                    .checkContactsServiceNavButtonIsHighlighted()
                    .checkCurrentURLIsCorrect('/trusts/contacts/in-dfe?uid=5527')
                    .checkAllServiceNavItemsPresent();
                trustContactsPage
                    .checkContactsInDfeSubHeaderPresent();
            });

            it('Should check that the Academies navigation button takes me to the academies details page', () => {
                // Contacts -> Academies
                cy.visit('/trusts/contacts/in-dfe?uid=5527');

                navigation
                    .clickAcademiesServiceNavButton()
                    .checkAcademiesServiceNavButtonIsHighlighted()
                    .checkCurrentURLIsCorrect('/trusts/academies/in-trust/details?uid=5527')
                    .checkAllServiceNavItemsPresent();
                academiesInTrustPage
                    .checkDetailsHeadersPresent();
            });

            it('Should check that the Ofsted navigation button takes me to the Ofsted single headline grades page', () => {
                //  Academies -> Ofsted
                cy.visit('/trusts/academies/in-trust/details?uid=5527');

                navigation
                    .clickOfstedServiceNavButton()
                    .checkOfstedServiceNavButtonIsHighlighted()
                    .checkCurrentURLIsCorrect('/trusts/ofsted/single-headline-grades?uid=5527')
                    .checkAllServiceNavItemsPresent();
                ofstedPage
                    .checkOfstedSHGPageHeaderPresent();
            });

            it('Should check that the Finance documents navigation button takes me to the Financial documents financial statements page', () => {
                //  Ofsted -> Financial documents 
                cy.visit('/trusts/ofsted/single-headline-grades?uid=5527');

                navigation
                    .clickFinancialDocumentsServiceNavButton()
                    .checkFinancialDocumentsServiceNavButtonIsHighlighted()
                    .checkCurrentURLIsCorrect('/trusts/financial-documents/financial-statements?uid=5527')
                    .checkAllServiceNavItemsPresent();
                financialDocumentsPage
                    .checkFinancialStatementsPageHeaderPresent();
            });

            it('Should check that the Governance navigation button takes me to the governance trust leadership page', () => {
                //  Financial documents -> Governance
                cy.visit('/trusts/financial-documents/financial-statements?uid=5527');

                navigation
                    .clickGovernanceServiceNavButton()
                    .checkGovernanceServiceNavButtonIsHighlighted()
                    .checkCurrentURLIsCorrect('/trusts/governance/trust-leadership?uid=5527')
                    .checkAllServiceNavItemsPresent();
                governancePage
                    .checkTrustLeadershipSubHeaderPresent();
            });
        });

        describe("Should check that the pipeline academies navigation works", () => {
            it('Navigates from In this trust to pipeline academies', () => {
                // Academies in Trust -> Pipeline Academies
                cy.visit('/trusts/academies/in-trust/details?uid=16002');

                navigation
                    .clickPipelineAcademiesNavButton()
                    .checkCurrentURLIsCorrect('/trusts/academies/pipeline/pre-advisory-board?uid=16002');
            });

            it('Navigates from In this trust to pipeline academies', () => {
                // Pipeline Academies -> Academies in Trust
                cy.visit('/trusts/academies/pipeline/pre-advisory-board?uid=16002');

                navigation
                    .clickAcademiesInThisTrustNavButton()
                    .checkCurrentURLIsCorrect('/trusts/academies/in-trust/details?uid=16002');
            });

            it('Navigates from In this trust to pipeline academies', () => {
                // Pipeline Academies page nav buttons
                cy.visit('/trusts/academies/pipeline/pre-advisory-board?uid=16002');

                navigation
                    .clickPipelineAcademiesPostAdvisoryNavButton()
                    .checkCurrentURLIsCorrect('/trusts/academies/pipeline/post-advisory-board?uid=16002')
                    .clickPipelineAcademiesFreeSchoolsNavButton()
                    .checkCurrentURLIsCorrect('/trusts/academies/pipeline/free-schools?uid=16002')
                    .clickPipelineAcademiesPreAdvisoryNavButton()
                    .checkCurrentURLIsCorrect('/trusts/academies/pipeline/pre-advisory-board?uid=16002');
            });
        });

        describe("Should check that the finance documents navigation works", () => {
            it('Navigates from financial statements to management letters', () => {
                // Financial statements -> management letters
                cy.visit('/trusts/financial-documents/financial-statements?uid=5527');

                navigation
                    .clickFinancialDocsManagementLettersButton()
                    .checkCurrentURLIsCorrect('/trusts/financial-documents/management-letters?uid=5527');
            });

            it('Navigates from management letters to internal scrutiny reports', () => {
                //Management letters -> Internal scrutiny reports
                cy.visit('/trusts/financial-documents/management-letters?uid=5527');

                navigation
                    .clickFinancialDocsInternalScrutinyReportsButton()
                    .checkCurrentURLIsCorrect('/trusts/financial-documents/internal-scrutiny-reports?uid=5527');
            });

            it('Navigates from internal scrutiny reports to self assessment checklist', () => {
                // Internal scrutiny reports -> Self assessment checklist
                cy.visit('/trusts/financial-documents/internal-scrutiny-reports?uid=5527');

                navigation
                    .clickFinancialDocsSelfAssessmentButton()
                    .checkCurrentURLIsCorrect('/trusts/financial-documents/self-assessment-checklists?uid=5527');
            });

            it('Navigates from self assessment checklist to financial statements', () => {
                // Self-assessment checklist -> Finance statements
                cy.visit('/trusts/financial-documents/self-assessment-checklists?uid=5527');

                navigation
                    .clickFinancialDocsFinancialStatementsButton()
                    .checkCurrentURLIsCorrect('/trusts/financial-documents/financial-statements?uid=5527');
            });
        });

        describe("Testing the trust academies page sub navigation", () => {
            // details -> pupil numbers        
            it('Should check that the academies Pupil numbers navigation button takes me to the correct page', () => {
                cy.visit('/trusts/academies/in-trust/details?uid=5527');
                navigation
                    .clickPupilNumbersAcademiesTrustButton()
                    .checkCurrentURLIsCorrect('/trusts/academies/in-trust/pupil-numbers?uid=5527')
                    .checkAllServiceNavItemsPresent()
                    .checkAllAcademiesNavItemsPresent();
                academiesInTrustPage
                    .checkPupilNumbersHeadersPresent();
            });

            // pupil numbers -> free school meals 
            it('Should check that the academies Free school meals navigation button takes me to the correct page', () => {
                cy.visit('/trusts/academies/in-trust/pupil-numbers?uid=5527');
                navigation
                    .clickFreeSchoolMealsAcademiesTrustButton()
                    .checkCurrentURLIsCorrect('/trusts/academies/in-trust/free-school-meals?uid=5527')
                    .checkAllServiceNavItemsPresent()
                    .checkAllAcademiesNavItemsPresent();
                academiesInTrustPage
                    .checkFreeSchoolMealsHeadersPresent();
            });

            // free school meals -> details  
            it('Should check that the academies Details navigation button takes me to the correct page', () => {
                cy.visit('/trusts/academies/in-trust/free-school-meals?uid=5527');
                navigation
                    .clickDetailsAcademiesTrustButton()
                    .checkCurrentURLIsCorrect('/trusts/academies/in-trust/details?uid=5527')
                    .checkAllServiceNavItemsPresent()
                    .checkAllAcademiesNavItemsPresent();
                academiesInTrustPage
                    .checkDetailsHeadersPresent();
            });

            it('Should check that the academies sub nav items are not present when I am not in the relevant academies page', () => {
                cy.visit('/trusts/overview/trust-details?uid=5527');
                navigation
                    .checkAcademiesSubNavNotPresent();
            });
        });
    });
}); 
