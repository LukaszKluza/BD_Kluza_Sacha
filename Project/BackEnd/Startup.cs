using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
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
         var jwtSettings = Configuration.GetSection("Jwt");
        var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);

        services.AddAuthentication(options =>
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
        });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("AuthenticatedUser", policy =>
                policy.RequireAuthenticatedUser());
        });

        services.AddControllersWithViews();
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

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}

