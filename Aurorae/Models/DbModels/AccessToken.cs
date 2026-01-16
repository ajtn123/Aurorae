using Microsoft.EntityFrameworkCore;

namespace Aurorae.Models.DbModels;

[PrimaryKey(nameof(Token))]
public class AccessToken
{
    public string Token { get; set; } = string.Empty;
}
