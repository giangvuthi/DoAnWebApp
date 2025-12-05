using DoAnWebApp.Models;
using DoAnWebApp.Service;
using Microsoft.AspNetCore.Mvc;

namespace DoAnWebApp.Controllers
{
    public class DongHoesController : Controller
    {
        private readonly DongHoService _service;
        private readonly DonHangService _dhService;

        public DongHoesController(DongHoService service, DonHangService dhService)
        {
            _service = service;
            _dhService = dhService;
        }

        // === Hiển thị tất cả đồng hồ ===
        public async Task<IActionResult> Index()
        {
            try
            {
                var list = await _service.GetAllAsync();
                ViewBag.ListSP = list;
                ViewBag.Loai = 0;

                string username = HttpContext.Session.GetString("Username");
                if (!string.IsNullOrEmpty(username))
                {
                    var lichSu = (await _dhService.GetByUserAsync(username))
                                 .Where(dh => dh.TrangThai == 1)
                                 .OrderByDescending(dh => dh.NgayLap)
                                 .ToList();
                    ViewBag.LichSu = lichSu;
                }
                else
                {
                    ViewBag.LichSu = new List<DonHang>();
                }

                return View("TrangChu");
            }
            catch
            {
                return View("Error");
            }
        }

        // === Hiển thị Lịch sử đơn hàng trên Trang chủ ===
        public async Task<IActionResult> LichSu()
        {
            var username = HttpContext.Session.GetString("Username");
            ViewBag.Loai = 0;

            // Lấy danh sách sản phẩm để vẫn hiển thị giống Trang chủ
            var list = await _service.GetAllAsync();
            ViewBag.ListSP = list;

            if (!string.IsNullOrEmpty(username))
            {
                // Chỉ lấy đơn đã xử lý
                var lichSu = (await _dhService.GetByUserAsync(username))
                             .Where(dh => dh.TrangThai == 1)
                             .OrderByDescending(dh => dh.NgayLap)
                             .ToList();
                ViewBag.LichSu = lichSu;
            }
            else
            {
                ViewBag.LichSu = new List<DonHang>();
            }

            return View("TrangChu");
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
                    return View("ChiTiet");
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

                var user = HttpContext.Session.GetString("Username");
                if (!string.IsNullOrEmpty(user))
                {
                    var lichSu = (await _dhService.GetByUserAsync(user))
                                 .Where(dh => dh.TrangThai == 1)
                                 .OrderByDescending(dh => dh.NgayLap)
                                 .ToList();
                    ViewBag.LichSu = lichSu;
                }

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
                ViewBag.Loai = 0;

                var user = HttpContext.Session.GetString("Username");
                if (!string.IsNullOrEmpty(user))
                {
                    var lichSu = (await _dhService.GetByUserAsync(user))
                                 .Where(dh => dh.TrangThai == 1)
                                 .OrderByDescending(dh => dh.NgayLap) 
                                 .ToList();
                    ViewBag.LichSu = lichSu;
                }

                return View("TrangChu");
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }
    }
}
