import homePage from '../../../pages/homePage';
import watchlistPage from '../../../pages/watchlist/watchlistPage';

describe('User navigates to the FAST site and by default \'Trust and school\' tab is displayed', () => {
    beforeEach(() => {
        cy.visit("/");

    });

    describe('Navigating the watchlist', () => {

        it('user should be able to navigate to the Watchlist tab', () => {
            homePage
                .verifyTrustAndSchoolTabIsDefault()
                .verifyHeading('Find information about schools and trusts')
                .clickWatchlistLink();

            watchlistPage
                .watchlistPageAppears();
        });

        it('user should be able to navigate to the Trust and school tab', () => {
            homePage
                .clickWatchlistLink();

            watchlistPage
                .verifyHeading('My watchlist')
                .schoolTabAppearsByDefault()
                .schoolTabContainsContent()
                .navigateToTrustsTab()
                .trustTabAppearsWithContent();
        });

        it('user should be able to navigate to the School tab from the Trust tab', () => {
            homePage
                .clickWatchlistLink();

            watchlistPage
                .schoolTabAppearsByDefault()
                .navigateToTrustsTab()
                .navigateToSchoolTab();
        });

        it('user should be able to add establishment - School to watchlist', () => {
            homePage
                .clickWatchlistLink();

            watchlistPage
                .clickAddToWatchlistButton()
                .selectSchoolEstablishmentType()
                .clickContinue()
                .searchForSchool('Fleet Primary School')
                .clickContinue()
                .confirmSchoolDetails('School name, URN,Trust, Local authority')
                .verifyButtonsOnConfirmationPage()
                .clickSaveAndComplete()
                .successMessageAppears('School added to watchlist')
                .schoolAppearsInWatchlistTable('Fleet Primary School');
        });

        it('user should be able to sort the columns in the School tab', () => {
            homePage
                .clickWatchlistLink();

            watchlistPage
                .schoolTabAppearsByDefault()
                .sortColumnsInSchoolTab();
        });

        it('user should navigate to the school overview pagewhen selected from the schools watchlist records', () => {

            homePage
                .clickWatchlistLink();

            watchlistPage
                .schoolTabAppearsByDefault()
                .clickFirstSchoolInWatchlist()
                .schoolPageAppears();
        });

        it('user should be able to remove any added school from watchlist', () => {
            homePage
                .clickWatchlistLink();

            watchlistPage
                .removeSchoolFromWatchlist()
                .successMessageAppears('School removed from watchlist');
        });

        it('user should not be able to find establishment - College or university in School Search', () => {
            homePage
                .clickWatchlistLink();

            watchlistPage
                .clickAddToWatchlistButton()
                .selectSchoolEstablishmentType()
                .clickContinue()
                .searchInvalidSchool('127066')
                .clickContinueButton()
                .verifyValidationMessageAppears('We could not find any schools matching your search criteria')

        });

        it('user should be able to add Trust to watchlist', () => {
            homePage
                .clickWatchlistLink();

            watchlistPage
                .clickAddToWatchlistButton()
                .selectTrustEstablishmentType()
                .clickContinue()
                .searchForTrust('UNITED LEARNING TRUST')
                .clickContinue()
                .confirmTrustDetails('Trust name, TRN, Region, Companies house number')
                .verifyButtonsOnConfirmationPage()
                .clickSaveAndComplete()
                .watchlistPageAppears()
                .successMessageAppears('Trust added to watchlist')
                .trustCountIncreases();
        });

        it('user should not be able to find an invalid trust trn in Trust search', () => {
            homePage
                .clickWatchlistLink();

            watchlistPage
            watchlistPage
                .clickAddToWatchlistButton()
                .selectTrustEstablishmentType()
                .clickContinue()
                .searchInvalidTrust('trn98701')
                .clickContinueButton()
                .verifyValidationMessageAppears('We could not find any trusts matching your search criteria')

        });

        it('user should be able to sort the columns in the Trust tab', () => {
            homePage
                .clickWatchlistLink();

            watchlistPage
                .navigateToTrustsTab()
                .sortColumnsInTrustTab();
        });

        it('user should navigate to the trust when selected from the trusts watchlist records', () => {
            homePage
                .clickWatchlistLink();

            watchlistPage
                .navigateToTrustsTab()
                .clickFirstTrustInWatchlist()
                .trustPageAppears();
        });

        it('user should be able to remove any trust from watchlist', () => {
            homePage
                .clickWatchlistLink();

            watchlistPage
                .navigateToTrustsTab()
                .removeTrustFromWatchlist()
                .successMessageAppears('Trust removed from watchlist');
        });
    });
});
