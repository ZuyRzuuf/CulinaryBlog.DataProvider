# CulinaryBlog.RecipesDataProvider
Microservice is a part of CulinaryBlog Application. 
Works as a data provider for culinary blog recipes.
Uses connection to CulinaryBlog DB to get recipes data.
Is not accessible from the Internet. 

## Code
* ASP.NET Core Web Application
* .NET 6.0
* Language version: C# 10.0
* REST API

## Tests
* Unit Test: xUnit
* Integration Test: xUnit
* Mutation Test: Stryker

## Code formatting
### Use `dotnet format`
In the first step, the code should be formatted with the tool `dotnet format`:
```
dotnet format
```
More information about this tool can be found in the official doc: https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-format
### Use reSharper
As an additional step , the code can be formatted with the reSharper:
```
jb cleanupcode LD.ListingDisplayLogic.sln
```
#### Install reSharper CLI
To install ReSharper command line tools as a global .NET Core tool in the default location, run the following command line:
```
dotnet tool install -g JetBrains.ReSharper.GlobalTools
```
