using System;
using TMS_Project.DataLayer.Context;

namespace TMS_Project.Model;

public static class DbContextSingleton
{
    private static readonly Lazy<TmsDbContext> LazyInstance = new Lazy<TmsDbContext>(() => new TmsDbContext());

    public static TmsDbContext Instance => LazyInstance.Value;
}