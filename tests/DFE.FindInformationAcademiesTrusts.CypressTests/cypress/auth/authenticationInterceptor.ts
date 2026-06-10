import { EnvAuthKey, EnvUrl } from "../support/cypressConstants";

export interface AuthenticationInterceptorParams {
    role?: string;
    username?: string;
}

export class AuthenticationInterceptor {

    register(params?: AuthenticationInterceptorParams) {
        const authKey = Cypress.env(EnvAuthKey) as string;

        cy.intercept(
            {
                url: `${Cypress.env(EnvUrl)}/**`,
                middleware: true,
            },
            (req) => {
                req.headers = {
                    ...req.headers,
                    Authorization: `Bearer ${authKey}`,
                    ...(params?.role && { 'X-test-role': params.role }),
                };
            }
        ).as('AuthInterceptor');
    }
}