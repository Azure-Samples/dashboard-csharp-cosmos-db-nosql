namespace Dashboard.Models;

public record Payload<T>(
    int Count,
    IEnumerable<T> Items
);