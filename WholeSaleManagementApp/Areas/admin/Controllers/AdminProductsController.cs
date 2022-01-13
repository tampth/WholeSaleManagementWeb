using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using PagedList.Core;
using WholeSalerWeb.Helpper;
using System.Web;
using System.Data;
using System.Drawing;
using OfficeOpenXml.Style;
using Microsoft.AspNetCore.Http;
using AspNetCoreHero.ToastNotification.Abstractions;
using WholeSaleManagementApp.Models;
using WholeSaleManagementApp.Data;

namespace WholeSalerWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminProductsController : Controller
    {
        private readonly  MyDbContext _context;

        public INotyfService _notyfService { get; }
        public AdminProductsController(MyDbContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }
        // GET: Admin/AdminProducts
        public async Task<IActionResult> Index(string sortOrder, int page = 1, int CatID = 0)
        {
            var pageNumber = page;
            var pageSize = 20;

            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

            //ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";

            var ps = from p in _context.Products
                     select p;

            switch (sortOrder)
            {
                case "name_desc":
                    ps = ps.OrderByDescending(s => s.ProductName);
                    break;
                default:
                    ps = ps.OrderBy(s => s.ProductName);
                    break;
            }

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
            var url = $"/admin/AdminProducts?CatID={CatID}";
            if (CatID == 0)
            {
                url = $"/admin/AdminProducts";
            }

            return Json(new { status = "success", redirectUrl = url });
        }
        // GET: Admin/AdminProducts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Cat)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Admin/AdminProducts/Create
        public IActionResult Create()
        {
            ViewData["DanhMuc"] = new SelectList(_context.Categories, "CatId", "CatName");
            return View();
        }

        // POST: Admin/AdminProducts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,ProductName,ShortDesc,Description,CatId,Price,Thumb,DateCreated,DateModified,UnitslnStock")] Product product, Microsoft.AspNetCore.Http.IFormFile fThumb)
        {
            if (ModelState.IsValid)
            {
                product.ProductName = Ultilities.ToTitleCase(product.ProductName);
                if (fThumb != null)
                {
                    string extension = Path.GetExtension(fThumb.FileName);
                    string image = Ultilities.SEOUrl(product.ProductName) + extension;
                    product.Thumb = await Ultilities.UploadFile(fThumb, @"products", image.ToLower());
                }
                if (string.IsNullOrEmpty(product.Thumb)) product.Thumb = "default.jpg";

                product.DateCreated = DateTime.Now;
                _context.Add(product);
                await _context.SaveChangesAsync();
                _notyfService.Success("Tao mới thành công");
                return RedirectToAction(nameof(Index));
            }
            ViewData["DanhMuc"] = new SelectList(_context.Categories, "CatId", "CatName", product.CatId);
            return View(product);
        }

        // GET: Admin/AdminProducts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["DanhMuc"] = new SelectList(_context.Categories, "CatId", "CatName", product.CatId);
            return View(product);
        }

        // POST: Admin/AdminProducts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,ProductName,ShortDesc,Description,CatId,Price,Thumb,DateCreated,DateModified,UnitslnStock")] Product product, Microsoft.AspNetCore.Http.IFormFile fThumb)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    product.ProductName = Ultilities.ToTitleCase(product.ProductName);
                    if (fThumb != null)
                    {
                        string extension = Path.GetExtension(fThumb.FileName);
                        string image = Ultilities.SEOUrl(product.ProductName) + extension;
                        product.Thumb = await Ultilities.UploadFile(fThumb, @"products", image.ToLower());
                    }
                    if (string.IsNullOrEmpty(product.Thumb)) product.Thumb = "default.jpg";

                    product.DateModified = DateTime.Now;
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                    _notyfService.Success("Cập nhật thành công");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
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
            ViewData["DanhMuc"] = new SelectList(_context.Categories, "CatId", "CatName", product.CatId);
            return View(product);
        }

        // GET: Admin/AdminProducts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Cat)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Admin/AdminProducts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }


        public IActionResult ExportExcel()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var data = from a in _context.Products
                       join b in _context.Categories
                       on a.CatId equals b.CatId
                       select new
                       {
                           productid = a.ProductId,
                           productname = a.ProductName,
                           catid = a.CatId,
                           price = a.Price,
                           qty = a.UnitslnStock,
                           catname = b.CatName
                       };


            var stream = new MemoryStream(); // luu bo nho stream

            MyDbContext context = HttpContext.RequestServices.GetService(typeof(WholeSaleManagementApp.Data.MyDbContext)) as MyDbContext;

            using (var package = new ExcelPackage(stream)) // de excel vao stream
            {
                var sheet = package.Workbook.Worksheets.Add("Danh sach san pham"); // doi ten sheet

                // sheet.Cells["A1:C1"].Merge = true; // merge cot a1 den c1

                sheet.Column(1).Width = 5;
                sheet.Column(2).Width = 5;
                sheet.Column(3).Width = 13.7;
                sheet.Column(4).Width = 71;
                sheet.Column(5).Width = 16.7;
                sheet.Column(6).Width = 11.5;
                sheet.Column(7).Width = 13;

                sheet.Row(1).Height = 30;
                sheet.Row(2).Height = 30;
                sheet.Row(3).Height = 30;
                sheet.Row(4).Height = 30;
                sheet.Row(5).Height = 30;
                sheet.Row(6).Height = 30;
                sheet.Row(7).Height = 30;

                sheet.Cells["B2:G2"].Merge = true;
                sheet.Cells["B2:G2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells["B2:G2"].Style.Font.Size = 14;
                sheet.Cells["B2:G2"].Value = "TTOD Trading Company Limited - Công ty TNHH Thương Mại TTOD";

                sheet.Cells["B3:G3"].Merge = true;
                sheet.Cells["B3:G3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells["B3:G3"].Style.Font.Size = 14;
                sheet.Cells["B3:G3"].Value = "Địa chỉ: 321 Dương Quảng Hàm, Phường 6, Quận Gò Vấp, Thành phố Hồ Chí Minh";

                sheet.Cells["B5:G5"].Merge = true;
                sheet.Cells["B5:G5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells["B5:G5"].Style.Font.Size = 18;
                sheet.Cells["B5:G5"].Style.Font.Bold = true;
                sheet.Cells["B5:G5"].Style.Font.Color.SetColor(0, 31, 73, 125);
                sheet.Cells["B5:G5"].Value = "THỐNG KÊ THÔNG TIN SẢN PHẨM";

                sheet.Cells["D6:E6"].Merge = true;
                sheet.Cells["D6:E6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells["D6:E6"].Style.Font.Size = 14;
                sheet.Cells["D6:E6"].Style.Font.Italic = true;
                sheet.Cells["D6:E6"].Value = $"Ngày xuất: {DateTime.Now.ToShortDateString()} , {DateTime.Now.ToLongTimeString()}";

                sheet.Cells["B7:C7"].Merge = true;
                sheet.Cells["B7:C7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells["B7:C7"].Style.Font.Size = 14;
                sheet.Cells["B7:C7"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells["B7:C7"].Style.Fill.BackgroundColor.SetColor(0, 142, 169, 219);
                sheet.Cells["B7:C7"].Style.Font.Color.SetColor(Color.White);
                sheet.Cells["B7:C7"].Value = "CHI TIẾT";

                sheet.Cells["D7:G7"].Merge = true;
                sheet.Cells["D7:G7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                sheet.Cells["D7:G7"].Style.Font.Size = 14;
                sheet.Cells["D7:G7"].Value = "(Đơn vị tính: VNĐ)";

                sheet.Row(8).Height = 30;
                sheet.Cells["A8:G8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells["D8:E8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                sheet.Cells["A8:G8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                sheet.Cells["B8:G8"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells["B8:G8"].Style.Fill.BackgroundColor.SetColor(0, 31, 73, 125);
                sheet.Cells["A8:G8"].Style.Font.Color.SetColor(Color.White);
                sheet.Cells["A8:G8"].Style.Font.Size = 13;

                sheet.Cells["B8:G8"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                sheet.Cells["B8:G8"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                sheet.Cells["B8:G8"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                sheet.Cells["B8:G8"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                sheet.Cells["B8:B8"].Value = "STT";
                sheet.Cells["C8:C8"].Value = "MÃ NỘI BỘ";
                sheet.Cells["D8:D8"].Value = "TÊN SẢN PHẨM";
                sheet.Cells["E8:E8"].Value = "DANH MỤC";
                sheet.Cells["F8:F8"].Value = "SỐ LƯỢNG";
                sheet.Cells["G8:G8"].Value = "GÍA BÁN";

                sheet.Cells["A1:G7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                int count; // dem so dong
                count = data.Count();
                if (count < 1)
                {
                    return Content("Không có sản phẩm nào trong danh sách");
                }

                int i = 1;
                int j = 9;
                count = count + 9 - 1;

                string general = $"B9:G{count}";
                var query2 = data.ToList();

                if (count >= 10)
                {
                    sheet.Cells[general].Style.Font.Size = 13;

                    sheet.Cells[general].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    sheet.Cells[general].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    sheet.Cells[general].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    sheet.Cells[general].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    sheet.Cells[general].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    sheet.Cells[general].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    sheet.Cells[$"D9:E{count}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    sheet.Cells[$"G9:G{count}"].Style.Numberformat.Format = "#,##0";

                    foreach (var item in data)
                    {
                        sheet.Row(j).Height = 30;
                        string stt = $"B{j}:B{j}";
                        string masp = $"C{j}:D{j}";
                        string tensp = $"D{j}:D{j}";
                        string danhmuc = $"E{j}:E{j}";
                        string soluong = $"F{j}:F{j}";
                        string gia = $"G{j}:G{j}";

                        sheet.Cells[stt].Value = i;
                        sheet.Cells[masp].Value = "dairy" + item.productid;
                        sheet.Cells[tensp].Value = item.productname;
                        sheet.Cells[danhmuc].Value = item.catname;
                        sheet.Cells[soluong].Value = item.qty;
                        sheet.Cells[gia].Value = item.price;

                        if (item.qty == 0)
                        {
                            // Ở đây chúng ta sẽ format lại theo dạng fromRow,fromCol,toRow,toCol
                            using (var range = sheet.Cells[j, 2, j, 7])
                            {
                                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                range.Style.Fill.BackgroundColor.SetColor(0, 209, 223, 247);
                            }
                        }

                        i++;
                        j++;
                    }

                    sheet.Row(j + 1).Height = 30;
                    sheet.Cells[$"B{j + 1}:D{j + 1}"].Merge = true;
                    sheet.Cells[$"B{j + 1}:D{j + 1}"].Value = "TỔNG SỐ LƯỢNG SẢN PHẨM";

                    sheet.Cells[$"E{j + 1}:G{j + 1}"].Merge = true;
                    sheet.Cells[$"E{j + 1}:G{j + 1}"].Formula = "SUM(F2:F" + (j - 1) + ")";

                    sheet.Cells[$"B{j + 1}:G{j + 1}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    sheet.Cells[$"B{j + 1}:G{j + 1}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    sheet.Cells[$"B{j + 1}:G{j + 1}"].Style.Font.Size = 14;
                    sheet.Cells[$"B{j + 1}:G{j + 1}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    sheet.Cells[$"B{j + 1}:G{j + 1}"].Style.Fill.BackgroundColor.SetColor(0, 142, 169, 219);
                    sheet.Cells[$"B{j + 1}:G{j + 1}"].Style.Font.Color.SetColor(Color.White);

                    sheet.Name = "SẢN PHẨM"; // ten sheet

                    package.Save();
                }

                stream.Position = 0;

                var tenfile = $"listProducts_{DateTime.Now}.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", tenfile);
            }
        }

        public IActionResult Report()
        {
            return View();
        }

        public IActionResult InventoryReport(int qty)
        {
            List<Product> listTK = (from sp in _context.Products
                                    where sp.UnitslnStock <= qty
                                    select new Product
                                    {
                                        ProductName = sp.ProductName,
                                        ProductId = sp.ProductId,
                                        Price = sp.Price,
                                        UnitslnStock = sp.UnitslnStock
                                    }).ToList();

            ViewBag.Date = DateTime.Now;
            return View(listTK);
        }
    }
}
