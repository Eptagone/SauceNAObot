# Telegram Bot API Server
FROM debian:stable-slim AS base
RUN apt update && apt upgrade -y
RUN apt install -y --no-install-recommends libc++-dev

# Telegram Bot API server - Build deps
FROM base AS build-deps
RUN apt install -y make git zlib1g-dev libssl-dev gperf cmake clang libc++abi-dev

# Telegram Bot API server - Build
FROM build-deps AS build
RUN git clone --recursive https://github.com/tdlib/telegram-bot-api.git
RUN mkdir telegram-bot-api/build
WORKDIR /telegram-bot-api/build
RUN CXXFLAGS="-stdlib=libc++" CC=/usr/bin/clang CXX=/usr/bin/clang++ cmake -DCMAKE_BUILD_TYPE=Release -DCMAKE_INSTALL_PREFIX:PATH=/usr/local ..
RUN cmake --build . --target install

# Compile multirun
FROM base AS multirun
WORKDIR /source
RUN apt update
RUN apt install -y --no-install-recommends wget ca-certificates cmake build-essential
RUN wget -c https://github.com/nicolas-van/multirun/archive/refs/tags/1.1.3.tar.gz -O - | tar -xz && \
    mv multirun-1.1.3 multirun-src && \
    cd multirun-src && \
    cmake -S . -B build && \
    cmake --build build

FROM base AS final-deps
RUN apt install -y --no-install-recommends nginx
COPY --from=multirun /source/multirun-src/build/multirun /bin/multirun

FROM final-deps AS runtime
WORKDIR /data
COPY ["nginx.conf", "/etc/nginx/sites-available/default"]
COPY --from=build /usr/local/bin/telegram-bot-api /usr/local/bin/telegram-bot-api
RUN ls -l /usr/local/bin/telegram-bot-api*
RUN mkdir -p /data
RUN chgrp -R www-data /data
RUN chmod -R 2775 /data

EXPOSE 80
ENTRYPOINT [ "multirun", "nginx -g 'daemon off;'", "telegram-bot-api --local" ]
