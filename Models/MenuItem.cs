namespace Somethingnew.Models
{
    public class MenuItem
    {
        public int MenuItemId { get; set; }
        public int CategoryId { get; set; }
        public string ItemName { get; set; }
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; }
    
}
}
