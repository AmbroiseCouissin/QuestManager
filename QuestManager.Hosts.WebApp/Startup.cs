using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QuestManager.Scoring.Repositories;
using QuestManager.Scoring.Repositories.EF.MSSQL;
using QuestManager.Quests.Repositories;
using QuestManager.Quests.Repositories.Json;
using System.Reflection;
using System.IO;
using QuestManager.Models;
using Newtonsoft.Json;
using QuestManager.Controllers;

namespace QuestManager
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc()
                .AddApplicationPart(Assembly.Load(new AssemblyName(typeof(ProgressController).Namespace))); // because TestServer bug when controller in other project

            // Dependency Injection
            var assemblyName = typeof(JsonQuestRepository).Namespace;
            var assembly = Assembly.Load(new AssemblyName(assemblyName));
            var resourceName = $"{assemblyName}.QuestConfiguration.json";
            QuestConfiguration questConfiguration = null;
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (var reader = new StreamReader(stream))
            {
                string questConfigurationJson = reader.ReadToEnd();
                questConfiguration = JsonConvert.DeserializeObject<QuestConfiguration>(questConfigurationJson);
            }
            services.AddSingleton<IQuestRepository>(new JsonQuestRepository(questConfiguration));

            // Add framework services.
            services.AddDbContext<ScoringDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddTransient<IPlayerScoringRepository, EfMsSqlPlayerScoringRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, ScoringDbContext context)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseMvc();

            context.Database.EnsureCreated();
        }
    }
}
