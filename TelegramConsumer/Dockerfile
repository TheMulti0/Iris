#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["TelegramConsumer/TelegramConsumer.csproj", "TelegramConsumer/"]
COPY ["Extensions/Extensions.csproj", "Extensions/"]
RUN dotnet restore "TelegramConsumer/TelegramConsumer.csproj"
COPY . .
WORKDIR "/src/TelegramConsumer"
RUN dotnet build "TelegramConsumer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TelegramConsumer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TelegramConsumer.dll"]