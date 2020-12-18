#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:5.0-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["FeedsProducer/FeedsProducer.csproj", "FeedsProducer/"]
COPY ["UpdatesProducer/UpdatesProducer.csproj", "UpdatesProducer/"]
COPY ["Extensions/Extensions.csproj", "Extensions/"]
COPY ["Common/Common.csproj", "Common/"]
RUN dotnet restore "FeedsProducer/FeedsProducer.csproj"
COPY . .
WORKDIR "/src/FeedsProducer"
RUN dotnet build "FeedsProducer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FeedsProducer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=build /src/FeedsProducer/requirements.txt .
RUN apt-get update -y && apt-get install python3-pip -y
RUN pip3 install --upgrade pip
RUN pip3 install -r "requirements.txt"
ENTRYPOINT ["dotnet", "FeedsProducer.dll"]