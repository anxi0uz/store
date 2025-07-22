namespace Core.Dtos;

public record AuthResponse(string Token, DateTime ExpiresAt);