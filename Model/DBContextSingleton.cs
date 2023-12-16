using System;
using TMS_Project.DataLayer.Context;

namespace TMS_Project.Model;

public static class DbContextSingleton
{
    // Lazy initialization to make sure thread-safe creation of the instance.
    private static readonly Lazy<TmsDbContext> LazyInstance = new Lazy<TmsDbContext>(() => new TmsDbContext());

    // Gets single instance of the db context
    public static TmsDbContext Instance => LazyInstance.Value;

}