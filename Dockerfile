FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
RUN useradd myLowPrivilegeUser
USER myLowPrivilegeUser
WORKDIR /app

ENV ASPNETCORE_URLS http://+:8000
EXPOSE 8000
EXPOSE 8443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["DBMicroservice.csproj", "./"]
RUN dotnet restore "DBMicroservice.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "DBMicroservice.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DBMicroservice.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
USER myLowPrivilegeUser
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DBMicroservice.dll"]
