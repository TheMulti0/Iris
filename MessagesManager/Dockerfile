#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:5.0-buster-slim AS base
RUN apt-get update && apt-get install -y libgdiplus
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0.102-ca-patch-buster-slim AS build
WORKDIR /src
COPY ["MessagesManager/MessagesManager.csproj", "MessagesManager/"]
COPY ["Extensions/Extensions.csproj", "Extensions/"]
COPY ["Common/Common.csproj", "Common/"]
COPY ["UserDataLayer/UserDataLayer.csproj", "UserDataLayer/"]
RUN dotnet restore "MessagesManager/MessagesManager.csproj"
COPY . .
WORKDIR "/src/MessagesManager"
RUN dotnet build "MessagesManager.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MessagesManager.csproj" -c Release -o /app/publish

FROM base AS final

WORKDIR /app
COPY --from=publish /app/publish .

COPY --from=build /src/MessagesManager/requirements.txt .
RUN apt-get update -y && apt-get install python3-pip -y
RUN pip3 install --upgrade pip
RUN pip3 install -r "requirements.txt"

ENTRYPOINT ["dotnet", "MessagesManager.dll"]