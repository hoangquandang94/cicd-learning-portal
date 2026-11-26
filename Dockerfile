# =====================================================================
# STAGE 1: BUILD & PUBLISH (.NET 10 LTS SDK)
# =====================================================================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# 1. Copy file .csproj trước để tối ưu Docker Layer Caching (chỉ restore lại khi đổi thư viện NuGet)
COPY ["src/CicdLearningPortal.Web/CicdLearningPortal.Web.csproj", "src/CicdLearningPortal.Web/"]
RUN dotnet restore "src/CicdLearningPortal.Web/CicdLearningPortal.Web.csproj"

# 2. Copy toàn bộ source code và publish bản Release tối ưu
COPY ["src/CicdLearningPortal.Web/", "src/CicdLearningPortal.Web/"]
RUN dotnet publish "src/CicdLearningPortal.Web/CicdLearningPortal.Web.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

# =====================================================================
# STAGE 2: PRODUCTION RUNTIME (.NET 10 LTS ASP.NET Runtime siêu nhẹ)
# =====================================================================
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# Nhận số phiên bản từ lúc build (CI/CD truyền vào qua --build-arg APP_VERSION=...)
ARG APP_VERSION=1.0.0-docker
ENV APP_VERSION=${APP_VERSION}
ENV ASPNETCORE_URLS=http://+:8080

EXPOSE 8080

# Copy kết quả đã biên dịch từ stage build sang stage final
COPY --from=build /app/publish .

# Chạy container với quyền non-root ($APP_UID có sẵn trong image .NET 8/9 của Microsoft)
USER $APP_UID

ENTRYPOINT ["dotnet", "CicdLearningPortal.Web.dll"]
