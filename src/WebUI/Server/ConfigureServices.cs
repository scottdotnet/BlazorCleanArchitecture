using BlazorCleanArchitecture.Shared.Common.Interfaces;
using BlazorCleanArchitecture.Shared.Common;
using BlazorCleanArchitecture.WebUI.Server.Common.Interceptors;
using BlazorCleanArchitecture.WebUI.Server.Common.Middlewares;
using BlazorCleanArchitecture.WebUI.Server.Controllers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.IdentityModel.Tokens;
using ProtoBuf.Grpc.Server;
using System.Net;
using System.Text;

namespace BlazorCleanArchitecture.WebUI.Server
{
    public static class ConfigureServices
    {
        public static void AddWebServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllersWithViews();
            services.AddRazorPages();
            services.AddHttpContextAccessor();

            services.AddGrpc(options =>
            {
                options.Interceptors.Add<GrpcExceptionInterceptor>();
                options.ResponseCompressionAlgorithm = "gzip";
                options.ResponseCompressionLevel = System.IO.Compression.CompressionLevel.Optimal;
                options.EnableDetailedErrors = true;
            });
            services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    new[] { "application/octet-stream" });
            });
            services.AddCodeFirstGrpc(config =>
            {
                config.EnableDetailedErrors = true;
                config.ResponseCompressionLevel = System.IO.Compression.CompressionLevel.Optimal;
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = configuration.GetSection("Jwt")["Issuer"],
                        ValidAudience = configuration.GetSection("Jwt")["Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetSection("Jwt")["Key"]))
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            context.Response.OnStarting(() =>
                            {
                                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                                return Task.CompletedTask;
                            });

                            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                                context.Response.Headers.Add("Token-Expired", "true");

                            return Task.CompletedTask;
                        }
                    };

                    options.MapInboundClaims = false;
                });

            services.AddLogging();
        }

        public static void UseWebServices(this WebApplication app)
        {
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMiddleware<TenantMiddleware>();

            app.MapGrpcService<AuthenticationController>();
        }
    }
}
