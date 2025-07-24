class DataDownload {
    elements = {
        downloadButton: () => cy.get('[data-testid="export-academy-data"]'),
    };

    // Method to click the download button
    public clickDownloadButton(): this {
        this.elements.downloadButton().click();
        return this;
    }

    // Method to find the latest downloaded file
    public findLatestDownloadedFile(): Cypress.Chainable<string> {
        return cy.task('findLatestFile', 'cypress/downloads').then((latestFile) => {
            if (!latestFile) {
                throw new Error('No downloaded file found');
            }
            return latestFile as string;
        });
    }

    // Method to check if the file is downloaded
    public checkFileDownloaded(): this {
        this.findLatestDownloadedFile().then((downloadFilePath) => {
            cy.readFile(downloadFilePath, 'binary', { timeout: 10000 }).should('exist');
        });
        return this;
    }

    // Method to check if the file has content
    public checkFileHasContent(): this {
        this.findLatestDownloadedFile().then((downloadFilePath) => {
            cy.readFile(downloadFilePath, 'binary', { timeout: 10000 }).then((fileContent) => {
                expect(fileContent.length).to.be.greaterThan(0);
            });
        });
        return this;
    }

    // Method to delete the downloaded file
    public deleteDownloadedFile(): this {
        this.findLatestDownloadedFile().then((downloadFilePath) => {
            cy.task<{ success: boolean; message?: string }>('deleteFile', downloadFilePath).then((result) => {
                if (!result.success) {
                    cy.log(result.message ?? 'Failed to delete file');
                }
            });
        });
        return this;
    }
}

const dataDownload = new DataDownload();
export default dataDownload;
