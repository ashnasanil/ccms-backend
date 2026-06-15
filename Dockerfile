FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["CCMS.API/CCMS.API.csproj", "CCMS.API/"]
COPY ["CCMS.Application/CCMS.Application.csproj", "CCMS.Application/"]
COPY ["CCMS.Domain/CCMS.Domain.csproj", "CCMS.Domain/"]
COPY ["CCMS.Infrastructure/CCMS.Infrastructure.csproj", "CCMS.Infrastructure/"]

RUN dotnet restore "CCMS.API/CCMS.API.csproj"

COPY . .
WORKDIR "/src/CCMS.API"
RUN dotnet build "CCMS.API.csproj" -c Release -o /app/build
RUN dotnet publish "CCMS.API.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
EXPOSE 443
ENTRYPOINT ["dotnet", "CCMS.API.dll"]
