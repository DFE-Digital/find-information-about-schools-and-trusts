import {EnvAuthKey, EnvUrl} from "../support/cypressConstants";

export class AuthenticationInterceptor {

    register(params?: AuthenticationInterceptorParams) {
        cy.env([EnvAuthKey]).then((envValues) => {
            const authKey = envValues[EnvAuthKey] as string;
            cy.intercept(
                {
                    url: Cypress.expose(EnvUrl) + "/**",
                    middleware: true,
                },
                (req) => {
                    req.headers = {
                        ...req.headers,
                        'Authorization': `Bearer ${authKey}`,
                        ...(params?.role && {'X-test-role': params.role}),
                    };
                }
            ).as("AuthInterceptor");
        });
    }
}

export interface AuthenticationInterceptorParams {
    role?: string;
    username?: string;
}
