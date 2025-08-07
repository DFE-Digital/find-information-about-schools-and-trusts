import commonPage from "../../../../pages/commonPage";
import schoolsPage from "../../../../pages/schools/schoolsPage";
import navigation from "../../../../pages/navigation";
import overviewPage from "../../../../pages/trusts/overviewPage";

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


    describe("Functional navigation tests", () => {

        context("Academy trust link navigation", () => {
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
        });

        context("School trust link validation", () => {
            it('Should check that an school does not have the link to the trust in the header', () => {
                cy.visit(`/schools/overview/details?urn=${navTestSchool.schoolURN}`);
                schoolsPage
                    .checkAcademyLinkNotPresentForSchool();
            });
        });
    });

    describe("Schools main navigation tests", () => {
        // School main navigation order: Overview → Contacts → Governance
        const navigationTestData = [
            { type: 'School', urn: navTestSchool.schoolURN },
            { type: 'Academy', urn: navTestAcademies[0].academyURN }
        ];

        navigationTestData.forEach(({ type, urn }) => {
            it(`Should validate complete navigation order for ${type}: Overview → Contacts → Governance`, () => {
                // Test complete navigation flow
                cy.visit(`/schools/overview/details?urn=${urn}`);

                // Overview → Contacts
                navigation
                    .clickSchoolsContactsButton()
                    .checkCurrentURLIsCorrect(`/schools/contacts/in-dfe?urn=${urn}`)
                    .checkAllSchoolServiceNavItemsPresent();

                // Contacts → Governance  
                navigation
                    .clickSchoolsGovernanceButton()
                    .checkCurrentURLIsCorrect(`/schools/governance/current?urn=${urn}`)
                    .checkGovernanceServiceNavButtonIsHighlighted();
                schoolsPage
                    .checkGovernancePageNamePresent();

                // Governance → Overview (completing the cycle)
                navigation
                    .clickOverviewServiceNavButton()
                    .checkCurrentURLIsCorrect(`/schools/overview/details?urn=${urn}`);
            });
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

    describe("Schools governance sub navigation tests", () => {
        context('School governance subnav navigation tests -- (School)', () => {
            it('Should navigate from current governors to historic governors and back', () => {
                // Start at current governors
                cy.visit(`/schools/governance/current?urn=${navTestSchool.schoolURN}`);
                navigation
                    .checkSchoolsGovernanceSubNavItemsPresent()
                    .checkCurrentGovernorsSubnavButtonIsHighlighted();
                schoolsPage
                    .checkCurrentGovernorsHeaderPresent()
                    .checkCurrentGovernorsSectionPresent();

                // Navigate to historic governors
                navigation
                    .clickHistoricGovernorsSubnavButton()
                    .checkCurrentURLIsCorrect(`/schools/governance/historic?urn=${navTestSchool.schoolURN}`)
                    .checkHistoricGovernorsSubnavButtonIsHighlighted();
                schoolsPage
                    .checkHistoricGovernorsHeaderPresent()
                    .checkHistoricGovernorsSectionPresent();

                // Navigate back to current governors
                navigation
                    .clickCurrentGovernorsSubnavButton()
                    .checkCurrentURLIsCorrect(`/schools/governance/current?urn=${navTestSchool.schoolURN}`)
                    .checkCurrentGovernorsSubnavButtonIsHighlighted();
                schoolsPage
                    .checkCurrentGovernorsHeaderPresent();
            });
        });

        context('School governance subnav navigation tests -- (Academy)', () => {
            it('Should navigate from current governors to historic governors and back', () => {
                // Start at current governors
                cy.visit(`/schools/governance/current?urn=${navTestAcademies[0].academyURN}`);
                navigation
                    .checkSchoolsGovernanceSubNavItemsPresent()
                    .checkCurrentGovernorsSubnavButtonIsHighlighted();
                schoolsPage
                    .checkCurrentGovernorsHeaderPresent()
                    .checkCurrentGovernorsSectionPresent();

                // Navigate to historic governors
                navigation
                    .clickHistoricGovernorsSubnavButton()
                    .checkCurrentURLIsCorrect(`/schools/governance/historic?urn=${navTestAcademies[0].academyURN}`)
                    .checkHistoricGovernorsSubnavButtonIsHighlighted();
                schoolsPage
                    .checkHistoricGovernorsHeaderPresent()
                    .checkHistoricGovernorsSectionPresent();

                // Navigate back to current governors
                navigation
                    .clickCurrentGovernorsSubnavButton()
                    .checkCurrentURLIsCorrect(`/schools/governance/current?urn=${navTestAcademies[0].academyURN}`)
                    .checkCurrentGovernorsSubnavButtonIsHighlighted();
                schoolsPage
                    .checkCurrentGovernorsHeaderPresent();
            });
        });
    });

    describe("Schools overview sub navigation round robin tests", () => {
        context('School overview subnav round robin tests -- (School)', () => {
            // school details --> federation details (school)
            it('School details → Federation details', () => {
                cy.visit(`/schools/overview/details?urn=${navTestSchool.schoolURN}`);
                navigation
                    .clickSchoolsFederationButton()
                    .checkCurrentURLIsCorrect(`/schools/overview/federation?urn=${navTestSchool.schoolURN}`)
                    .checkAllSchoolServiceNavItemsPresent()
                    .checkAllSchoolOverviewSubNavItemsPresent();
                schoolsPage
                    .checkFederationDetailsHeaderPresent();
            });

            // federation details --> Reference Numbers (school)
            it('Federation details → Reference numbers', () => {
                cy.visit(`/schools/overview/federation?urn=${navTestSchool.schoolURN}`);
                navigation
                    .clickSchoolsReferenceNumberButton()
                    .checkCurrentURLIsCorrect(`/schools/overview/referencenumbers?urn=${navTestSchool.schoolURN}`)
                    .checkSchoolsReferenceNumbersButtonIsHighlighted()
                    .checkAllSchoolServiceNavItemsPresent()
                    .checkAllSchoolOverviewSubNavItemsPresent();
                schoolsPage
                    .checkSchoolReferenceNumbersHeaderPresent();
            });

            // Reference Numbers --> SEN (school)
            it('Reference numbers → SEN', () => {
                cy.visit(`/schools/overview/referencenumbers?urn=${navTestSchool.schoolURN}`);
                navigation
                    .clickSchoolsSENButton()
                    .checkCurrentURLIsCorrect(`/schools/overview/sen?urn=${navTestSchool.schoolURN}`)
                    .checkAllSchoolServiceNavItemsPresent()
                    .checkAllSchoolOverviewSubNavItemsPresent();
                schoolsPage
                    .checkSENSubpageHeaderCorrect();
            });

            // SEN --> school details (school)
            it('SEN → School details', () => {
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
            // academy details --> Reference Numbers (academy)
            it('Academy details → Reference numbers', () => {
                cy.visit(`/schools/overview/details?urn=${navTestAcademies[0].academyURN}`);
                navigation
                    .clickSchoolsReferenceNumberButton()
                    .checkCurrentURLIsCorrect(`/schools/overview/referencenumbers?urn=${navTestAcademies[0].academyURN}`)
                    .checkSchoolsReferenceNumbersButtonIsHighlighted()
                    .checkAllSchoolServiceNavItemsPresent()
                    .checkAllAcademyOverviewSubNavItemsPresent();
                schoolsPage
                    .checkSchoolReferenceNumbersHeaderPresent();
            });

            // Reference Numbers --> SEN (academy)
            it('Reference numbers → SEN', () => {
                cy.visit(`/schools/overview/referencenumbers?urn=${navTestAcademies[0].academyURN}`);
                navigation
                    .clickSchoolsSENButton()
                    .checkCurrentURLIsCorrect(`/schools/overview/sen?urn=${navTestAcademies[0].academyURN}`)
                    .checkAllSchoolServiceNavItemsPresent()
                    .checkAllAcademyOverviewSubNavItemsPresent();
                schoolsPage
                    .checkSENSubpageHeaderCorrect();
            });

            // SEN --> academy details (academy)
            it('SEN → Academy details', () => {
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
    });

    describe("School contacts edit navigation tests", () => {
        context('School contact edit page navigation', () => {
            it('Should check that the browser title is correct on the in DfE contacts page', () => {
                cy.visit(`/schools/contacts/in-dfe?urn=${navTestSchool.schoolURN}`);
                commonPage
                    .checkThatBrowserTitleMatches('In DfE - Contacts - Abbey Green Nursery School - Find information about schools and trusts');
            });

            it('Should check that the browser title is correct on the edit Regions group LA lead contact page', () => {
                cy.visit(`/schools/contacts/editregionsgrouplocalauthoritylead?urn=${navTestSchool.schoolURN}`);
                commonPage
                    .checkThatBrowserTitleMatches('Edit Regions group local authority lead details - Contacts - Abbey Green Nursery School - Find information about schools and trusts');
            });

            it('Should check that cancelling the edit returns to the correct page', () => {
                cy.visit(`/schools/contacts/in-dfe?urn=${navTestSchool.schoolURN}`);
                schoolsPage
                    .editRegionsGroupLaLeadWithoutSaving("Should Notbe Seen", "exittest@education.gov.uk")
                    .clickContactUpdateCancelButton();

                navigation
                    .checkCurrentURLIsCorrect(`/schools/contacts/in-dfe?urn=${navTestSchool.schoolURN}`);
            });
        });
    });
});
