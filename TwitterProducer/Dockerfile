#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:5.0-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["TwitterProducer/TwitterProducer.csproj", "TwitterProducer/"]
COPY ["UpdatesProducer/UpdatesProducer.csproj", "UpdatesProducer/"]
COPY ["Extensions/Extensions.csproj", "Extensions/"]
COPY ["Common/Common.csproj", "Common/"]
RUN dotnet restore "TwitterProducer/TwitterProducer.csproj"
COPY . .
WORKDIR "/src/TwitterProducer"
RUN dotnet build "TwitterProducer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TwitterProducer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=build /src/TwitterProducer/requirements.txt .
RUN apt-get update -y && apt-get install python3-pip -y
RUN pip3 install --upgrade pip
RUN pip3 install -r "requirements.txt"
ENTRYPOINT ["dotnet", "TwitterProducer.dll"]