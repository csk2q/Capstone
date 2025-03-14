# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Include PostgreSQL in the image
FROM postgres:latest AS db

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ServerBee/ServerBee.csproj", "ServerBee/"]
RUN dotnet restore "./ServerBee/ServerBee.csproj"
COPY . .
WORKDIR "/src/ServerBee"
RUN dotnet build "./ServerBee.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ServerBee.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Create migration script for inclusion (Would require installing ef-tools earlier in build)
# RUN dotnet ef migrations script --idempotent -o "/app/publish/migration.sql"
# Either manually generate SQL script and manually apply to database.

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ServerBee.dll"]