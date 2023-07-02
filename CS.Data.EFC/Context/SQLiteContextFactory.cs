using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace CS.Data.EFC.Context
{
    /// <summary>
    /// Create context for migrations
    /// </summary>
    public class SQLiteContextFactory : IDesignTimeDbContextFactory<SQLiteContext>
    {
        private readonly string _connectionString;
        private readonly IServiceProvider? _serviceProvider;

        public SQLiteContextFactory()
        {
            _connectionString = "";
        }

        public SQLiteContextFactory(
            string connectionString)
        {
            _connectionString = connectionString;
        }

        public SQLiteContextFactory(
            string connectionString, IServiceProvider serviceProvider)
        {
            _connectionString = connectionString;
            _serviceProvider = serviceProvider;
        }

        public SQLiteContext CreateDbContext(
            string[] args)
        {
            var assemblyName = this.GetType().GetTypeInfo().Assembly.GetName().Name;
            string basePath;
            var builder = new DbContextOptionsBuilder<SQLiteContext>();
            var contentRootPath = Directory.GetCurrentDirectory();

            Console.WriteLine("Current dir: " + contentRootPath);

            if (contentRootPath.IndexOf(Path.DirectorySeparatorChar + assemblyName, StringComparison.Ordinal) >= 0)
            {
                Console.WriteLine("Change path");

                // Pick up the configuration from the main project
                basePath = Path.Combine(
                    new DirectoryInfo(AppContext.BaseDirectory).Parent?.Parent?.Parent?.Parent?.FullName ?? "", "CollectionSite");
            }
            else
            {
                Console.WriteLine("Not change path");

                basePath = contentRootPath;
            }
            Console.WriteLine("Base path: " + basePath);
            Console.WriteLine($"_serviceProvider is null: {_serviceProvider == null}");

            var loggerFactory = _serviceProvider?.GetRequiredService<ILoggerFactory>();
            Console.WriteLine($"Assembly: {assemblyName}");

            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (string.IsNullOrEmpty(environmentName))
                environmentName = "Development";
            Console.WriteLine($"Environment: {environmentName}");

            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environmentName}.json", optional: true)
                .AddJsonFile("appsettings.unversioned.json", optional: true, reloadOnChange: true);

            var config = configBuilder.Build();
            var connectionString = string.IsNullOrEmpty(_connectionString)
                ? config.GetConnectionString("SQLite")
                : _connectionString;

            builder.UseLoggerFactory(loggerFactory);
            builder.UseSqlite(connectionString);

            Console.WriteLine("Connection string: " + connectionString);

            var context = new SQLiteContext(builder.Options);

            return context;
        }
    }
}