namespace Aurorae.Services;

public sealed class FileSyncWorker : BackgroundService
{
    private readonly IServiceScopeFactory factory;
    private readonly ILogger<FileSyncWorker> logger;
    private readonly FileSystemWatcher watcher = new()
    {
        Path = LocalPath.Gallery,
        IncludeSubdirectories = true,
        NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName,
    };

    public FileSyncWorker(IServiceScopeFactory factory, ILogger<FileSyncWorker> logger)
    {
        this.factory = factory;
        this.logger = logger;

        watcher.Created += (s, e) => _ = RequestSyncAsync();
        watcher.Deleted += (s, e) => _ = RequestSyncAsync();
        watcher.Renamed += (s, e) => _ = RequestSyncAsync();
        watcher.Error += (s, e) => logger.LogError(e.GetException(), "FileSystemWatcher Error");
    }

    private readonly Lock debounceLock = new();
    private CancellationTokenSource debounce = new();
    private readonly SemaphoreSlim semaphore = new(1);

    public async Task RequestSyncAsync()
    {
        CancellationTokenSource captured;
        lock (debounceLock)
        {
            debounce.Cancel();
            debounce.Dispose();
            captured = debounce = new();
        }

        try
        {
            await Task.Delay(TimeSpan.FromSeconds(10), captured.Token);
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
        catch (Exception ex)
        {
            logger.LogError(ex, "FileSyncService Error");
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
        debounce.Dispose();
        semaphore.Dispose();
        base.Dispose();
    }
}
