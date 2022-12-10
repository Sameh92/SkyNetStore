using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Errors;
using API.Helper;
using API.Middleware;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Infrastructure.Service;

namespace API.Extensions
{
    public static class ApplicationServicesExtensions
    {
        public static IServiceCollection addApplicationServices(this IServiceCollection services)
        {

            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IBasketRepository, BasketRepository>();
            services.AddScoped<ITokenService, TokenService>();


            //To configure and override our ApiController Attribute behavior and this configure must be after addController
            services.Configure<ApiBehaviorOptions>(options =>
            {

                options.InvalidModelStateResponseFactory = ActionContext =>
                {
                    var erros = ActionContext.ModelState.Where(e => e.Value.Errors.Count > 0)
                   .SelectMany(x => x.Value.Errors)
                   .Select(x => x.ErrorMessage).ToArray();
                    var errorResponse = new ApiValidationErrorResponse
                    {
                        Errors = erros
                    };
                    return new BadRequestObjectResult(errorResponse);
                };

            });
            // before override ApiController response was :
            //             {
            //     "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            //     "title": "One or more validation errors occurred.",
            //     "status": 400,
            //     "traceId": "00-8cdd712070b4cc43bd6983b213b8bad9-b2129334e4a25c42-00",
            //     "errors": {
            //         "id": [
            //             "The value 'five' is not valid."
            //         ]
            //     }
            // }

            // after override ApiControllerthe current resposne is:
            //             {
            //     "errors": [
            //         "The value 'five' is not valid."
            //     ],
            //     "statusCode": 400,
            //     "message": "A bad request ,you have made"
            // }

            return services;
        }
    }
}