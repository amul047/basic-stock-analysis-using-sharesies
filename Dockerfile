#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["StockAnalysisWithSharesiesApp/StockAnalysisWithSharesiesApp.csproj", "StockAnalysisWithSharesiesApp/"]
RUN dotnet restore "StockAnalysisWithSharesiesApp/StockAnalysisWithSharesiesApp.csproj"
COPY ./StockAnalysisWithSharesiesApp ./StockAnalysisWithSharesiesApp
WORKDIR "/src/StockAnalysisWithSharesiesApp"
RUN dotnet build "StockAnalysisWithSharesiesApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "StockAnalysisWithSharesiesApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "StockAnalysisWithSharesiesApp.dll"]