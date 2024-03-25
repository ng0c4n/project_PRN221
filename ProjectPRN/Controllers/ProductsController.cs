using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using ProjectPRN.Data;
using ProjectPRN.Filter;
using ProjectPRN.Models;
using ProjectPRN.Utils;
using SignalRAssignment;

namespace ProjectPRN.Controllers
{
    [FilterUser]
    public class ProductsController : Controller
    {
        private readonly AppDBContext _context;
        private readonly SignalHub _signalHub;
        private readonly IHostEnvironment _env;


        public ProductsController(AppDBContext context, SignalHub signalHub, IHostEnvironment env)
        {
            _context = context;
            _signalHub = signalHub;
            _env = env;
        }

        // GET: Products
        public async Task<IActionResult> Shop()
        {
            //var appDBContext = _context.Product.Include(p => p.Category);
            //return View(await appDBContext.ToListAsync());

            return View();
        }

        [FilterUser(RequireAdmin = true)]
        public async Task<IActionResult> Index()
        {
            return View();
        }

        [Route("/Products/GetAllProduct")]
        [HttpGet]
        public IActionResult GetAllProduct(int page = 1, int pageSize = 8)
        {
            var posts = _context.Product
                                .OrderByDescending(p => p.CreateAt)
                                .Skip((page - 1) * pageSize)
                                .Take(pageSize)
                                .ToList();

            return Json(posts);
        }


        [Route("/Products/Search/{search}")]
        [HttpGet]
        public IActionResult Search(string search, int page = 1, int pageSize = 8)
        {
            var posts = _context.Product
                                .Where(p => p.Name.Contains(search) || p.Name.Contains(search))
                                .OrderByDescending(p => p.CreateAt)
                                .Skip((page - 1) * pageSize)
                                .Take(pageSize)
                                .ToList();

            return Json(posts);
        }


        //Add to Carts
        [HttpPost]
        [Route("/Products/AddToCart")]
        public async Task<IActionResult> AddToCart([FromBody] CartItemDto cartItem)
        {
            try
            {
                Order order = new Order();
                OrderDetail orderDetail = new OrderDetail();
                if (!ModelState.IsValid || _context.Order == null || _context.OrderDetail == null
                    || orderDetail == null || order == null)
                {
                    return RedirectToAction("Index", "Products");
                }

                order = new Order
                {
                    /*UserID = SaveUserId.GetUserID(HttpContext),*/
                    UserID = SaveUserId.GetSessionValue<int>(HttpContext,"UserId"),
                    StatusID = 1,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now
                        
                };

                _context.Order.Add(order);

                _context.SaveChanges();

                orderDetail = new OrderDetail
                {
                    OrderID = order.ID,
                    ProductID = int.Parse(cartItem.ProductId),
                    Quantity = int.Parse(cartItem.Quantity),
                };

                _context.OrderDetail.Add(orderDetail);

                _context.SaveChanges();
                await _signalHub.Clients.All.SendAsync("LoadDashboards");

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        [FilterUser(RequireAdmin = true)]
        public IActionResult Create()
        {
            ViewData["CategoryID"] = new SelectList(_context.Category, "ID", "Name");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,CategoryID,Name,Description,Price,CreateAt,UpdateAt")] Product product)
        {
            if (product != null)
            {
                product.Image = "";
                _context.Add(product);
                _context.SaveChanges();

                await UploadImage(product, product.ID);
                await _context.SaveChangesAsync();
                await _signalHub.Clients.All.SendAsync("LoadProducts");
                await _signalHub.Clients.All.SendAsync("LoadDashboards");
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryID"] = new SelectList(_context.Category, "ID", "Name", product.CategoryID);

            return View(product);
        }

        public async Task UploadImage(Product product, int nameImage)
        {
            var files = HttpContext.Request.Form.Files;
            if (files != null && files.Count > 0)
            {
                foreach (var file in files)
                {
                    var newFileName = nameImage.ToString() + ".png";
                    var filePath = Path.Combine(_env.ContentRootPath, "wwwroot\\image", newFileName);
                    product.Image = "/image/" + newFileName;

                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                }
            }
        }

        public async Task DeleteImage(Product product)
        {
            var filePath = Path.Combine(_env.ContentRootPath, "wwwroot\\image", Path.GetFileName(product.Image));
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }


        // GET: Products/Edit/5
        [FilterUser(RequireAdmin = true)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CategoryID"] = new SelectList(_context.Category, "ID", "Name", product.CategoryID);

            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,CategoryID,Name,Description,Price,Image,CreateAt,UpdateAt")] Product product)
        {
            if (id != product.ID)
            {
                return NotFound();
            }


            try
            {
                _context.Update(product);
                await UploadImage(product, product.ID);
                await _context.SaveChangesAsync();
                await _signalHub.Clients.All.SendAsync("LoadProducts");
                await _signalHub.Clients.All.SendAsync("LoadDashboards");

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(product.ID))
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

        // GET: Products/Delete/5
        [FilterUser(RequireAdmin = true)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Product.FindAsync(id);
            if (product != null)
            {
                _context.Product.Remove(product);
            }
            await DeleteImage(product);
            await _context.SaveChangesAsync();
            await _signalHub.Clients.All.SendAsync("LoadProducts");
            await _signalHub.Clients.All.SendAsync("LoadDashboards");

            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Product.Any(e => e.ID == id);
        }
    }
}
public class CartItemDto
{
    public string ProductId { get; set; }
    public string Quantity { get; set; }
}
