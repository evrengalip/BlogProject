using BlogProject.Data.Repositories.Abstractions;
using BlogProject.Data.Repositories.Concretes;
using BlogProject.Data.UnitOfWorks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BlogProject.Data.Context;
using BlogProject.Data.Repositories.Abstractions;
using BlogProject.Data.Repositories.Concretes;
using BlogProject.Data.UnitOfWorks;

namespace BlogProject.Data.Extensions
{
    public static class DataLayerExtensions
    {
        public static IServiceCollection LoadDataLayerExtension(this IServiceCollection services, IConfiguration config)
        {
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(config.GetConnectionString("DefaultConnection")));

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            return services;
        }
    }
}