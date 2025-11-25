using dago.Data;
using dago.Repository;
using dago.Services;
using dago.Services.Tests;
using dago.Services.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);
Env.Load();



var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins("http://localhost:5173", "http://localhost:3000") // endereço do Vite
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY");
Console.WriteLine("JWT_KEY length: " + jwtKey?.Length);
var jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER");
var jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE");



builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))

        };
    });

// Add services to the container.
var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
var dbPort = Environment.GetEnvironmentVariable("DB_PORT");
var dbName = Environment.GetEnvironmentVariable("DB_NAME");
var dbUser = Environment.GetEnvironmentVariable("DB_USER");
var dbPass = Environment.GetEnvironmentVariable("DB_PASSWORD");

var connectionString = $"Host={dbHost};Port={dbPort};Database={dbName};Username={dbUser};Password={dbPass}";

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<HashService>();
builder.Services.AddScoped<ClienteRepository>();
builder.Services.AddScoped<ClienteService>();
builder.Services.AddScoped<CargoRepository>();
builder.Services.AddScoped<CargoService>();
builder.Services.AddScoped<UsuarioRepository>();
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddMemoryCache();
builder.Services.AddScoped<ICtrcRepository, CtrcRepository>();
builder.Services.AddScoped<ICtrcImportService, CtrcImportService>();
builder.Services.AddSingleton<IBusinessDayService, BusinessDayService>();
builder.Services.AddScoped<ICtrcGridService, CtrcGridService>();
builder.Services.AddScoped<UnidadeService>();
builder.Services.AddScoped<UnidadeRepository>();
builder.Services.AddScoped<IConfiguracaoEsporadicoService, ConfiguracaoEsporadicoService>();
builder.Services.AddScoped<IConfiguracaoEsporadicoRepository, ConfiguracaoEsporadicoRepository>();
builder.Services.AddScoped<AgendaService>();
builder.Services.AddScoped<CtrcNormalizer>();





builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseCors("_myAllowSpecificOrigins");
app.UseAuthentication();
app.UseAuthorization();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Dago API v1");
});


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
//await DateValidationTest.RunAsync(app.Services);
//await dago.Tests.CtrcImportTest.RunAsync(app.Services);
//await CtrcCustomTestRunner.RunAsync(app.Services);
//await dago.Tests.TesteNormalizerCnpj.RunAsync(app.Services);



app.Run();


