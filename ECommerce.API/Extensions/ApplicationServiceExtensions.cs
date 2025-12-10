using ECommerce.API.Middleware;
using ECommerce.Business.DTOs.Errors;
using ECommerce.Business.Interfaces;
using ECommerce.Business.Mappings;
using ECommerce.Business.Services;
using ECommerce.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ECommerce.API.Extensions
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
            services.AddScoped<IAddressService, AddressService>();
            services.AddScoped<ICartService, CartService>();

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

                        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                        logger.LogWarning("400 Validation Error: {Error}", errors);

                        var errorResponse = new ApiErrorResponseDto
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
            services.AddOpenApi(options =>
            {
                // 1. Define the Bearer Scheme globally (The "Definition")
                options.AddDocumentTransformer((document, context, cancellationToken) =>
                {
                    document.Info = new OpenApiInfo
                    {
                        Title = "ECommerce API",
                        Version = "v1",
                        Description = "API for E-Commerce Backend with JWT Authentication"
                    };

                    document.Components ??= new OpenApiComponents();
                    document.Components.SecuritySchemes.Add("Bearer", new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer",
                        BearerFormat = "JWT",
                        In = ParameterLocation.Header,
                        Description = "Enter your valid token."
                    });

                    return Task.CompletedTask;
                });

                // 2. Apply the Scheme ONLY to protected endpoints (The "Logic")
                options.AddOperationTransformer((operation, context, cancellationToken) =>
                {
                    var metadata = context.Description.ActionDescriptor.EndpointMetadata;

                    // Check if endpoint has [Authorize] attribute
                    bool hasAuthorize = metadata.Any(m => m is Microsoft.AspNetCore.Authorization.AuthorizeAttribute);
                    // Check if endpoint has [AllowAnonymous] attribute (overrides Authorize)
                    bool hasAnonymous = metadata.Any(m => m is Microsoft.AspNetCore.Authorization.AllowAnonymousAttribute);

                    // Only add the lock if Authorized AND NOT Anonymous
                    if (hasAuthorize && !hasAnonymous)
                    {
                        operation.Security = new List<OpenApiSecurityRequirement>
                        {
                            new()
                            {
                                {
                                    new OpenApiSecurityScheme
                                    {
                                        Reference = new OpenApiReference
                                        {
                                            Type = ReferenceType.SecurityScheme,
                                            Id = "Bearer"
                                        }
                                    },
                                    Array.Empty<string>()
                                }
                            }
                        };
                    }

                    return Task.CompletedTask;
                });
            });

            return services;
        }
    }
}
