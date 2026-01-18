using Microsoft.EntityFrameworkCore;

namespace Aurorae.Models.DbModels;

[PrimaryKey(nameof(FilePath))]
public class FileMeta
{
    public string FilePath { get; set; } = null!;
    public bool Favorite { get; set; }
}
