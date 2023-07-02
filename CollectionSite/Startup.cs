using CollectionSite.Services;
using CS.Data;
using CS.Data.EFC;
using CS.Data.EFC.Context;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace CollectionSite;

public partial class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddRazorPages();
        services.AddControllers();

        services.AddMvc()
            .AddJsonOptions(jsonOptions =>
            {
                jsonOptions.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            }); ;

        var sqlite = Configuration.GetConnectionString("SQLite");
        services.AddEntityFrameworkSqlite()
            .AddDbContextFactory<SQLiteContext>(options =>
            {
                options.UseSqlite(sqlite);
            }, ServiceLifetime.Transient);

        var pqsql = Configuration.GetConnectionString("Main");
        services.AddEntityFrameworkNpgsql()
            .AddDbContextFactory<PGContext>(options =>
            {
                options.UseNpgsql(pqsql);
            }, ServiceLifetime.Transient);

        services.AddTransient<IConsumerRepository, ConsumerRepository>();
        services.AddTransient<IHostRepository, HostRepository>();
        services.AddTransient<IBaseDataProvider, BaseDataProvider>();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseStaticFiles();

        app.UseRouting();
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }
        app.UseCors();

        app.UseHttpsRedirection();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapRazorPages();
            endpoints.MapControllers();
        });
    }
}