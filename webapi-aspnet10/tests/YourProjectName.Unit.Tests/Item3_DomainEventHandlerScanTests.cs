using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;
using YourProjectName.Domain.Shared;

namespace YourProjectName.Unit.Tests;

/// <summary>
/// Item 3 — Domain event handler Scrutor scan + AO4 signature fix (CancellationToken non-nullable).
/// Target: Scrutor scan registers IDomainEventHandler<> as scoped; CancellationToken (non-nullable).
/// </summary>
public sealed class Item3_DomainEventHandlerScanTests
{
    // Dummy types to prove the Scrutor scan pattern picks up IDomainEventHandler<> implementations.
    // The production code scans the Application assembly; here we scan the test assembly.
    public sealed class TestDomainEvent : IDomainEvent;
    public sealed class TestDomainEventHandler : IDomainEventHandler<TestDomainEvent>
    {
        public Task Handle(TestDomainEvent domainEvent, CancellationToken cancellationToken = default)
            => Task.CompletedTask;
    }

    private static readonly string RepoRootPath = RepoRoot.Find();

    private static readonly string AppDiPath = Path.Combine(RepoRootPath, "webapi-aspnet10", "src",
        "YourProjectName.Application", "DependencyInjection.cs");

    // ---------------------------------------------------------------
    // Unit: Scrutor scan pattern wires IDomainEventHandler<> correctly
    // ---------------------------------------------------------------

    /// <summary>
    /// Proves the Scrutor scan lambda shape used in production (scan.FromAssemblies,
    /// AddClasses, AssignableTo, AsImplementedInterfaces, WithScopedLifetime)
    /// correctly discovers and registers IDomainEventHandler&lt;&gt; implementations.
    ///
    /// Production scans the Application assembly; this test scans the test assembly
    /// where a dummy handler is defined.
    /// </summary>
    [Fact]
    public void ScrutorScanPattern_Discovers_IDomainEventHandler_Implementations()
    {
        var services = new ServiceCollection();

        // Same scan lambda shape the production code uses against the Application assembly
        services.Scan(scan => scan
            .FromAssemblies(typeof(TestDomainEventHandler).Assembly)
            .AddClasses(classes => classes.AssignableTo(typeof(IDomainEventHandler<>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        var provider = services.BuildServiceProvider();

        var handlers = provider.GetServices<IDomainEventHandler<TestDomainEvent>>().ToList();

        Assert.Single(handlers);
        Assert.IsType<TestDomainEventHandler>(handlers[0]);
    }

    // ---------------------------------------------------------------
    // Reflection: IDomainEventHandler<>.Handle cancellation token signature
    // ---------------------------------------------------------------

    [Fact]
    public void IDomainEventHandler_Handle_CancellationToken_IsNonNullable()
    {
        // Get the concrete closed generic type IDomainEventHandler<IDomainEvent>
        var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(typeof(IDomainEvent));

        // Get the Handle method
        var handleMethod = handlerType.GetMethod("Handle");
        Assert.NotNull(handleMethod);

        // Parameter at index 1 (second parameter, 0-indexed) should be CancellationToken
        var parameters = handleMethod!.GetParameters();
        Assert.True(parameters.Length >= 2,
            "Expected Handle to have at least 2 parameters.");
        var cancellationParam = parameters[1];

        Assert.Equal(typeof(CancellationToken), cancellationParam.ParameterType);
    }

    // ---------------------------------------------------------------
    // Source-level: Application/DependencyInjection.cs
    // ---------------------------------------------------------------

    [Fact]
    public void AppDependencyInjection_Contains_IDomainEventHandler()
    {
        var content = File.ReadAllText(AppDiPath);
        Assert.True(content.Contains("IDomainEventHandler", StringComparison.Ordinal),
            "Expected Application/DependencyInjection.cs to reference IDomainEventHandler " +
            "for the Scrutor scan registration.");
    }

    [Fact]
    public void AppDependencyInjection_Contains_ScanCall()
    {
        var content = File.ReadAllText(AppDiPath);
        Assert.True(content.Contains(".Scan(", StringComparison.Ordinal),
            "Expected Application/DependencyInjection.cs to call .Scan() for Scrutor assembly scanning.");
    }
}
