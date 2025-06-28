namespace Core.Models;

public class Order
{
    public Guid Id { get; set; }
    
    public Guid UserId { get; set; }
    
    public virtual User User { get; set; }
    
    public DateTime OrderDate { get; set; }
    
    public decimal TotalAmount { get; set; }
    
    public string ShippingAddress { get; set; }
    
    public virtual ICollection<OrderItem> OrderItems { get; set; }
}