# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy toàn bộ code (gồm cả Business, Data, Shared...) vào container
COPY . .

# Restore project (Lưu ý đường dẫn trỏ vào folder WebAPI)
RUN dotnet restore "FashionStore.WebAPI/FashionStore.WebAPI.csproj"

# Build và Publish
RUN dotnet publish "FashionStore.WebAPI/FashionStore.WebAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Run
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Mở port 8080 (Mặc định của .NET 8)
EXPOSE 8080

ENTRYPOINT ["dotnet", "FashionStore.WebAPI.dll"]