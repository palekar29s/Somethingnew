namespace Somethingnew.Models
{
    public class RestaurantTable
    {
        public int TableId { get; set; }

        public int TableNumber { get; set; }

        public int Capacity { get; set; }

        public string Status { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}
