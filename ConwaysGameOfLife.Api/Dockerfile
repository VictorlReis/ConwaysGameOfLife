FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ConwaysGameOfLife.Api/ConwaysGameOfLife.Api.csproj", "ConwaysGameOfLife.Api/"]
RUN dotnet restore "ConwaysGameOfLife.Api/ConwaysGameOfLife.Api.csproj"
COPY . .
WORKDIR "/src/ConwaysGameOfLife.Api"
RUN dotnet build "ConwaysGameOfLife.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ConwaysGameOfLife.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ConwaysGameOfLife.Api.dll"]
