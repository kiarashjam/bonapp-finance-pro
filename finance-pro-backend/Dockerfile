FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/FinancePro.Api/FinancePro.Api.csproj", "FinancePro.Api/"]
COPY ["src/FinancePro.Application/FinancePro.Application.csproj", "FinancePro.Application/"]
COPY ["src/FinancePro.Infrastructure/FinancePro.Infrastructure.csproj", "FinancePro.Infrastructure/"]
COPY ["src/FinancePro.Domain/FinancePro.Domain.csproj", "FinancePro.Domain/"]
RUN dotnet restore "FinancePro.Api/FinancePro.Api.csproj"
COPY src/ .
RUN dotnet build "FinancePro.Api/FinancePro.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FinancePro.Api/FinancePro.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FinancePro.Api.dll"]
