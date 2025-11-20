using DoAnWebApp.Models;
using DoAnWebApp.Service;
using Microsoft.AspNetCore.Mvc;

namespace DoAnWebApp.Controllers
{
    public class AuthController : Controller
    {
        private readonly NguoiDungService _service;

        public AuthController(NguoiDungService service)
        {
            _service = service;
        }

        // === Đăng ký ===
        [HttpPost]
        public async Task<IActionResult> Register(string madangnhap, string tennd, string matkhau)
        {
            if (string.IsNullOrEmpty(madangnhap) || string.IsNullOrEmpty(tennd) || string.IsNullOrEmpty(matkhau))
            {
                TempData["AuthTask"] = "dangky";
                TempData["Error"] = "Vui lòng điền đầy đủ thông tin!";
                return RedirectToAction("Index", "DongHoes"); // Trang chủ
            }

            var existingUser = await _service.GetByIDAsync(madangnhap);
            if (existingUser != null)
            {
                TempData["AuthTask"] = "dangky";
                TempData["Error"] = "Tên đăng nhập của bạn bị trùng! Vui lòng nhập tên khác.";
            }
            else
            {
                var newUser = new NguoiDung
                {
                    MaDangNhap = madangnhap,
                    TenND = tennd,
                    MatKhau = matkhau,
                    Quyen = 1 // quyền mặc định
                };
                await _service.InsertAsync(newUser);

                TempData["AuthTask"] = "dangnhap";
                TempData["Message"] = "Đăng ký thành công! Mời bạn đăng nhập.";
            }

            return RedirectToAction("Index", "DongHoes");
        }

        // === Đăng nhập ===
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                TempData["AuthTask"] = "dangnhap";
                TempData["Error"] = "Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu!";
                return RedirectToAction("Index", "DongHoes");
            }

            var user = await _service.GetByIDAsync(username);

            if (user != null && user.MatKhau == password)
            {
                // Lưu thông tin vào session
                HttpContext.Session.SetString("Username", user.MaDangNhap);
                HttpContext.Session.SetString("TenND", user.TenND);
                HttpContext.Session.SetInt32("Quyen", user.Quyen);

                if (user.Quyen == 0)
                {
                    return RedirectToAction("Index", "DHAdmin"); // Trang admin
                }
                else
                {
                    return RedirectToAction("Index", "DongHoes"); // Trang người dùng
                }
            }
            else
            {
                TempData["AuthTask"] = "dangnhap";
                TempData["Error"] = "Sai tên đăng nhập hoặc mật khẩu!";
                return RedirectToAction("Index", "DongHoes");
            }
        }

        // === Đăng xuất ===
        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // Xóa session
            return RedirectToAction("Index", "DongHoes");
        }



        // === Xem thông tin tài khoản ===
        public async Task<IActionResult> Info()
        {
            // Lấy username từ session
            string username = HttpContext.Session.GetString("Username");

            if (string.IsNullOrEmpty(username))
            {
                // Nếu chưa đăng nhập, chuyển về trang đăng nhập
                return RedirectToAction("Login", "Auth");
            }

            // Lấy thông tin người dùng từ service
            var user = await _service.GetByIDAsync(username);
            if (user == null)
            {
                // Nếu không tìm thấy người dùng, chuyển về đăng nhập
                return RedirectToAction("Login", "Auth");
            }

            // Truyền dữ liệu sang view
            ViewBag.MaDangNhap = user.MaDangNhap;
            ViewBag.TenND = user.TenND;
            ViewBag.MatKhau = user.MatKhau;

            return View("ThongTin"); // Views/Account/ThongTin.cshtml
        }
    }
}
