using Microsoft.Extensions.Options;
using MultiverseScout.Api.Data;
using RethinkDb.Driver.Net;

namespace MultiverseScout.Api.HostedServices;

public sealed class RethinkDbStartupService : IHostedService
{
    private readonly IConnection _conn;
    private readonly string _db;

    public RethinkDbStartupService(IConnection conn, IOptions<RethinkDbOptions> options)
    {
        _conn = conn;
        _db = options.Value.Database;
    }

    public async Task StartAsync(CancellationToken ct)
    {
        await RethinkDbSetup.EnsureAllAsync(_conn, _db, ct).ConfigureAwait(false);
    }

    public Task StopAsync(CancellationToken ct) => Task.CompletedTask;
}
