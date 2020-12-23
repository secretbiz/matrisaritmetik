using System;
using System.IO;
using System.Text;
using MatrisAritmetik.Core.Services;
using MatrisAritmetik.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MatrisAritmetik
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages().AddSessionStateTempDataProvider(); ;
            services.AddDistributedMemoryCache();

            services.AddSession(options =>
              {
                  options.Cookie.HttpOnly = true;
                  options.Cookie.IsEssential = true;
              });

            services.AddScoped<IFrontService, FrontService>();
            services.AddScoped<IUtilityService<dynamic>, UtilityService<dynamic>>();
            services.AddScoped<IMatrisArithmeticService<dynamic>, MatrisArithmeticService<dynamic>>();
            services.AddScoped<ISpecialMatricesService, SpecialMatricesService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
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

            app.UseExceptionHandler(errorApp =>
              {
                  errorApp.Run(async context =>
                  {
                      Console.WriteLine("Content-Length: " + context.Request.Headers["Content-Length"]);
                      string tmp;
                      try
                      {
                          context.Request.Body.Seek(0, SeekOrigin.Begin);
                      }
                      catch (Exception ex)
                      {
                          if (ex.InnerException != null)
                          {
                              Console.WriteLine(ex.Message);
                          }
                          else
                          {
                              Console.WriteLine("Can't rewind body stream. " + ex.Message);
                          }
                      }
                      using (StreamReader reader = new StreamReader(context.Request.Body, Encoding.UTF8))
                      {
                          tmp = await reader.ReadToEndAsync().ConfigureAwait(false);
                      }

                      Console.WriteLine("Body: " + tmp);

                  });
              });

            app.Use((context, next) =>
              {
                  context.Request.EnableBuffering();
                  return next();
              });

            app.UseDefaultFiles();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSession();

            app.UseEndpoints(endpoints =>
              {
                  endpoints.MapRazorPages();
              });

        }
    }
}
