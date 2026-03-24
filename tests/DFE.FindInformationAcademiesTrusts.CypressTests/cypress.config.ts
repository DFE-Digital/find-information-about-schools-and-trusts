/* eslint-disable @typescript-eslint/no-unsafe-assignment */
/* eslint-disable @typescript-eslint/no-unsafe-call */
/* eslint-disable @typescript-eslint/no-require-imports */

import { defineConfig } from "cypress";
import fs from 'fs';
import path from 'path';

module.exports = defineConfig({
  userAgent: 'FindInformationAcademiesTrusts/1.0 Cypress',
  // Runtime environment variables
  env: {
    // Enable accessibility voice for interactive testing
    enableAccessibilityVoice: false,
  },
  e2e: {
    allowCypressEnv: false,
    trashAssetsBeforeRuns: false,
    experimentalRunAllSpecs: true,
    reporter: 'cypress-multi-reporters',
    reporterOptions: {
      reporterEnabled: 'mochawesome',
      mochawesomeReporterOptions: {
        reportDir: 'cypress/reports/mocha',
        quite: true,
        overwrite: false,
        html: false,
        json: true,
      }
    },

    setupNodeEvents(on, config) {
      config.baseUrl = config.env.URL as string;

      // Add accessibility tasks from wick-a11y
      const addAccessibilityTasks = require('wick-a11y/accessibility-tasks');
      addAccessibilityTasks(on);

      on('task', {
        // Custom task to find the most recent .xlsx file in the downloads folder
        findLatestFile(folderPath: string) {
          const files = fs.readdirSync(folderPath);
          const xlsxFiles = files.filter((file: string) => file.endsWith('.xlsx'));

          if (xlsxFiles.length === 0) return null;

          // Sort files by modified date, latest first
          const latestFile = xlsxFiles
            .map((fileName: string) => ({
              name: fileName,
              time: fs.statSync(path.join(folderPath, fileName)).mtime.getTime()
            }))
            .sort((a: { time: number; }, b: { time: number; }) => b.time - a.time)[0];

          return path.join(folderPath, latestFile.name);
        },

        // Custom task to delete a file
        deleteFile(filePath: string) {
          if (fs.existsSync(filePath)) {
            fs.unlinkSync(filePath);
            return { success: true };
          }
          return { success: false, message: 'File not found' };
        },

        // Custom task to check if files exist in the downloads folder
        checkForFiles(folderPath: string) {
          if (fs.existsSync(folderPath)) {
            const files = fs.readdirSync(folderPath);
            return files.length > 0 ? files : null;
          }
          return null;
        },

        // Custom task to delete all files in the downloads folder
        clearDownloads(folderPath: string) {
          if (fs.existsSync(folderPath)) {
            const files = fs.readdirSync(folderPath);
            files.forEach((file: string) => {
              fs.unlinkSync(path.join(folderPath, file));
            });
          }
          return { success: true };
        }
      });

      return config;
    },

    downloadsFolder: 'cypress/downloads',
  },
});
