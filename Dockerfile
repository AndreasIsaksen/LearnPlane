FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY src/LearnPlane.Web/LearnPlane.Web.csproj src/LearnPlane.Web/
RUN dotnet restore src/LearnPlane.Web/LearnPlane.Web.csproj
COPY src/LearnPlane.Web/ src/LearnPlane.Web/
RUN dotnet publish src/LearnPlane.Web/LearnPlane.Web.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
RUN apt-get update \
    && apt-get install -y --no-install-recommends libgssapi-krb5-2 \
    && rm -rf /var/lib/apt/lists/* \
    && mkdir -p /app/data-protection-keys \
    && chown -R $APP_UID:$APP_UID /app/data-protection-keys
ENV ASPNETCORE_URLS=http://+:8080 \
    ASPNETCORE_FORWARDEDHEADERS_ENABLED=true
EXPOSE 8080
COPY --from=build /app/publish .
USER $APP_UID
ENTRYPOINT ["dotnet", "LearnPlane.Web.dll"]
