# SauceNAO WebApp
FROM alpine:latest AS base
ENV APP_UID=1654
ENV DOTNET_RUNNING_IN_CONTAINER=true
RUN apk update && apk upgrade
RUN apk add ca-certificates-bundle libgcc libssl3 libstdc++ zlib icu-libs icu-data-full

FROM base AS dotnet-sdk
RUN apk add bash
RUN wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh
RUN chmod +x ./dotnet-install.sh
RUN bash ./dotnet-install.sh --install-dir /opt/microsoft/dotnet
RUN ln -s /opt/microsoft/dotnet/dotnet /usr/bin/dotnet

FROM base AS install-runtime
RUN apk add bash
RUN wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh
RUN chmod +x ./dotnet-install.sh
RUN bash ./dotnet-install.sh --install-dir /opt/microsoft/dotnet --runtime aspnetcore

FROM base AS app-runtime
RUN apk add ffmpeg
COPY --from=install-runtime /opt/microsoft/dotnet /opt/microsoft/dotnet
RUN ln -s /opt/microsoft/dotnet/dotnet /usr/bin/dotnet

FROM dotnet-sdk AS build
RUN apk add nodejs
# Remove this line after the version of corepack is updated in the Node image. See https://github.com/nodejs/corepack/issues/627
RUN npm install -g corepack@latest
RUN corepack enable pnpm
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

FROM app-runtime AS runtime
USER root
WORKDIR /app
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SauceNAO.WebApp.dll"]
