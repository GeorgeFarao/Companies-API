using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using TodoApi2.Models;
using TodoApi2.Models.Builders;
using TodoApi2.Models.Lookups;
using TodoApi2.Models.Deleters;
using TodoApi2.Data;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using TodoApi2.Service;
using TodoApi2.Middleware;
using TodoApi2.Validator;
using static System.Net.Mime.MediaTypeNames;

namespace TodoApi2
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
            services.AddControllers();
            //services.AddDbContext<TodoContext>(opt =>
            //                                   opt.UseInMemoryDatabase("TodoList"));
            services.AddControllers(x => x.AllowEmptyInputInBodyModelBinding = true);
            services.AddScoped<BranchService>();
            services.AddScoped<CompanyService>();
            services.AddScoped<EmployeeService>();
            services.AddScoped<WorkStationService>();

            services.AddScoped<BranchBuilder>();
            services.AddScoped<WorkStationBuilder>();
            services.AddScoped<EmployeeBuilder>();
            services.AddScoped<CompanyBuilder>();

            services.AddScoped<CompanyDeleter>();
            services.AddScoped<BranchDeleter>();
            services.AddScoped<WorkStationDeleter>();
            services.AddScoped<EmployeeDeleter>();

            services.AddControllers().AddNewtonsoftJson();

            services.AddLocalization(opt => { opt.ResourcesPath = "Resources"; });
            
            services.Configure<RequestLocalizationOptions>(options =>
            {
                List<CultureInfo> supportedCultures = new List<CultureInfo>
                {
                    new CultureInfo("en-US"),
                    new CultureInfo("fr-Fr"),
                    new CultureInfo("gr-GR"),
                    new CultureInfo("en-GB")
                };

                options.DefaultRequestCulture = new RequestCulture("en-GB");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });

            //services.AddFluentValidation(fv =>
            //{
            //    fv.RegisterValidatorsFromAssemblyContaining<EmployeeValidator>();
            //    fv.RegisterValidatorsFromAssemblyContaining<WorkStationValidator>();
            //    fv.RegisterValidatorsFromAssemblyContaining<BranchValidator>();
            //    fv.RegisterValidatorsFromAssemblyContaining<CompanyValidator>();
            //    // fv.RunDefaultMvcValidationAfterFluentValidationExecutes = false;
            //});
            

            services.AddDbContext<MyAppContext>(options => 
            options.UseSqlServer(Configuration.GetConnectionString("EmployeesConnectionString")));

            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseExceptionHandler(exceptionHandlerApp =>
            //{
            //    exceptionHandlerApp.Run(async context =>
            //    {
            //        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            //        context.Response.ContentType = "text/html";
            //        var ex = context.Features.Get<IExceptionHandlerFeature>();
            //        if (ex != null)
            //        {
            //            var err = $"<h1>Error: {ex.Error.Message}</h1>";
            //            await context.Response.WriteAsync(err).ConfigureAwait(false);
            //        }
            //    });
            //});

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseMiddleware<ErrorHandlerMiddleware>();

            app.UseMiddleware<MyMiddleware>();

            var options = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(options.Value);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
