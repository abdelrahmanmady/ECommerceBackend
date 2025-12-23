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
                .ForMember(d => d.ProductsCount, o => o.MapFrom(s => s.Products.Count));

            CreateMap<Brand, AdminBrandDetailsDto>();

            CreateMap<AdminCreateBrandDto, Brand>();

            CreateMap<AdminUpdateBrandDto, Brand>();

            CreateMap<Brand, BrandDto>()
                .ForMember(d => d.ProductsCount, o => o.MapFrom(s => s.Products.Count));

            //Category Mapping
            CreateMap<Category, AdminCategoryDto>()
                .ForMember(d => d.ParentCategoryName, o => o.MapFrom(s => s.Parent == null ? "Root Category" : s.Parent.Name))
                .ForMember(d => d.PathFromRoot, o => o.MapFrom(s => $"Root\\{s.HierarchyPath}"));

            CreateMap<Category, AdminCategoryDetailsDto>()
                .ForMember(d => d.SubcategoriesNames, o => o.Ignore())
                .ForMember(d => d.PathFromRoot, o => o.MapFrom(s => $"Root\\{s.HierarchyPath}"));

            CreateMap<AdminCreateCategoryDto, Category>();

            CreateMap<AdminUpdateCategoryDto, Category>();

            CreateMap<Category, CategoryDto>()
                .ForMember(d => d.Subcategories, o => o.Ignore());

            //Product Mapping
            CreateMap<Product, AdminProductDto>()
                .ForMember(d => d.ThumbnailUrl, o => o.MapFrom(s => s.Images.Where(pi => pi.IsMain).Select(pi => pi.ImageUrl).FirstOrDefault()))
                .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.Category.Name))
                .ForMember(d => d.BrandName, o => o.MapFrom(s => s.Brand.Name))
                .ForMember(d => d.InStock, o => o.MapFrom(s => s.StockQuantity > 0));

            CreateMap<Product, AdminProductDetailsDto>();

            CreateMap<AdminCreateProductDto, Product>();

            CreateMap<AdminUpdateProductDto, Product>();

            CreateMap<Product, ProductDto>()
                .ForMember(d => d.ThumbnailUrl, o => o.MapFrom(s => s.Images.Where(pi => pi.IsMain).Select(pi => pi.ImageUrl).FirstOrDefault()))
                .ForMember(d => d.BrandedName, o => o.MapFrom(s => s.Brand.Name + " " + s.Name));

            CreateMap<Product, ProductDetailsDto>()
                .ForMember(d => d.BrandName, o => o.MapFrom(s => s.Brand.Name))
                .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.Category.Name));

            //ProductImage Mapping
            CreateMap<ProductImage, ProductImageDto>();

            //ShoppingCart Mapping
            CreateMap<ShoppingCart, ShoppingCartDto>();

            CreateMap<CartItem, CartItemDto>()
                .ForMember(d => d.ProductThumbnailUrl, o => o.MapFrom(s => s.Product.Images.Where(pi => pi.IsMain).Select(pi => pi.ImageUrl).FirstOrDefault()))
                .ForMember(d => d.ProductName, o => o.MapFrom(s => s.Product.Name))
                .ForMember(d => d.ProductPrice, o => o.MapFrom(s => s.Product.Price));

            //Checkout Mapping
            CreateMap<ShoppingCart, CheckoutPreviewDto>();

            CreateMap<CartItem, OrderItem>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.OrderedProduct, o => o.MapFrom(s => new OrderedProduct
                {
                    Id = s.Product.Id,
                    Name = s.Product.Name,
                    Description = s.Product.Description,
                    Price = s.Product.Price,
                    ThumbnailUrl = s.Product.Images.Where(i => i.IsMain).Select(i => i.ImageUrl).First()
                }));

            //Address Mapping
            CreateMap<Address, AddressDto>();

            CreateMap<CreateAddressDto, Address>();

            CreateMap<UpdateAddressDto, Address>();

            CreateMap<Address, OrderAddress>();

            //Order Mapping
            CreateMap<Order, AdminOrderDto>()
                .ForMember(d => d.UserName, o => o.MapFrom(s => s.User.FirstName + " " + s.User.LastName))
                .ForMember(d => d.ItemsCount, o => o.MapFrom(s => s.Items.Count));

            CreateMap<Order, AdminOrderDetailsDto>()
                .ForMember(d => d.UserName, o => o.MapFrom(s => s.User.FirstName + " " + s.User.LastName));

            CreateMap<Order, OrderDto>()
                .ForMember(d => d.ItemsCount, o => o.MapFrom(s => s.Items.Count));

            //OrderItem Mapping
            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(d => d.ProductId, o => o.MapFrom(s => s.OrderedProduct.Id))
                .ForMember(d => d.ProductName, o => o.MapFrom(s => s.OrderedProduct.Name))
                .ForMember(d => d.ProductThumbnailUrl, o => o.MapFrom(s => s.OrderedProduct.ThumbnailUrl));

            //OrderTrackingMilestone Mapping
            CreateMap<OrderTrackingMilestone, OrderTrackingMilestoneDto>();

            //User Mapping
            CreateMap<RegisterDto, ApplicationUser>();
            CreateMap<ApplicationUser, UserSessionDto>()
                .ForMember(d => d.Name, o => o.MapFrom(s => s.FirstName + " " + s.LastName));
        }
    }
}
