# ===== Build =====
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia solução e csproj para restaurar
COPY *.sln ./
COPY GestaoAutomotiva/*.csproj GestaoAutomotiva/
RUN dotnet restore

# Copia tudo e publica
COPY . .
RUN dotnet publish GestaoAutomotiva/GestaoAutomotiva.csproj -c Release -o /app/publish

# ===== Runtime =====
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Pasta gravável para o SQLite
RUN mkdir -p /data

# Render (ou containers em geral) expõe uma PORT; escute em 0.0.0.0
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Copia build publicado
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "GestaoAutomotiva.dll"]
