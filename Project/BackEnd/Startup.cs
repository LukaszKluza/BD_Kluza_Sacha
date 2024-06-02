using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using MongoDB.Bson;
using BCrypt.Net;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;


public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {   
        services.AddControllersWithViews();

        var jwtTokenSettings = new JwtTokenModel
        {
            Issuer = "your_issuer",
            Audience = "your_audience",
            SecretKey = Convert.ToBase64String(Encoding.UTF8.GetBytes("Twój sekretny klucz")),
            ExpiryTime = TimeSpan.FromMinutes(15)
        };

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtTokenSettings.Issuer,
                    ValidAudience = jwtTokenSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtTokenSettings.SecretKey))
                };
            });
        services.AddAuthorization();

        services.AddCors(options =>
        {
            options.AddPolicy("AllowAllOrigins",
                builder =>
                {
                    builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
        });
        services.AddControllers();

        services.AddSingleton<MongoDbContext>();

        services.AddScoped<ICarService, CarService>();
        services.AddScoped<ICarsModelsService, CarsModelsService>();
        services.AddScoped<IRentalService, RentalService>();
        services.AddScoped<IClientService, ClientService>();
        services.AddScoped<IStatisticsService, StatisticsService>();

        services.AddScoped<IMongoCollection<Car>>(provider =>
        {
            var dbContext = provider.GetRequiredService<MongoDbContext>();
            return dbContext.GetCollection<Car>("Cars");
        });
        services.AddScoped<IMongoCollection<CarModel>>(provider =>
        {
            var dbContext = provider.GetRequiredService<MongoDbContext>();
            return dbContext.GetCollection<CarModel>("CarsModels");
        });
        services.AddScoped<IMongoCollection<Rental>>(provider =>
        {
            var dbContext = provider.GetRequiredService<MongoDbContext>();
            return dbContext.GetCollection<Rental>("Rentals");
        });
        services.AddScoped<IMongoCollection<Client>>(provider =>
        {
            var dbContext = provider.GetRequiredService<MongoDbContext>();
            return dbContext.GetCollection<Client>("Clients");
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }
        
        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseCors("AllowAllOrigins");

        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}

