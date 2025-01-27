using System;
using System.Text;
using API.Data;
using API.Interfaces;
using API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace API.Extensions;

public static class ApplicationServiceExtensions
{
  /// <summary>
  /// Adds application-specific services to the IServiceCollection.
  /// </summary>
  /// <param name="services">The IServiceCollection to add services to.</param>
  /// <param name="config">The IConfiguration instance used to retrieve configuration settings.</param>
  /// <returns>The updated IServiceCollection with the added services.</returns>
  /// <exception cref="ArgumentNullException">Thrown when the TokenKey is not found in the configuration.</exception>
  public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
  {
    services.AddDbContext<DataContext>(opt =>
    {
      opt.UseSqlite(config.GetConnectionString("DefaultConnection"));
    });

    services.AddCors();

    services.AddScoped<ITokenService, TokenService>();

    return services;
  }
}
