using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApp.API.Data;
using MyApp.API.Interfaces;
using MyApp.API.Mappings;
using MyApp.API.Middleware;
using MyApp.API.Services;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MyApp.API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            //Global Error Handling
            services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddProblemDetails();

            // Register DbConext
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(config.GetConnectionString("constr"))
            );

            //Register Automapper
            services.AddAutoMapper(cfg =>
                cfg.AddProfile<MappingProfile>()
            );

            //Register HttpContextAccessor to access User Claims in Services
            services.AddHttpContextAccessor();

            //Register Custom Services in IOC Container
            services.AddScoped<IBrandService, BrandService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IProductImageService, ProductImageService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ITokenService, TokenService>();

            //Controllers Configuration
            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    // accept enums as strings, case insensitive
                    options.JsonSerializerOptions.Converters.Add(
                        new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                    );
                })
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.InvalidModelStateResponseFactory = context =>
                    {
                        var errors = context.ModelState
                            .Where(e => e.Value?.Errors.Count > 0)
                            .Select(e => $"{e.Key}: {e.Value?.Errors.First().ErrorMessage}")
                            .ToList();

                        var errorResponse = new MyApp.API.DTOs.Errors.ApiErrorResponseDto
                        {
                            StatusCode = 400,
                            Message = "Validation failed",
                            Detail = string.Join("; ", errors),
                            TimeStamp = DateTime.UtcNow
                        };

                        return new BadRequestObjectResult(errorResponse);
                    };
                });


            //OpenApi
            services.AddOpenApi();

            return services;
        }
    }
}
