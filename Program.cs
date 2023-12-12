using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using NLog.Extensions.Logging;
using NLog.Web;
using RestaurantAPI;
using RestaurantAPI.Authorization;
using RestaurantAPI.Entities;
using RestaurantAPI.Middleware;
using RestaurantAPI.Models;
using RestaurantAPI.Models.Validators;
using RestaurantAPI.Repositories;
using RestaurantAPI.Seeders;
using RestaurantAPI.Services;
using RestaurantAPI.WeatherForecastItems;
using System;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// NLog: Setup NLog for Dependency injection
builder.Logging.ClearProviders();
builder.Host.UseNLog();

// dodawanie tokentów jwt
var authenticationSettings = new AuthenticationSettings();
builder.Configuration.GetSection("Authentication").Bind(authenticationSettings);
builder.Services.AddSingleton(authenticationSettings);
builder.Services.AddAuthentication(option => {
    option.DefaultAuthenticateScheme = "Bearer";
    option.DefaultScheme = "Bearer";
    option.DefaultChallengeScheme = "Bearer";
}).AddJwtBearer(cfg => {
    cfg.RequireHttpsMetadata = false; // nie wymusza https
    cfg.SaveToken = true; // zapis tokena po stronie serwera
    cfg.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = authenticationSettings.JwtIssuer, // wydawca tokentu - ta aplikacja
        ValidAudience = authenticationSettings.JwtIssuer, // kto mo¿e u¿ywaæ - tylko klienci tej aplikacji
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationSettings.JwtKey)),
    };
});

// Definiowanie w³asnych parametrów autoryzacji

builder.Services.AddAuthorization(option => {
    option.AddPolicy("HasNationality", a => a.RequireClaim("Nationality", "Germany", "Poland")); // Nationality == Germany / Poland
    option.AddPolicy("Atleast20", a => a.AddRequirements(new MinimumAgeRequirement(20)));
    option.AddPolicy("CreatedAtleast2Restaurants", a => a.AddRequirements(new CreatedMultipleRestaurantsRequirement(2)));
});

builder.Services.AddScoped<IAuthorizationHandler, MinimumAgeRequirementHandler>();
builder.Services.AddScoped<IAuthorizationHandler, ResourceOperationRequirementHandler>();
builder.Services.AddScoped<IAuthorizationHandler, CreatedMultipleRestaurantsRequirementHandler>();

// Add services to the container.

builder.Services.AddControllers().AddFluentValidation(); // dodanie walidatora

// dodanie po³¹czenia z baz¹ danych z pliku konfiguracyjnego

builder.Services.AddDbContext<RestaurantDbContext>(options => 
        options.UseMySql(builder.Configuration.GetConnectionString("RestaurantDbConnection"), 
                        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("RestaurantDbConnection"))));

// dodanie seederów 

builder.Services.AddScoped<RestaurantSeeder>();
builder.Services.AddScoped<RoleSeeder>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies()); // dodanie automapera w celu rzutowania encji na dto

// rejestracja interfejsów i serwisów

builder.Services.AddScoped<IWeatherForecastService, WeatherForecastService>();
builder.Services.AddScoped<IRestaurantService, RestaurantService>(); 
builder.Services.AddScoped<IDishService, DishService>();
builder.Services.AddScoped<IRestaurantRepository, RestaurantRepository>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IUserContextService, UserContextService>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>(); // dodanie hashowania has³a
builder.Services.AddScoped<IValidator<RegisterUserDto>, RegisterUserDtoValidator>(); // rejestracja walidatora dla klasy RegisterUserDto
builder.Services.AddScoped<IValidator<RestaurantQuery>, RestaurantQueryValidator>();// rejestracja walidatora dla klasy RestaurantQuery

// rejestracja middleware

builder.Services.AddScoped<ErrorHandlingMiddleware>();
builder.Services.AddScoped<RequestTimeMiddleware>();

builder.Services.AddSwaggerGen(); // dodanie swaggera

builder.Services.AddLogging(loggingBuilder => { // dodanie nLoga
    loggingBuilder.ClearProviders();
    loggingBuilder.AddNLog();
});

builder.Services.AddCors(options => { // dodanie polityki CORS
    options.AddPolicy("FrontEndClient", x => x.AllowAnyMethod().AllowAnyHeader().WithOrigins(builder.Configuration["AllowedOrigins"]));
}); 

var app = builder.Build();

app.UseResponseCaching(); // wywo³anie u¿ycia cachowania odpowiedzi po stronie klineta

app.UseStaticFiles(); // wywo³anie u¿ycia plików statycznych np. Regulamin strony

app.UseCors("FrontEndClient"); // wywo³anie u¿ycia polityki CORS

// My Injection for seedes

using (IServiceScope scope = app.Services.CreateScope())
{
    var restaurantSeeder = scope.ServiceProvider.GetService<RestaurantSeeder>();
    restaurantSeeder?.Seed();

    var roleSeeder = scope.ServiceProvider.GetService<RoleSeeder>();
    roleSeeder?.Seed();
}

if(app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// wywo³anie u¿ycia middleware

app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<RequestTimeMiddleware>();

// Configure the HTTP request pipeline.

app.UseAuthentication(); // wywo³anie jwt

app.UseHttpsRedirection();

// wywo³anie u¿ycia swaggera

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Restaurant API"));

app.UseAuthorization();

app.MapControllers();

app.Run();