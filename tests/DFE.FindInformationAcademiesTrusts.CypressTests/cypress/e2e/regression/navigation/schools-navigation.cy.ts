import { TestDataStore } from "../../../support/test-data-store";
import commonPage from "../../../pages/commonPage";
import schoolsPage from "../../../pages/schools/schoolsPage";
import navigation from "../../../pages/navigation";
import overviewPage from "../../../pages/trusts/overviewPage";

describe('Schools Navigation Tests', () => {
    const navTestAcademies = [
        {
            academyURN: 140214,
            trustAcademyName: "ABBEY ACADEMIES TRUST",
            trustUID: 2044
        },

        {
            academyURN: 140884,
            trustAcademyName: "MARYLEBONE SCHOOL LTD", //school in SAT
            trustUID: 3874
        }];

    const navTestSchool = {
        schoolURN: 107188,
    };

    describe("Routing tests", () => {

        const schoolTypesNotToShow = [
            { urn: 136083, unsupportedSchoolType: "Independent schools" },
            { urn: 150163, unsupportedSchoolType: "Online provider" },
            { urn: 131832, unsupportedSchoolType: "Other types" },
            { urn: 133793, unsupportedSchoolType: "Universities" },
            { urn: 137210, unsupportedSchoolType: "Closed academy" },
            { urn: 142109, unsupportedSchoolType: "Closed school" },
        ];

        schoolTypesNotToShow.forEach(({ urn, unsupportedSchoolType }) => {
            TestDataStore.GetAllSchoolSubpagesForUrn(urn).forEach(({ pageName, subpages }) => {

                describe(pageName, () => {

                    subpages.forEach(({ subpageName, url }) => {
                        it(`Should check that navigating to subpages for unsupported and closed school types display the 404 not found page ${pageName} > ${subpageName} > ${unsupportedSchoolType}`, () => {
                            // Set up an interceptor to check that the page response is a 404
                            commonPage.interceptAndVerifyResponseHas404Status(url);

                            // Go to the given subpage
                            cy.visit(url, { failOnStatusCode: false });

                            // Check that the 404 response interceptor was called
                            cy.wait('@checkTheResponseIs404');

                            // Check that the data sources component has a subheading for each subnav
                            commonPage
                                .checkPageNotFoundDisplayed();
                        });
                    });
                });
            });
        });

        navTestAcademies.forEach(({ academyURN, trustAcademyName, trustUID }) => {
            it('Should check that an academy has the link to the trust in the header and it takes me to the correct trust', () => {
                cy.visit(`/schools/overview/details?urn=${academyURN}`);
                schoolsPage
                    .checkAcademyLinkPresentAndCorrect(`${trustAcademyName}`)
                    .clickAcademyTrustLink();
                navigation
                    .checkCurrentURLIsCorrect(`/trusts/overview/trust-details?uid=${trustUID}`);
                overviewPage
                    .checkTrustDetailsSubHeaderPresent();

            });
        });

        it('Should check that an school does not have the link to the trust in the header', () => {
            cy.visit(`/schools/overview/details?urn=${navTestSchool.schoolURN}`);
            schoolsPage
                .checkAcademyLinkNotPresentForSchool();
        });
    });

    describe("Schools main navigation tests", () => {
        it('Should check that the schools main navigation is present and correct', () => {
            // School Overview --> School Contacts in DfE (School)
            cy.visit(`/schools/overview/details?urn=${navTestSchool.schoolURN}`);
            navigation
                .clickSchoolsContactsButton()
                .checkCurrentURLIsCorrect(`/schools/contacts/in-dfe?urn=${navTestSchool.schoolURN}`)
                .checkAllSchoolServiceNavItemsPresent();
            schoolsPage
                .checkInDfeContactsSubpageHeaderIsCorrect();

            // School Overview --> School Contacts in DfE (Academy)
            cy.visit(`/schools/overview/details?urn=${navTestAcademies[0].academyURN}`);
            navigation
                .clickSchoolsContactsButton()
                .checkCurrentURLIsCorrect(`/schools/contacts/in-dfe?urn=${navTestAcademies[0].academyURN}`)
                .checkAllSchoolServiceNavItemsPresent();
            schoolsPage
                .checkInDfeContactsSubpageHeaderIsCorrect();
        });
    });

    describe("Schools contacts sub navigation tests", () => {
        context('School contacts subnav navigation tests -- (School)', () => {
            it('Should navigate from in DfE contacts to "In this school" contacts and back', () => {
                // Start at in DfE contacts
                cy.visit(`/schools/contacts/in-dfe?urn=${navTestSchool.schoolURN}`);
                navigation
                    .checkSchoolsContactsSubNavItemsPresent()
                    .checkSchoolsContactsInDfeSubnavButtonIsHighlighted();
                schoolsPage
                    .checkInDfeContactsSubpageHeaderIsCorrect()
                    .checkRegionsGroupLaLeadContactCardPresent();

                // Navigate to "In this school" contacts
                navigation
                    .clickSchoolsContactsInThisSchoolSubnavButton()
                    .checkCurrentURLIsCorrect(`/schools/contacts/in-the-school?urn=${navTestSchool.schoolURN}`)
                    .checkSchoolsContactsInThisSchoolSubnavButtonIsHighlighted();
                schoolsPage
                    .checkSubpageHeaderIsCorrect()
                    .checkHeadTeacherContactCardPresent();

                // Navigate back to in DfE contacts
                navigation
                    .clickSchoolsContactsInDfeSubnavButton()
                    .checkCurrentURLIsCorrect(`/schools/contacts/in-dfe?urn=${navTestSchool.schoolURN}`)
                    .checkSchoolsContactsInDfeSubnavButtonIsHighlighted();
                schoolsPage
                    .checkInDfeContactsSubpageHeaderIsCorrect();
            });
        });

        context('School contacts subnav navigation tests -- (Academy)', () => {
            it('Should navigate from in DfE contacts to "In this academy" contacts and back', () => {
                // Start at DfE contacts
                cy.visit(`/schools/contacts/in-dfe?urn=${navTestAcademies[0].academyURN}`);
                navigation
                    .checkSchoolsContactsSubNavItemsPresent()
                    .checkSchoolsContactsInDfeSubnavButtonIsHighlighted();
                schoolsPage
                    .checkInDfeContactsSubpageHeaderIsCorrect();
                // TODO: Not checking specific contact cards as academy content structure will change - update once confirmed what is here

                // Navigate to "In this academy" contacts
                navigation
                    .clickSchoolsContactsInThisSchoolSubnavButton()
                    .checkCurrentURLIsCorrect(`/schools/contacts/in-the-school?urn=${navTestAcademies[0].academyURN}`)
                    .checkSchoolsContactsInThisSchoolSubnavButtonIsHighlighted();
                schoolsPage
                    .checkSubpageHeaderIsCorrect()
                    .checkHeadTeacherContactCardPresent();

                // Navigate back to DfE contacts
                navigation
                    .clickSchoolsContactsInDfeSubnavButton()
                    .checkCurrentURLIsCorrect(`/schools/contacts/in-dfe?urn=${navTestAcademies[0].academyURN}`)
                    .checkSchoolsContactsInDfeSubnavButtonIsHighlighted();
                schoolsPage
                    .checkInDfeContactsSubpageHeaderIsCorrect();
                // TODO: Not checking specific contact cards as academy content structure will change - update once confirmed what is here
            });
        });

        context('School contacts subnav content tests', () => {
            it('Should show correct subnav text for school', () => {
                cy.visit(`/schools/contacts/in-dfe?urn=${navTestSchool.schoolURN}`);
                navigation
                    .checkSchoolsContactsSubNavItemsPresent();

                // Verify the subnav shows "In this school" text
                cy.get('[data-testid="contacts-in-this-school-subnav"]')
                    .should('be.visible')
                    .should('contain.text', 'In this school');
            });

            it('Should show correct subnav text for academy', () => {
                cy.visit(`/schools/contacts/in-dfe?urn=${navTestAcademies[0].academyURN}`);
                navigation
                    .checkSchoolsContactsSubNavItemsPresent();

                // Verify the subnav shows "In this academy" text
                cy.get('[data-testid="contacts-in-this-school-subnav"]')
                    .should('be.visible')
                    .should('contain.text', 'In this academy');
            });
        });
    });

    describe("Schools overview sub navigation round robin tests", () => {
        context('School overview subnav round robin tests -- (School)', () => {
            // school details --> Federation details (school)
            it('Should check that the school details navigation button takes me to the correct page for a schools type subnav', () => {
                cy.visit(`/schools/overview/details?urn=${navTestSchool.schoolURN}`);
                navigation
                    .clickSchoolsFederationButton()
                    .checkCurrentURLIsCorrect(`/schools/overview/federation?urn=${navTestSchool.schoolURN}`)
                    .checkAllSchoolServiceNavItemsPresent()
                    .checkAllSchoolOverviewSubNavItemsPresent();
                schoolsPage
                    .checkFederationDetailsHeaderPresent();
            });

            // federation details --> SEN (school)
            it('Should check that the school details navigation button takes me to the SEN page for a schools type subnav', () => {
                cy.visit(`/schools/overview/federation?urn=${navTestSchool.schoolURN}`);
                navigation
                    .clickSchoolsSENButton()
                    .checkCurrentURLIsCorrect(`/schools/overview/sen?urn=${navTestSchool.schoolURN}`)
                    .checkAllSchoolServiceNavItemsPresent()
                    .checkAllSchoolOverviewSubNavItemsPresent();
                schoolsPage
                    .checkSENSubpageHeaderCorrect();
            });

            // SEN --> school details (school)
            it('Should check that the school details navigation button takes me to the correct page for a schools type subnav', () => {
                cy.visit(`/schools/overview/sen?urn=${navTestSchool.schoolURN}`);
                navigation
                    .clickSchoolsDetailsButton()
                    .checkCurrentURLIsCorrect(`/schools/overview/details?urn=${navTestSchool.schoolURN}`)
                    .checkAllSchoolServiceNavItemsPresent()
                    .checkAllSchoolOverviewSubNavItemsPresent();
                schoolsPage
                    .checkSchoolDetailsHeaderPresent();
            });
        });

        context('School overview subnav round robin tests -- (Academy)', () => {
            // school details --> SEN (academy)
            it('Should check that the school details navigation button takes me to the correct page for a schools type subnav', () => {
                cy.visit(`/schools/overview/details?urn=${navTestAcademies[0].academyURN}`);
                navigation
                    .clickSchoolsSENButton()
                    .checkCurrentURLIsCorrect(`/schools/overview/sen?urn=${navTestAcademies[0].academyURN}`)
                    .checkAllSchoolServiceNavItemsPresent()
                    .checkAllAcademyOverviewSubNavItemsPresent();
                schoolsPage
                    .checkSENSubpageHeaderCorrect();
            });

            // SEN --> school details (academy)
            it('Should check that the school details navigation button takes me to the correct page for a schools type subnav', () => {
                cy.visit(`/schools/overview/sen?urn=${navTestAcademies[0].academyURN}`);
                navigation
                    .clickSchoolsDetailsButton()
                    .checkCurrentURLIsCorrect(`/schools/overview/details?urn=${navTestAcademies[0].academyURN}`)
                    .checkAllSchoolServiceNavItemsPresent()
                    .checkAllAcademyOverviewSubNavItemsPresent();
                schoolsPage
                    .checkAcademyDetailsHeaderPresent();
            });
        });

        // school details --> Reference Numbers (school)
        it('Should check that the school details navigation button takes me to the correct page for a schools type subnav', () => {
            cy.visit(`/schools/overview/details?urn=${navTestSchool.schoolURN}`);
            navigation
                .clickSchoolsReferenceNumberButton()
                .checkCurrentURLIsCorrect(`/schools/overview/referencenumbers?urn=${navTestSchool.schoolURN}`)
                .checkAllSchoolServiceNavItemsPresent()
                .checkAllSchoolOverviewSubNavItemsPresent();
            schoolsPage
                .checkSchoolReferenceNumbersHeaderPresent();
        });

        // Reference Numbers --> school details (academy)
        it('Should check that the school details navigation button takes me to the details page for a schools type subnav', () => {
            cy.visit(`/schools/overview/referencenumbers?urn=${navTestSchool.schoolURN}`);
            navigation
                .clickSchoolsDetailsButton()
                .checkCurrentURLIsCorrect(`/schools/overview/details?urn=${navTestSchool.schoolURN}`)
                .checkAllSchoolServiceNavItemsPresent()
                .checkAllSchoolOverviewSubNavItemsPresent();
            schoolsPage
                .checkSchoolDetailsHeaderPresent();
        });
    });

    describe("School contacts edit navigation tests", () => {
        context('School contact edit page navigation', () => {
            it('Should check that the browser title is correct on the in DfE contacts page', () => {
                cy.visit(`/schools/contacts/in-dfe?urn=${navTestSchool.schoolURN}`);
                commonPage
                    .checkThatBrowserTitleMatches('In DfE - Contacts - Abbey Green Nursery School - Find information about academies and trusts');
            });

            it('Should check that the browser title is correct on the edit Regions group LA lead contact page', () => {
                cy.visit(`/schools/contacts/editregionsgrouplocalauthoritylead?urn=${navTestSchool.schoolURN}`);
                commonPage
                    .checkThatBrowserTitleMatches('Edit Regions group local authority lead details - Contacts - Abbey Green Nursery School - Find information about academies and trusts');
            });

            it('Should check that cancelling the edit returns to the correct page', () => {
                cy.visit(`/schools/contacts/in-dfe?urn=${navTestSchool.schoolURN}`);
                schoolsPage
                    .editRegionsGroupLaLeadWithoutSaving("Should Notbe Seen", "exittest@education.gov.uk")
                    .clickContactUpdateCancelButton();

                navigation
                    .checkCurrentURLIsCorrect(`/schools/contacts/in-dfe?urn=${navTestSchool.schoolURN}`);
            });

            it('Should check breadcrumb navigation is correct on the in DfE contacts page', () => {
                cy.visit(`/schools/contacts/in-dfe?urn=${navTestSchool.schoolURN}`);
                navigation
                    .checkPageNameBreadcrumbPresent("Contacts");
            });
        });
    });
});
