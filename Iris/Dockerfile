#
# Build stage
#
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build

LABEL maintainer="multi@codeprecise.com"

WORKDIR /source
COPY . .

RUN dotnet restore 
RUN dotnet publish -c release -o /app --no-restore

#
# Production stage
#

FROM mcr.microsoft.com/dotnet/core/runtime:3.1

WORKDIR /app
COPY --from=build /app .

ENTRYPOINT ["./Iris"]