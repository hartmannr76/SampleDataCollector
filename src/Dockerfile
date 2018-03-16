FROM microsoft/aspnetcore:2.0.3 as base
WORKDIR /app
EXPOSE 8000

FROM microsoft/aspnetcore-build:2.0.3 AS build
WORKDIR /src
COPY . .
RUN dotnet restore
RUN dotnet build --no-restore -c Release -o /app

FROM build AS publish
RUN dotnet publish --no-restore -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENV ASPNETCORE_URLS http://*:8000

ENTRYPOINT [ "dotnet", "SampleDataCollector.dll" ]