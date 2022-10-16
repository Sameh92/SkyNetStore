using API.Extensions;
using API.Helper;
using API.Middleware;
using Infrastructure.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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


            services.AddDbContext<StoreContextDB>(options =>
            {
                options.UseSqlite(_configuration.GetConnectionString("DefaultConnection"));
            });

            services.addApplicationServices();

            services.AddAutoMapper(typeof(MappingProfiles));
            services.AddSwaggerExtension();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline. and we use it to Vaildation error Response
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<ExceptionMiddleware>();


            app.UseStatusCodePagesWithReExecute("/errors/{0}"); //to handle the not Exist API endpoint

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseStaticFiles(); //To enable our API to serve Image and static contant 

            app.UseAuthorization();
            app.UseSwaggerExtension();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
