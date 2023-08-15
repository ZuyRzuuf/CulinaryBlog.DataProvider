namespace DataProvider.Domain.Entities;

public record Recipe
{
    public Guid Id { get; init; }
    public string? Title { get; set; }
}