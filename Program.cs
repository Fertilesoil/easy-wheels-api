using System.Text;
using System.Text.Json.Serialization;
using EasyWheelsApi.Data;
using EasyWheelsApi.Facade;
using EasyWheelsApi.GlobalExceptionHandler;
using EasyWheelsApi.Models.Entities;
using EasyWheelsApi.Services;
using EasyWheelsApi.Services.Impl;
using EasyWheelsApi.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

QuestPDF.Settings.License = LicenseType.Community;

// Add services to the container.

// Configuração do banco de dados

if (builder.Configuration["Environment:Start"] == "PROD")
{
    // Conex�o com o PostgresSQL - Nuvem

    builder
        .Configuration.SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("secrets.json");

    var connectionString = builder.Configuration.GetConnectionString("ProdConnection");

    builder.Services.AddDbContext<RentalDbContext>(options => options.UseNpgsql(connectionString));
}
else
{
    builder.Services.AddDbContext<RentalDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DevConnect"))
    );
}

// Interfaces registradas

builder.Services.AddHttpClient<IViaCepService, ViaCepServiceImpl>();
builder.Services.AddScoped<IAddressFacade, AddressFacade>();
builder.Services.AddScoped<ILessorService, LessorService>();
builder.Services.AddScoped<ILesseeService, LesseeService>();
builder.Services.AddScoped<ICarService, CarService>();
builder.Services.AddScoped<IRentalService, RentalService>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddProblemDetails();

builder
    .Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    Uri url = new("https://www.linkedin.com/in/fernandodiascosta-dotnet/");
    c.SwaggerDoc(
        "v1",
        new OpenApiInfo
        {
            Title = "EasyWheelsApi",
            Version = "v1",
            Description = "Hell Yeah",
            Contact = new OpenApiContact
            {
                Email = "fdias132@gmail.com",
                Name = "Fernando Dias Costa",
                Url = url
            }
        }
    );

    // Configuração para permitir autenticação JWT no Swagger
    c.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "JWT Bearer authorization header using the Bearer scheme."
        }
    );

    c.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        }
    );

    c.EnableAnnotations();
});

builder.Services.AddAuthorization();

// Configuração do Identity

builder
    .Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<RentalDbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequiredUniqueChars = 0;
    options.Password.RequiredLength = 8;
    options.User.RequireUniqueEmail = false;
});

// Configuração para o uso de Cookies

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/api/Auth/login";
    options.LogoutPath = "/api/Auth/logout";
    options.ExpireTimeSpan = TimeSpan.FromDays(3);
    options.SlidingExpiration = true;
});

// Configuração do Authentication para o uso de Cookies e Jwt

var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]!);

builder
    .Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    })
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        name: "MyPolicy",
        policy =>
        {
            policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
        }
    );
});

var app = builder.Build();

// Criar o Banco de dados e as tabelas Automaticamente
using (var scope = app.Services.CreateAsyncScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<RentalDbContext>();
    await dbContext.Database.EnsureCreatedAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "EasyWheelsApi v1");
    });
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "EasyWheelsApi v1");
        c.RoutePrefix = string.Empty;
    });
}

// using (var scope = app.Services.CreateScope())
// {
//     var services = scope.ServiceProvider;
//     try
//     {
//         var context = services.GetRequiredService<RentalDbContext>();
//         context.Database.Migrate();
//     }
//     catch (Exception ex)
//     {
//         // Log the error or handle it as needed
//         Console.WriteLine("An error occurred while migrating the database: " + ex.Message);
//     }
// }

app.UseExceptionHandler(opt => {});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();