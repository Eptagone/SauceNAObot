#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["SauceNAO.Service/SauceNAO.Service.csproj", "SauceNAO.Service/"]
COPY ["SauceNAO.Infrastructure/SauceNAO.Infrastructure.csproj", "SauceNAO.Infrastructure/"]
COPY ["SauceNAO.Core/SauceNAO.Core.csproj", "SauceNAO.Core/"]
RUN dotnet restore "SauceNAO.Service/SauceNAO.Service.csproj"
COPY . .
WORKDIR "/src/SauceNAO.Service"
RUN dotnet build "SauceNAO.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SauceNAO.Service.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SauceNAO.Service.dll"]