using API.Extensions;
using API.Helper;
using API.Middleware;
using Infrastructure.Data;
using Infrastructure.Data.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace API
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
            //Configuration = configuration;
        }

        //public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();

            // add Application dbcontext 
            services.AddDbContext<StoreContextDB>(options =>
            {
                options.UseSqlite(_configuration.GetConnectionString("DefaultConnection"));
            });

            // add Identity dbcontext 
            services.AddDbContext<AppSamRanIdentityDbContext>(options =>
            {
                options.UseSqlite(_configuration.GetConnectionString("IdentityConnection"));
            });

            // add redis configuration 

            services.AddSingleton<IConnectionMultiplexer>(c =>
            {
                var configuration = ConfigurationOptions.Parse(_configuration.GetConnectionString("Redis"), true);
                return ConnectionMultiplexer.Connect(configuration);
            });


            services.addApplicationServices();
            services.AddIdentityServices(_configuration);
            services.AddAutoMapper(typeof(MappingProfiles));
            services.AddSwaggerExtension();
            services.AddCors(opt =>
            {
                opt.AddPolicy("CorsPolicy", policy =>
                {
                    policy.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:4200");
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline. and we use it to Vaildation error Response
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<ExceptionMiddleware>();


            app.UseStatusCodePagesWithReExecute("/errors/{0}"); //to handle the not Exist API endpoint

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseStaticFiles(); //To enable our API to serve Image and static contant 
            app.UseCors("CorsPolicy");
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSwaggerExtension();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
