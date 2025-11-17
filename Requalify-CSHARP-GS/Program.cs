using System.Reflection;
using Requalify.Connection;
using Requalify.Services;
using Requalify.Services.Abstractions;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);

// ====== CONTROLLERS + JSON ENUMS ======
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

// ====== HEALTH CHECKS ======
builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy("API funcionando corretamente."))
    .AddDbContextCheck<AppDbContext>("OracleConnection");

// ====== VERSIONAMENTO ======
builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = ApiVersion.Parse("1.0");
    options.ReportApiVersions = true;
});

// ====== SWAGGER ======
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // Versão 1 para a API de Bike
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Trackfy API - Bike",
        Version = "v1",
        Description = "A Trackfy API é uma solução completa para o gerenciamento de bicicletas.\n" +
                      "A versão 1 da API permite o cadastro, atualização, exclusão e consulta de bicicletas\n" +
                      "utilizando recursos RESTful. Nosso objetivo é fornecer uma maneira fácil e eficiente\n" +
                      "para os administradores do sistema gerenciarem o ciclo de vida das bicicletas, garantindo\n" +
                      "que todos os dados estejam acessíveis e atualizados em tempo real.",
        Contact = new OpenApiContact
        {
            Name = "Equipe Trackfy",
            Email = "contato@trackfy.com"
        }
    });

    // Versão 2 para a API de Pending
    c.SwaggerDoc("v2", new OpenApiInfo
    {
        Title = "Trackfy API - Pending",
        Version = "v2",
        Description = "A versão 2 da Trackfy API é focada no gerenciamento de pendências dentro do sistema.\n" +
                      "Ela permite aos usuários e administradores controlar e resolver pendências de forma\n" +
                      "centralizada e intuitiva. A API permite a visualização, criação, atualização e exclusão de\n" +
                      "pendências, garantindo um fluxo de trabalho organizado e eficiente.",
        Contact = new OpenApiContact
        {
            Name = "Equipe Trackfy",
            Email = "contato@trackfy.com"
        }
    });

    // Versão 3 para a API de Usuário
    c.SwaggerDoc("v3", new OpenApiInfo
    {
        Title = "Trackfy API - Usuário",
        Version = "v3",
        Description = "A Trackfy API de Usuário é projetada para fornecer um conjunto completo de funcionalidades\n" +
                      "para o gerenciamento de usuários dentro do sistema. A versão 3 da API permite a criação,\n" +
                      "atualização e exclusão de perfis de usuário, além de permitir a autenticação, autorização e\n" +
                      "gestão de permissões. O objetivo é garantir a segurança e a personalização da experiência\n" +
                      "do usuário no sistema Trackfy.",
        Contact = new OpenApiContact
        {
            Name = "Equipe Trackfy",
            Email = "contato@trackfy.com"
        }
    });

    // Documentação XML para summaries no Swagger
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
    }


    // Segurança via API KEY no Swagger
    c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Description = "Insira sua API Key no cabeçalho da requisição. Exemplo: X-API-KEY: mottu-secret-key",
        Name = "X-API-KEY",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "ApiKeyScheme"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "ApiKey"
                }
            },
            new string[] {}
        }
    });
});

// ====== DATABASE ======
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("OracleConnection")));

// ====== SERVICES ======
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<IEducationService, EducationService>();

var app = builder.Build();

// ====== SWAGGER UI ======
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        // Configura os endpoints de versão específicos
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Trackfy API v1");
        c.SwaggerEndpoint("/swagger/v2/swagger.json", "Trackfy API v2");
        c.SwaggerEndpoint("/swagger/v3/swagger.json", "Trackfy API v3");

        // Faz o Swagger UI ser carregado na raiz (localhost:7041)
        c.RoutePrefix = string.Empty;

        // Permite que o Swagger UI mostre os endpoints de cada versão de forma clara
        c.DocumentTitle = "Trackfy API - Multiple Versions";
    });
}

// ====== MIDDLEWARE DE API KEY ======
app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/swagger") ||
        context.Request.Path.StartsWithSegments("/health"))
    {
        await next();
        return;
    }

    if (!context.Request.Headers.TryGetValue("X-API-KEY", out var apiKey))
    {
        context.Response.StatusCode = 401;
        await context.Response.WriteAsync("API Key não informada.");
        return;
    }

    var validApiKey = builder.Configuration["ApiKey"] ?? "Trackfy@2025#ApiKey!";

    if (apiKey != validApiKey)
    {
        context.Response.StatusCode = 403;
        await context.Response.WriteAsync("API Key inválida.");
        return;
    }

    await next();
});

// ====== ENDPOINT DE HEALTH CHECK ======
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

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
