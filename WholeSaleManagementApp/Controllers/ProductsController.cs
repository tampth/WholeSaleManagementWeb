using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PagedList.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WholeSaleManagementApp.Data;
using WholeSaleManagementApp.Models;
using WholeSalerWeb.Models;
using Microsoft.AspNetCore.Session;


namespace WholeSalerWeb.Controllers
{
    public class ProductsController : Controller
    {
        private readonly MyDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IEmailSender _sendMail;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(MyDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int page = 1, int CatID = 0)
        {
            var pageNumber = page;
            var pageSize = 24;

            var ps = from p in _context.Products
                     select p;

            List<Product> lsProducts = new List<Product>();

            if (CatID != 0)
            {
                lsProducts = _context.Products.AsNoTracking()
                                              .Where(x => x.CatId == CatID)
                                              .Include(x => x.Cat)
                                              .OrderByDescending(x => x.ProductId).ToList();
            }
            else
            {
                lsProducts = _context.Products.AsNoTracking()
                                              .Include(x => x.Cat)
                                              .OrderByDescending(x => x.ProductId).ToList();
            }

            PagedList<Product> models = new PagedList<Product>(lsProducts.AsQueryable(), pageNumber, pageSize);

            ViewBag.CurrentCateID = CatID;
            ViewBag.CurrentPage = pageNumber;


            ViewData["DanhMuc"] = new SelectList(_context.Categories, "CatId", "CatName", CatID);
            ViewData["SanPham"] = new SelectList(_context.Products, "ProductId", "ProductName");
            return View(models);
        }
        public IActionResult Filter(int CatID = 0)
        {
            var url = $"/Products?CatID={CatID}";
            if (CatID == 0)
            {
                url = $"/Products";
            }

            return Json(new { status = "success", redirectUrl = url });
        }

        public IActionResult List(int CatID, int page = 1)
        {
            var pageSize = 9;
            var danhmuc = _context.Categories.Find(CatID);

            var lsCategories = _context.Products.AsNoTracking().Where(x => x.CatId == CatID).OrderByDescending(x => x.CatId);

            PagedList<Product> models = new PagedList<Product>(lsCategories, page, pageSize);

            ViewBag.CurrentCat = danhmuc;
            ViewBag.CurrentPage = page;
            return View(models);
        }

        public IActionResult Details(int id)
        {
            var product = _context.Products.Include(x => x.Cat).FirstOrDefault(x => x.ProductId == id);
            if (product == null)
            {
                return RedirectToAction("Index");
            }
            return View(product);
        }

        //// Key lưu chuỗi json của Cart
        public const string CARTKEY = "cart";

        ////// Lấy cart từ Session (danh sách giỏ hàng)
        List<Cart> GetCartItems()
        {

            var session = HttpContext.Session;
            string jsoncart = session.GetString(CARTKEY);
            if (jsoncart != null)
            {
                return JsonConvert.DeserializeObject<List<Cart>>(jsoncart);
            }
            return new List<Cart>();
        }

        ////// Xóa giỏ khỏi session
        void ClearCart()
        {
            var session = HttpContext.Session;
            session.Remove(CARTKEY);
        }

        ////// Lưu Cart (Danh sách giỏ hàng) vào session
        void SaveCartSession(List<Cart> ls)
        {
            var session = HttpContext.Session;
            string jsoncart = JsonConvert.SerializeObject(ls);
            session.SetString(CARTKEY, jsoncart);
        }

        public IActionResult AddToCart([FromRoute] int productid)
        {
            // var product = _context.Products.Include(x => x.Cat).FirstOrDefault(x => x.ProductId == id);
            productid = 11;

            var sanpham = _context.Products
                .Where(p => p.ProductId == productid)
                .FirstOrDefault();

            if (sanpham == null)
                return NotFound("Không có sản phẩm " + productid);

            // Xử lý đưa vào Cart ...
            var cart = GetCartItems();
            var item = cart.Find(p => p.Product.ProductId == productid);

            if (item != null)
            {
                if (item.amount + 1 > sanpham.UnitslnStock)
                {
                    item.amount = sanpham.UnitslnStock;
                }
                else
                {
                    item.amount++;
                }
            }
            else
            {
                item = new Cart
                {
                    amount = 1,
                    Product = sanpham
                };
                cart.Add(item);
            }

            //    // Lưu cart vào Session
            SaveCartSession(cart);
            //    // Chuyển đến trang hiện thị Cart
            return RedirectToAction(nameof(Cart));
        }

        ////Hiển thị card
        public IActionResult Cart()
        {
            return View(GetCartItems());
        }

      

        ///// xóa item trong cart
        public IActionResult RemoveCart([FromRoute] int ProductId)
        {
            ProductId = 11;
            var cart = GetCartItems();
            var item = cart.Find(p => p.Product.ProductId == ProductId);

            if (item != null)
            {
                cart.Remove(item);
            }

            //    // Lưu cart vào Session
            SaveCartSession(cart);
            ViewData["Cart"] = "Xóa thành công";
            //    // Chuyển đến trang hiện thị Cart
            return RedirectToAction(nameof(Cart));
        }

        public IActionResult RemoveToCart([FromRoute] int productID)
        {
            // var product = _context.Products.Include(x => x.Cat).FirstOrDefault(x => x.ProductId == id);
            productID = 11;

            var sanpham = _context.Products
                .Where(p => p.ProductId == productID)
                .FirstOrDefault();

            var cart = GetCartItems();
            var item = cart.Find(p => p.Product.ProductId == productID);

            if (sanpham == null)
                return NotFound("Không có sản phẩm " + productID);

            // Xử lý đưa vào Cart ...
            if (item != null)
            {
                if (item.amount == 1)
                {
                    cart.Remove(item);
                }
                else
                {
                    item.amount--;
                }

            }

            //    // Lưu cart vào Session
            SaveCartSession(cart);
            ViewData["Cart"] = "Giảm thành công!";
            //    // Chuyển đến trang hiện thị Cart
            return RedirectToAction(nameof(Cart));
        }

        [HttpPost]
        public IActionResult AddToCartQuantity([FromForm] int id, [FromForm] int quantity)
        {
            Console.WriteLine("{0} is", id);
            Console.WriteLine("{0} q", quantity);
            var sanpham = _context.Products
                .Where(p => p.ProductId == id)
                .FirstOrDefault();

            var cart = GetCartItems();
            var cartitem = cart.Find(p => p.Product.ProductId == id);

            if (sanpham == null)
                return NotFound("Không có sản phẩm" + id);

            if (sanpham.UnitslnStock == 0)
            {
                return Redirect("/Products");
            }
            else
            {
                //Kiểm tra số lượng thỏa mãn hay không
                if (quantity > sanpham.UnitslnStock)
                {
                    cartitem.amount = sanpham.UnitslnStock;
                    Console.WriteLine("Không thể thêm số lượng vì kho không đủ");
                }
                else
                {
                    // Xử lý đưa vào Cart ...
                    if (cartitem != null)
                    {
                        if (cartitem.amount + quantity > sanpham.UnitslnStock)
                        {
                            cartitem.amount = sanpham.UnitslnStock;
                            Console.WriteLine("Không thể thêm số lượng vì kho không đủ");
                        }
                        else
                        {
                            cartitem.amount += quantity;
                        }
                    }
                    else
                    {
                        cartitem = new Cart
                        {
                            amount = quantity,
                            Product = sanpham
                        };
                        cart.Add(cartitem);
                    }
                }
                // Lưu cart vào Session
                SaveCartSession(cart);
                ViewData["Cart"] = "Thêm thành công!";
                // Chuyển đến trang hiện thị Cart
                return RedirectToAction(nameof(Cart));
            }
        }



        //[HttpPost]
        //public IActionResult UpdateCart([FromForm] int id, [FromForm] int quantity)
        //{
        //    Console.WriteLine("Voupdate");
        //    Console.WriteLine("{0}", quantity);
        //    Console.WriteLine("{0}", id);
        //    Product sp = _context.Products.Find(id);
        //    var cart = GetCartItems();
        //    var cartitem = cart.Find(p => p.Product.ProductId == id);
        //    if (quantity > sp.UnitslnStock)
        //    {
        //        cartitem.UnitslnStock = sp.UnitslnStock;
        //        ViewData["Cart"] = "Số lượng sản phẩm hiện tại không đủ";
        //    }
        //    else
        //    {
        //        // Cập nhật Cart thay đổi số lượng quantity ...

        //        if (cartitem != null)
        //        {
        //            cartitem.UnitslnStock = quantity;
        //            cart.Find(p => p.Product.ProductId == id).UnitslnStock = cartitem.UnitslnStock;
        //        }

        //    }
        //    SaveCartSession(cart);

        //    // Trả về mã thành công (không có nội dung gì - chỉ để Ajax gọi)
        //    return RedirectToAction(nameof(Cart));
        //}

        private async Task<AppUser> GetCurrentUser()
        {
            return await _userManager.GetUserAsync(HttpContext.User);
        }








        //[HttpPost]
        //public async Task<IActionResult> CheckOutAsync([FromForm] string hoten, [FromForm] string diachi, [FromForm] string sdt, [FromForm] string email)
        //{
        //    Console.WriteLine("vô hàm thử");
        //    Console.WriteLine("{0}", hoten);
        //    Console.WriteLine("{0}", diachi);
        //    Console.WriteLine("{0}", sdt);
        //    Console.WriteLine("{0}", email);

        //    //Xử lý khi đặt hàng
        //    var cart = GetCartItems();
        //    ViewData["email"] = email;
        //    ViewData["address"] = diachi;
        //    ViewData["phone"] = sdt;
        //    ViewData["cart"] = cart;

        //    foreach (var item in cart)
        //    {
        //        Console.WriteLine("{0}", item.amount);
        //    }

        //    var id = from hoadon in _context.Orders
        //             orderby hoadon.OrderId descending
        //             select hoadon.OrderId;
        //    int temp;
        //    if (id.Count() == 0)
        //    {
        //        temp = 1;
        //    }
        //    else
        //    {
        //        temp = id.First();
        //        temp++;
        //    }
        //    Console.WriteLine("{0}", temp);
        //    if (!string.IsNullOrEmpty(email))
        //    {
        //        Order hd = new Order();
        //        long? total = 0;
        //        hd.OrderId = temp;
        //        hd.FullName = hoten;
        //        hd.Address = diachi;
        //        hd.Phone = sdt;
        //        hd.OrderDate = DateTime.Now;
        //        var user = await GetCurrentUser();
        //        hd.UserId = user.Id;

        //        foreach (var item in cart)
        //        {
        //            total += item.Product.Price * item.amount;
        //        }

        //        long ship;
        //        if (total > 500000)
        //        {
        //            hd.Total = total;
        //             ship = 0;
        //        }
        //        else
        //        {
        //            hd.Total = total + 20000;
        //            ship = 20000;
        //        }


        //        hd.TransactStatusId = 1; // chờ xử lý

        //        _context.Orders.Add(hd);
        //        _context.SaveChanges();

        //        int lastID = hd.OrderId;


        //        // gửi mail thử
        //        string subject = "ĐƠN ĐẶT HÀNG";
        //        string body =  " <p> Xin chào" + hoten + " </p> " +
        //                       " <br></br> <div style=\" width: 700px; \">" +
        //                       " <br></br> " +
        //                       " <p> TTOD đã nhận được thông tin đặt hàng của bạn với mã đơn hàng: " + hd.OrderId + "</p>" +
        //                       " <p> Vào ngày: " + hd.OrderDate + "</p>" +
        //                       " <div style =\"width: 95%; padding: 10px;\" > " +
        //                       " <h4 style = \"text-align: center;\" > THÔNG TIN ĐƠN HÀNG - DÀNH CHO NGƯỜI MUA </h4>" +
        //                       " <p> Mã đơn hàng: " + hd.OrderId + " </p>" +
        //                       " <p> Ngày đặt hàng: " + hd.OrderDate + "</p>" +
        //                       " <p> Địa chỉ nhận hàng:  " + diachi + "</p>" +

        //                       " <h4 style = \"text-align: center;\" > CHI TIẾT ĐƠN HÀNG </h4> " +
        //                       "<table  style=\"width: 100%; border: 1px solid black; border-collapse: collapse; \"> " +
        //                             " <tr style = \" border:1px solid black; border-collapse: collapse;background-color: #fff200;color: #5f5f5f;\" > " +
        //                             " <th style = \" border:1px solid black; border-collapse: collapse;min-width:150px;\" > Sản phẩm </th>" +
        //                             " <th style = \" border:1px solid black; border-collapse: collapse;min-width:70px;\" > Số lượng </th> " +
        //                             " <th style = \" border:1px solid black; border-collapse: collapse;min-width:100px;\" > Đơn giá </th> " +
        //                             " <th style = \" border:1px solid black; border-collapse: collapse;min-width:100px;\" > Số tiền </th> " +
        //                             " </tr> ";
        //        foreach (var item in cart)
        //        {
        //            Orderdetail ct = new Orderdetail();
        //            ct.OrderId = lastID;
        //            ct.ProductId = item.Product.ProductId;
        //            ct.Quantity = item.amount;
        //            long? tong = item.amount * item.Product.Price;
        //            _context.Add(ct);
        //            _context.SaveChanges();

        //            // Cập nhật lại số lượng sản phẩm
        //            Product sp = _context.Products.Where(sp => sp.ProductId == item.Product.ProductId).First();
        //            sp.UnitslnStock -= item.amount;
        //            _context.SaveChanges();

        //            body = body +
        //                     " <tr style = \" border:1px solid black; border-collapse: collapse; \"> " +
        //                             " <th style = \" border:1px solid black; border-collapse: collapse;\" > " + item.Product.ProductName + "</th>" +
        //                             " <th style = \" border:1px solid black; border-collapse: collapse;\" > " + item.amount + "</th>" +
        //                             " <th style = \" border:1px solid black; border-collapse: collapse;\" > " + item.Product.Price + "</th>" +
        //                             " <th style = \" border:1px solid black; border-collapse: collapse;\" > " + tong + "</th>" +
        //                     " </tr> ";
        //        }

        //        body = body +
        //           " </table> " +
        //           " <p> </p>" +
        //           " <table style = \"text-align: center;  position:relative; left:70%; width:30%; border:1px solid black; border-collapse: collapse; \" > " +
        //                " <tr style = \" border:1px solid black;border-collapse: collapse;\" > " +
        //                "     <th style = \" border:1px solid black; border-collapse: collapse;\" > Thành tiền </th> " +
        //                 "    <th style = \" border:1px solid black;\" > " + total?.ToString("#,##0 VNĐ") + " </th>" +
        //                 " </tr> " +
        //                 " <tr style = \" border:1px solid black;border-collapse: collapse;\" > " +
        //                  "   <th style = \" border:1px solid black; border-collapse: collapse; \" > Phí giao hàng </th> " +
        //                   "  <th style = \" border:1px solid black; \" >" + ship.ToString("#,##0 VNĐ") + "</th> " +
        //                  " </tr> " +
        //                  " <tr style = \" border:1px solid black;border-collapse: collapse; background-color: #ff9100;color: white;line-height: 20px; \" > " +
        //                    "   <th style = \" border:1px solid black; border-collapse: collapse;\" > Tổng tiền </th> " +
        //                     "  <th style = \" border:1px solid black;\" >" + hd.Total?.ToString("#,##0 VNĐ") + " </th> " +
        //                  " </tr> " +
        //           " </table> " +
        //           " <p> Chúc bạn luôn có những trải nghiệm tuyệt vời khi mua sắm tại TTOD Trading C0., Limited! </p>" +
        //           " </div> " +
        //           " </div>";


        //        await _sendMail.SendEmailAsync(email, subject, body);

        //        ClearCart();
        //        RedirectToAction(nameof(Index));
        //    }

        //    return Redirect("/Home/Index");
        //}

    }
}
