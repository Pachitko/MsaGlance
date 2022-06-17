using System.ComponentModel.DataAnnotations.Schema;

namespace TelegramBot.Api.Domain.Entities;

#nullable disable
public class BaseEntity<TKey>
{
    [Column("id")]
    public TKey Id { get; set; }
}