## Hosting FileValidator.Blazor project in [Express](https://expressjs.com/)

Since FileValidator.Blazor geberates a WebAseembly client it can be hosted as static files.

**Install Express**
```
npm install express
```

Or install it without saving it to the dependencies list
```
npm install express --no-save
```

## Hosting FileValidator.Blazor

**Build and Publish the project**

Execute in the project folder
```
dotnet publish -c Release
```

Files are published to `DataProcessor\FileValidator.Blazor\bin\Release\net6.0\publish\wwwroot`

**Start Express**
```
node app.js
```

