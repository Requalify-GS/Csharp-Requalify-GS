using System.Diagnostics;
using System.Reflection;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Trace;
using Requalify.Connection;
using Requalify.ML;
using Requalify.Services;
using Requalify.Services.Abstractions;

var builder = WebApplication.CreateBuilder(args);

// =============== CONTROLLERS ===============
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing =>
    {
        tracing
            .AddAspNetCoreInstrumentation()   
            .AddHttpClientInstrumentation()  
            .AddEntityFrameworkCoreInstrumentation() 
            .AddSource("Requalify")           
            .SetSampler(new OpenTelemetry.Trace.AlwaysOnSampler())
            .AddConsoleExporter();        
    });

builder.Services.AddSingleton(new ActivitySource("Requalify"));
builder.Services.AddSingleton<InterestPredictionService>();

// =============== HEALTH CHECKS ===============
builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy("API is running."))
    .AddDbContextCheck<AppDbContext>("OracleConnection");

// =============== API VERSIONING ===============
builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = ApiVersion.Parse("1.0");
    options.ReportApiVersions = true;
});

// =============== SWAGGER CONFIG ===============
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // ------- DOCUMENTATION FOR ALL 4 VERSIONS -------
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Requalify API - Users",
        Version = "v1",
        Description = "Professional Reskilling Platform (Reskilling AI).\n" +
                      "User management and professional profiles."
    });

    c.SwaggerDoc("v2", new OpenApiInfo
    {
        Title = "Requalify API - Skills",
        Version = "v2",
        Description = "Management of user skills and competencies."
    });

    c.SwaggerDoc("v3", new OpenApiInfo
    {
        Title = "Requalify API - Education",
        Version = "v3",
        Description = "Management of users' educational history."
    });

    c.SwaggerDoc("v4", new OpenApiInfo
    {
        Title = "Requalify API - Courses",
        Version = "v4",
        Description = "Management of courses assigned to users."
    });

    c.SwaggerDoc("ml", new OpenApiInfo
    {
        Title = "Requalify API - Machine Learning",
        Version = "ml",
        Description = "ML.NET prediction endpoint for professional area recommendation."
    });

    // ------- TAGS PRIORITY BY VERSION -------
    c.TagActionsBy(api =>
    {
        if (api.GroupName == "v1") return new[] { "User" };
        if (api.GroupName == "v2") return new[] { "Skill" };
        if (api.GroupName == "v3") return new[] { "Education" };
        if (api.GroupName == "v4") return new[] { "Course" };
        return new[] { api.GroupName ?? "Health" };
    });

    // ------- ORDER ENDPOINTS IN SWAGGER -------
    c.OrderActionsBy(api =>
    {
        if (api.GroupName == "v1") return "0-User-" + api.RelativePath;
        if (api.GroupName == "v2") return "1-Skill-" + api.RelativePath;
        if (api.GroupName == "v3") return "2-Education-" + api.RelativePath;
        if (api.GroupName == "v4") return "3-Course-" + api.RelativePath;
        return "9-" + api.RelativePath;
    });

    // ------- XML DOCS (COMMENTS IN CONTROLLERS) -------
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    if (File.Exists(xmlPath))
        c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
});

// =============== DATABASE CONFIG ===============
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("OracleConnection")));

// =============== DEPENDENCY INJECTION ===============
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<IEducationService, EducationService>();
builder.Services.AddScoped<ISkillService, SkillService>();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

// =============== SWAGGER UI ===============
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Requalify API - Users (v1)");
        c.SwaggerEndpoint("/swagger/v2/swagger.json", "Requalify API - Skills (v2)");
        c.SwaggerEndpoint("/swagger/v3/swagger.json", "Requalify API - Education (v3)");
        c.SwaggerEndpoint("/swagger/v4/swagger.json", "Requalify API - Courses (v4)");
        c.SwaggerEndpoint("/swagger/ml/swagger.json", "Requalify API - Machine Learning (ml)");

        c.RoutePrefix = string.Empty;
        c.DocumentTitle = "Requalify API - Reskilling AI";
    });

}

// =============== HEALTHCHECK ENDPOINT ===============
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";

        var json = System.Text.Json.JsonSerializer.Serialize(new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                description = e.Value.Description
            })
        });

        await context.Response.WriteAsync(json);
    }
});

// =============== PIPELINE ===============
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
