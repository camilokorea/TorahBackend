using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using TorahBackend.Application.Interfaces;
using TorahBackend.Application.Services;
using TorahBackend.Infrastructure.Repositories;
using TorahBackend.Domain.Entities;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var appsettingsPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");

builder.Configuration
    .AddJsonFile(appsettingsPath, optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

var connectionString = builder.Configuration.GetConnectionString("DbConnection");

Console.WriteLine("DBString:");
Console.WriteLine(connectionString);

var jwtConfig = builder.Configuration.GetSection("Jwt").Get<JwtConfig>();

var torahJsonRepository = new TorahJsonRepository();

var dataRepository = new DataRepository(connectionString, torahJsonRepository);

var usuarioService = new UsuarioService(dataRepository, jwtConfig);

var versionControladorService = new VersionControladorService(dataRepository);

var libroService = new LibroService(dataRepository, versionControladorService);

var glosarioService = new GlosarioService(dataRepository, versionControladorService);

var dedicatoriaService = new DedicatoriaService(dataRepository, versionControladorService);

builder.Services.AddSingleton<IDataRepository>(provider => dataRepository);

builder.Services.AddSingleton<IUsuarioService>(provider => usuarioService);

builder.Services.AddSingleton<ILibroService>(provider => libroService);

builder.Services.AddSingleton<IGlosarioService>(provider => glosarioService);

builder.Services.AddSingleton<IDedicatoriaService>(provider => dedicatoriaService);

builder.Services.AddSingleton<IVersionControladorService>(provider => versionControladorService);

builder.Services.AddSingleton<ITorahJsonRepository>(provider => torahJsonRepository);

// data seed initial torah data
await dataRepository.DataSeed();

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Torah_API",
        Version = "v1"
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = $@"JWT Authorization header using the Bearer scheme. 
                        \r\n\r\n Enter prefix (Bearer), space, and then your token. 
                        Example: 'Bearer 1231233kjsdlkajdksad'"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
            Reference = new OpenApiReference{
                Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string [] { }
        }
    });
});

//services cors
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins(
                "https://biblia.comunidadmenorah.com",
                "https://bibliaadministrador.comunidadmenorah.com"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials(); // Si usas cookies o Authorization headers
    });
});

builder.WebHost.UseUrls("http://10.0.1.2:5000");

// Agregar autenticación JWT
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Secret"]);
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Use CORS
app.UseCors("AllowReactApp");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
