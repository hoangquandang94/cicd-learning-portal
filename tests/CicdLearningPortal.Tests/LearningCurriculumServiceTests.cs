using CicdLearningPortal.Web.Services;
using Microsoft.Extensions.Configuration;

namespace CicdLearningPortal.Tests;

public class LearningCurriculumServiceTests
{
    private static LearningCurriculumService CreateService(Dictionary<string, string?>? configValues = null)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configValues ?? new Dictionary<string, string?>())
            .Build();

        return new LearningCurriculumService(configuration);
    }

    [Fact]
    public void GetAllModules_ShouldReturnSixCompleteProductionSteps()
    {
        // Arrange
        var service = CreateService();

        // Act
        var modules = service.GetAllModules();

        // Assert
        Assert.Equal(6, modules.Count);
        Assert.All(modules, module =>
        {
            Assert.False(string.IsNullOrWhiteSpace(module.Id));
            Assert.False(string.IsNullOrWhiteSpace(module.Title));
            Assert.NotEmpty(module.KeyConcepts);
            Assert.NotEmpty(module.PracticalCommands);
            Assert.NotEmpty(module.ProductionChecklist);
        });
    }

    [Theory]
    [InlineData("foundations-git-mvc", 1)]
    [InlineData("docker-fundamentals", 2)]
    [InlineData("ci-github-actions", 3)]
    [InlineData("code-quality-security-gates", 4)]
    [InlineData("container-registry-packaging", 5)]
    [InlineData("cd-production-rollback", 6)]
    public void GetModuleById_ShouldReturnMatchingStepNumber(string moduleId, int expectedStepNumber)
    {
        // Arrange
        var service = CreateService();

        // Act
        var module = service.GetModuleById(moduleId);

        // Assert
        Assert.NotNull(module);
        Assert.Equal(expectedStepNumber, module.StepNumber);
    }

    [Theory]
    [InlineData(0, 4, 0)]
    [InlineData(2, 4, 50)]
    [InlineData(3, 4, 75)]
    [InlineData(4, 4, 100)]
    public void CalculateReadinessScore_ValidInputs_ShouldReturnExpectedPercentage(
        int completed, int total, int expectedScore)
    {
        // Arrange
        var service = CreateService();

        // Act
        var score = service.CalculateReadinessScore(completed, total);

        // Assert
        Assert.Equal(expectedScore, score);
    }

    [Fact]
    public void CalculateReadinessScore_ZeroTotal_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        var service = CreateService();

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => service.CalculateReadinessScore(0, 0));
    }

    [Fact]
    public void GetRuntimeInfo_ShouldUseConfiguredAppVersion()
    {
        // Arrange
        var service = CreateService(new Dictionary<string, string?>
        {
            ["APP_VERSION"] = "2.1.0-sha-abc1234"
        });

        // Act
        var runtimeInfo = service.GetRuntimeInfo("Staging");

        // Assert
        Assert.Equal("2.1.0-sha-abc1234", runtimeInfo.AppVersion);
        Assert.Equal("Staging", runtimeInfo.EnvironmentName);
        Assert.False(string.IsNullOrWhiteSpace(runtimeInfo.MachineOrContainerName));
    }
}
