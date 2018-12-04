using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using EmailingSystem.Common.Providers.Interface;
using EmailingSystem.Common.Providers.Implementation;
using EmailingSystem.Common.DataModels;
using Microsoft.Net.Http.Headers;
using EmailingSystem.Api.Formatters;
using EmailingSystem.Api.Managers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Cors.Internal;
using Microsoft.AspNetCore.Http;

namespace EmailingSystem.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddResponseCompression();
            services.AddTransient<IProcessManager, ProcessManager>();
            services.AddSingleton<ILogProvider, LogProvider>();
            services.AddSingleton<IDataProvider, MongoDBProvider>(settings => new MongoDBProvider(
                Configuration["MongoDbSettings:Host"].ToString(),
                Configuration["MongoDbSettings:Database"].ToString(),
                Configuration["MongoDbSettings:UserCollection"].ToString()
                )

            );

            services.AddSingleton<IPubSubProvider, KafkaProvider>();

            services.Configure<KafkaSettings>(Configuration.GetSection("KafkaSettings"));
            services.Configure<GraylogSettings>(Configuration.GetSection("GraylogSettings"));
            services.Configure<MongoDbSettings>(Configuration.GetSection("MongoDbSettings"));
            services.AddOptions();

            services.AddSwaggerGen(c =>
            {
                c.DescribeAllEnumsAsStrings();
                c.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "EmailingSystem.Api",
                    Description = "EmailingSystem.Api",
                    TermsOfService = "None",
                });

            });

            services.Configure<MvcOptions>(options =>
            {
                options.Filters.Add(new CorsAuthorizationFilterFactory("AllowSpecificOrigin"));
            });

            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin",
                    builder => builder.WithOrigins("*").
                    AllowAnyHeader().AllowAnyMethod()
                    );
            });

            services.AddMvc();
            services.AddMvc(options =>
            {
                options.InputFormatters.Add(new ProtobufInputFormatter());
                options.OutputFormatters.Add(new ProtobufOutputFormatter());
                options.FormatterMappings.SetMediaTypeMappingForFormat("protobuf", MediaTypeHeaderValue.Parse("application/x-protobuf"));
                options.FormatterMappings.SetMediaTypeMappingForFormat("js", MediaTypeHeaderValue.Parse("application/json"));
            });
        }
        
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseResponseCompression();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.DocumentTitle("EmailingSystem.Api");
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "EmailingSystem.Api V1");
            });

            app.UseStaticFiles();
            app.UseCors("AllowSpecificOrigin");
            app.UseMvc();
            app.Run(async context =>
            {
                await RunContext(context);
            });
        }

        public async Task RunContext(HttpContext context) {
            await Task.Run(() =>
            {
                context.Response.Redirect("/swagger");
            });
            
        }
    }
}
