using Aurorae.Models.DbModels;
using Microsoft.EntityFrameworkCore;
using Scighost.PixivApi;

namespace Aurorae.Services;

public class PixivAdapter(AuroraeDb db, PixivClient client, ILogger<PixivAdapter> logger)
{
    private static readonly SemaphoreSlim semaphore = new(1);
    public async Task<PixivIllustInfo> GetPixivIllustInfoAsync(int pid)
    {
        await semaphore.WaitAsync();
        try
        {
            if (await db.PixivIllustInfos.AsNoTracking().FirstOrDefaultAsync(x => x.Id == pid) is { } info)
                return info;

            info = new(await client.GetIllustInfoAsync(illustId: pid));

            db.PixivIllustInfos.Add(info);
            await db.SaveChangesAsync();

            return info;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "GetPixivIllustInfoAsync(pid: {pid})", pid);

            var info = new PixivIllustInfo
            {
                Id = pid,
                Error = ex switch
                {
                    HttpRequestException { StatusCode: { } code } => CommonUtils.GetStatusString((int)code),
                    PixivException => "600 Response Error",
                    _ => ex.GetType().Name
                }
            };

            if (ex is PixivException or HttpRequestException { StatusCode: System.Net.HttpStatusCode.NotFound })
            {
                db.PixivIllustInfos.Add(info);
                await db.SaveChangesAsync();
            }

            return info;
        }
        finally
        {
            semaphore.Release();
        }
    }
}
