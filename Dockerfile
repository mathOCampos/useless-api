# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# Create NuGet.config
RUN echo '<?xml version="1.0" encoding="utf-8"?>\n\
<configuration>\n\
  <packageSources>\n\
    <clear />\n\
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />\n\
  </packageSources>\n\
</configuration>' > NuGet.config

# Copy csproj and restore dependencies
COPY UsersAPI/*.csproj ./UsersAPI/

# Restore packages
WORKDIR /source/UsersAPI
RUN dotnet restore

# Copy everything else and build
COPY UsersAPI/. .
RUN dotnet publish -c Release -o /app

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app ./

# Create non-root user
RUN adduser --system --group appuser
USER appuser

ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 80

ENTRYPOINT ["dotnet", "UsersAPI.dll"]