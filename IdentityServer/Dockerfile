FROM mcr.microsoft.com/dotnet/core/sdk:3.0 AS build-env

WORKDIR /app
COPY . ./

RUN dotnet publish --output /app/out --configuration Release

FROM mcr.microsoft.com/dotnet/core/aspnet:3.0

# Install System.Drawing native dependencies
RUN apt-get update && apt-get install -y libgdiplus && rm -rf /var/lib/apt/lists/*

WORKDIR /app
COPY --from=build-env /app/out .

# Do not run as root user
RUN chown -R www-data:www-data /app
USER www-data

# Select non-root port
ENV ASPNETCORE_URLS=http://+:5000

# Launch dll
ENTRYPOINT ["dotnet", "IdentityServer.dll"]
