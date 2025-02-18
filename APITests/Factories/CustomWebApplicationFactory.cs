using System;
using System.ComponentModel;
using System.Data.Common;
using API.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace APITests.Factories;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{ 
  protected override void ConfigureWebHost(IWebHostBuilder builder)
  {
    builder.ConfigureServices(services =>
    {
      services.AddSingleton<DbConnection>(container => {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        return connection;
      });

      services.AddDbContext<DataContext>((container, options) => {
        var connection = container.GetRequiredService<DbConnection>();
        options.UseSqlite(connection);
      });
    });

    builder.UseEnvironment("Testing");
  }
}
