import homePage from '../../../pages/homePage';
import localAuthoritiesPage from '../../../pages/local-authorities/localAuthoritiesPage';

describe('Testing the main/home page search functionality', () => {
  beforeEach(() => {
    cy.visit("/");
    homePage
      .verifyTrustAndSchoolTabIsDefault()
      .verifyHeading('Find information about schools and trusts')
      .clickLocalAuthoritiesTab();
  });

  describe('Local authority search', () => {
    const localAuthorityName = 'Leeds';
    const localAuthorityCode = '383';

    it('returns a matching local authority when searching by name', () => {
      localAuthoritiesPage
        .searchFor(localAuthorityName)
        .assertSearchResultsContain(localAuthorityName);
    });

    it('returns matching local authorities when searching by partial name', () => {
      localAuthoritiesPage
        .searchFor('Lee')
        .assertSearchResultsContain(localAuthorityName);
    });

    it('returns matching results regardless of search text casing', () => {
      localAuthoritiesPage
        .searchFor('lEeDs')
        .assertSearchResultsContain(localAuthorityName);
    });

    it('returns the matching local authority when searching by LA code', () => {
      localAuthoritiesPage
        .searchFor(localAuthorityCode)
        .assertSearchResultsContain(localAuthorityName);
    });

    it('displays a no-results message for an unknown local authority', () => {
      localAuthoritiesPage
        .assertNoResultsMessageVisible('nonla12345');
    });

    it('displays a no-results message for an empty la search', () => {
      localAuthoritiesPage
        .assertNoResultsMessageVisible('');
    });
  });
});
