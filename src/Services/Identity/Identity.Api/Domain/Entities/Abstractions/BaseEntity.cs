#nullable disable
namespace Identity.Api.Domain.Entities.Abstractions;
public class BaseEntity<TKey>
{
    public TKey Id { get; set; }
}