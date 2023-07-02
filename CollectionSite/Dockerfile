FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
RUN apt-get update && apt-get -y install locales procps mc curl nmap
RUN sed -i '/en_US.UTF-8/s/^# //g' /etc/locale.gen && locale-gen

WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/CS.Model/CS.Model.csproj", "CS.Model/"]
COPY ["src/CS.Data/CS.Data.csproj", "CS.Data/"]
COPY ["src/CS.Data.EFC/CS.Data.EFC.csproj", "CS.Data.EFC/"]
COPY ["src/CollectionSite/CollectionSite.csproj", "CollectionSite/"]
RUN dotnet restore CollectionSite/CollectionSite.csproj -s https://api.nuget.org/v3/index.json --use-current-runtime
COPY src .
WORKDIR /src/CollectionSite
RUN dotnet build CollectionSite.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish CollectionSite.csproj -c Release -o /app

FROM base AS final
ENV TZ=Europe/Moscow
WORKDIR /app
COPY --from=publish /app .

EXPOSE 80
ENTRYPOINT ["dotnet", "CollectionSite.dll"]