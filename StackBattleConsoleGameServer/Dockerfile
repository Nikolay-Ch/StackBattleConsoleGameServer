#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["StackBattleConsoleGameServer/StackBattleConsoleGameServer.csproj", "StackBattleConsoleGameServer/"]
RUN dotnet restore "StackBattleConsoleGameServer/StackBattleConsoleGameServer.csproj"
COPY . .
WORKDIR "/src/StackBattleConsoleGameServer"
RUN dotnet build "StackBattleConsoleGameServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "StackBattleConsoleGameServer.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StackBattleConsoleGameServer.dll"]