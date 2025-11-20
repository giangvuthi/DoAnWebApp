using DoAnWebApp.Service;
using Microsoft.AspNetCore.Mvc;

namespace DoAnWebApp.Controllers
{
    public class DongHoesController : Controller
    {
        private readonly DongHoService _service;

        public DongHoesController(DongHoService service)
        {
            _service = service;
        }

        // === Hiển thị tất cả đồng hồ ===
        public async Task<IActionResult> Index()
        {
            try
            {
                var list = await _service.GetAllAsync();
                ViewBag.ListSP = list;
                return View("TrangChu"); // Views/DongHo/TrangChu.cshtml
            }
            catch
            {
                return View("Error");
            }
        }

        // === Chi tiết đồng hồ theo MaSP ===
        public async Task<IActionResult> Details(string MaSP)
        {
            if (string.IsNullOrEmpty(MaSP))
                return RedirectToAction("Index");

            try
            {
                var sp = await _service.GetByIDAsync(MaSP);
                if (sp != null)
                {
                    ViewBag.sp = sp;
                    return View("ChiTiet"); // Views/DongHo/ChiTiet.cshtml
                }
                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }

        // === Lọc đồng hồ theo loại ===
        public async Task<IActionResult> Filter(int loai)
        {
            try
            {
                var list = await _service.SearchByLoaiAsync(loai);
                ViewBag.ListSP = list;
                ViewBag.Loai = loai;
                return View("TrangChu");
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }

        // === Tìm kiếm đồng hồ theo tên ===
        public async Task<IActionResult> Search(string keyword)
        {
            try
            {
                var list = await _service.SearchByNameAsync(keyword);
                ViewBag.ListSP = list;
                return View("TrangChu");
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }
    }
}
