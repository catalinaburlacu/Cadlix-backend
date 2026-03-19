FROM mcr.microsoft.com/dotnet/sdk:10.0 AS builder

WORKDIR /app

COPY ./ .

RUN dotnet publish -c Release --self-contained false

FROM mcr.microsoft.com/dotnet/aspnet:10.0

WORKDIR /app

COPY --from=builder /app/Cadlix_backend.Api/bin/Release/net10.0/publish .

CMD ["dotnet", "Cadlix_backend.Api.dll"]