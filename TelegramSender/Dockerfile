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

# Install ffmpeg and curl and pip3
RUN apt update -y
RUN apt install -y ffmpeg curl python3-pip

# Download youtube-dl
RUN pip3 install youtube-dl

ENTRYPOINT ["dotnet", "TelegramSender.dll"]