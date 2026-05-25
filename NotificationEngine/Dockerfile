FROM mcr.microsoft.com/dotnet/aspnet:8.0-nanoserver-ltsc2022 AS base
WORKDIR /app
EXPOSE 5205

ENV ASPNETCORE_URLS=http://+:5205

FROM mcr.microsoft.com/dotnet/sdk:8.0-nanoserver-ltsc2022 AS build
ARG configuration=Release
WORKDIR /src
COPY ["NotificationEngine/NotificationEngine.csproj", "NotificationEngine/"]
RUN dotnet restore "NotificationEngine\NotificationEngine.csproj"
COPY . .
WORKDIR "/src/NotificationEngine"
RUN dotnet build "NotificationEngine.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "NotificationEngine.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NotificationEngine.dll"]
