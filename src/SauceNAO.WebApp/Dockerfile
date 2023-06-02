#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["SauceNAO.WebApp/SauceNAO.WebApp.csproj", "SauceNAO.WebApp/"]
COPY ["SauceNAO.Infrastructure/SauceNAO.Infrastructure.csproj", "SauceNAO.Infrastructure/"]
COPY ["SauceNAO.Core/SauceNAO.Core.csproj", "SauceNAO.Core/"]
RUN dotnet restore "SauceNAO.WebApp/SauceNAO.WebApp.csproj"
COPY . .
WORKDIR "/src/SauceNAO.WebApp"
RUN dotnet build "SauceNAO.WebApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SauceNAO.WebApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SauceNAO.WebApp.dll"]