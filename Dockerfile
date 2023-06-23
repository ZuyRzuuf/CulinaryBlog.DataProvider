
# https://stackoverflow.com/questions/47103570/asp-net-core-2-0-multiple-projects-solution-docker-file
# https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/docker/visual-studio-tools-for-docker?view=aspnetcore-2.1#existing-app
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base

WORKDIR /app

EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

COPY *.sln .

WORKDIR /src
COPY src/DataProvider.API/*.csproj ./DataProvider.API/
COPY src/DataProvider.Domain/*.csproj ./DataProvider.Domain/
COPY src/DataProvider.Infrastructure/*.csproj ./DataProvider.Infrastructure/

WORKDIR /test
COPY test/DataProvider.API.Test.Integration/*.csproj ./DataProvider.API.Test.Integration/
COPY test/DataProvider.API.Test.Unit/*.csproj ./DataProvider.API.Test.Unit/
COPY test/DataProvider.Domain.Test.Integration/*.csproj ./DataProvider.Domain.Test.Integration/
COPY test/DataProvider.Domain.Test.Unit/*.csproj ./DataProvider.Domain.Test.Unit/
COPY test/DataProvider.Infrastructure.Test.Integration/*.csproj ./DataProvider.Infrastructure.Test.Integration/
COPY test/DataProvider.Infrastructure.Test.Unit/*.csproj ./DataProvider.Infrastructure.Test.Unit/

WORKDIR /
RUN dotnet restore

COPY . .

WORKDIR /src/DataProvider.API
RUN dotnet build -c Release -o /app

WORKDIR /src/DataProvider.Domain
RUN dotnet build -c Release -o /app

WORKDIR /src/DataProvider.Infrastructure
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS runtime

WORKDIR /app
COPY --from=publish /app .

ENTRYPOINT ["dotnet", "DataProvider.API.dll"]
