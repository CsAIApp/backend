# 1. Aşama: Derleme (Build)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Tüm dosyaları kopyala
COPY . ./

# Projeyi derle ve yayınla
RUN dotnet restore
RUN dotnet publish TeamLink.API -c Release -o out

# 2. Aşama: Çalıştırma (Runtime)
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .

# Port ayarı (Render otomatik atar ama biz yine de açalım)
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# Uygulamayı başlat
ENTRYPOINT ["dotnet", "TeamLink.API.dll"]