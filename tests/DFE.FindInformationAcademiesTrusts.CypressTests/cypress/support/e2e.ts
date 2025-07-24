/* eslint-disable @typescript-eslint/no-namespace */ //required to enable custom Cypress commands typescript support
import { AuthenticationInterceptorParams } from '../auth/authenticationInterceptor';
import './commands';
import 'wick-a11y';

declare global {
    namespace Cypress {
        interface Chainable {
            login(params?: AuthenticationInterceptorParams): Chainable<Element>;

            /**
             * Expands a details element
             * 
             * As of Jan 2025, Cypress is unable to expand a HTML details element using a click. This is a workaround.
             */
            expandDetailsElement(): Cypress.Chainable<JQuery<HTMLElement>>;

            /**
             * Checks accessibility of the page or specific elements using wick-a11y
             * @param context - CSS selector, DOM element, or object to define scope
             * @param options - Configuration options for the accessibility check
             */
            checkAccessibility(context?: unknown, options?: unknown): Chainable<void>;
        }
    }
}

//Always log in before every test
beforeEach(() => {
    cy.login();
});
