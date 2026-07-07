namespace Somethingnew.Models
{
    public class Order
    {
        public int OrderId { get; set; }

        public int TableId { get; set; }

        public int WaiterId { get; set; }

        public string OrderStatus { get; set; } = string.Empty;

        public decimal TotalAmount { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
