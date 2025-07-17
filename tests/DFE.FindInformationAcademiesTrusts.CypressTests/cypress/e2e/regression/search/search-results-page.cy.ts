import searchPage from "../../../pages/searchPage";
import homePage from "../../../pages/homePage";
import { testSchoolData } from "../../../support/test-data-store";
import commonPage from "../../../pages/commonPage";

describe("Testing the components of the search results page", () => {

    beforeEach(() => {
        cy.visit('/');
    });

    it("Should check that the search page returns result no found when something does not exist", () => {
        homePage
            .enterMainSearchText("KnowWhere")
            .clickMainSearchButton();

        searchPage
            .checkNoSearchResultsFound();
    });

    it("Checks that the user can edit their search and search for a new trust or school from the search page", () => {
        homePage
            .enterMainSearchText("West")
            .clickMainSearchButton();

        searchPage
            .checkSearchResultsReturned("West");

        searchPage
            .enterSearchResultsSearchText("East")
            .clickSearchPageSearchButton();

        searchPage
            .checkSearchResultsReturned("East");
    });

    it("Validates that it returns the amount of results stated in the search text", () => {
        homePage
            .enterMainSearchText("West")
            .clickMainSearchButton();

        searchPage
            .validateSearchResultsCountWithPagination();
    });

    it("Should return the correct trust when searching by TRN", () => {
        homePage
            .enterMainSearchText("TR02343")
            .clickMainSearchButton();

        searchPage
            .checkSearchResultsReturned("UNITED LEARNING TRUST");
    });

    it("Should display 'no results found' when searching with a non-existent TRN", () => {
        homePage
            .enterMainSearchText("TR99999")
            .clickMainSearchButton();

        searchPage
            .checkNoSearchResultsFound();
    });

    it("Should allow searching by TRN from the search results page", () => {
        homePage
            .enterMainSearchText("West")
            .clickMainSearchButton();

        searchPage
            .checkSearchResultsReturned("West");

        searchPage
            .enterSearchResultsSearchText("TR02343")
            .clickSearchPageSearchButton();

        searchPage
            .checkSearchResultsReturned("UNITED LEARNING TRUST");
    });

    it("Should return the correct trust when searching with a partial TRN", () => {
        homePage
            .enterMainSearchText("TR0234")
            .clickMainSearchButton();

        searchPage
            .checkSearchResultsReturned("UNITED LEARNING TRUST");
    });

    it(`When searching on a trusts TRN it should display the correct trust count and not return school numbers`, () => {
        homePage
            .enterMainSearchText("TR02343")
            .clickMainSearchButton();

        searchPage
            .checkSearchResultsReturned("UNITED LEARNING TRUST")
            .checkSearchResultsInfoReturnsCorrectInfo("Found 1 trusts and 0 schools, including academies");
    });

    it('Checks that you cant go to a url for a search results page that does not exist', () => {
        [`/search?keywords=west&pagenumber=1000`, `/search?keywords=west&pagenumber=0`].forEach((url) => {

            commonPage
                .interceptAndVerifyResponseHas404Status(url);

            cy.visit(url, { failOnStatusCode: false });

            cy.wait('@checkTheResponseIs404');

            commonPage
                .checkPageNotFoundDisplayed();
        });
    });

    testSchoolData.forEach(({ typeOfSchool, urn, schoolName }) => {
        it(`When searching on a schools URN it should display the correct school and not return any trusts for a ${typeOfSchool}`, () => {
            homePage
                .enterMainSearchText(urn.toString())
                .clickMainSearchButton();

            searchPage
                .checkSearchResultsReturned(schoolName)
                .checkSearchResultsInfoReturnsCorrectInfo("Found 0 trusts and 1 schools, including academies");
        });

        it("Validates that the schools correct type and URN are shown in its results overview details", () => {
            homePage
                .enterMainSearchText(urn.toString())
                .clickMainSearchButton();

            searchPage
                .checkSearchResultsReturned(schoolName)
                .checkEstablishmentType(typeOfSchool)
                .checkCorrectURN(urn.toString());
        });
    });

});
