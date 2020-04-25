FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
MAINTAINER TheMulti0 "multi@codeprecise.com"

# Copy src code to /src
WORKDIR /src
COPY src .

# Build to output directory /app
RUN dotnet publish -c Release -o /app

# Build runtime image (copy compiled output to /app)
FROM mcr.microsoft.com/dotnet/core/runtime:3.1

WORKDIR /app
COPY --from=build /app .

# Run Iris. 'dotnet Iris.dll'
ENTRYPOINT ["dotnet", "Iris.dll"]