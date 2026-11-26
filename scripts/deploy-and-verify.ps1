param(
    [string]$VersionTag = "1.0.0-docker",
    [string]$ImageName = "cicd-learning-portal:latest",
    [string]$HealthUrl = "http://localhost:5080/health",
    [int]$MaxRetries = 10,
    [int]$RetryDelaySeconds = 2
)

$ErrorActionPreference = "Stop"

Write-Host "==========================================================" -ForegroundColor Cyan
Write-Host "🚀 BẮT ĐẦU DEPLOY DOCKER CONTAINER (Version: $VersionTag)" -ForegroundColor Cyan
Write-Host "==========================================================" -ForegroundColor Cyan

# 1. Lưu lại Image ID đang chạy ổn định hiện tại (để phục vụ Auto-Rollback nếu bản mới lỗi)
$previousImageId = ""
$existingContainer = docker ps -q -f "name=cicd-learning-portal"
if ($existingContainer) {
    $previousImageId = (docker inspect --format="{{.Image}}" cicd-learning-portal).Trim()
    Write-Host "📦 Đã ghi nhận bản ổn định hiện tại (Backup Image ID: $previousImageId)" -ForegroundColor Yellow
}

# 2. Thiết lập biến môi trường và chạy Docker Compose
$env:APP_VERSION = $VersionTag
$env:IMAGE_NAME = $ImageName

Write-Host "🐳 Đang đóng gói và khởi chạy Container mới..." -ForegroundColor Cyan
docker compose up -d --build --force-recreate
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Lỗi khi chạy docker compose up!" -ForegroundColor Red
    exit $LASTEXITCODE
}

# 3. Thực hiện Smoke Test (Gọi thử vào /health xem web có sống và trả về Healthy không)
Write-Host "🩺 Đang thực hiện Smoke Test tại: $HealthUrl ..." -ForegroundColor Cyan
$isHealthy = $false

for ($i = 1; $i -le $MaxRetries; $i++) {
    Start-Sleep -Seconds $RetryDelaySeconds
    try {
        $response = Invoke-RestMethod -Uri $HealthUrl -Method Get -TimeoutSec 5
        if ($response.status -eq "Healthy") {
            $isHealthy = $true
            Write-Host "✅ SMOKE TEST THÀNH CÔNG! (Lần thử $i/$MaxRetries)" -ForegroundColor Green
            Write-Host "   - Status     : $($response.status)"
            Write-Host "   - Version    : $($response.version)"
            Write-Host "   - Container  : $($response.machineOrContainer)"
            Write-Host "   - In Docker  : $($response.isRunningInContainer)"
            break
        }
    }
    catch {
        Write-Host "⏳ Đang chờ Container khởi động... (Lần thử $i/$MaxRetries)" -ForegroundColor DarkGray
    }
}

# 4. Nếu Smoke Test thất bại -> Tự động Rollback về Image cũ
if (-not $isHealthy) {
    Write-Host "❌ SMOKE TEST THẤT BẠI! Bản deploy mới không phản hồi Healthy tại /health." -ForegroundColor Red
    if ($previousImageId) {
        Write-Host "⏪ ĐANG TỰ ĐỘNG ROLLBACK VỀ BẢN ỔN ĐỊNH TRƯỚC ĐÓ ($previousImageId)..." -ForegroundColor Yellow
        docker tag $previousImageId $ImageName
        docker compose up -d
        Write-Host "✔ Đã khôi phục lại Container phiên bản cũ thành công!" -ForegroundColor Green
    }
    exit 1
}

Write-Host "==========================================================" -ForegroundColor Green
Write-Host "🎉 DEPLOY HOÀN TẤT! Mở ngay: http://localhost:5080" -ForegroundColor Green
Write-Host "==========================================================" -ForegroundColor Green
