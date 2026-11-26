using CicdLearningPortal.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHealthChecks();
builder.Services.AddScoped<ILearningCurriculumService, LearningCurriculumService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseRouting();
app.UseAuthorization();

app.MapStaticAssets();

// Health Check Endpoint chuẩn cho Docker & CD Smoke Test
app.MapGet("/health", (ILearningCurriculumService curriculumService, IWebHostEnvironment env) =>
{
    var runtime = curriculumService.GetRuntimeInfo(env.EnvironmentName);
    return Results.Ok(new
    {
        status = "Healthy",
        version = runtime.AppVersion,
        environment = runtime.EnvironmentName,
        machineOrContainer = runtime.MachineOrContainerName,
        isRunningInContainer = runtime.IsRunningInContainer,
        timestampUtc = runtime.ServerTimeUtc
    });
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();

public partial class Program { }
