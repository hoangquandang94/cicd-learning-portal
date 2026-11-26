using CicdLearningPortal.Web.Models;

namespace CicdLearningPortal.Web.Services;

public interface ILearningCurriculumService
{
    List<LearningModule> GetAllModules();
    LearningModule? GetModuleById(string id);
    LearningModule? GetModuleByStep(int stepNumber);
    SystemRuntimeInfo GetRuntimeInfo(string environmentName);
    int CalculateReadinessScore(int completedChecklistCount, int totalChecklistCount);
}
