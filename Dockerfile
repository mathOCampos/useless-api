# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# Copy csproj and restore dependencies
COPY UsersAPI/*.csproj ./UsersAPI/
RUN dotnet restore UsersAPI/*.csproj

# Copy all files and build
COPY UsersAPI/. ./UsersAPI/
RUN dotnet publish UsersAPI/*.csproj -c Release -o /app --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Create non-root user
RUN adduser --system --group appuser

# Copy built application
COPY --from=build /app ./
USER appuser

# Configure for production
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 80

ENTRYPOINT ["dotnet", "UsersAPI.dll"]
