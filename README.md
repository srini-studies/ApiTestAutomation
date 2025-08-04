# ApiTestAutomation
```
.Net xUnit Api Test Automation Project

Automated API tests using xUnit, .NET (C#), and HttpClient. 


Project Name:
    CategoryApiTestAutomation


This Test Project makes GET call to Api endpoint https://api.tmsandbox.co.nz/v1/Categories/6327/Details.json?catalogue=false
and Asserts the Acceptance Criteria is met.


Project Reposotory:
    git clone https://github.com/srini-studies/ApiTestAutomation.git


Technologies Used:
    .NET 8.0
    xUnit
    Microsoft Visual Studio Community 2022
    JsonNode
    FluentAssertions
    Github Actions Workflow


Project Design / Design Patterns:
    The project used Service Object Pattern
    The Api is encapsulated in Service class 
    Tests are separate to Api call
    This pattern ensures Separation od Concerns, Reusability, Maitainability and Readability


Project Structure:
    Constants
        ApiConstants            # API enpoint details
        CategoryConstants       # Data/Model Constants
    Service
        ApiService              # Api calls
    Settings
        ApiSettings             # Base Url config mapping
    Tests
        ApiTests                # Tests and Assertions
    appsettings.json            # Base Url and Logging
    ReadMe.md                   # Readme file


Set up the project:
    dotnet restore              # downloads dependent packages
    
Configuration:
    appsettings.json            # specify BaseUrl and Logging information
    ApiConstants.cs             # specify Api endpoint details 

Build: 
    dotnet build                # builds the project 

Running the Tests:
    dotnet run                  # runs the tests tagged with [Fact] attribute
   

CI/CD Integration:
    Github                      # Github with Github Actions Workflow

Github Actions Workflow:

name: .NET xUnit Test

on:
  push:
    branches: [ main ]

jobs:
  build-and-test:
    runs-on: ubuntu-latest

    steps:
    - name: Checkout code
      uses: actions/checkout@v4

    - name: Setup .NET SDK
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: '8.0.x'
    - name: Restore dependencies
      run: dotnet restore

    - name: Build solution
      run: dotnet build --no-restore --configuration Release

    - name: Run tests
      run: dotnet test --no-build --configuration Release --verbosity normal

```

