using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using ProjectPRN.Data;
using ProjectPRN.Models;
using ProjectPRN.Utils;
using ProjectPRN.ViewModels;
using SignalRAssignment;

namespace ProjectPRN.Controllers
{
    public class OrderDetailsController : Controller
    {
        private readonly AppDBContext _context;
        private readonly SignalHub _signalHub;

        public OrderDetailsController(AppDBContext context, SignalHub signalHub)
        {
            _context = context;
            _signalHub = signalHub;
        }

        // GET: OrderDetails
        public async Task<IActionResult> Cart()
        {
            List<Cart> cart = new List<Cart>();
            List<Order> order = await _context.Order.Where(or => or.UserID == SaveUserId.GetSessionValue<int>(HttpContext, "UserId")).ToListAsync();
            if(order != null)
            {
                foreach (var o in order)
                {
                    var ord = await _context.OrderDetail.FirstOrDefaultAsync(od => od.OrderID == o.ID);
                    var pro = await _context.Product.FirstOrDefaultAsync(p => p.ID == ord.ProductID);
                    var user = await _context.User.FirstOrDefaultAsync(u => u.ID == SaveUserId.GetSessionValue<int>(HttpContext, "UserId"));

                    var c = new Cart
                    {
                        OrderID = ord.OrderID,
                        ProductID = ord.ProductID,
                        UserName = user.Name,
                        ProductName = pro.Name,
                        Image = pro.Image,
                        Price = pro.Price,
                        Quantity = ord.Quantity,
                        CreatedDate = o.CreatedDate
                    };
                    cart.Add(c);
                }
                return View(cart.ToList());
            }
            return null;
        }

       

        // GET: OrderDetails/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderDetail = await _context.OrderDetail
                .Include(o => o.Order)
                .Include(o => o.Product)
                .FirstOrDefaultAsync(m => m.OrderID == id);
            if (orderDetail == null)
            {
                return NotFound();
            }

            return View(orderDetail);
        }

        // GET: OrderDetails/Create
        public IActionResult Create()
        {
            ViewData["OrderID"] = new SelectList(_context.Order, "ID", "ID");
            ViewData["ProductID"] = new SelectList(_context.Product, "ID", "ID");
            return View();
        }

        // POST: OrderDetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,OrderID,ProductID,Quantity")] OrderDetail orderDetail)
        {
            if (ModelState.IsValid)
            {
                _context.Add(orderDetail);
                await _context.SaveChangesAsync();
                await _signalHub.Clients.All.SendAsync("LoadDashboards");
                return RedirectToAction(nameof(Index));
            }
            ViewData["OrderID"] = new SelectList(_context.Order, "ID", "ID", orderDetail.OrderID);
            ViewData["ProductID"] = new SelectList(_context.Product, "ID", "ID", orderDetail.ProductID);
            return View(orderDetail);
        }

        // GET: OrderDetails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderDetail = await _context.OrderDetail.FindAsync(id);
            if (orderDetail == null)
            {
                return NotFound();
            }
            ViewData["OrderID"] = new SelectList(_context.Order, "ID", "ID", orderDetail.OrderID);
            ViewData["ProductID"] = new SelectList(_context.Product, "ID", "ID", orderDetail.ProductID);
            return View(orderDetail);
        }

        // POST: OrderDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,OrderID,ProductID,Quantity")] OrderDetail orderDetail)
        {
            if (id != orderDetail.OrderID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(orderDetail);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderDetailExists(orderDetail.OrderID))
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
            ViewData["OrderID"] = new SelectList(_context.Order, "ID", "ID", orderDetail.OrderID);
            ViewData["ProductID"] = new SelectList(_context.Product, "ID", "ID", orderDetail.ProductID);
            await _signalHub.Clients.All.SendAsync("LoadDashboards");
            return View(orderDetail);
        }

        // GET: OrderDetails/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderDetail = await _context.OrderDetail
                .Include(o => o.Order)
                .Include(o => o.Product)
                .FirstOrDefaultAsync(m => m.OrderID == id);
            if (orderDetail == null)
            {
                return NotFound();
            }

            return View(orderDetail);
        }

        // POST: OrderDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int orderID, int productID)
        {
            var orderDetail = await _context.OrderDetail.FindAsync(orderID, productID);
            var order = await _context.Order.FirstOrDefaultAsync(o => o.ID == orderID);
            if (orderDetail != null)
            {
                _context.OrderDetail.Remove(orderDetail);
                _context.Order.Remove(order);
            }

            await _context.SaveChangesAsync();
            await _signalHub.Clients.All.SendAsync("LoadDashboards");
            return RedirectToAction("Cart", "OrderDetails");
        }


        private bool OrderDetailExists(int id)
        {
            return _context.OrderDetail.Any(e => e.OrderID == id);
        }
    }
}
