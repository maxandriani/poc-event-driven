#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Poc.EventDriven.App.RegimeApi/Poc.EventDriven.App.RegimeApi.csproj", "src/Poc.EventDriven.App.RegimeApi/"]
COPY ["src/Poc.EventDriven.EntityFramework/Poc.EventDriven.EntityFramework.csproj", "src/Poc.EventDriven.EntityFramework/"]
COPY ["src/Poc.EventDriven.Core/Poc.EventDriven.Core.csproj", "src/Poc.EventDriven.Core/"]
COPY ["src/Poc.EventDriven.Regimes/Poc.EventDriven.Regimes.csproj", "src/Poc.EventDriven.Regimes/"]
COPY ["src/Poc.EventDriven.Common/Poc.EventDriven.Common.csproj", "src/Poc.EventDriven.Common/"]
RUN dotnet restore "src/Poc.EventDriven.App.RegimeApi/Poc.EventDriven.App.RegimeApi.csproj"
COPY . .
WORKDIR "/src/src/Poc.EventDriven.App.RegimeApi"
RUN dotnet build "Poc.EventDriven.App.RegimeApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Poc.EventDriven.App.RegimeApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Poc.EventDriven.App.RegimeApi.dll"]