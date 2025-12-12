using AutoMapper;
using ECommerce.Business.DTOs.Addresses;
using ECommerce.Business.DTOs.Auth;
using ECommerce.Business.DTOs.Brands;
using ECommerce.Business.DTOs.Categories;
using ECommerce.Business.DTOs.OrderItems;
using ECommerce.Business.DTOs.Orders;
using ECommerce.Business.DTOs.ProductImages;
using ECommerce.Business.DTOs.Products;
using ECommerce.Business.DTOs.ShoppingCart;
using ECommerce.Business.DTOs.Users;
using ECommerce.Core.Entities;

namespace ECommerce.Business.Mappings
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

            //Product Mapping
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(mapExpression: src => src.Category.Name))
                .ForMember(dest => dest.BrandName, opt => opt.MapFrom(mapExpression: src => src.Brand.Name))
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src =>
                        src.Images.Where(i => i.IsMain).Select(i => i.ImageUrl).FirstOrDefault()));
            CreateMap<CreateProductDto, Product>();
            CreateMap<UpdateProductDto, Product>();

            //ProductImage Mapping
            CreateMap<ProductImage, ProductImageDto>();

            //Order Mapping
            CreateMap<Order, OrderDto>();

            //OrderItem Mapping
            CreateMap<OrderItem, OrderItemDto>();

            //User Mapping
            CreateMap<RegisterDto, ApplicationUser>();
            CreateMap<ApplicationUser, RegisterResponseDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.FirstName + src.LastName));
            CreateMap<ApplicationUser, UserDetailsDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.FirstName + src.LastName))
                .ForMember(dest => dest.TotalOrders, opt => opt.MapFrom(src => src.Orders.Count));
            CreateMap<ApplicationUser, UserManagementDto>()
                .ForMember(dest => dest.TotalOrders, opt => opt.MapFrom(src => src.Orders.Count));
            CreateMap<UpdateUserDto, ApplicationUser>();


            CreateMap<ApplicationUser, UserManagementDto>();
            CreateMap<UpdateUserDto, ApplicationUser>();
            //Address Mapping
            CreateMap<Address, AddressDto>();
            CreateMap<Address, AddressWithUserDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email));
            CreateMap<CreateAddressDto, Address>();
            CreateMap<UpdateAddressDto, Address>();
            CreateMap<Address, OrderAddress>();

            //ShoppingCart Mapping
            CreateMap<ShoppingCart, ShoppingCartDto>();
            CreateMap<CartItem, CartItemDto>()
                .ForMember(dest => dest.ProductName, opt
                    => opt.MapFrom(src
                        => src.Product.Name))
                .ForMember(dest => dest.ProductImageUrl, opt
                    => opt.MapFrom(src
                        => src.Product.Images
                        .Where(i => i.IsMain)
                        .Select(i => i.ImageUrl)
                        .FirstOrDefault()))
                .ForMember(dest => dest.Price, opt
                    => opt.MapFrom(src
                        => src.Product.Price));

            //Checkout Mapping
            CreateMap<CartItem, OrderItem>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.Product.Price));
        }
    }
}
