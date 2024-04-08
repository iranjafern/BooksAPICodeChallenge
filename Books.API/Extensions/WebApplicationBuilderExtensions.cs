using Books.API.Constants;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;

namespace Books.API.Extensions
{
    public static class WebApplicationBuilderExtensions
    {
        public static WebApplicationBuilder AddSwagger(this WebApplicationBuilder builder)
        {
            builder.Services.AddSwaggerGen(option =>
            {
                option.AddSecurityDefinition(name: JwtBearerDefaults.AuthenticationScheme, securityScheme: new OpenApiSecurityScheme
                {
                    Name = AppConstants.Authorization,
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = JwtBearerDefaults.AuthenticationScheme
                });
                option.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = JwtBearerDefaults.AuthenticationScheme
                            }
                        }, new string[]{ }
                    }
                });
            });
            return builder;
        }

        public static WebApplicationBuilder AddAllowedOrigins(this WebApplicationBuilder builder)
        {
            var allowedOrigins = builder.Configuration.GetSection(AppConstants.AllowedOriginsConfigKey).GetChildren();

            if (allowedOrigins != null)
            {
                var origins = allowedOrigins.Select(x => x.Value).ToArray();
                builder.Services.AddCors(options =>
                {
                    options.AddPolicy(
                        name: AppConstants.AllowedOriginsConfigKey,
                        policy =>
                        {
                            policy.WithOrigins(origins)
                                    .WithMethods(new string[] { "" })
                                    .WithHeaders(new string[] { "" })
                                    .WithExposedHeaders(new string[] { });
                        });
                });
            }
            return builder;
        }
    }
}
