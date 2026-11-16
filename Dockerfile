# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0@sha256:dccacae9e0e103b5d3ba5fbe442dd8d3316b990cd8f2ec9be0a0ec9db0c0e520 AS base
WORKDIR /app
EXPOSE 7297

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:11.0@sha256:65f3c7c6a25a239ab3e6719f2896147e7b68242ff2d8818dd70498bde6ef8bea AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["bobby-api.csproj", "."]
RUN dotnet restore "./bobby-api.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "./bobby-api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./bobby-api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS="http://*:7297"
USER app
ENTRYPOINT ["dotnet", "bobby-api.dll"]