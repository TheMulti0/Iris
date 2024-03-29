#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:5.0-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0.102-ca-patch-buster-slim AS build
WORKDIR /src
COPY ["TelegramReceiver/TelegramReceiver.csproj", "TelegramReceiver/"]
COPY ["Common/Common.csproj", "Common/"]
COPY ["SubscriptionsDb/SubscriptionsDb.csproj", "SubscriptionsDb/"]
RUN dotnet restore "TelegramReceiver/TelegramReceiver.csproj"
COPY . .
WORKDIR "/src/TelegramReceiver"
RUN dotnet build "TelegramReceiver.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TelegramReceiver.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TelegramReceiver.dll"]