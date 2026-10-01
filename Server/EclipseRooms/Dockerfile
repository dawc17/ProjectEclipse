# Build from the repository root:
#   docker build -f Server/EclipseRooms/Dockerfile -t eclipse-rooms .
#   docker run -d --restart unless-stopped --env-file Server/EclipseRooms/playtest.env -p 7301:7301/udp eclipse-rooms
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY Assets/Scripts/Eclipse/Multiplayer/Online/ Assets/Scripts/Eclipse/Multiplayer/Online/
COPY Server/EclipseRooms/ Server/EclipseRooms/
RUN dotnet publish Server/EclipseRooms/EclipseRooms.csproj -c Release -o /out

FROM mcr.microsoft.com/dotnet/runtime:8.0
WORKDIR /app
COPY --from=build /out .
EXPOSE 7301/udp
ENTRYPOINT ["dotnet", "EclipseRooms.dll"]
