#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Poc.EventDriven.App.NfIngestorGrpcServer/Poc.EventDriven.App.NfIngestorGrpcServer.csproj", "src/Poc.EventDriven.App.NfIngestorGrpcServer/"]
COPY ["src/Poc.EventDriven.Core/Poc.EventDriven.Core.csproj", "src/Poc.EventDriven.Core/"]
COPY ["src/Poc.EventDriven.MessageBus/Poc.EventDriven.MessageBus.csproj", "src/Poc.EventDriven.MessageBus/"]
COPY ["src/Poc.EventDriven.Common/Poc.EventDriven.Common.csproj", "src/Poc.EventDriven.Common/"]
COPY ["src/Poc.EventDriven.Dw/Poc.EventDriven.Dw.csproj", "src/Poc.EventDriven.Dw/"]
RUN dotnet restore "src/Poc.EventDriven.App.NfIngestorGrpcServer/Poc.EventDriven.App.NfIngestorGrpcServer.csproj"
COPY . .
WORKDIR "/src/src/Poc.EventDriven.App.NfIngestorGrpcServer"
RUN dotnet build "Poc.EventDriven.App.NfIngestorGrpcServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Poc.EventDriven.App.NfIngestorGrpcServer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Poc.EventDriven.App.NfIngestorGrpcServer.dll"]