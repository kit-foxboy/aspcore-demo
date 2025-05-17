using System;
using API.Data;
using APITests.Factories;
using Microsoft.Extensions.DependencyInjection;

namespace APITests.Helpers;

public class DbTest
{
  protected static DataContext? _context;

  [ClassInitialize]
  public static async Task InitializeAsync(TestContext testContext)
  {
    var factory = new CustomWebApplicationFactory();
    _context = factory.Services.GetRequiredService<DataContext>();

    await _context.Database.EnsureDeletedAsync();
    await _context.Database.EnsureCreatedAsync();

    await Seed.SeedUsers(_context);
  }
}
