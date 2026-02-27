describe('Trust export and content verification', () => {
  beforeEach(() => {
    cy.visit('/trusts/academies/in-trust/details?uid=5712');

    // Clear the downloads folder before running each test
    cy.task('checkForFiles', 'cypress/downloads').then((files) => {
      if (files) {
        cy.task('clearDownloads', 'cypress/downloads');
      }
    });
  });
});
