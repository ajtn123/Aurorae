using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Aurorae.Services;

public sealed class FileSyncService(AuroraeDb db)
{
    private readonly EnumerationOptions enumeration = new()
    {
        AttributesToSkip = FileAttributes.Hidden | FileAttributes.System,
        IgnoreInaccessible = true,
        RecurseSubdirectories = true,
    };

    public async Task SyncAsync(string root)
    {
        await using var conn = (NpgsqlConnection)db.Database.GetDbConnection();
        await conn.OpenAsync();
        await using var tx = await conn.BeginTransactionAsync();

        await using (var cmd = new NpgsqlCommand("""
            CREATE TEMP TABLE tmp_file_index
            (path TEXT PRIMARY KEY)
            ON COMMIT DROP;
            """, conn, tx))
            await cmd.ExecuteNonQueryAsync();

        await using (var writer = await conn.BeginBinaryImportAsync("""
            COPY tmp_file_index (path)
            FROM STDIN (FORMAT BINARY)
            """))
        {
            foreach (var file in Directory.EnumerateFiles(root, "*", enumeration))
            {
                await writer.StartRowAsync();
                await writer.WriteAsync(Path.GetRelativePath(root, file), NpgsqlTypes.NpgsqlDbType.Text);
            }

            await writer.CompleteAsync();
        }

        await using (var cmd = new NpgsqlCommand("""
            INSERT INTO "FileMetas" ("FilePath")
            SELECT path FROM tmp_file_index
            ON CONFLICT DO NOTHING;
            """, conn, tx))
            await cmd.ExecuteNonQueryAsync();

        await using (var cmd = new NpgsqlCommand("""
            DELETE FROM "FileMetas" f
            WHERE NOT EXISTS (
                SELECT 1 FROM tmp_file_index t
                WHERE t.path = f."FilePath"
            );
            """, conn, tx))
            await cmd.ExecuteNonQueryAsync();

        await tx.CommitAsync();
    }
}