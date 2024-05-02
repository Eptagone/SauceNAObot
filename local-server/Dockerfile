# TBAS.Dockerfile
# Telegram Bot API Server
FROM alpine:latest AS base

FROM base AS build
RUN apk update
RUN apk upgrade
RUN apk add alpine-sdk linux-headers git zlib-dev openssl-dev gperf cmake
RUN git clone --recursive https://github.com/tdlib/telegram-bot-api.git
RUN cd telegram-bot-api
RUN rm -rf build
RUN mkdir build
WORKDIR /telegram-bot-api/build
RUN cmake -DCMAKE_BUILD_TYPE=Release -DCMAKE_INSTALL_PREFIX:PATH=/usr/local ..
RUN cmake --build . --target install

FROM base AS runtime
RUN apk update
RUN apk upgrade
RUN apk add zlib openssl libstdc++
WORKDIR /opt/tba
COPY --from=build /usr/local/bin/telegram-bot-api /usr/local/bin/telegram-bot-api
RUN ls -l /usr/local/bin/telegram-bot-api*

EXPOSE 8081
ENTRYPOINT [ "telegram-bot-api", "--local" ]
