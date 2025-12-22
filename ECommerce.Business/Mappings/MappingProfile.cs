using AutoMapper;
using ECommerce.Business.DTOs.Addresses;
using ECommerce.Business.DTOs.Auth;
using ECommerce.Business.DTOs.Brands.Admin;
using ECommerce.Business.DTOs.Brands.Store;
using ECommerce.Business.DTOs.Categories.Admin;
using ECommerce.Business.DTOs.Categories.Store;
using ECommerce.Business.DTOs.Checkout;
using ECommerce.Business.DTOs.OrderItems;
using ECommerce.Business.DTOs.Orders.Admin;
using ECommerce.Business.DTOs.Orders.Profile;
using ECommerce.Business.DTOs.OrderTrackingMilestones;
using ECommerce.Business.DTOs.ProductImages;
using ECommerce.Business.DTOs.Products.Admin;
using ECommerce.Business.DTOs.Products.Store;
using ECommerce.Business.DTOs.ShoppingCart;
using ECommerce.Business.DTOs.Users.Auth;
using ECommerce.Core.Entities;

namespace ECommerce.Business.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //Brand Mapping
            CreateMap<Brand, AdminBrandDto>()
                .ForMember(d => d.ProductsCount,
                o => o.MapFrom(s => s.Products.Count));
            CreateMap<Brand, AdminBrandDetailsDto>();
            CreateMap<AdminCreateBrandDto, Brand>();
            CreateMap<AdminUpdateBrandDto, Brand>();
            CreateMap<Brand, BrandDto>()
                .ForMember(d => d.ProductsCount,
                o => o.MapFrom(s => s.Products.Count));

            //Category Mapping
            CreateMap<Category, AdminCategoryDto>()
                .ForMember(d => d.ParentCategoryName,
                o => o.MapFrom(s => s.Parent == null ? "Root Category" : s.Parent.Name))
                .ForMember(d => d.PathFromRoot,
                o => o.MapFrom(s => $"Root\\{s.HierarchyPath}"));
            CreateMap<Category, AdminCategoryDetailsDto>()
                .ForMember(d => d.SubcategoriesNames,
                o => o.Ignore())
                .ForMember(d => d.PathFromRoot,
                o => o.MapFrom(s => $"Root\\{s.HierarchyPath}"));
            CreateMap<AdminCreateCategoryDto, Category>();
            CreateMap<AdminUpdateCategoryDto, Category>();
            CreateMap<Category, CategoryDto>()
                .ForMember(d => d.Subcategories, o => o.Ignore());

            //Product Mapping
            CreateMap<Product, AdminProductDto>()
                .ForMember(d => d.ThumbnailUrl,
                o => o.MapFrom(s => s.Images
                                                            .Where(pi => pi.IsMain)
                                                            .Select(pi => pi.ImageUrl)
                                                            .FirstOrDefault()))
                .ForMember(d => d.CategoryName,
                o => o.MapFrom(s => s.Category.Name))
                .ForMember(d => d.BrandName,
                o => o.MapFrom(s => s.Brand.Name))
                .ForMember(d => d.InStock,
                o => o.MapFrom(s => s.StockQuantity > 0));
            CreateMap<Product, AdminProductDetailsDto>();
            CreateMap<AdminCreateProductDto, Product>();
            CreateMap<AdminUpdateProductDto, Product>();
            CreateMap<Product, ProductDto>()
                .ForMember(d => d.ThumbnailUrl,
                o => o.MapFrom(s => s.Images
                                                            .Where(pi => pi.IsMain)
                                                            .Select(pi => pi.ImageUrl)
                                                            .FirstOrDefault()))
                .ForMember(d => d.BrandedName,
                o => o.MapFrom(s => s.Brand.Name + " " + s.Name));
            CreateMap<Product, ProductDetailsDto>()
                .ForMember(d => d.BrandName,
                o => o.MapFrom(s => s.Brand.Name))
                .ForMember(d => d.CategoryName,
                o => o.MapFrom(s => s.Category.Name));

            //ProductImage Mapping
            CreateMap<ProductImage, ProductImageDto>();

            //ShoppingCart Mapping
            CreateMap<ShoppingCart, ShoppingCartDto>();
            CreateMap<CartItem, CartItemDto>()
                .ForMember(dest => dest.ProductThumbnailUrl,
                opt => opt.MapFrom(src => src.Product.Images
                                                                .Where(pi => pi.IsMain)
                                                                .Select(pi => pi.ImageUrl)
                                                                .FirstOrDefault()))
                .ForMember(dest => dest.ProductName,
                opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.ProductPrice,
                opt => opt.MapFrom(src => src.Product.Price));

            //Checkout Mapping
            CreateMap<ShoppingCart, CheckoutPreviewDto>();
            CreateMap<CartItem, OrderItem>()
                .ForMember(d => d.UnitPrice,
                o => o.MapFrom(s => s.Product.Price))
                .ForMember(d => d.TotalPrice,
                o => o.MapFrom(s => s.Product.Price * s.Quantity));


            //Order Mapping
            CreateMap<Order, AdminOrderDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.FirstName + " " + src.User.LastName))
                .ForMember(dest => dest.ItemsCount, opt => opt.MapFrom(src => src.Items.Count));
            CreateMap<Order, AdminOrderDetailsDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.FirstName + " " + src.User.LastName));
            CreateMap<Order, OrderDto>();

            //OrderItem Mapping
            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(dest => dest.ProductId,
                opt => opt.MapFrom(src => src.ProductOrdered.ProductId))
                .ForMember(dest => dest.ProductName,
                opt => opt.MapFrom(src => src.ProductOrdered.ProductName))
                .ForMember(dest => dest.ProductThumbnailUrl, opt
                => opt.MapFrom(src => src.ProductOrdered.PictureUrl));

            //OrderTrackingMilestone Mapping
            CreateMap<OrderTrackingMilestone, OrderTrackingMilestoneDto>();


            //Address Mapping
            CreateMap<Address, AddressDto>();
            CreateMap<Address, AddressWithUserDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email));
            CreateMap<CreateAddressDto, Address>();
            CreateMap<UpdateAddressDto, Address>();
            CreateMap<Address, OrderAddress>();


            //User Mapping
            CreateMap<RegisterDto, ApplicationUser>();
            CreateMap<ApplicationUser, UserSessionDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.FirstName + " " + src.LastName));
        }
    }
}
