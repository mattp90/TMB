using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Web.Framework.Infrastructure.Extensions;

namespace Nop.Web
{
    /// <summary>
    /// Represents startup class of application
    /// </summary>
    public class Startup
    {
        #region Fields

        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        #endregion

        #region Ctor

        public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        #endregion

        /// <summary>
        /// Add services to the application and configure service provider
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureApplicationServices(_configuration, _webHostEnvironment);
            
            services.AddCors(options =>
            {
                options.AddPolicy("AllowOrigin",
                    builder => builder.WithOrigins(
                            "http://92.223.223.101",
                            "https://92.223.223.101",
                            "http://92.223.223.101:8001",
                            "https://92.223.223.101:8001",
                            "http://192.168.178.34",
                            "http://192.168.178.34:8001",
                            "https://192.168.178.34:8001",
                            "https://192.168.178.34:8001"
                            )
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
            });
        }

        /// <summary>
        /// Configure the application HTTP request pipeline
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public void Configure(IApplicationBuilder application)
        {
            application.ConfigureRequestPipeline();
            application.StartEngine();
            application.UseCors(options => options.WithOrigins(
                    "http://92.223.223.101",
                    "https://92.223.223.101",
                    "http://92.223.223.101:8001",
                    "https://92.223.223.101:8001",
                    "http://192.168.178.34",
                    "http://192.168.178.34:8001",
                    "https://192.168.178.34:8001",
                    "https://192.168.178.34:8001"
                ).AllowAnyHeader());
        }
    }
}