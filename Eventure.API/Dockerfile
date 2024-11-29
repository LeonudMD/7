FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Eventure.API/Eventure.API.csproj", "Eventure.API/"]
COPY ["Eventure.Application/Eventure.Application.csproj", "Eventure.Application/"]
COPY ["Eventure.DB/Eventure.DB.csproj", "Eventure.DB/"]
COPY ["Eventure.Domain/Eventure.Domain.csproj", "Eventure.Domain/"]
RUN dotnet restore "Eventure.API/Eventure.API.csproj"
COPY . .
WORKDIR "/src/Eventure.API"
RUN dotnet build "Eventure.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Eventure.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Eventure.API.dll"]
