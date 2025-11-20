using Microsoft.AspNetCore.Mvc;

namespace DoAnWebApp.Controllers
{
    public class MuaNgayController : Controller
    {
        public IActionResult MuaNgay(string maSP)
        {
            // Kiểm tra session
            string user = HttpContext.Session.GetString("Username");

            if (string.IsNullOrEmpty(user))
            {
                // Người dùng chưa đăng nhập -> quay lại Trang Chu, bật popup đăng nhập
                TempData["AuthTask"] = "dangnhap"; // sẽ dùng trong TrangChu.cshtml
                return RedirectToAction("Index", "DongHoes");
            }

            // Người dùng đã đăng nhập -> chuyển thẳng sang Thanh Toán
            // Lưu thông tin sản phẩm mua ngay vào session
            HttpContext.Session.SetString("MuaNgaySP", maSP);

            return RedirectToAction("MuaNgay", "ThanhToan");
        }
    }
}
