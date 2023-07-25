using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Inventorii.Data;
using Inventorii.Models;

using Microsoft.AspNetCore.Authorization;
using ClosedXML.Excel;
using System.Data;
using System.Reflection;
using Microsoft.AspNetCore.Identity;

namespace Inventorii.Controllers
{
    public class ItemsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ItemsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Items
        [Authorize]
        public async Task<IActionResult> Index(string search, string filter)
        {
            var items = from Item in _context.Items select Item;
            //TempData["email"] = _userManager.GetUserAsync(User).Result.Email;
            items = items.Where(s => s.UserEmail.Equals(_userManager.GetUserAsync(User).Result.Email));
            if (!String.IsNullOrEmpty(search)){
                items = items.Where(s => s.ItemName.Contains(search));
            }

            ViewData["ItemName"] = String.IsNullOrEmpty(filter) ? "NameDesc" : "";
            ViewData["Quantity"] = filter == "QtyAsc" ? "QtyDesc" : "QtyAsc";
            ViewData["MinimumStock"] = filter == "StockAsc" ? "StockDesc" : "StockAsc";

            switch (filter)
            {
                case "NameDesc":
                    items = items.OrderByDescending(i => i.ItemName);
                    break;
                case "QtyDesc":
                    items = items.OrderByDescending(i => i.Quantity);
                    break;
                case "QtyAsc":
                    items = items.OrderBy(i => i.Quantity);
                    break;
                case "StockDesc":
                    items = items.OrderByDescending(i => i.MinimumStockQty);
                    break;
                case "StockAsc":
                    items = items.OrderBy(i => i.MinimumStockQty);
                    break;
                default:
                    items = items.OrderBy(i => i.ItemName);
                    break;
            }

            return View(await items.ToListAsync());

            //return _context.Items != null ? 
            //              View(await _context.Items.ToListAsync()) :
            //              Problem("Entity set 'ApplicationDbContext.Items'  is null.");
        }

        // GET: Items/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Items == null)
            {
                return NotFound();
            }

            var item = await _context.Items
                .FirstOrDefaultAsync(m => m.Id == id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        // GET: Items/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Items/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ItemName,Quantity,MinimumStockQty,UserEmail")] Item item)
        {
            if (item.Quantity < 0) ModelState.AddModelError("", "Quantity should be greater or equal to 0.");

            if (item.MinimumStockQty < 0) ModelState.AddModelError("", "Minimum Stock Quantity should be greater or equal to 0.");

            if (item.Quantity < item.MinimumStockQty) ModelState.AddModelError("", "Quantity should be greater or equal to Minimum Stock Quantity.");


            if (ModelState.IsValid)
            {
                _context.Add(item);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(item);
        }

        // GET: Items/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            //TempData["here"] = "nothing here";
            if (id == null || _context.Items == null)
            {
                return NotFound();
            }

            var item = await _context.Items.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            return View(item);
        }

        // POST: Items/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ItemName,Quantity,MinimumStockQty,UserEmail")] Item item)
        {
            
            if (item.Quantity < 0) ModelState.AddModelError("", "Quantity should be greater or equal to 0.");

            if (item.MinimumStockQty < 0) ModelState.AddModelError("", "Minimum Stock Quantity should be greater or equal to 0.");

            //if (item.Quantity < item.MinimumStockQty) ModelState.AddModelError("", "Quantity should be greater or equal to Minimum Stock Quantity.");

            if (id != item.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                //return NotFound();/*
                try
                {
                    _context.Update(item);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ItemExists(item.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            //TempData["here"] = "model state doesn't seem to be valid.";
            return View(item);
        }

        // GET: Items/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Items == null)
            {
                return NotFound();
            }

            var item = await _context.Items
                .FirstOrDefaultAsync(m => m.Id == id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        // POST: Items/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Items == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Items'  is null.");
            }
            var item = await _context.Items.FindAsync(id);
            if (item != null)
            {
                _context.Items.Remove(item);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ItemExists(int id)
        {
            return (_context.Items?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        public IActionResult ExportToExcel()
        {
            try
            {
                var data = _context.Items.Where(d => d
                .UserEmail.Equals(_userManager.GetUserAsync(User).Result.Email))
                    .Select(table => new {table.ItemName, table.Quantity, table.MinimumStockQty})
                    .ToList();
                if (data != null & data.Count > 0)
                {
                    using(XLWorkbook wb = new XLWorkbook())
                    {
                        wb.Worksheets.Add(ToConvertDataTable(data.ToList()));
                        using (MemoryStream ms = new MemoryStream())
                        {
                            wb.SaveAs(ms);

                            string fileName = $"Items.xlsx";
                            return File(ms.ToArray(), "application/vnd.openxmlformats-officeddocuments.spreadsheetml.sheet", fileName);
                        }
                    }
                }
                TempData["Error"] = "Data not found!";
            }
            catch (Exception ex)
            {

            }

            return RedirectToAction("index");
        }

        public DataTable ToConvertDataTable<T>(List<T> items)
        {

            DataTable dt = new DataTable(typeof(T).Name);
            PropertyInfo[] propInfo = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            
            foreach(PropertyInfo prop in propInfo)
            {
                dt.Columns.Add(prop.Name);
            }
            foreach(T item in items)
            {
                var values = new object[propInfo.Length];
                for(int i = 0; i < propInfo.Length; i++)
                {
                    values[i] = propInfo[i].GetValue(item, null);
                }
                dt.Rows.Add(values);
            }

            return dt;
        }
    }
}
