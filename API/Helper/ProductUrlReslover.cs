
using API.Dtos;
using AutoMapper;
using Core.Entities;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;

namespace API.Helper
{
    public class ProductUrlReslover : IValueResolver<Product, ProductToReturnDto, string>
    {
        private readonly IConfiguration _config;

        public ProductUrlReslover(IConfiguration config)
        {
            _config = config;
        }
        public string Resolve(Product source, ProductToReturnDto destination, string destMember, ResolutionContext context)
        {
            if (!string.IsNullOrEmpty(source.PictureUrl))
            {
                return _config["ApiUrl"] + source.PictureUrl;
            }
            return null;
        }
    }
}