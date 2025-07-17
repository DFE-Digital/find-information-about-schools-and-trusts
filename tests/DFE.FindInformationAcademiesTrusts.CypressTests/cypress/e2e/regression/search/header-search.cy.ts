import searchPage from '../../../pages/searchPage';
import headerPage from '../../../pages/headerPage';

describe(`Testing the components of the header search`, () => {

    [`/schools/overview/details?urn=123452`, `/trusts/overview/trust-details?uid=5527`].forEach((url) => {
        beforeEach(() => {
            cy.visit(url);
        });

        describe(`Checking that the header search bar and autocomplete is present and functional for ${url}`, () => {

            it(`Should check that the header search bar and autocomplete is present and functional for ${url})`, () => {
                headerPage
                    .enterHeaderSearchText(`West`)
                    .checkHeaderAutocompleteIsPresent()
                    .checkHeaderSearchButtonPresent()
                    .checkAutocompleteContainsTypedText(`West`);
            });

            it(`Should check that the autocomplete does not return results when entry does not exist for ${url}`, () => {
                headerPage
                    .enterHeaderSearchText(`KnowWhere`)
                    .checkHeaderAutocompleteIsPresent()
                    .checkAutocompleteContainsTypedText(`No results found`);
            });

            it('Checks that when a URN is entered the autocomplete lists the correct school', () => {
                headerPage
                    .enterHeaderSearchText('123452')
                    .checkHeaderAutocompleteIsPresent()
                    .checkAutocompleteContainsTypedText('The Meadows Primary School');
            });
        });

        describe(`Checking the search results functionality for ${url}`, () => {
            it(`Should check that search results are returned with a valid name entered when using the header search bar for ${url}`, () => {
                headerPage
                    .enterHeaderSearchText(`west`)
                    .clickHeaderSearchButton();

                searchPage
                    .checkSearchResultsReturned('west');
            });

            it(`Should return the correct trust when searching by TRN using the header search for ${url}`, () => {
                headerPage
                    .enterHeaderSearchText(`TR02343`)
                    .clickHeaderSearchButton();

                searchPage
                    .checkSearchResultsReturned(`UNITED LEARNING TRUST`);
            });
        });
    });
});
