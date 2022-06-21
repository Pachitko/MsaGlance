#nullable disable
namespace Identity.Domain.Entities.Abstractions;
public class BaseEntity<TKey>
{
    public TKey Id { get; set; }
}