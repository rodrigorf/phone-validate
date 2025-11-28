FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["PhoneValidate/PhoneValidate.csproj", "PhoneValidate/"]
COPY ["PhoneValidate.Infra.Data/PhoneValidate.Infra.Data.csproj", "PhoneValidate.Infra.Data/"]
COPY ["PhoneValidate.Domain.Service/PhoneValidate.Domain.Service.csproj", "PhoneValidate.Domain.Service/"]
RUN dotnet restore "PhoneValidate/PhoneValidate.csproj"
COPY . .
WORKDIR "/src/PhoneValidate"
RUN dotnet build "PhoneValidate.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "PhoneValidate.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PhoneValidate.dll"]