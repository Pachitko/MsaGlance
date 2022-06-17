using System.ComponentModel.DataAnnotations.Schema;
using TelegramBot.Api.Commands;

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
    public UserState State { get; set; }
}