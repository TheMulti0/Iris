#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:5.0-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0.102-ca-patch-buster-slim AS build
WORKDIR /src
COPY ["FacebookScraper/FacebookScraper.csproj", "FacebookScraper/"]
COPY ["UpdatesScraper/UpdatesScraper.csproj", "UpdatesScraper/"]
COPY ["Extensions/Extensions.csproj", "Extensions/"]
COPY ["Common/Common.csproj", "Common/"]
RUN dotnet restore "FacebookScraper/FacebookScraper.csproj"
COPY . .
WORKDIR "/src/FacebookScraper"
RUN dotnet build "FacebookScraper.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FacebookScraper.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=build /src/FacebookScraper/requirements.txt .
RUN apt-get update -y && apt-get install python3-pip tor -y 
RUN pip3 install --upgrade pip
RUN pip3 install -r "requirements.txt"
ENTRYPOINT ["dotnet", "FacebookScraper.dll"]