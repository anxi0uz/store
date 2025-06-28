public record UserResponse(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    string PhoneNumber,
    string Address,
    Guid RoleId,
    string RoleName);