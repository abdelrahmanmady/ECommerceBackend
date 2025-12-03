using AutoMapper;
using MyApp.API.DTOs.Brands;
using MyApp.API.DTOs.Categories;
using MyApp.API.Entities;

namespace MyApp.API.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //Brand Mapping
            CreateMap<Brand, BrandDto>();
            CreateMap<CreateBrandDto, Brand>();
            CreateMap<UpdateBrandDto, Brand>();

            //Category Mapping
            CreateMap<Category, CategoryDto>();
            CreateMap<CreateCategoryDto, Category>();
            CreateMap<UpdateCategoryDto, Category>();

        }
    }
}
