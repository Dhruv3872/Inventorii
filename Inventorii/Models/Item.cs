<<<<<<< HEAD
﻿using DocumentFormat.OpenXml.Wordprocessing;
using System.ComponentModel.DataAnnotations;

namespace Inventorii.Models
=======
﻿namespace Inventorii.Models
>>>>>>> 1f6b739a5a80686a831b40a0b34d225be83fc681
{
    public class Item
    {
        public int Id { get; set; }
<<<<<<< HEAD

        [Display(Name = "Item Name")]
        public string ItemName { get; set; }
        public int? Quantity { get; set; }

        [Display(Name = "Minimun Stock Qty")]
=======
        public string ItemName { get; set; }
        public int? Quantity { get; set; }
>>>>>>> 1f6b739a5a80686a831b40a0b34d225be83fc681
        public int? MinimumStockQty { get; set; }

    }
}
