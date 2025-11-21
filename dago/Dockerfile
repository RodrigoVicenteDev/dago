# ============================
#   ETAPA 1 — BUILD
# ============================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY . .
RUN dotnet publish -c Release -o /app/publish

# ============================
#   ETAPA 2 — RUNTIME
# ============================
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/publish .
EXPOSE 80

ENTRYPOINT ["dotnet", "dago.dll"]
