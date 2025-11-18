import searchPage from '../../../pages/searchPage';
import headerPage from '../../../pages/headerPage';
import { TestDataStore } from '../../../support/test-data-store';

describe('Header Search Tests', () => {

    describe('Header Search Presence Tests', () => {

        describe('School Pages', () => {
            const schoolUrn = 107188;

            TestDataStore.GetAllSchoolSubpagesForUrn(schoolUrn).forEach(({ pageName, subpages }) => {
                describe(`${pageName}`, () => {
                    subpages.forEach(({ subpageName, url }) => {
                        it(`Should have header search present - ${pageName} > ${subpageName}`, () => {
                            cy.visit(url);
                            headerPage.checkHeaderSearchTogglePresent();
                        });
                    });
                });
            });
        });

        describe('Trust Pages', () => {
            const trustUid = 5527;

            TestDataStore.GetAllTrustSubpagesForUid(trustUid).forEach(({ pageName, subpages }) => {
                describe(`${pageName}`, () => {
                    subpages.forEach(({ subpageName, url }) => {
                        it(`Should have header search present - ${pageName} > ${subpageName}`, () => {
                            cy.visit(url);
                            headerPage.checkHeaderSearchTogglePresent();
                        });
                    });
                });
            });
        });
    });

    describe('Header Search Functionality Tests', () => {

        const testPages = [
            {
                type: 'school',
                url: '/schools/overview/details?urn=107188',
                urnTest: { urn: 107188, name: 'Abbey Green Nursery School' }
            },
            {
                type: 'trust',
                url: '/trusts/overview/trust-details?uid=5527',
                urnTest: { urn: 123452, name: 'The Meadows Primary School' }
            }
        ];

        testPages.forEach(({ type, url, urnTest }) => {
            describe(`${type} page functionality`, () => {
                beforeEach(() => {
                    cy.visit(url);
                });

                it(`Should check that the header search bar and autocomplete is present and functional - ${type}`, () => {
                    headerPage
                        .clickHeaderSearchToggle()
                        .enterHeaderSearchText('West')
                        .checkHeaderSearchTogglePresent()
                        .checkHeaderAutocompleteIsPresent()
                        .checkHeaderSearchButtonPresent()
                        .checkAutocompleteContainsTypedText('West');
                });

                it(`Should check that the autocomplete does not return results when entry does not exist - ${type}`, () => {
                    headerPage
                        .clickHeaderSearchToggle()
                        .enterHeaderSearchText('KnowWhere')
                        .checkHeaderAutocompleteIsPresent()
                        .checkAutocompleteContainsTypedText('No results found');
                });

                it(`Checks that when a URN is entered the autocomplete lists the correct school - ${type}`, () => {
                    headerPage
                        .clickHeaderSearchToggle()
                        .enterHeaderSearchText(urnTest.urn.toString())
                        .checkHeaderAutocompleteIsPresent()
                        .checkAutocompleteContainsTypedText(urnTest.name);
                });

                it(`Should check that search results are returned with a valid name entered - ${type}`, () => {
                    const searchTerm = type === 'trust' ? 'TR02343' : 'west';
                    const expectedResult = type === 'trust' ? 'UNITED LEARNING TRUST' : 'west';

                    headerPage
                        .clickHeaderSearchToggle()
                        .enterHeaderSearchText(searchTerm)
                        .clickHeaderSearchButton();

                    searchPage
                        .checkSearchResultsReturned(expectedResult);
                });
            });
        });
    });
});
