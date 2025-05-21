# ----- BASE IMAGE FOR BUILDING APP (SDK + FFMPEG)
FROM ubuntu:noble AS base

ARG BUILD_CONFIGURATION=Release

# install .NET deps
# https://github.com/dotnet/dotnet-docker/blob/main/documentation/scenarios/installing-dotnet.md#installing-net-binaries
ENV DOTNET_URL="https://builds.dotnet.microsoft.com/dotnet/Sdk/9.0.300/dotnet-sdk-9.0.300-linux-x64.tar.gz"
ENV DOTNET_SHA512="dcab6daef3164390d09edc169d4bf8ec3480af1288e9766c07d20d3c7b70517d263083c3900381fda59c3a7f0aef3fd75ee4f604173c889e8222d6449091d843"

RUN apt-get update \
    && apt-get install -y --no-install-recommends \
        ca-certificates \
        curl \
        xz-utils \
        libc6 \
        libgcc-s1 \
        libicu74 \
        libssl3t64 \
        libstdc++6 \
        tzdata \
    && rm -rf /var/lib/apt/lists/*

# install .NET
RUN curl -fSL --output dotnet.tar.gz $DOTNET_URL \
    && dotnet_sha512=$DOTNET_SHA512 \
    && echo "$DOTNET_SHA512 dotnet.tar.gz" | sha512sum -c - \
    && mkdir -p /usr/share/dotnet \
    && tar -zxf dotnet.tar.gz -C /usr/share/dotnet \
    && rm dotnet.tar.gz \
    && ln -s /usr/share/dotnet/dotnet /usr/bin/dotnet

# download ffmpeg
ENV FFMPEG_URL="https://github.com/BtbN/FFmpeg-Builds/releases/download/latest/ffmpeg-master-latest-linux64-lgpl.tar.xz"

RUN curl -fSL --output ffmpeg.tar.xz $FFMPEG_URL \
    && tar xf ffmpeg.tar.xz \
    && mv ./ffmpeg-master-latest-linux64-lgpl /usr/share/ffmpeg \
    && rm ffmpeg.tar.xz 


# ----- BUILD AND PUBLISH APP
FROM base AS build

# restore nuget packages
WORKDIR /src

COPY ["src/web/Kodesiana.BogorIntelliTraffic.Web.csproj", "web/"]

RUN dotnet restore "web/Kodesiana.BogorIntelliTraffic.Web.csproj"

# copy app source
COPY ./src .

# build and publish app
WORKDIR "/src/web"

RUN dotnet build "Kodesiana.BogorIntelliTraffic.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build
RUN dotnet publish "Kodesiana.BogorIntelliTraffic.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# build production image
FROM mcr.microsoft.com/dotnet/aspnet:9.0-noble AS final

USER $APP_UID

WORKDIR /app

EXPOSE 8080
EXPOSE 8081

COPY --from=build /app/publish .
COPY --from=base /usr/share/ffmpeg/bin/ffmpeg ./ffmpeg

ENTRYPOINT ["dotnet", "Kodesiana.BogorIntelliTraffic.Web.dll"]
