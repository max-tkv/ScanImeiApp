FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Установка библиотек для Tesseract.
RUN apt-get update && \
    apt-get install -y git cmake build-essential && \
    mkdir /leptonica && \
    git clone --depth 1 --branch 1.82.0 https://github.com/DanBloomberg/leptonica.git /leptonica && \
    cd /leptonica && \
    mkdir build && \
    cd build && \
    cmake ..

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/ScanImeiApp/ScanImeiApp.csproj", "src/ScanImeiApp/"]
RUN dotnet restore "src/ScanImeiApp/ScanImeiApp.csproj"
COPY . .
WORKDIR "/src/ScanImeiApp"

RUN dotnet build "ScanImeiApp.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ScanImeiApp.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "ScanImeiApp.dll"]