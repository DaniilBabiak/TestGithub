using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Practice.Api.Services;
using Practice.Data;
using Practice.Entities.Entities;
using Practice.QueueProducers;
using Practice.QueueProducers.Interfaces;
using Practice.Service.Helpers;
using Practice.Service.Interfaces;
using Practice.Service.Services;
using Practice.WorkerService;
using System;
using System.Text;
using WebApi.Configurations;
using WebApi.Services;

namespace WebApi.Extensions
{
    public static class ServiceExtension
    {
        public static void ConfigureSqlServerContext(this IServiceCollection services, IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("SqlConnection");
            services.AddDbContext<DBContext>(options =>
            {
                options.UseSqlServer(
                    connectionString,
                    x => x.UseNetTopologySuite()
                    );
            });
        }

        public static void ConfigureRabbitMQ(this IServiceCollection services, IConfiguration configuration)
        {
            var rabbitMQSection = configuration.GetSection("RabbitMQSettings");
            services.Configure<RabbitMQSettings>(rabbitMQSection);
        }

        public static void ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddIdentity<User, IdentityRole<Guid>>().AddEntityFrameworkStores<DBContext>();

            var jwtSection = configuration.GetSection("JwtBearerTokenSettings");
            services.Configure<JwtBearerTokenSettings>(jwtSection);
            var jwtBearerTokenSettings = jwtSection.Get<JwtBearerTokenSettings>();

            var key = Encoding.ASCII.GetBytes(jwtBearerTokenSettings.SecretKey);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidIssuer = jwtBearerTokenSettings.Issuer,
                    ValidAudience = jwtBearerTokenSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                };
            });
        }

        public static void ConfigureScops(this IServiceCollection services)
        {
            services.AddScoped<IFileService, FileSerivce>();
            services.AddScoped(typeof(Practice.Data.Interfaces.IRepository<>), typeof(Practice.Data.Repository<>));
            services.AddScoped<IChallengeService, ChallengeService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IChallengeCommitService, ChallengeCommitService>();
            services.AddScoped<IUserChallengeService, UserChallengeService>();
            services.AddScoped<IChallengeTypeService, ChallengeTypeService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<INotificationProducer, NotificationProducer>();
            services.AddScoped<INotificationSender, NotificationSender>();
            services.AddScoped(typeof(ISortHelper<>), typeof(SortHelper<>));
            services.AddScoped<IAchievementService, AchievementService>();
            services.AddScoped<IUserNotificationsSettingsService, UserNotificationSettingsService>();
            services.AddScoped<IVenueService, VenueService>();
        }
    }
}
