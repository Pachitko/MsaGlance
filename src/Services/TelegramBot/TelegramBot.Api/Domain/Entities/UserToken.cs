using System.ComponentModel.DataAnnotations.Schema;

namespace TelegramBot.Api.Domain.Entities;

#nullable disable
public class UserToken
{
    [Column("user_id")]
    public long UserId { get; set; }
    [Column("login_provider")]
    public string LoginProvider { get; set; }
    [Column("name")]
    public string Name { get; set; }
    [Column("value")]
    public string Value { get; set; }
}