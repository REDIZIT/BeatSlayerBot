using BeatSlayerServer.Models.Configuration;
using BeatSlayerServer.Services;
using BeatSlayerServer.Services.Messaging;
using BeatSlayerServer.Services.Messaging.Discord;
using BeatSlayerServer.Utils.Email;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using Serilog;
using Microsoft.Extensions.Logging;

namespace BeatSlayerBot
{
    public class Startup
    {
        private IConfiguration AppConfiguration { get; set; }

        public Startup(IConfiguration AppConfiguration)
        {
            this.AppConfiguration = AppConfiguration;
        }


        public void ConfigureServices(IServiceCollection services)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Seq("http://localhost:5341/")
                .CreateLogger();

            services.Configure<ServerSettings>(AppConfiguration);

            services.AddControllersWithViews();

            services.AddSingleton<SettingsWrapper>();

            services.AddSingleton<TimerService>();

            services.AddSingleton<DiscordBotService>();
            services.AddSingleton<VkBotService>();
            services.AddSingleton<EmailService>();

            services.AddCors(action =>
            action.AddPolicy("CorsPolicy", builder =>
              builder
                .AllowAnyMethod()
                .AllowAnyHeader()
                .WithOrigins()
                .AllowCredentials()));
        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory, DiscordBotService bot, SettingsWrapper wrapper, ILogger<Startup> logger)
        {
            loggerFactory.AddSerilog();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseCors("CorsPolicy");

            app.UseHsts();
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseRouting();

            app.Use(async (context, next) =>
            {
                string remoteIpAddress = context.Connection.RemoteIpAddress.MapToIPv4().ToString();
                bool containsXForwarded = context.Request.Headers.ContainsKey("X-Forwarded-For");
                if (containsXForwarded)
                    remoteIpAddress = context.Request.Headers["X-Forwarded-For"];

                // Allow to work with messager only from localhost
                if (remoteIpAddress == "127.0.0.1" || remoteIpAddress == "0.0.0.1" || wrapper.settings.Host.AllowedIPs.Contains(remoteIpAddress))
                {
                    logger.LogInformation("Requested " + context.Request.GetDisplayUrl());
                    await next.Invoke();
                }
                else
                {
                    context.Response.StatusCode = 405;
                    await context.Response.WriteAsync("<html><img src='https://i.pinimg.com/736x/07/2d/50/072d50bbdb38ec870b76fd5bcf12a0f3.jpg'/></html>");
                }
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Bot}/{action=Index}/{id?}");
            });
        }
    }
}
