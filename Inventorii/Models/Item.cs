namespace Inventorii.Models
{
    public class Item
    {
        public int Id { get; set; }
        public string ItemName { get; set; }
        public int? Quantity { get; set; }
        public int? MinimumStockQty { get; set; }

    }
}
