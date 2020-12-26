#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["PollRulesManager/PollRulesManager.csproj", "PollRulesManager/"]
COPY ["Extensions/Extensions.csproj", "Extensions/"]
COPY ["Common/Common.csproj", "Common/"]
COPY ["UserDataLayer/UserDataLayer.csproj", "UserDataLayer/"]
RUN dotnet restore "PollRulesManager/PollRulesManager.csproj"
COPY . .
WORKDIR "/src/PollRulesManager"
RUN dotnet build "PollRulesManager.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PollRulesManager.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PollRulesManager.dll"]