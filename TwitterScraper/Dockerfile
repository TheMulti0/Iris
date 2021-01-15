#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:5.0-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["TwitterScraper/TwitterScraper.csproj", "TwitterScraper/"]
COPY ["UpdatesScraper/UpdatesScraper.csproj", "UpdatesScraper/"]
COPY ["Extensions/Extensions.csproj", "Extensions/"]
COPY ["Common/Common.csproj", "Common/"]
RUN dotnet restore "TwitterScraper/TwitterScraper.csproj"
COPY . .
WORKDIR "/src/TwitterScraper"
RUN dotnet build "TwitterScraper.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TwitterScraper.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TwitterScraper.dll"]