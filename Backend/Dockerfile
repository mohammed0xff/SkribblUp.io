#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app

EXPOSE 3000

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["DrawAndGuess/DrawAndGuess.csproj", "DrawAndGuess/"]
RUN dotnet restore "DrawAndGuess/DrawAndGuess.csproj"
COPY . .
WORKDIR "/src/DrawAndGuess"
RUN dotnet build "DrawAndGuess.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DrawAndGuess.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DrawAndGuess.dll"]