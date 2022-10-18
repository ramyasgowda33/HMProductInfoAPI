using AutoMapper;
using HMProductInfoAPI.DTO;
using HMProductInfoAPI.Models;

namespace HMProductInfoAPI
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Product, ProductDto>();
            CreateMap<Article, ArticleDto>();

            CreateMap<ProductDto, Product>();
            CreateMap<ArticleDto, Article>();
        }
    }
}
