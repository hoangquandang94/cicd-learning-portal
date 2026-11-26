namespace CicdLearningPortal.Web.Models;

public class LearningModule
{
    public int StepNumber { get; set; }
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
    public string BadgeText { get; set; } = string.Empty;
    public string BadgeColor { get; set; } = "primary";
    public string Summary { get; set; } = string.Empty;
    public string OldWayVsNewWay { get; set; } = string.Empty;
    public List<string> KeyConcepts { get; set; } = new();
    public List<CommandExample> PracticalCommands { get; set; } = new();
    public List<string> ProductionChecklist { get; set; } = new();
}

public class CommandExample
{
    public string Title { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Explanation { get; set; } = string.Empty;
}

public class SystemRuntimeInfo
{
    public string AppVersion { get; set; } = "1.0.0-local";
    public string EnvironmentName { get; set; } = "Development";
    public string MachineOrContainerName { get; set; } = string.Empty;
    public string OsDescription { get; set; } = string.Empty;
    public string FrameworkDescription { get; set; } = string.Empty;
    public bool IsRunningInContainer { get; set; }
    public DateTime ServerTimeUtc { get; set; }
}

public class PortalDashboardViewModel
{
    public SystemRuntimeInfo RuntimeInfo { get; set; } = new();
    public List<LearningModule> Modules { get; set; } = new();
}
