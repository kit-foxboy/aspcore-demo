using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace API.Extensions;

public static class IdentityServiceExtensions
{
  /// <summary>
  /// Adds identity services to the IServiceCollection.
  /// </summary>
  /// <param name="services">The IServiceCollection to add services to.</param>
  /// <param name="config">The IConfiguration instance used to retrieve configuration settings.</param>
  /// <returns>The updated IServiceCollection with the added services.</returns>
  public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config)
  {
    services.AddAuthentication(opt =>
    {
      opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
      opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(opt =>
    {
      var tokenKey = config["TokenKey"] ?? throw new ArgumentNullException(nameof(config), "TokenKey not found in appsettings.json. Please ensure it is included in the configuration.");
      opt.TokenValidationParameters = new TokenValidationParameters
      {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey)),
        ValidateIssuer = false,
        ValidateAudience = false,
      };
    });

    return services;
  }
}
