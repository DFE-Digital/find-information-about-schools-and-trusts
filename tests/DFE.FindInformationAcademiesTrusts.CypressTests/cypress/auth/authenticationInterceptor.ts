
export class AuthenticationInterceptor {

    register(params?: AuthenticationInterceptorParams) {
        cy.intercept(
            {
                url: Cypress.env("URL") + "/**",
                middleware: true,
            },
            (req) => {
                // Set an auth header on every request made by the browser
                req.headers = {
                    ...req.headers,
                    'Authorization': `Bearer ${Cypress.env("AUTH_KEY")}`
                };
            }
        ).as("AuthInterceptor");
    }
}

export interface AuthenticationInterceptorParams {
    role?: string;
    username?: string;
}
