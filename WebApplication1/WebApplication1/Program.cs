using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Console;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MySqlConnector;
using System.Text;
using WebApplication1.Repository.Context;

//Link do curso: https://ibm-learning.udemy.com/course/net-6-web-api-do-zero-ao-avancado/learn/lecture/30472502#questions

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DatabaseConnection");
builder.Services.AddDbContext<DataBaseContext>(options => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Para uso no dapper, � necess�rio adicionar esta conex�o com o banco de dados
builder.Services.AddTransient(x =>
  new MySqlConnection(connectionString));

// Adiciona o IdentityUser(Usuario) e IdentityRole(Permiss�o) ao projeto para autentica��o e autoriza��o de usu�rios
// https://docs.microsoft.com/pt-br/aspnet/core/security/authentication/identity?view=aspnetcore-5.0&tabs=visual-studio
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 2;
}).AddEntityFrameworkStores<DataBaseContext>();
//.AddDefaultTokenProviders(); // Adiciona o provedor de token padr�o usado para gerar tokens de confirma��o e redefini��o de senha.

// Valida as pol�ticas de autoriza��o
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("usuarioPolicy", p => 
        p.RequireAuthenticatedUser().RequireClaim("EmployeeCode"));
    options.AddPolicy("Employee005Policy", p =>
        p.RequireAuthenticatedUser().RequireClaim("EmployeeCode", "005"));
}
);
// Valida o token recebido pelo usu�rio
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(option =>
{
    // Configura��es do token
    // Far� a valida��o do token recebido pelo usu�rio
    // com base nos dados em appsettings.json
    option.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateActor = true,
        ValidateAudience = true,
        ValidateIssuer = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero, // Por padr�o, o token expira realmente depois de 5 minutos do prazo. No caso, deixei zerado.
        ValidIssuer = builder.Configuration["JwtBearerTokenSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtBearerTokenSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["JwtBearerTokenSettings:SecretKey"]))
    };
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
// Adiciona campo the token no swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "WebApplication1", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description =   "JWT Authorization header using the Bearer scheme." +
                        "\r\n\r\n Enter 'Bearer' [space] and then your token in the text input below." +
                        "\r\n\r\nExample: \"Bearer 12345abcdef\"",
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                new string[] {}
        }
    });
});

// Configurar as pol�ticas de CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Configura timestamp e cores para os logs enviados ao console
builder.Logging.AddSimpleConsole(options =>
{
    options.IncludeScopes = false;
    options.SingleLine = true;
    options.ColorBehavior = LoggerColorBehavior.Enabled;
    options.TimestampFormat = "[HH:mm:ss] ";
});

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();


// Habilitar o uso das pol�ticas de CORS
app.UseCors();

app.Environment.IsProduction();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.UseExceptionHandler("/error");
app.Map("/error", app =>
{
    app.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";

        var error = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
        if (error != null)
        {
            if(error.Error is MySqlException)
            {
                var ex = error.Error;

                await context.Response.WriteAsync(new
                {
                    StatusCode = 500,
                    Message = "DataBase est� fora do ar",
                }.ToString());
            } else
            {
                var ex = error.Error;

                await context.Response.WriteAsync(new
                {
                    StatusCode = 500,
                    Message = "An error ocurred",
                    StackTrace = ex.StackTrace
                }.ToString());
            }

        }
    });
});


app.Run();
