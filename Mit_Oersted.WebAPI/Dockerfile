#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80/tcp
EXPOSE 443/tcp

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["WebAPI/WebAPI.csproj", "WebAPI/"]
COPY ["Domain/Domain.csproj", "Domain/"]
RUN dotnet restore "WebAPI/Mit_Oersted.WebAPI.csproj"
COPY . ./
WORKDIR "/src/WebAPI"
RUN dotnet build "Mit_Oersted.WebAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Mit_Oersted.WebAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
RUN dotnet dev-certs https --trust
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Mit_Oersted.WebAPI.dll"]
