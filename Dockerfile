# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# Copy csproj and restore dependencies
COPY backend.csproj ./
RUN dotnet restore

# Copy all source files
COPY Src/ ./Src/
COPY Migrations/ ./Migrations/
COPY appsettings.json ./

# Build and publish
RUN dotnet publish -c Release -o out

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/out .

# Install the browser version matched to the Microsoft.Playwright package.
ENV PLAYWRIGHT_BROWSERS_PATH=/ms-playwright
RUN ./.playwright/node/linux-x64/node ./.playwright/package/cli.js install --with-deps chromium

# Expose the ASP.NET Core container port.
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "backend.dll"]
