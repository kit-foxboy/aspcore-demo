using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using API.Data;
using API.Entities;
using API.DTOs;
using API.Controllers;
using System.Security.Cryptography;
using System.Text;

public class DatabaseFixture : IDisposable
{
    public ServiceProvider ServiceProvider { get; private set; }

    public DatabaseFixture()
    {
        var services = new ServiceCollection();
        services.AddDbContext<DataContext>(opt => opt.UseInMemoryDatabase("TestDatabase"));
        services.AddScoped<AccountController>();
        ServiceProvider = services.BuildServiceProvider();

        SetupDb();
    }

    private void SetupDb()
    {
        using var scope = ServiceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();

        using var hmac = new HMACSHA512();

        context.Users.Add(new AppUser
        {
            Id = 1,
            UserName = "test",
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("password")),
            PasswordSalt = hmac.Key
        });
        context.SaveChanges();
    }

    public void Dispose()
    {
        using var scope = ServiceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();
        context.Database.EnsureDeleted();
    }
}
