namespace Somethingnew.Models
{
    public class CreateOrderRequest
    {
        public int TableId { get; set; }

        public int WaiterId { get; set; }

        public List<OrderItemRequest> Items { get; set; } = new();
    }
}
