FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["MapServer/MapServer.csproj", "MapServer/"]
COPY ["DataContracts/DataContracts.csproj", "DataContracts/"]
COPY ["ObjectLayerModule/ObjectLayerModule.csproj", "ObjectLayerModule/"]
COPY ["DAL.Core/DAL.Core.csproj", "DAL.Core/"]
COPY ["Common/Common.csproj", "Common/"]
COPY ["RegionModule/RegionModule.csproj", "RegionModule/"]
COPY ["TileMapCore/TileMapCore.csproj", "TileMapCore/"]
COPY ["Constants/Constants.csproj", "Constants/"]
RUN dotnet restore "MapServer/MapServer.csproj"
COPY . .
WORKDIR "/src/MapServer"
RUN dotnet build "./MapServer.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MapServer.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MapServer.dll"]
