const express = require('express')
const app = express()
const port = 3000

// app.get('/', (req, res) => {
//   res.send('Hello World!')
// })

app.use(express.static('.\\..\\DataProcessor\\FileValidator.Blazor\\bin\\Release\\net6.0\\publish\\wwwroot'))

app.listen(port, () => {
  console.log(`Example app listening at http://localhost:${port}`)
})