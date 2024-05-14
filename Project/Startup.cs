using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using MongoDB.Bson;

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

        services.AddSingleton<MongoDbContext>();

        services.AddScoped<ICarService, CarService>();
        services.AddScoped<ICarsModelsService, CarsModelsService>();

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

        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
        });
    }
}

