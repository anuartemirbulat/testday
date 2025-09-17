FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["MapClient/MapClient.csproj", "MapClient/"]
COPY ["DataContracts/DataContracts.csproj", "DataContracts/"]
COPY ["Constants/Constants.csproj", "Constants/"]
RUN dotnet restore "MapClient/MapClient.csproj"
COPY . .
WORKDIR "/src/MapClient"
RUN dotnet build "./MapClient.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MapClient.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MapClient.dll"]
