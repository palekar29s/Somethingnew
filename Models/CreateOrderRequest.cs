namespace Somethingnew.Models
{
    public class CreateOrderRequest
    {
        public int TableId { get; set; }

        public int Userid { get; set; }

        public List<OrderItemRequest> Items { get; set; } = new();
    }
}
