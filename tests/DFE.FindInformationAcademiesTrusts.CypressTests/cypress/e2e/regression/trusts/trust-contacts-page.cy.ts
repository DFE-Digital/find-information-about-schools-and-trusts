import trustContactsPage from "../../../pages/trusts/trustContactsPage";
import commonPage from "../../../pages/commonPage";
import navigation from "../../../pages/navigation";
import { testTrustData } from "../../../support/test-data-store";

function generateNameAndEmail() {
    const randomNumber = Math.floor(Math.random() * 9999);
    return {
        name: `Name${randomNumber}`,
        email: `email${randomNumber}@education.gov.uk`
    };
}

const incorrectEmailFormatMessage = 'Enter a DfE email address in the correct format, e.g. joe.bloggs@education.gov.uk';

describe("Testing the components of the Trust contacts page", () => {
    testTrustData.forEach(({ typeOfTrust, uid }) => {
        describe(`On the contacts in DfE page for a ${typeOfTrust}`, () => {
            beforeEach(() => {
                cy.visit(`/trusts/contacts/in-dfe?uid=${uid}`);
            });

            it("Checks the browser title is correct", () => {
                commonPage
                    .checkThatBrowserTitleForTrustPageMatches('In DfE - Contacts - {trustName} - Find information about schools and trusts');
            });

            it("Checks the breadcrumb shows the correct page name", () => {
                navigation
                    .checkPageNameBreadcrumbPresent("Contacts");
            });

            it(`Can change Trust relationship manager contact details`, () => {
                const { name, email } = generateNameAndEmail();

                trustContactsPage
                    .editTrustRelationshipManager(name, email)
                    .checkTrustRelationshipManagerIsSuccessfullyUpdated(name, email)
                    .checkTrustRelationshipManagerDatasourceLastUpdatedByUser('Automation User - email');

                commonPage
                    .checkSuccessPopup('Changes made to the Trust relationship manager name and email were updated')
                    .checkErrorPopupNotPresent();
            });

            it(`Can change Schools financial support oversight lead contact details`, () => {
                const { name, email } = generateNameAndEmail();

                trustContactsPage
                    .editSfsoLead(name, email)
                    .checkSfsoLeadIsSuccessfullyUpdated(name, email)
                    .checkSfsoLeadDatasourceLastUpdatedByUser('Automation User - email');

                commonPage
                    .checkSuccessPopup('Changes made to the SFSO (Schools financial support and oversight) lead name and email were updated')
                    .checkErrorPopupNotPresent();
            });

            it(`Checks that when cancelling the edit of a TRM contact that I am taken back to the previous page and that entered data is not saved`, () => {
                trustContactsPage
                    .editTrustRelationshipManagerWithoutSaving("Should Notbe Seen", "exittest@education.gov.uk")
                    .clickContactUpdateCancelButton()
                    .checkTrustRelationshipManagerIsNotUpdated("Should Notbe Seen", "exittest@education.gov.uk");

                navigation
                    .checkCurrentURLIsCorrect(`/trusts/contacts/in-dfe?uid=${uid}`);
            });

            it(`Checks that when cancelling the edit of a SFSO contact that I am taken back to the previous page and that entered data is not saved`, () => {
                trustContactsPage
                    .editSfsoLeadWithoutSaving("Should Notbe Seen", "exittest@education.gov.uk")
                    .clickContactUpdateCancelButton()
                    .checkSfsoLeadIsNotUpdated("Should Notbe Seen", "exittest@education.gov.uk");

                navigation
                    .checkCurrentURLIsCorrect(`/trusts/contacts/in-dfe?uid=${uid}`);
            });
        });

        describe(`On the edit Trust relationship manager contact details page for a ${typeOfTrust}`, () => {
            beforeEach(() => {
                cy.visit(`/trusts/contacts/edittrustrelationshipmanager?uid=${uid}`);
            });

            it("Checks the browser title is correct", () => {
                commonPage
                    .checkThatBrowserTitleForTrustPageMatches('Edit Trust relationship manager details - Contacts - {trustName} - Find information about schools and trusts');
            });
        });

        describe(`On the edit SFSO lead contact details page for a ${typeOfTrust}`, () => {
            beforeEach(() => {
                cy.visit(`/trusts/contacts/editsfsolead?uid=${uid}`);
            });

            it("Checks the browser title is correct", () => {
                commonPage
                    .checkThatBrowserTitleForTrustPageMatches('Edit SFSO (Schools financial support and oversight) lead details - Contacts - {trustName} - Find information about schools and trusts');
            });
        });

        describe(`On the contacts in the trust page for a ${typeOfTrust}`, () => {
            beforeEach(() => {
                cy.visit(`/trusts/contacts/in-the-trust?uid=${uid}`);
            });

            it("Checks the browser title is correct", () => {
                commonPage
                    .checkThatBrowserTitleForTrustPageMatches('In this trust - Contacts - {trustName} - Find information about schools and trusts');
            });

            it("Checks the breadcrumb shows the correct page name", () => {
                navigation
                    .checkPageNameBreadcrumbPresent("Contacts");
            });

            it(`Checks a trusts external contact details are present`, () => {
                trustContactsPage
                    .checkAccountingOfficerPresent()
                    .checkChairOfTrusteesPresent()
                    .checkChiefFinancialOfficerPresent();
            });
        });
    });

    describe('Checks the update error handling', () => {
        beforeEach(() => {
            cy.visit('/trusts/contacts/in-dfe?uid=5527');
        });

        it("Checks that a full non DFE email entered returns the correct error message on a TRM ", () => {
            trustContactsPage
                .editTrustRelationshipManager("Name", "email@hotmail.co.uk");
            commonPage
                .checkErrorPopup(incorrectEmailFormatMessage);
        });

        it("Checks that an incorrect email entered returns the correct error message on a TRM ", () => {
            trustContactsPage
                .editTrustRelationshipManager("Name", "email");
            commonPage
                .checkErrorPopup(incorrectEmailFormatMessage);
        });

        it("Checks that illegal characters entered returns the correct error message on a TRM ", () => {
            trustContactsPage
                .editTrustRelationshipManager("Name", "@£$$^&");
            commonPage
                .checkErrorPopup(incorrectEmailFormatMessage);
        });

        it("Checks that whitespace entered returns the correct error message on a TRM ", () => {
            trustContactsPage
                .editTrustRelationshipManager("Name", "a     b");
            commonPage
                .checkErrorPopup(incorrectEmailFormatMessage);
        });

        it("Checks that a full non DFE email entered returns the correct error message on a SFSO ", () => {
            trustContactsPage
                .editSfsoLead("Name", "email@hotmail.co.uk");
            commonPage
                .checkErrorPopup(incorrectEmailFormatMessage);
        });

        it("Checks that an incorrect email entered returns the correct error message on a SFSO ", () => {
            trustContactsPage
                .editSfsoLead("Name", "email");
            commonPage
                .checkErrorPopup(incorrectEmailFormatMessage);
        });

        it("Checks that illegal characters entered returns the correct error message on a SFSO ", () => {
            trustContactsPage
                .editSfsoLead("Name", "@£$$^&");
            commonPage
                .checkErrorPopup(incorrectEmailFormatMessage);
        });

        it("Checks that whitespace entered returns the correct error message on a SFSO ", () => {
            trustContactsPage
                .editSfsoLead("Name", "a     b");
            commonPage
                .checkErrorPopup(incorrectEmailFormatMessage);
        });

        context('Checks that an email address with an incorrect prefix formats entered returns the correct error message on a TRM', () => {
            const incorrectPrefixes = ['@education.gov.uk', 'joe..bloggs@education.gov.uk', '.@education.gov.uk', '""@education.gov.uk'];
            incorrectPrefixes.forEach(prefix => {
                it(`Checks that an email address with an incorrect prefix ${prefix} entered returns the correct error message`, () => {
                    trustContactsPage
                        .editTrustRelationshipManager("Name", prefix);
                    commonPage
                        .checkErrorPopup(incorrectEmailFormatMessage);
                });
            });
        });

        context('Checks that an email address with an incorrect prefix formats entered returns the correct error message on a SFSO Lead', () => {
            const incorrectPrefixes = ['@education.gov.uk', 'joe..bloggs@education.gov.uk', '.@education.gov.uk', '""@education.gov.uk'];
            incorrectPrefixes.forEach(prefix => {
                it(`Checks that an email address with an incorrect prefix ${prefix} entered returns the correct error message`, () => {
                    trustContactsPage
                        .editSfsoLead("Name", prefix);
                    commonPage
                        .checkErrorPopup(incorrectEmailFormatMessage);
                });
            });
        });
    });

    describe("Testing the contacts sub navigation", () => {

        it('Should check that the contacts in dfe navigation button takes me to the correct page', () => {
            cy.visit('/trusts/contacts/in-the-trust?uid=5527');

            trustContactsPage
                .clickContactsInDfeSubnavButton()
                .checkContactsInDfeSubHeaderPresent();

            navigation
                .checkCurrentURLIsCorrect('/trusts/contacts/in-dfe?uid=5527');

            trustContactsPage
                .checkAllSubNavItemsPresent()
                .checkSfsoLeadIsPresent()
                .checkTrustRelationshipManagerIsPresent();
        });

        it('Should check that the contacts in this trust navigation button takes me to the correct page', () => {
            cy.visit('/trusts/contacts/in-dfe?uid=5527');

            trustContactsPage
                .clickContactsInTheTrustSubnavButton()
                .checkContactsInTheTrustSubHeaderPresent();

            navigation
                .checkCurrentURLIsCorrect('/trusts/contacts/in-the-trust?uid=5527');

            trustContactsPage
                .checkAllSubNavItemsPresent()
                .checkAccountingOfficerPresent()
                .checkChairOfTrusteesPresent()
                .checkChiefFinancialOfficerPresent();
        });

        it('Should check that the contacts sub nav items are not present when I am not on the contacts page', () => {
            cy.visit('/trusts/overview/trust-details?uid=5527');

            trustContactsPage
                .checkSubNavNotPresent();
        });

        describe("Testing a trust that has no contacts within it to ensure the issue of a 500 page appearing does not happen", () => {
            beforeEach(() => {
                commonPage.interceptAndVerifyNo500Errors();
            });

            ['/trusts/contacts/in-dfe?uid=17728', '/trusts/contacts/in-the-trust?uid=17728'].forEach((url) => {
                it(`Should have no 500 error on ${url}`, () => {
                    cy.visit(url);
                });
            });
        });
    });

});