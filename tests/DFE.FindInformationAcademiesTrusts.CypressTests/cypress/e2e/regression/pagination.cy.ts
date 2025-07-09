import commonPage from "../../pages/commonPage";
import navigation from "../../pages/navigation";
import paginationPage from "../../pages/paginationPage";

describe('Pagination Tests', () => {


    beforeEach(() => {
        cy.visit('/search?keywords=west');
    });

    it('Should display multiple pagination buttons', () => {
        paginationPage.getTotalPaginationButtons().should('be.greaterThan', 1);
    });


    it('Should navigate to a specific page when a page number is clicked on a large result page', () => {

        cy.visit('/search?keywords=tru');
        paginationPage
            .clickPageNumber(2);
        navigation
            .checkCurrentURLIsCorrect('pagenumber=2');
        commonPage
            .checkThatBrowserTitleMatches('Search (page 2 of 72) - tru - Find information about schools and trusts');

        paginationPage
            .clickPageNumber(3);

        navigation
            .checkCurrentURLIsCorrect('pagenumber=3');
        commonPage
            .checkThatBrowserTitleMatches('Search (page 3 of 72) - tru - Find information about schools and trusts');

        paginationPage
            .clickPageNumber(72);

        navigation
            .checkCurrentURLIsCorrect('pagenumber=72');
        commonPage
            .checkThatBrowserTitleMatches('Search (page 72 of 72) - tru - Find information about schools and trusts');
    });

    it('Should navigate to the next page on next button click', () => {
        paginationPage
            .clickNext();

        navigation
            .checkCurrentURLIsCorrect('pagenumber=2');

        paginationPage
            .clickNext();

        navigation
            .checkCurrentURLIsCorrect('pagenumber=3');

    });

    it('Should navigate to the previous page on previous button click', () => {
        paginationPage
            .clickNext();

        navigation
            .checkCurrentURLIsCorrect('pagenumber=2');

        paginationPage
            .clickPrevious();

        navigation
            .checkCurrentURLIsCorrect('pagenumber=1');

    });

    it('Checks that the previous page button is not present on the first page of results', () => {
        paginationPage
            .checkPreviousButtonNotPresent();
    });

    it('Checks that the next page button is not present on the first page of results', () => {
        paginationPage
            .clickPageNumber(21)
            .checkNextButtonNotPresent();
    });

    it('Checks that the previous and next page buttons are not present on the no results found page', () => {
        cy.visit('/search?keywords=knowhere');

        paginationPage
            .checkPreviousButtonNotPresent()
            .checkNextButtonNotPresent();
    });

    it('Checks that I see the pages I would expect mid pagination and dont see the ones that should be hidden', () => {
        cy.visit('/search?keywords=tru&pagenumber=32');

        paginationPage
            .checkExpectedPageNumberInPaginationBar(1)
            .checkResultIsNotInPaginationBar(2)
            .checkResultIsNotInPaginationBar(30)
            .checkExpectedPageNumberInPaginationBar(31)
            .checkExpectedPageNumberInPaginationBar(32)
            .checkExpectedPageNumberInPaginationBar(33)
            .checkResultIsNotInPaginationBar(34)
            .checkExpectedPageNumberInPaginationBar(72)
            .checkResultIsNotInPaginationBar(74);
    });

    it('Checks that on a single result page only the page number is present', () => {
        cy.visit('/search?keywords=henley-in-arden');

        paginationPage
            .checkPreviousButtonNotPresent()
            .checkNextButtonNotPresent()
            .checkSingleResultOnlyHasOnePage(1);

        commonPage
            .checkThatBrowserTitleMatches('Search - henley-in-arden - Find information about schools and trusts');
    });

    it('Should navigate to the previous page on previous button click', () => {
        paginationPage
            .clickNext();

        navigation
            .checkCurrentURLIsCorrect('pagenumber=2');

        paginationPage.getResults().then(secondPageResults => {
            const secondPageFirstResultText = secondPageResults.first().text();

            paginationPage.clickPrevious();

            navigation
                .checkCurrentURLIsCorrect('pagenumber=1');

            paginationPage.getResults().first().should('not.have.text', secondPageFirstResultText);
        });
    });

    it('Should iterate through all pagination pages and verify results are different', () => {
        paginationPage.getTotalPaginationButtons().then(totalPages => {
            let previousFirstResultText = '';

            for (let page = 1; page <= totalPages; page++) {
                paginationPage.clickPageNumber(page);

                paginationPage.getResults().first().then(currentFirstResult => {
                    const currentFirstResultText = currentFirstResult.text();

                    if (page > 1) {
                        expect(currentFirstResultText).to.not.equal(previousFirstResultText);
                    }

                    previousFirstResultText = currentFirstResultText;
                });
            }
        });
    });
});
