using Scighost.PixivApi.Common;
using Scighost.PixivApi.Illust;

namespace Aurorae.Models.DbModels;

public class PixivIllustInfo
{
    public PixivIllustInfo() { }
    public PixivIllustInfo(IllustInfo illust)
    {
        Id = illust.Id;
        Title = illust.Title;
        Description = illust.Description;
        IllustType = illust.IllustType;
        CreateAt = illust.CreateDate;
        UploadAt = illust.UploadDate;
        XRestrict = illust.XRestrict;
        Tags = [.. illust.Tags.Select(x => x.Tag)];
        UserId = illust.UserId;
        UserName = illust.UserName;
        UserAccount = illust.UserAccount;
        PageCount = illust.PageCount;
        IsOriginal = illust.IsOriginal;
    }

    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public IllustType IllustType { get; set; }
    public DateTimeOffset CreateAt { get; set; }
    public DateTimeOffset UploadAt { get; set; }
    public XRestrict XRestrict { get; set; }
    public string[]? Tags { get; set; }
    public int UserId { get; set; }
    public string? UserName { get; set; }
    public string? UserAccount { get; set; }
    public int PageCount { get; set; }
    public bool IsOriginal { get; set; }

    public string? Error { get; set; } = null;
}
