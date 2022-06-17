using System.ComponentModel.DataAnnotations.Schema;

namespace TelegramBot.Api.Domain.Entities;

#nullable disable
public class TelegramUser : BaseEntity<long>
{
    [Column("identity_id")]
    public long? IdentityId { get; set; }
    [Column("caht_id")]
    public long ChatId { get; set; }
    [Column("username")]
    public string Username { get; set; }
    [Column("state")]
    public string State { get; set; }
}