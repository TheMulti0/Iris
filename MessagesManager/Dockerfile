#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:5.0-buster-slim AS base
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

COPY --from=build /src/MessagesManager/package.json .
RUN apt-get install npm -y
RUN npm install

ARG CHROME_VERSION="87.0.4280.141-1"
RUN apt-get -f install && apt-get -y install wget gnupg2 apt-utils
RUN wget --no-verbose -O /tmp/chrome.deb http://dl.google.com/linux/chrome/deb/pool/main/g/google-chrome-stable/google-chrome-stable_${CHROME_VERSION}_amd64.deb \
    && apt-get update \
    && apt-get install -y /tmp/chrome.deb --no-install-recommends --allow-downgrades fonts-ipafont-gothic fonts-wqy-zenhei fonts-thai-tlwg fonts-kacst fonts-freefont-ttf \
    && rm /tmp/chrome.deb
    
RUN groupadd -r pptruser && useradd -r -g pptruser -G audio,video pptruser \
    && mkdir -p /home/pptruser/Downloads \
    && chown -R pptruser:pptruser /home/pptruser
RUN chown -R pptruser:pptruser /app
    
ENV PUPPETEER_EXECUTABLE_PATH "/usr/bin/google-chrome-stable"
USER pptruser

ENTRYPOINT ["dotnet", "MessagesManager.dll"]