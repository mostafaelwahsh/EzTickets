using AutoMapper;
using System.Text;
using Data;
using EzTickets.Repository;
using EzTickets.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Models;

namespace EzTicket
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();

            builder.Services.AddScoped<IApplicationBuilder, ApplicationBuilder>();
            builder.Services.AddScoped<IEventRepository, EventRepository>();
            builder.Services.AddScoped<ITicketRepository, TicketRepository>();
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IInfoRepository, InfoRepository>();

            builder.Services.AddAutoMapper(typeof(MappingProfile));

            // Register DataContext with Connection String
            builder.Services.AddDbContext<DataContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("CS"),
                    sqlOptions => sqlOptions.MigrationsAssembly("EzTickets")
                ));

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<DataContext>()
                .AddDefaultTokenProviders();

            // Allow CORS
            builder.Services.AddCors(options =>
                options.AddPolicy("EzPolicy", policy =>
                    policy.AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin()
                ));

            // ? JWT Authentication Configuration (Safe Key Access)
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["JWT:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["JWT:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            builder.Configuration["JWT:Key"]
                            ?? throw new InvalidOperationException("JWT:Key is missing in appsettings.json")
                        )
                    )
                };
            });

            // ?? Swagger Settings
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(swagger =>
            {
                swagger.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "ASP.NET 8 Web API",
                    Description = "ITI Project"
                });

                // Enable JWT auth in Swagger
                swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' [space] and then your valid token.\nExample: \"Bearer eyJhbGci...\""
                });

                swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                        new string[] {}
                    }
                });
            });

            var app = builder.Build();

            // Seed Roles
            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var roles = new[] { "Admin", "User" };

                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                        await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage(); // ? Add this
                app.UseSwagger();
                app.UseSwaggerUI();
            }


            app.UseStaticFiles();
            app.UseCors("EzPolicy");

            app.UseAuthentication(); // ? Add this before UseAuthorization
            app.UseAuthorization();

            app.MapControllers();
            app.Run();
        }
    }
}
