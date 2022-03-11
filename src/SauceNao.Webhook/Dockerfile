#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["SauceNAO.Webhook/SauceNAO.Webhook.csproj", "SauceNAO.Webhook/"]
COPY ["SauceNAO.Infrastructure/SauceNAO.Infrastructure.csproj", "SauceNAO.Infrastructure/"]
COPY ["SauceNAO.Core/SauceNAO.Core.csproj", "SauceNAO.Core/"]
RUN dotnet restore "SauceNAO.Webhook/SauceNAO.Webhook.csproj"
COPY . .
WORKDIR "/src/SauceNAO.Webhook"
RUN dotnet build "SauceNAO.Webhook.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SauceNAO.Webhook.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SauceNAO.Webhook.dll"]