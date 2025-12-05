using DoAnWebApp.Models;
using DoAnWebApp.Service;
using Microsoft.AspNetCore.Mvc;

namespace DoAnWebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class LoaiDHAdminController : Controller
    {
        private readonly LoaiDHService _service;

        public LoaiDHAdminController(LoaiDHService service)
        {
            _service = service;
        }

        // GET: Admin/LoaiDHAdmin
        public async Task<IActionResult> Index(string task, string keyword, int? Check)
        {
            try
            {
                List<LoaiDH> list;

                if (!string.IsNullOrEmpty(keyword))
                {
                    list = await _service.SearchByNameAsync(keyword.Trim());
                }
                else
                {
                    list = await _service.GetAllAsync();
                }

                // Nếu bấm XÓA -> kiểm tra số sản phẩm của loại
                if (task == "count" && Check.HasValue)
                {
                    int count = await _service.CountProductThisTypeAsync(Check.Value);
                    TempData["Count"] = count;

                    LoaiDH LoaiCheck = await _service.GetByIDAsync(Check.Value);
                    TempData["LoaiCheck"] = LoaiCheck;
                    // Khi ViewBag.Count có giá trị, view sẽ tự mở modal
                }

                return View(list);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return View(new List<LoaiDH>());
            }
        }

        // POST: Insert / Update / Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(string task, int? ma, string ten)
        {
            try
            {
                if (task == "delete")
                {
                    if (!ma.HasValue)
                    {
                        TempData["ThongBao"] = "Không có mã loại để xóa.";
                        TempData["MaLoai"] = ma;
                        TempData["TenLoai"] = ten;
                        return RedirectToAction("Index");
                    }

                    // Thực hiện xóa
                    await _service.DeleteAsync(ma.Value);
                    TempData["ThongBao"] = $"Đã xóa loại {ma.Value}.";
                    return RedirectToAction("Index");
                }

                // Insert / Update
                if (!ma.HasValue || string.IsNullOrWhiteSpace(ten))
                {
                    TempData["ThongBao"] = "Vui lòng nhập đầy đủ dữ liệu!";
                    TempData["TaskTra"] = task;
                    TempData["MaLoai"] = ma;
                    TempData["TenLoai"] = ten;
                    return RedirectToAction("Index");
                }

                var loai = new LoaiDH { MaLoai = ma.Value, TenLoai = ten };

                if (task == "insert")
                {
                    var exist = await _service.GetByIDAsync(ma.Value);
                    if (exist != null)
                    {
                        TempData["ThongBao"] = "Mã loại đã tồn tại! Vui lòng nhập mã khác.";
                        TempData["TaskTra"] = task;
                        TempData["MaLoai"] = ma;
                        TempData["TenLoai"] = ten;
                        return RedirectToAction("Index");
                    }
                    await _service.InsertAsync(loai);
                }
                else if (task == "update")
                {
                    await _service.UpdateAsync(loai);
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                TempData["ThongBao"] = "Có lỗi xảy ra!";
                TempData["MaLoai"] = ma;
                TempData["TenLoai"] = ten;
                return RedirectToAction("Index");
            }
        }

    }
}
