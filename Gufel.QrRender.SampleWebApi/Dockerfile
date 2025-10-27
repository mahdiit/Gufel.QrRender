# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Gufel.QrRender.Models/Gufel.QrRender.Models.csproj", "Gufel.QrRender.Models/"]
COPY ["Gufel.QrRender.Providers/Gufel.QrRender.Providers.csproj", "Gufel.QrRender.Providers/"]
COPY ["Gufel.QrRender.SampleWebApi/Gufel.QrRender.SampleWebApi.csproj", "Gufel.QrRender.SampleWebApi/"]
RUN dotnet restore "./Gufel.QrRender.SampleWebApi/Gufel.QrRender.SampleWebApi.csproj"

COPY . .
WORKDIR "/src/Gufel.QrRender.SampleWebApi"
RUN dotnet build "./Gufel.QrRender.SampleWebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Gufel.QrRender.SampleWebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
USER root
# Install Inkscape
RUN apt-get update && \
    apt-get install -y inkscape && \
    apt-get clean && \
    rm -rf /var/lib/apt/lists/*

# Print Inkscape path and set environment variable
RUN which inkscape
ENV INKSCAPE_PATH=/usr/bin/inkscape

WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Gufel.QrRender.SampleWebApi.dll"]