using System;
using API.Data;
using APITests.Factories;
using Microsoft.Extensions.DependencyInjection;

namespace APITests.Helpers;

public class DbTest
{
  [TestInitialize]
  public async Task InitializeAsync()
  {
    var factory = new CustomWebApplicationFactory();
    var context = factory.Services.GetRequiredService<DataContext>();

    await context.Database.EnsureDeletedAsync();
    await context.Database.EnsureCreatedAsync();

    await Seed.SeedUsers(context);
  }
}
