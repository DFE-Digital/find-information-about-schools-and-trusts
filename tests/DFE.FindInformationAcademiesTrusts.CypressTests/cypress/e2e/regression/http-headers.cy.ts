describe('Testing HTTP Headers', () => {
    describe("Testing the homepage", () => {
        beforeEach(() => {
            cy.login();
        });

        it("Should check that the home page serves the correct HTTP Headers in the response", () => {
            cy.request('/').then((response) => {
                expect(response.status).to.eq(200)
                expect(response).to.have.property('headers')
                expect(response.headers).to.have.property('Strict-Transport-Security')
                expect(response.headers).to.have.property('X-Xss-Protection')
                expect(response.headers).to.have.property('X-Frame-Options')
                expect(response.headers).to.have.property('X-Content-Type-Options')
                expect(response.headers).to.have.property('Content-Security-Policy')
            })
        });
    })
})
