namespace Somethingnew.Models
{
    public class KitchenQueue
    {
        public int KitchenId { get; set; }
        public int OrderId { get; set; }
        public string Status { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}
