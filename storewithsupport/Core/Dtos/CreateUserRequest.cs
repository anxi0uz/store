public record CreateUserRequest(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    string PhoneNumber,
    string Address,
    Guid RoleId);