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

var builder = WebApplication.CreateBuilder(args);

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins("http://localhost:5173") // endereço do Vite
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});


var key = builder.Configuration["Jwt:Key"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
        };
    });

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

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
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
//await DateValidationTest.RunAsync(app.Services);
//await dago.Tests.CtrcImportTest.RunAsync(app.Services);
//await CtrcCustomTestRunner.RunAsync(app.Services);
//await dago.Tests.TesteNormalizerCnpj.RunAsync(app.Services);



app.Run();


