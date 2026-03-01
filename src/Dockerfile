# SauceNAO WebApp
FROM debian:stable-slim AS base
ENV APP_UID=1654
ENV DOTNET_RUNNING_IN_CONTAINER=true
RUN apt update && apt upgrade && apt install -y --no-install-recommends wget ca-certificates
RUN wget https://packages.microsoft.com/config/debian/13/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
RUN dpkg -i packages-microsoft-prod.deb
RUN rm packages-microsoft-prod.deb
RUN apt update

FROM base AS dotnet-sdk
RUN apt install -y dotnet-sdk-10.0

FROM base AS dotnet-runtime
RUN apt install -y ffmpeg aspnetcore-runtime-10.0

FROM dotnet-sdk AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["App/SauceNAO.App.csproj", "App/"]
COPY ["Core/SauceNAO.Core.csproj", "Core/"]
COPY ["Infrastructure/SauceNAO.Infrastructure.csproj", "Infrastructure/"]
COPY ["Directory.Packages.props", "Directory.Build.props", "./"]
RUN dotnet restore "./App/SauceNAO.App.csproj"
COPY . .
WORKDIR "/src/App"
RUN dotnet build "./SauceNAO.App.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SauceNAO.App.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM dotnet-runtime AS runtime
USER root
WORKDIR /app
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SauceNAO.App.dll"]
