FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["BackCompression/BackCompression.csproj", "BackCompression/"]
RUN dotnet restore "BackCompression/BackCompression.csproj"
COPY . .
WORKDIR "/src/BackCompression"
RUN dotnet build "BackCompression.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BackCompression.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BackCompression.dll"]
