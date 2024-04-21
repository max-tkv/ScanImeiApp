ARG REPO=mcr.microsoft.com/dotnet/aspnet
FROM $REPO:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Install libs for Tesseract. Despite this build a few libraries still will be not available
RUN apt-get update
RUN apt-get install -y git cmake build-essential
RUN mkdir leptonica
RUN git clone --depth 1 --branch 1.82.0 https://github.com/DanBloomberg/leptonica.git /leptonica 
RUN cd leptonica

WORKDIR /leptonica
RUN mkdir build
WORKDIR /leptonica/build
RUN cmake ..

ARG REPO=mcr.microsoft.com/dotnet/aspnet
FROM $REPO:7.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ScanImeiApp/ScanImeiApp.csproj", "ScanImeiApp/"]
RUN dotnet restore "ScanImeiApp/ScanImeiApp.csproj"
COPY . .
WORKDIR "/src/ScanImeiApp"
RUN dotnet build "ScanImeiApp.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ScanImeiApp.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# copy Tesseract missing dependencies
COPY --from=build /src/ProjectName/runtimes ./runtimes

ENTRYPOINT ["dotnet", "ScanImeiApp.dll"]
