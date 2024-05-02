#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
RUN apt update && apt upgrade -y
RUN apt install ffmpeg -y

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["WebApp/SauceNAO.WebApp.csproj", "WebApp/"]
COPY ["Core/Application/SauceNAO.Application.csproj", "Core/Application/"]
COPY ["Core/Domain/SauceNAO.Domain.csproj", "Core/Domain/"]
COPY ["Infrastructure/SauceNAO.Infrastructure.csproj", "Infrastructure/"]
COPY ["Directory.Packages.props", "Directory.Build.props", "./"]
RUN dotnet restore "./WebApp/SauceNAO.WebApp.csproj"
COPY . .
WORKDIR "/src/WebApp"
RUN dotnet build "./SauceNAO.WebApp.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SauceNAO.WebApp.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS runtime
USER root
WORKDIR /app
EXPOSE 8080
EXPOSE 8081
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SauceNAO.WebApp.dll"]
