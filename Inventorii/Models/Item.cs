
﻿using DocumentFormat.OpenXml.Wordprocessing;
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
/*=======
        public string ItemName { get; set; }
        public int? Quantity { get; set; }
>>>>>>> 1f6b739a5a80686a831b40a0b34d225be83fc681 */
        public int? MinimumStockQty { get; set; }

        [Display(Name = "User Email")]
        public string UserEmail { get; set; }

    }
}
