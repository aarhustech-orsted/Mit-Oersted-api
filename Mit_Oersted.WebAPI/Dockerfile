#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Mit_Oersted.WebAPI/Mit_Oersted.WebAPI.csproj", "Mit_Oersted.WebAPI/"]
COPY ["Mit_Oersted.Domain/Mit_Oersted.Domain.csproj", "Mit_Oersted.Domain/"]
RUN dotnet restore "Mit_Oersted.WebAPI/Mit_Oersted.WebAPI.csproj"
COPY . ./
WORKDIR "/src/Mit_Oersted.WebAPI"
RUN dotnet build "Mit_Oersted.WebAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Mit_Oersted.WebAPI.csproj" -c Release -o /app/publish
RUN dotnet dev-certs https --trust

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Mit_Oersted.WebAPI.dll"]
