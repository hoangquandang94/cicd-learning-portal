using System.Reflection;
using System.Runtime.InteropServices;
using CicdLearningPortal.Web.Models;

namespace CicdLearningPortal.Web.Services;

public class LearningCurriculumService : ILearningCurriculumService
{
    private readonly IConfiguration _configuration;

    public LearningCurriculumService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public List<LearningModule> GetAllModules()
    {
        return new List<LearningModule>
        {
            new LearningModule
            {
                StepNumber = 1,
                Id = "foundations-git-mvc",
                Title = "Bước 1: Kiến trúc Project MVC & Chiến lược Git chuẩn Production",
                Subtitle = "Thiết kế ứng dụng sẵn sàng cho tự động hóa (Observability, Health Check, Testability)",
                BadgeText = "Nền tảng",
                BadgeColor = "primary",
                Summary = "Trước khi viết bất kỳ dòng cấu hình CI/CD nào, ứng dụng MVC phải được thiết kế để pipeline có thể kiểm tra (Unit Test), đóng gói (Configuration qua biến môi trường) và giám sát sức khỏe (Health Check endpoint).",
                OldWayVsNewWay = "Trước đây (Bonobo + IIS): Sửa code trực tiếp trên nhánh master, lưu cứng chuỗi kết nối trong web.config, copy đè file DLL vào IIS và mở trình duyệt F5 xem web sống hay chết. Hiện đại: Phân nhánh main/develop/feature, cấu hình qua Environment Variables, có project xUnit đi kèm và endpoint /health trả trạng thái JSON.",
                KeyConcepts = new List<string>
                {
                    "Separation of Concerns: Tách bạch src/ (Source code) và tests/ (Unit/Integration Tests).",
                    "Health Check Endpoint (/health): Giúp Docker và CD Pipeline tự động kiểm tra container mới khởi động có hoạt động bình thường hay không.",
                    "Version & Metadata Injection: Hiển thị số phiên bản (Git Tag / Commit SHA) và tên Container ngay trên giao diện để biết chính xác bản build nào đang chạy.",
                    "Branching Strategy (GitHub Flow): Không push thẳng lên nhánh main. Mọi thay đổi đi qua nhánh feature/* -> Tạo Pull Request -> CI chạy Pass hết mới được Merge."
                },
                PracticalCommands = new List<CommandExample>
                {
                    new CommandExample
                    {
                        Title = "Chạy ứng dụng và kiểm tra Unit Test tại máy Local",
                        Code = "dotnet restore\ndotnet build --configuration Release\ndotnet test --no-build --configuration Release --verbosity normal",
                        Explanation = "Đây chính là 3 câu lệnh cốt lõi mà máy chủ CI (GitHub Actions) sẽ tự động thực thi mỗi khi bạn đẩy code lên."
                    },
                    new CommandExample
                    {
                        Title = "Quy trình tạo nhánh tính năng mới với Git",
                        Code = "git checkout -b feature/add-cicd-docs\ngit add .\ngit commit -m \"feat: cap nhat tai lieu hoc docker va cicd\"\ngit push -u origin feature/add-cicd-docs",
                        Explanation = "Tạo nhánh riêng giúp cô lập code đang phát triển, kích hoạt CI kiểm tra tự động trước khi gộp vào nhánh chính."
                    }
                },
                ProductionChecklist = new List<string>
                {
                    "Solution có tách riêng thư mục src/ và tests/ rõ ràng.",
                    "Endpoint /health trả về HTTP 200 OK kèm thông tin trạng thái.",
                    "Không lưu mật khẩu, API Key cứng trong appsettings.json.",
                    "File .gitignore loại bỏ hoàn toàn bin/, obj/, .vs/."
                }
            },
            new LearningModule
            {
                StepNumber = 2,
                Id = "docker-fundamentals",
                Title = "Bước 2: Làm chủ Docker từ Con Số 0 cho Lập trình viên .NET",
                Subtitle = "Hiểu bản chất Image, Container, Multi-stage Dockerfile và Docker Compose",
                BadgeText = "Docker Core",
                BadgeColor = "info",
                Summary = "Thay vì phải cài đặt .NET Hosting Bundle, cấu hình AppPool, cấp quyền thư mục trên IIS và hay gặp lỗi 'Máy em chạy được mà lên Server lại lỗi', Docker đóng gói cả code đã biên dịch lẫn môi trường chạy (.NET Runtime) vào một khối duy nhất gọi là Image.",
                OldWayVsNewWay = "Trước đây (IIS): Phải cài đúng phiên bản .NET Runtime lên Windows Server, cấu hình IIS, dễ xung đột giữa các web chung server. Hiện đại (Docker): Đóng gói thành Docker Image chuẩn Linux nhẹ chỉ ~100MB, chạy độc lập trong Container, chạy ở máy bạn thế nào thì lên Server Production chạy y hệt 100%.",
                KeyConcepts = new List<string>
                {
                    "Dockerfile (Bản thiết kế): File văn bản chứa các bước hướng dẫn để tạo ra Image (giống như công thức nấu ăn).",
                    "Docker Image (Khuôn đúc / File cài đặt): Bản đóng gói bất biến (Immutable) chứa file DLL đã publish + .NET ASP.NET Runtime.",
                    "Docker Container (Tiến trình đang chạy): Một phiên bản thực thi sống động được khởi tạo từ Image (giống như 1 AppPool + Website đang chạy độc lập).",
                    "Multi-stage Build (Build đa giai đoạn): Giai đoạn 1 dùng Image SDK nặng (~900MB) để biên dịch code; Giai đoạn 2 chỉ copy đúng thư mục đã publish sang Image Runtime siêu nhẹ (~220MB) để chạy thực tế -> Bảo mật và tối ưu dung lượng.",
                    "Port Mapping (-p 8080:8080): Nối cổng 8080 của máy thật (Host) vào cổng 8080 bên trong Container."
                },
                PracticalCommands = new List<CommandExample>
                {
                    new CommandExample
                    {
                        Title = "1. Đóng gói ứng dụng MVC thành Docker Image",
                        Code = "docker build -t cicd-learning-portal:v1 .",
                        Explanation = "Đọc file Dockerfile ở thư mục hiện tại (.) và đóng gói thành Image có tên 'cicd-learning-portal' với nhãn phiên bản 'v1'."
                    },
                    new CommandExample
                    {
                        Title = "2. Khởi chạy Container từ Image vừa tạo",
                        Code = "docker run -d -p 5080:8080 --name my-mvc-portal -e APP_VERSION=1.0.0-docker cicd-learning-portal:v1",
                        Explanation = "-d: chạy ngầm; -p 5080:8080: mở cổng http://localhost:5080 trên máy bạn; --name: đặt tên container; -e: truyền biến môi trường."
                    },
                    new CommandExample
                    {
                        Title = "3. Các lệnh quản lý và gỡ lỗi Container hay dùng nhất",
                        Code = "docker ps                      # Xem các container đang chạy\ndocker logs -f my-mvc-portal   # Xem log trực tiếp của ứng dụng\ndocker stop my-mvc-portal      # Dừng container\ndocker compose up -d --build   # Khởi chạy chuẩn bằng Docker Compose",
                        Explanation = "Chỉ cần nắm vững 4 lệnh này là bạn đã có thể vận hành và kiểm tra ứng dụng trên Docker hàng ngày."
                    }
                },
                ProductionChecklist = new List<string>
                {
                    "Có file .dockerignore để không copy bin/, obj/, .git/ vào ngữ cảnh build.",
                    "Sử dụng Multi-stage build (sdk để build, aspnet runtime để chạy).",
                    "Chạy ứng dụng trong Container bằng tài khoản non-root (APP_UID) để chống tấn công leo thang đặc quyền.",
                    "Khai báo HEALTHCHECK ngay trong Dockerfile hoặc docker-compose.yml."
                }
            },
            new LearningModule
            {
                StepNumber = 3,
                Id = "ci-github-actions",
                Title = "Bước 3: Xây dựng Pipeline CI tự động với GitHub Actions",
                Subtitle = "Tự động Build, Chạy Unit Test và Chặn Code lỗi ngay khi tạo Pull Request",
                BadgeText = "CI Pipeline",
                BadgeColor = "success",
                Summary = "Continuous Integration (Tích hợp liên tục) đảm bảo mỗi khi có bất kỳ ai đẩy code lên GitHub, một máy ảo sạch sẽ (Runner) sẽ tự động tải code về, khôi phục thư viện NuGet, biên dịch và chạy toàn bộ bộ Unit Test.",
                OldWayVsNewWay = "Trước đây (Jenkins cơ bản): Chỉ cấu hình lệnh MSBuild, nếu đồng nghiệp viết hỏng logic nghiệp vụ nhưng không lỗi cú pháp thì Jenkins vẫn build thành công và đẩy thẳng lỗi lên IIS. Hiện đại: Pipeline bắt buộc chạy qua cổng Unit Test (xUnit) — chỉ cần 1 test thất bại, GitHub sẽ khóa nút Merge và báo đỏ ngay lập tức.",
                KeyConcepts = new List<string>
                {
                    "Workflow (.github/workflows/ci-cd.yml): File YAML định nghĩa toàn bộ quy trình tự động hóa.",
                    "Triggers (on: push, pull_request): Sự kiện kích hoạt pipeline chạy tự động.",
                    "Jobs & Steps: Các tác vụ chạy song song hoặc tuần tự (ví dụ: Job 'build-and-test' phải thành công thì Job 'docker-package' mới được chạy).",
                    "NuGet Caching: Lưu bộ nhớ đệm các gói thư viện NuGet giúp giảm thời gian chạy CI từ vài phút xuống còn 15-20 giây."
                },
                PracticalCommands = new List<CommandExample>
                {
                    new CommandExample
                    {
                        Title = "Cấu trúc chuẩn của một Job CI trong GitHub Actions",
                        Code = "jobs:\n  build-and-test:\n    runs-on: ubuntu-latest\n    steps:\n      - uses: actions/checkout@v4\n      - uses: actions/setup-dotnet@v4\n        with:\n          dotnet-version: '10.0.x'\n      - run: dotnet restore\n      - run: dotnet build --no-restore -c Release\n      - run: dotnet test --no-build -c Release",
                        Explanation = "GitHub cấp miễn phí máy ảo Ubuntu để biên dịch và kiểm thử code .NET 10 LTS của bạn trên Cloud."
                    }
                },
                ProductionChecklist = new List<string>
                {
                    "Pipeline tự động kích hoạt trên cả sự kiện push và pull_request.",
                    "Tích hợp Cache cho thư mục ~/.nuget/packages.",
                    "Xuất báo cáo kết quả Test (Test Results Artifact) để dễ tra cứu khi có test lỗi.",
                    "Thiết lập Branch Protection Rule trên GitHub yêu cầu CI xanh (Pass) mới cho phép Merge."
                }
            },
            new LearningModule
            {
                StepNumber = 4,
                Id = "code-quality-security-gates",
                Title = "Bước 4: Cổng Kiểm soát Chất lượng Code & Bảo mật (DevSecOps)",
                Subtitle = "Static Code Analysis, Format Check, Quét lỗ hổng NuGet và Bảo mật Secrets",
                BadgeText = "DevSecOps",
                BadgeColor = "warning",
                Summary = "Trong môi trường Production chuyên nghiệp, code chạy đúng chưa đủ mà còn phải sạch (đúng chuẩn coding convention, không có cảnh báo tiềm ẩn bug) và an toàn (không dùng thư viện dính lỗ hổng CVE, không lộ mật khẩu).",
                OldWayVsNewWay = "Trước đây: Review code bằng mắt thủ công, thư viện NuGet cài từ nhiều năm trước có lỗ hổng bảo mật cũng không ai biết. Hiện đại: Đưa công cụ quét tự động vào CI Pipeline — vi phạm chuẩn format hoặc dùng package có lỗ hổng bảo mật mức High/Critical sẽ tự động cảnh báo hoặc chặn build.",
                KeyConcepts = new List<string>
                {
                    "Code Formatting & Analyzers: Dùng 'dotnet format --verify-no-changes' và Roslyn Analyzers để giữ code đồng nhất.",
                    "Dependency Vulnerability Scan: Dùng 'dotnet list package --vulnerable --include-transitive' để phát hiện ngay các gói NuGet chứa lỗ hổng bảo mật.",
                    "GitHub Secrets: Nơi lưu trữ an toàn Token, SSH Key, Connection String trên GitHub (được mã hóa, không bao giờ hiện ra trên log).",
                    "Container Image Scanning (Trivy): Quét Docker Image sau khi đóng gói để kiểm tra lỗ hổng của hệ điều hành nền."
                },
                PracticalCommands = new List<CommandExample>
                {
                    new CommandExample
                    {
                        Title = "Các lệnh kiểm tra chất lượng & bảo mật .NET",
                        Code = "# Kiểm tra lỗ hổng bảo mật trong các thư viện NuGet\ndotnet list CicdLearningPortal.sln package --vulnerable --include-transitive\n\n# Kiểm tra định dạng chuẩn code\ndotnet format CicdLearningPortal.sln --verify-no-changes",
                        Explanation = "Chạy các lệnh này ngay trên máy local trước khi commit hoặc tích hợp thẳng vào file YAML của GitHub Actions."
                    }
                },
                ProductionChecklist = new List<string>
                {
                    "Quét lỗ hổng NuGet tự động trong mỗi lần chạy CI.",
                    "Bật cảnh báo Dependabot trên GitHub để tự động nhận PR nâng cấp thư viện lỗi.",
                    "Toàn bộ thông tin nhạy cảm được lấy từ GitHub Actions Secrets (${{ secrets.MY_SECRET }})."
                }
            },
            new LearningModule
            {
                StepNumber = 5,
                Id = "container-registry-packaging",
                Title = "Bước 5: Đóng gói & Lưu trữ Docker Image lên Container Registry (GHCR)",
                Subtitle = "Tự động đánh Tag phiên bản (Commit SHA / SemVer) và đẩy lên GitHub Container Registry",
                BadgeText = "Artifact & Registry",
                BadgeColor = "secondary",
                Summary = "Sau khi code vượt qua hết các bài kiểm tra CI và Security ở Bước 3 & 4, Pipeline sẽ đóng gói ứng dụng thành Docker Image và đẩy lên kho lưu trữ (Container Registry) kèm theo nhãn phiên bản rõ ràng.",
                OldWayVsNewWay = "Trước đây: Build ra thư mục bin/Release/Publish rồi nén zip hoặc copy trực tiếp sang server, không lưu lại lịch sử các bản build cũ nên khi lỗi rất khó quay lại bản hôm qua. Hiện đại: Mỗi lần merge vào main sinh ra 1 Docker Image gắn tag mã commit (ví dụ: ghcr.io/user/app:sha-a1b2c3d) lưu trên Registry. Muốn quay lại bản nào chỉ cần đổi đúng tên tag đó.",
                KeyConcepts = new List<string>
                {
                    "Container Registry (GHCR / Docker Hub): Giống như GitHub dùng để chứa Source Code, thì Container Registry dùng để chứa các Docker Image đã đóng gói.",
                    "GitHub Container Registry (ghcr.io): Tích hợp sẵn miễn phí với tài khoản GitHub của bạn, xác thực tự động bằng GITHUB_TOKEN mà không cần tạo tài khoản bên thứ ba.",
                    "Image Tagging Strategy: Luôn gắn ít nhất 2 tag cho mỗi lần build thành công trên nhánh main: tag 'latest' và tag bất biến theo Commit SHA hoặc Version (ví dụ: '1.0.24' hoặc 'sha-8f3d1a2')."
                },
                PracticalCommands = new List<CommandExample>
                {
                    new CommandExample
                    {
                        Title = "Gắn tag và đẩy Image lên GitHub Container Registry",
                        Code = "docker tag cicd-learning-portal:v1 ghcr.io/<ten-github-cua-ban>/cicd-learning-portal:latest\ndocker push ghcr.io/<ten-github-cua-ban>/cicd-learning-portal:latest",
                        Explanation = "Trong thực tế, bước này sẽ được GitHub Actions thực hiện hoàn toàn tự động khi bạn merge code vào nhánh main."
                    }
                },
                ProductionChecklist = new List<string>
                {
                    "Chỉ thực hiện đóng gói & push Image lên Registry khi nhánh là 'main' (không push rác từ nhánh feature).",
                    "Truyền biến build-arg (APP_VERSION) vào Dockerfile để đóng dấu phiên bản ngay trong Image.",
                    "Không chỉ dùng mỗi tag ':latest' mà bắt buộc phải có tag định danh phiên bản cụ thể để phục vụ Rollback."
                }
            },
            new LearningModule
            {
                StepNumber = 6,
                Id = "cd-production-rollback",
                Title = "Bước 6: CD Triển khai Tự động bằng Self-Hosted Runner, Smoke Test & Auto Rollback",
                Subtitle = "Tự động kéo Image mới về Server/Máy Local, khởi chạy Docker, kiểm tra /health và tự động khôi phục khi lỗi",
                BadgeText = "Production CD",
                BadgeColor = "danger",
                Summary = "Đây là mảnh ghép cuối cùng và quan trọng nhất: Continuous Deployment (CD). Chúng ta sẽ cài đặt GitHub Self-Hosted Runner ngay trên máy chủ/máy cá nhân để tự động nhận lệnh từ GitHub, tải Docker Image mới về chạy, tự gọi vào /health để nghiệm thu (Smoke Test) và tự động Rollback về bản cũ nếu phát hiện lỗi!",
                OldWayVsNewWay = "Trước đây: Copy đè file lên IIS -> Web bị gián đoạn hoặc lỗi cấu hình trả về HTTP 500 -> Khách hàng báo lỗi mới cuống cuồng tìm bản backup cũ chép đè lại. Hiện đại: CD Pipeline tự động khởi chạy Container mới -> Gọi /health kiểm tra -> Nếu Pass mới hoàn tất; nếu Fail tự động quay lại Image phiên bản liền trước chỉ trong vài giây.",
                KeyConcepts = new List<string>
                {
                    "Self-Hosted Runner (Miễn phí 100% không giới hạn số phút): Một dịch vụ nhỏ của GitHub chạy trên máy Windows/Linux nội bộ của bạn, kết nối bảo mật lên GitHub để nhận lệnh deploy mà không cần mở port Firewall ra Internet.",
                    "GitHub Environments & Approval Gate: Cấu hình môi trường 'production' trên GitHub — có thể bật chế độ bắt buộc Leader bấm nút 'Approve' thì mới cho phép chạy bước Deploy.",
                    "Post-Deployment Smoke Test: Ngay sau khi 'docker compose up -d', script tự động gửi request HTTP tới 'http://localhost:5080/health' để xác nhận ứng dụng thực sự sống khỏe.",
                    "Automated Rollback: Lưu lại Image Tag đang chạy ổn định trước khi cập nhật. Nếu Smoke Test thất bại, script tự động kích hoạt lại Image Tag cũ."
                },
                PracticalCommands = new List<CommandExample>
                {
                    new CommandExample
                    {
                        Title = "Kịch bản Deploy & Smoke Test tự động (PowerShell / Bash)",
                        Code = "# 1. Khởi chạy container với bản build mới\ndocker compose up -d --pull always\n\n# 2. Chờ và kiểm tra Health Check (Smoke Test)\n$response = Invoke-RestMethod -Uri \"http://localhost:5080/health\"\nif ($response.status -ne \"Healthy\") {\n    Write-Error \"Smoke test that bai! Tien hanh Rollback...\"\n    exit 1\n}",
                        Explanation = "Đoạn kịch bản này đảm bảo mọi bản cập nhật lên Production đều được nghiệm thu tự động."
                    }
                },
                ProductionChecklist = new List<string>
                {
                    "Đã cài đặt và kết nối Self-Hosted Runner (hoặc SSH Deploy) thành công.",
                    "Cấu hình Docker Compose có chính sách tự khởi động lại (restart: unless-stopped).",
                    "Có bước Smoke Test kiểm tra endpoint /health sau khi deploy.",
                    "Có kịch bản Rollback tự động khi Smoke Test không đạt."
                }
            }
        };
    }

    public LearningModule? GetModuleById(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return null;
        }

        return GetAllModules().FirstOrDefault(m =>
            string.Equals(m.Id, id, StringComparison.OrdinalIgnoreCase));
    }

    public LearningModule? GetModuleByStep(int stepNumber)
    {
        return GetAllModules().FirstOrDefault(m => m.StepNumber == stepNumber);
    }

    public SystemRuntimeInfo GetRuntimeInfo(string environmentName)
    {
        var configuredVersion = _configuration["APP_VERSION"]
            ?? Environment.GetEnvironmentVariable("APP_VERSION")
            ?? Assembly.GetExecutingAssembly().GetName().Version?.ToString()
            ?? "1.0.0-local";

        var runningInContainer = string.Equals(
            Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER"),
            "true",
            StringComparison.OrdinalIgnoreCase);

        return new SystemRuntimeInfo
        {
            AppVersion = configuredVersion,
            EnvironmentName = string.IsNullOrWhiteSpace(environmentName) ? "Production" : environmentName,
            MachineOrContainerName = Environment.MachineName,
            OsDescription = RuntimeInformation.OSDescription,
            FrameworkDescription = RuntimeInformation.FrameworkDescription,
            IsRunningInContainer = runningInContainer,
            ServerTimeUtc = DateTime.UtcNow
        };
    }

    public int CalculateReadinessScore(int completedChecklistCount, int totalChecklistCount)
    {
        if (totalChecklistCount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(totalChecklistCount), "Tổng số mục kiểm tra phải lớn hơn 0.");
        }

        if (completedChecklistCount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(completedChecklistCount), "Số mục hoàn thành không được âm.");
        }

        if (completedChecklistCount >= totalChecklistCount)
        {
            return 100;
        }

        return (int)Math.Round((double)completedChecklistCount / totalChecklistCount * 100);
    }
}
