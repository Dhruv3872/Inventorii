using DocumentFormat.OpenXml.Wordprocessing;
using System.ComponentModel.DataAnnotations;

namespace Inventorii.Models
{
    public class Item
    {
        public int Id { get; set; }

        [Display(Name = "Item Name")]
        public string ItemName { get; set; }
        public int? Quantity { get; set; }

        [Display(Name = "Minimun Stock Qty")]
        public int? MinimumStockQty { get; set; }

    }
}
