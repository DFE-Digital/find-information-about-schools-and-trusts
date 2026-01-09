const fs = require('fs')
const path = require('path')

const filePath = path.resolve('node_modules/@ministryofjustice/frontend/moj/vendor/govuk-frontend/_base.scss')

// Read the SCSS file
fs.readFile(filePath, 'utf8', (err, data) => {
  if (err) {
    console.error('Error reading SCSS file:', err)
    return
  }

  // Modify the import path by removing the node_modules/ part
  const result = data.replace(
    'node_modules/govuk-frontend/dist/govuk/base',
    'govuk-frontend/dist/govuk/base'
  )

  // Write the modified content back to the file
  fs.writeFile(filePath, result, 'utf8', (err) => {
    if (err) {
      console.error('Error writing SCSS file:', err)
    } else {
      console.log('SCSS import path fixed successfully.')
    }
  })
})
