FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["SingleInstanceApp/SingleInstanceApp.csproj", "SingleInstanceApp/"]
RUN dotnet restore "SingleInstanceApp/SingleInstanceApp.csproj"
COPY . .
WORKDIR "/src/SingleInstanceApp"
RUN dotnet build "SingleInstanceApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SingleInstanceApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

COPY --from=build /src/SingleInstanceApp/requirements.txt .
RUN apt-get update -y && apt-get install python3-pip -y 
RUN pip3 install --upgrade pip
RUN pip3 install -r "requirements.txt"

ENTRYPOINT ["dotnet", "SingleInstanceApp.dll"]
