namespace Somethingnew.Models
{
    public class MenuItem
    {
        public int MenuItemId { get; set; }

        public int CategoryId { get; set; }

        public string ItemName { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public string? ImageUrl { get; set; }

        public bool IsAvailable { get; set; }

        public DateTime CreatedAt { get; set; }

    }
}
