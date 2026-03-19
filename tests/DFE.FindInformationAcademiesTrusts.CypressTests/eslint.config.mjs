import eslint from '@eslint/js';
import pluginCypress from 'eslint-plugin-cypress'
import tseslint from 'typescript-eslint';

export default tseslint.config(
  eslint.configs.recommended,
  tseslint.configs.recommendedTypeChecked,
  tseslint.configs.stylisticTypeChecked,
  {
    languageOptions: {
      parserOptions: {
        projectService: true,
        tsconfigRootDir: import.meta.dirname,
      },
    },
  },
  pluginCypress.configs.recommended,
  {
    rules: {
      'cypress/no-async-before': 'error',
      'cypress/assertion-before-screenshot': 'error',
      'cypress/no-force': 'warn',
      'cypress/no-pause': 'error',
      'cypress/no-debug': 'error'
    }
  },
  {
    files: ['eslint.config.mjs'],
    extends: [tseslint.configs.disableTypeChecked],
  },
);
