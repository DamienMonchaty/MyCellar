using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyCellar.API.Context;
using MyCellar.API.Models;
using MyCellar.API.Repository.Impl;
using MyCellar.API.Repository;
using System.Text;
using MyCellar.API.ElasticHelpers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Declaration du gestionaire d'entité et sa chaine de connexion a la db
builder.Services.AddDbContext<ModelDbContext>(options => {

    string mySqlConnectionStr = builder.Configuration.GetConnectionString("DefaultConnection"); 
    var serverVersion = new MySqlServerVersion(new Version(8, 0, 29));
    options.UseMySql(mySqlConnectionStr, serverVersion)
                .LogTo(Console.WriteLine, LogLevel.Information)
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors();
});

// Configuration d'un container DI (injection de dependance)
// Il renvoie une nouvelle instance du repository ou une instance existante
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => {
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration.GetValue<string>("JwtIssuer"),
        ValidAudience = builder.Configuration.GetValue<string>("JwtAudience"),
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("JwtSecretKey")))
    };
});

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddTransient<IRepository<Product>, ProductRepository>();
builder.Services.AddTransient<ICategoryRepository, CategoryRepository>();
builder.Services.AddTransient<IRecipeRepository, RecipeRepository>();
builder.Services.AddTransient<IUserRepository, UserRepository>();

// Autorise la communication entre application (Back/Front)
builder.Services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
{
    builder.AllowAnyHeader()
       .AllowAnyMethod()
       .SetIsOriginAllowed((host) => true)
       .AllowCredentials();
}));

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{

}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
