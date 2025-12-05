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
        //dang ky
        [HttpPost]
        public async Task<IActionResult> Register(string madangnhap, string tennd, string matkhau, string returnUrl)
        {
            if (string.IsNullOrEmpty(madangnhap) || string.IsNullOrEmpty(tennd) || string.IsNullOrEmpty(matkhau))
            {
                TempData["AuthTask"] = "dangky";
                TempData["Error"] = "Vui lòng điền đầy đủ thông tin!";
                return Redirect(returnUrl ?? "/DongHoes");
            }

            var existingUser = await _service.GetByIDAsync(madangnhap);
            if (existingUser != null)
            {
                TempData["AuthTask"] = "dangky";
                TempData["Error"] = "Tên đăng nhập bị trùng! Vui lòng nhập tên khác.";
                return Redirect(returnUrl ?? "/DongHoes");
            }

            var newUser = new NguoiDung
            {
                MaDangNhap = madangnhap,
                TenND = tennd,
                MatKhau = matkhau,
                Quyen = 1
            };
            await _service.InsertAsync(newUser);

            TempData["AuthTask"] = "dangnhap";
            TempData["Message"] = "Đăng ký thành công! Mời bạn đăng nhập.";

            return Redirect(returnUrl ?? "/DongHoes");
        }


        // === Đăng nhập ===
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password, string returnUrl)
        {
            var user = await _service.GetByIDAsync(username);

            if (user != null && user.MatKhau == password)
            {
                HttpContext.Session.SetString("Username", user.MaDangNhap);
                HttpContext.Session.SetString("TenND", user.TenND);
                HttpContext.Session.SetInt32("Quyen", user.Quyen);

                return Redirect(returnUrl ?? "/DongHoes");
            }
            else
            {
                TempData["AuthTask"] = "dangnhap";
                TempData["Error"] = "Sai tên đăng nhập hoặc mật khẩu!";
                return Redirect(returnUrl ?? "/DongHoes");
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
