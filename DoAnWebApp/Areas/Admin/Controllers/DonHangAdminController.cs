using DoAnWebApp.Models;
using DoAnWebApp.Service;
using Microsoft.AspNetCore.Mvc;

namespace DoAnWebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DonHangAdminController : Controller
    {
        private readonly DonHangService _donHangService;
        private readonly ChiTietDonHangService _chiTietService;

        public DonHangAdminController(
            DonHangService donHangService,
            ChiTietDonHangService chiTietService)
        {
            _donHangService = donHangService;
            _chiTietService = chiTietService;
        }

        /// Hiển thị danh sách đơn hàng + chi tiết nếu có chọn đơn
        public async Task<IActionResult> Index(string? keyword, string? maDH)
        {
            // 1. Lấy danh sách đơn hàng (có tìm kiếm)
            List<DonHang> listDonHang;

            if (string.IsNullOrEmpty(keyword))
            {
                listDonHang = await _donHangService.GetAllAdminAsync();
            }
            else
            {
                ViewBag.Keyword = keyword;
                listDonHang = await _donHangService.SearchByIDAsync(keyword);
            }

            // 2. Nếu người dùng chọn 1 mã đơn → load chi tiết
            List<ChiTietDonHang> listChiTiet = new();
            if (!string.IsNullOrEmpty(maDH))
            {
                listChiTiet = await _chiTietService.GetAllByDonHangAsync(maDH);
                ViewBag.MaDH = maDH;
            }

            // 3. Gửi dữ liệu sang View
            ViewBag.DonHangList = listDonHang;
            ViewBag.ChiTietList = listChiTiet;

            return View();
        }
    }
}
