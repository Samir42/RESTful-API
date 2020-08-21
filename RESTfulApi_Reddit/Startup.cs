using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RESTfulApi_Reddit.Abstractions;
using RESTfulApi_Reddit.AppServices.User;
using RESTfulApi_Reddit.DbContexts;
using RESTfulApi_Reddit.Entities;
using RESTfulApi_Reddit.Helpers;
using RESTfulApi_Reddit.Services;
using RESTfulApi_Reddit.Utils;
using System;
using System.Linq;
using static RESTfulApi_Reddit.AppServices.User.DeleteUserCommand;
using static RESTfulApi_Reddit.AppServices.User.GetListQuery;
using static RESTfulApi_Reddit.AppServices.User.GetUserQuery;

namespace RESTfulApi_Reddit {
    public class Startup {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services) {
            services.AddMvc(option => option.EnableEndpointRouting = false)
                .AddNewtonsoftJson(serv => serv.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore); ;

            //services.AddNewtonsoftJson(setupAction =>
            //{
            //    setupAction.SerializerSettings.ContractResolver =
            //       new CamelCasePropertyNamesContractResolver();
            //})

            services.Configure<MvcOptions>(config => {
                var newtonsoftJsonOutputFormatter = config.OutputFormatters
                      .OfType<NewtonsoftJsonOutputFormatter>()?.FirstOrDefault();

                if (newtonsoftJsonOutputFormatter != null) {
                    newtonsoftJsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.marvin.hateoas+json");
                }
            });

            services.AddDbContext<RedditDbContext>(options => options.UseSqlServer(
                    @"Server=(localdb)\mssqllocaldb;Database=RedditDB;Trusted_Connection=True;"));
            

            //User
            services.AddScoped<ICommandHandler<DeleteUserCommand>,DeleteUserCommandHandler>();
            services.AddScoped<IQueryHandler<GetListQuery,PagedList<User>>,GetListQueryHandler>();
            services.AddScoped<IQueryHandler<GetUserQuery,User>,GetUserQueryHandler>();

            //Post


            //services.AddHandlers();

            services.AddScoped<IPostRepository, PostRepository>();
            services.AddScoped<IUserRepository, UserRepository>();


            services.AddTransient<IPropertyCheckerService, PropertyCheckerService>();

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddSingleton<Messages>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseMvc();
        }
    }
}
