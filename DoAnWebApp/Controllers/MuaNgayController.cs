using Microsoft.AspNetCore.Mvc;
using DoAnWebApp.Service;

namespace DoAnWebApp.Controllers
{
    public class MuaNgayController : Controller
    {
        private readonly MuaNgayService _muaNgayService;

        public MuaNgayController(MuaNgayService muaNgayService)
        {
            _muaNgayService = muaNgayService;
        }

        [HttpGet]
        public IActionResult MuaNgay(string maSP)
        {
            string user = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(user))
            {
                // Chưa đăng nhập -> bật popup đăng ký
                TempData["AuthTask"] = "dangky"; // hoặc "dangnhap"
                                                 // Lưu tạm sản phẩm muốn mua vào session
                HttpContext.Session.SetString("MuaNgaySP", maSP);
                HttpContext.Session.SetString("MuaNgaySoLuong", "1");
                return RedirectToAction("Index", "DongHoes");
            }

            // Nếu đã đăng nhập, chuyển thẳng sang ThanhToan
            HttpContext.Session.SetString("MuaNgaySP", maSP);
            HttpContext.Session.SetString("MuaNgaySoLuong", "1");
            return RedirectToAction("MuaNgay", "ThanhToan");
        }


        // Xử lý mua ngay từ trang chi tiết (có số lượng)
        [HttpPost]
        public async Task<IActionResult> MuaNgay(string maSP, int soLuong)
        {
            // Kiểm tra đăng nhập
            string user = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(user))
            {
                // Chưa đăng nhập -> quay lại trang chi tiết, hiện thông báo
                TempData["Error"] = "Vui lòng đăng nhập để mua hàng!";
                TempData["AuthTask"] = "dangnhap";
                return RedirectToAction("Details", "DongHoes", new { MaSP = maSP });
            }

            // Validate số lượng
            if (soLuong < 1)
            {
                TempData["Error"] = "Số lượng phải lớn hơn 0!";
                return RedirectToAction("Details", "DongHoes", new { MaSP = maSP });
            }

            // Sử dụng service để xử lý mua ngay
            bool result = await _muaNgayService.ThucHienMuaNgay(
                HttpContext.Session,
                maSP,
                soLuong
            );

            if (!result)
            {
                // Lấy thông báo lỗi từ session (nếu có)
                string errorMsg = HttpContext.Session.GetString("errorMsg");
                if (!string.IsNullOrEmpty(errorMsg))
                {
                    TempData["Error"] = errorMsg;
                    HttpContext.Session.Remove("errorMsg");
                }
                else
                {
                    TempData["Error"] = "Có lỗi xảy ra. Vui lòng thử lại!";
                }

                return RedirectToAction("Details", "DongHoes", new { MaSP = maSP });
            }

            // Thành công -> chuyển sang trang thanh toán
            return RedirectToAction("MuaNgay", "ThanhToan");
        }
        private void LuuSanPhamMuaNgay(string maSP, int soLuong = 1)
        {
            HttpContext.Session.SetString("MuaNgaySP", maSP);
            HttpContext.Session.SetString("MuaNgaySoLuong", soLuong.ToString());
        }

    }
}