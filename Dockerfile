FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["src/MoneyTracker.Web/MoneyTracker.Web.csproj", "src/MoneyTracker.Web/"]
RUN dotnet restore "src/MoneyTracker.Web/MoneyTracker.Web.csproj"
COPY . .
WORKDIR "/src/src/MoneyTracker.Web"
RUN dotnet publish "MoneyTracker.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MoneyTracker.Web.dll"]
