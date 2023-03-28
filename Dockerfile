
# https://stackoverflow.com/questions/47103570/asp-net-core-2-0-multiple-projects-solution-docker-file
# https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/docker/visual-studio-tools-for-docker?view=aspnetcore-2.1#existing-app
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime

WORKDIR /app

EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

WORKDIR /src
COPY *.sln .
COPY src/RecipesDataProvider.API/*.csproj ./RecipesDataProvider.API/
COPY src/RecipesDataProvider.Domain/*.csproj ./RecipesDataProvider.Domain/
COPY src/RecipesDataProvider.Infrastructure/*.csproj ./RecipesDataProvider.Infrastructure/

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
