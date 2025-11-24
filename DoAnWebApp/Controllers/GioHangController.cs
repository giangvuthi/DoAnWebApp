using DoAnWebApp.Service;
using Microsoft.AspNetCore.Mvc;

namespace DoAnWebApp.Controllers
{
    public class GioHangController : Controller
    {
        private readonly GioHangService _gioHangService;

        public GioHangController(GioHangService gioHangService)
        {
            _gioHangService = gioHangService;
        }

        // =============== 1. Thêm vào giỏ ==================
        [HttpPost]
        public async Task<IActionResult> ThemVaoGio(string maSP, int soLuong)
        {
            if (HttpContext.Session.GetString("Username") == null)
            {
                TempData["AuthTask"] = "dangnhap";   // báo View mở popup đăng nhập
                TempData["Error"] = "Bạn cần đăng nhập để thêm vào giỏ hàng.";

                // quay lại đúng trang đang xem
                return Redirect(Request.Headers["Referer"].ToString());
            }

            string user = HttpContext.Session.GetString("Username");
            await _gioHangService.ThemVaoGioAsync(user, maSP, soLuong);

            TempData["Message"] = "Đã thêm vào giỏ!";
            return Redirect(Request.Headers["Referer"].ToString());
        }


        // =============== 2. Cập nhật số lượng ==================
        [HttpPost]
        public async Task<IActionResult> CapNhat(string maSP, int soLuong)
        {
            string user = HttpContext.Session.GetString("Username");
            await _gioHangService.CapNhatSoLuongAsync(user, maSP, soLuong);

            return Ok("Cập nhật thành công");
        }

        // =============== 3. Xóa khỏi giỏ ==================
        public async Task<IActionResult> Xoa(string maSP)
        {
            string user = HttpContext.Session.GetString("Username");
            await _gioHangService.XoaKhoiGioAsync(user, maSP);

            return RedirectToAction("Index");
        }

        // =============== Hiển thị giỏ hàng ==================
        public async Task<IActionResult> Index()
        {
            string user = HttpContext.Session.GetString("Username");
            var gh = await _gioHangService.GetGioHangAsync(user);
            return View(gh);
        }
    }
}