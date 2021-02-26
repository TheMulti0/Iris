#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0.102-ca-patch-buster-slim AS build
WORKDIR /src
COPY ["SubscriptionsManager/SubscriptionsManager.csproj", "SubscriptionsManager/"]
COPY ["Extensions/Extensions.csproj", "Extensions/"]
COPY ["Common/Common.csproj", "Common/"]
COPY ["SubscriptionsDataLayer/SubscriptionsDataLayer.csproj", "SubscriptionsDataLayer/"]
RUN dotnet restore "SubscriptionsManager/SubscriptionsManager.csproj"
COPY . .
WORKDIR "/src/SubscriptionsManager"
RUN dotnet build "SubscriptionsManager.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SubscriptionsManager.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SubscriptionsManager.dll"]