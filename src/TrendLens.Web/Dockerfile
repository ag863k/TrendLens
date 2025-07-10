FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/TrendLens.Web/TrendLens.Web.csproj", "src/TrendLens.Web/"]
COPY ["src/TrendLens.Application/TrendLens.Application.csproj", "src/TrendLens.Application/"]
COPY ["src/TrendLens.Core/TrendLens.Core.csproj", "src/TrendLens.Core/"]
COPY ["src/TrendLens.Infrastructure/TrendLens.Infrastructure.csproj", "src/TrendLens.Infrastructure/"]
RUN dotnet restore "src/TrendLens.Web/TrendLens.Web.csproj"
COPY . .
WORKDIR "/src/src/TrendLens.Web"
RUN dotnet build "TrendLens.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TrendLens.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TrendLens.Web.dll"]
