# Etapa 1: Imagem base para dependências
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Etapa 2: Build da aplicação
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia o arquivo csproj e restaura as dependências
COPY ["EasyWheelsApi.csproj", "."]
RUN dotnet restore "./EasyWheelsApi.csproj"

# Copia todo o código para o container e faz o build
COPY . .
RUN dotnet build "EasyWheelsApi.csproj" -c Release -o /app/build

# Etapa 3: Publicação da aplicação
FROM build AS publish
RUN dotnet publish "EasyWheelsApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Etapa 4: Imagem final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EasyWheelsApi.dll"]







# FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
# WORKDIR /app

# # Copiar e restaurar dependências
# COPY EasyWheels.sln .
# COPY EasyWheelsApi.csproj .
# RUN dotnet restore EasyWheels.sln

# # Copiar todo o código e compilar
# COPY . .
# RUN dotnet publish -c Release -o out

# # Etapa de produção
# FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
# WORKDIR /app
# COPY --from=build /app/out .
# ENTRYPOINT ["dotnet", "EasyWheelsApi.dll"]