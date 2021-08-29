#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:5.0-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0.102-ca-patch-buster-slim AS build
WORKDIR /src
COPY ["TelegramSender/TelegramSender.csproj", "TelegramSender/"]
COPY ["Common/Common.csproj", "Common/"]
COPY ["SubscriptionsDb/SubscriptionsDb.csproj", "SubscriptionsDb/"]
RUN dotnet restore "TelegramSender/TelegramSender.csproj"
COPY . .
WORKDIR "/src/TelegramSender"
RUN dotnet build "TelegramSender.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TelegramSender.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Install youtube-dl
RUN curl -L https://yt-dl.org/downloads/latest/youtube-dl -o /usr/local/bin/youtube-dl
RUN chmod a+rx /usr/local/bin/youtube-dl

# Install ffmpeg
RUN apt update
RUN apt install ffmpeg

ENTRYPOINT ["dotnet", "TelegramSender.dll"]