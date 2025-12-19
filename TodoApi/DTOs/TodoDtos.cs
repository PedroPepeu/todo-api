namespace TodoApi.DTOs;

public record CreateTodoDto(string Title, string Description);
public record UpdateTodoDto(string Title, string Description);
public record TodoResponseDto(int Id, string Title, string Description);
public record PaginatedReponse<T>(IEnumerable<T> Data, int Page, int Limit, int Total);
