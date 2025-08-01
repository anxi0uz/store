﻿namespace Core.Models;

public class User
{
    public Guid Id { get; set; }
    
    public string Email { get; set; }
    
    public string Password { get; set; }
    
    public string FirstName { get; set; }
    
    public string LastName { get; set; }
    
    public string PhoneNumber { get; set; }
    
    public string Address { get; set; }
    
    public Guid RoleId { get; set; }
    
    public virtual Role Role { get; set; }
    
    public virtual ICollection<Order> Orders{ get; set; } = new List<Order>();
}