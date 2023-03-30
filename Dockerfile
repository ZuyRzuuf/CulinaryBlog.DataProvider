
# https://stackoverflow.com/questions/47103570/asp-net-core-2-0-multiple-projects-solution-docker-file
# https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/docker/visual-studio-tools-for-docker?view=aspnetcore-2.1#existing-app
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base

WORKDIR /app

EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

COPY *.sln .

WORKDIR /src
COPY src/RecipesDataProvider.API/*.csproj ./RecipesDataProvider.API/
COPY src/RecipesDataProvider.Domain/*.csproj ./RecipesDataProvider.Domain/
COPY src/RecipesDataProvider.Infrastructure/*.csproj ./RecipesDataProvider.Infrastructure/

WORKDIR /test
COPY test/RecipesDataProvider.API.Test.Integration/*.csproj ./RecipesDataProvider.API.Test.Integration/
COPY test/RecipesDataProvider.API.Test.Unit/*.csproj ./RecipesDataProvider.API.Test.Unit/
COPY test/RecipesDataProvider.Domain.Test.Integration/*.csproj ./RecipesDataProvider.Domain.Test.Integration/
COPY test/RecipesDataProvider.Domain.Test.Unit/*.csproj ./RecipesDataProvider.Domain.Test.Unit/
COPY test/RecipesDataProvider.Infrastructure.Test.Integration/*.csproj ./RecipesDataProvider.Infrastructure.Test.Integration/
COPY test/RecipesDataProvider.Infrastructure.Test.Unit/*.csproj ./RecipesDataProvider.Infrastructure.Test.Unit/

WORKDIR /
RUN dotnet restore

COPY . .

WORKDIR /src/RecipesDataProvider.API
RUN dotnet build -c Release -o /app

WORKDIR /src/RecipesDataProvider.Domain
RUN dotnet build -c Release -o /app

WORKDIR /src/RecipesDataProvider.Infrastructure
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS runtime

WORKDIR /app
COPY --from=publish /app .

ENTRYPOINT ["dotnet", "RecipesDataProvider.API.dll"]
