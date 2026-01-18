namespace Aurorae.Services;

public sealed class FileSyncWorker : BackgroundService
{
    private readonly IServiceScopeFactory factory;
    private readonly FileSystemWatcher watcher = new()
    {
        Path = LocalPath.Gallery,
        IncludeSubdirectories = true,
        NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName,
    };

    public FileSyncWorker(IServiceScopeFactory factory)
    {
        this.factory = factory;
        watcher.Created += async (s, e) => _ = RequestSyncAsync();
        watcher.Deleted += async (s, e) => _ = RequestSyncAsync();
        watcher.Renamed += async (s, e) => _ = RequestSyncAsync();
        // watcher.Changed += async (s, e) => _ = await RequestSyncAsync();
        // watcher.Error += async (s, e) => _ = await RequestSyncAsync();
    }

    private CancellationTokenSource? debounce;
    private readonly SemaphoreSlim semaphore = new(1);
    public async Task RequestSyncAsync()
    {
        debounce?.Cancel();
        debounce?.Dispose();
        debounce = new();

        try
        {
            await Task.Delay(TimeSpan.FromSeconds(10), debounce.Token);
        }
        catch (TaskCanceledException)
        {
            return;
        }

        if (!await semaphore.WaitAsync(0))
            return;

        try
        {
            using var scope = factory.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<FileSyncService>();
            await service.SyncAsync(watcher.Path);
        }
        finally
        {
            semaphore.Release();
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        watcher.EnableRaisingEvents = true;
        await RequestSyncAsync();

        try { await Task.Delay(Timeout.Infinite, stoppingToken); }
        catch (TaskCanceledException) { }
    }

    public override void Dispose()
    {
        watcher.EnableRaisingEvents = false;
        watcher.Dispose();
        debounce?.Dispose();
        semaphore.Dispose();
        base.Dispose();
    }
}
