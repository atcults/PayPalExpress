using System;
using System.IO;
using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace LibCloud.Core
{
    //Setting up multiple environments https://www.billboga.com/posts/environment-name-in-aspnet-core-10-rc2
    public class Startup
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Starting up application");

            var config = new ConfigurationBuilder()
                .AddCommandLine(args)
                .Build();

            var host = new WebHostBuilder()
                .ConfigureLogging(options => options.AddConsole())
                .ConfigureLogging(options => options.AddDebug())
                .UseConfiguration(config)
                .UseKestrel()
                //Default location of static contents
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }

        public Startup(IHostingEnvironment env)
        {
            Console.WriteLine($"Environment Name: {env.EnvironmentName}");

            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; set; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            //add NLog to ASP.NET Core
            loggerFactory.AddNLog();
             
            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseExceptionHandler(builder =>
            {
                builder.Run(
                async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "text/html";

                    var error = context.Features.Get<IExceptionHandlerFeature>();
                    if (error != null)
                    {
                        await context.Response.WriteAsync($"<h1>Error: {error.Error.Message}</h1>").ConfigureAwait(false);
                        var logger = loggerFactory.CreateLogger("Catchall Handler");
                        logger.LogInformation($"Error: {error.Error.Message}");
                    }
                });
            });

            app.UseMvc();

            //From Microsoft.AspNetCore.Diagnostics
            //Do not enable if you want to implement catch-all response
            //app.UseWelcomePage();
            // Create a catch-all response
            app.Run(async context =>
            {
                if (context.Request.Path.Value.Contains("boom"))
                {
                    throw new Exception("boom!");
                }
                var logger = loggerFactory.CreateLogger("Catchall Endpoint");
                logger.LogInformation("No endpoint found for request {path}", context.Request.Path);
                await context.Response.WriteAsync("No endpoint found.");
            });

        }
    }
}