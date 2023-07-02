FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
RUN useradd myLowPrivilegeUser
USER myLowPrivilegeUser
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
RUN useradd myLowPrivilegeUser
USER myLowPrivilegeUser
WORKDIR /src
COPY ["DBMicroservice.csproj", "./"]
RUN dotnet restore "DBMicroservice.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "DBMicroservice.csproj" -c Release -o /app/build

FROM build AS publish
RUN useradd myLowPrivilegeUser
USER myLowPrivilegeUser
RUN dotnet publish "DBMicroservice.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
RUN useradd myLowPrivilegeUser
USER myLowPrivilegeUser
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DBMicroservice.dll"]
