FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
ENV ASPNETCORE_URLS http://*:80
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["EasyWheelsApi.csproj", "."]
RUN dotnet restore "./EasyWheelsApi.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "EasyWheelsApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EasyWheelsApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EasyWheelsApi.csproj.dll"]






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