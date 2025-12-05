using DoAnWebApp.Data;
using DoAnWebApp.Models;
using DoAnWebApp.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace DoAnWebApp.Controllers
{
    public class ThanhToanController : Controller
    {
        private readonly DoAnWebAppContext _context;
        private readonly GioHangService _gioHangService;

        public ThanhToanController(DoAnWebAppContext context, GioHangService gioHangService)
        {
            _context = context;
            _gioHangService = gioHangService;
        }

        // Hiển thị trang thanh toán cho mua ngay hoặc thanh toán giỏ hàng
        [HttpGet]
        public async Task<IActionResult> MuaNgay()
        {
            string user = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(user))
            {
                TempData["Error"] = "Vui lòng đăng nhập!";
                return RedirectToAction("Index", "DongHoes");
            }

            List<string> danhSachMaSP;
            Dictionary<string, int> soLuongMap;

            var maSP = HttpContext.Session.GetString("MuaNgaySP");
            var soLuongStr = HttpContext.Session.GetString("MuaNgaySoLuong");

            if (!string.IsNullOrEmpty(maSP) && !string.IsNullOrEmpty(soLuongStr))
            {
                // MUA NGAY
                danhSachMaSP = new List<string> { maSP };
                soLuongMap = new Dictionary<string, int> { { maSP, int.Parse(soLuongStr) } };
            }
            else
            {
                // THANH TOÁN GIỎ HÀNG
                string danhSachJson = HttpContext.Session.GetString("danhSachThanhToan");
                string soLuongJson = HttpContext.Session.GetString("soLuongThanhToan");

                if (string.IsNullOrEmpty(danhSachJson) || string.IsNullOrEmpty(soLuongJson))
                {
                    TempData["Error"] = "Không có sản phẩm để thanh toán!";
                    return RedirectToAction("Index", "DongHoes");
                }

                danhSachMaSP = JsonSerializer.Deserialize<List<string>>(danhSachJson);
                soLuongMap = JsonSerializer.Deserialize<Dictionary<string, int>>(soLuongJson);
            }

            var danhSachSP = await _context.DongHos
                .Where(sp => danhSachMaSP.Contains(sp.MaSP))
                .ToListAsync();

            decimal tongTien = 0m;
            var chiTietDonHang = new List<object>();

            foreach (var sp in danhSachSP)
            {
                int soLuong = soLuongMap.ContainsKey(sp.MaSP) ? soLuongMap[sp.MaSP] : 1;
                decimal thanhTien = (decimal)sp.DonGia * soLuong;
                tongTien += thanhTien;

                chiTietDonHang.Add(new
                {
                    sp.MaSP,
                    sp.TenSP,
                    DonGia = (decimal)sp.DonGia,
                    sp.Anh,
                    SoLuong = soLuong,
                    ThanhTien = thanhTien
                });
            }

            ViewBag.ChiTietDonHang = chiTietDonHang;
            ViewBag.TongTien = tongTien;

            return View("MuaNgay");
        }

        // Xác nhận đặt hàng từ MuaNgay
        [HttpPost]
        public async Task<IActionResult> XacNhanMuaNgay(string hoTen, string sdt, string diaChi, string phuongThuc, string ghiChu)
        {
            string user = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(user))
            {
                TempData["Error"] = "Vui lòng đăng nhập!";
                return RedirectToAction("Index", "DongHoes");
            }

            if (string.IsNullOrWhiteSpace(hoTen) || string.IsNullOrWhiteSpace(sdt) || string.IsNullOrWhiteSpace(diaChi))
            {
                TempData["Error"] = "Vui lòng điền đầy đủ thông tin!";
                return RedirectToAction("MuaNgay");
            }

            List<string> danhSachMaSP;
            Dictionary<string, int> soLuongMap;

            var maSP = HttpContext.Session.GetString("MuaNgaySP");
            var soLuongStr = HttpContext.Session.GetString("MuaNgaySoLuong");

            if (!string.IsNullOrEmpty(maSP) && !string.IsNullOrEmpty(soLuongStr))
            {
                // MUA NGAY
                danhSachMaSP = new List<string> { maSP };
                soLuongMap = new Dictionary<string, int> { { maSP, int.Parse(soLuongStr) } };
            }
            else
            {
                // THANH TOÁN GIỎ HÀNG
                string danhSachJson = HttpContext.Session.GetString("danhSachThanhToan");
                string soLuongJson = HttpContext.Session.GetString("soLuongThanhToan");

                if (string.IsNullOrEmpty(danhSachJson) || string.IsNullOrEmpty(soLuongJson))
                {
                    TempData["Error"] = "Không có sản phẩm để thanh toán!";
                    return RedirectToAction("Index", "DongHoes");
                }

                danhSachMaSP = JsonSerializer.Deserialize<List<string>>(danhSachJson);
                soLuongMap = JsonSerializer.Deserialize<Dictionary<string, int>>(soLuongJson);
            }

            var danhSachSP = await _context.DongHos
                .Where(sp => danhSachMaSP.Contains(sp.MaSP))
                .ToListAsync();

            // Kiểm tra tồn kho
            foreach (var sp in danhSachSP)
            {
                int sl = soLuongMap[sp.MaSP];
                if (sl > sp.SoLuongTon)
                {
                    TempData["Error"] = $"Sản phẩm {sp.TenSP} không đủ số lượng trong kho!";
                    return RedirectToAction("MuaNgay");
                }
            }

            decimal tongTien = danhSachSP.Sum(sp => (decimal)sp.DonGia * soLuongMap[sp.MaSP]);

            // Tạo đơn hàng mới
            var donHang = new DonHang
            {
                MaDH = GenerateMaDH_Small(),
                MaDangNhap = user,
                NgayLap = DateTime.Now,
                TongThanhTien = (double)tongTien,
                TrangThai = 1,
                 Sdt = sdt,
                DiaChi = diaChi,

                // ⭐ Xử lý trạng thái thanh toán theo yêu cầu
                TrangThaiThanhToan = phuongThuc == "COD" ? false : true
            };
            


            _context.DonHangs.Add(donHang);

            // Thêm chi tiết đơn hàng và trừ tồn kho
            foreach (var sp in danhSachSP)
            {
                int sl = soLuongMap[sp.MaSP];
                var ct = new ChiTietDonHang
                {
                    MaDH = donHang.MaDH,
                    MaSP = sp.MaSP,
                    SoLuong = sl,
                    ThanhTien = sp.DonGia * sl
                };
                _context.ChiTietDonHangs.Add(ct);

                // Trừ tồn kho
                sp.SoLuongTon -= sl;
            }

            // ✅ FIX: Xóa giỏ hàng của user hiện tại bằng GioHangService
            foreach (var maSanPham in danhSachMaSP)
            {
                await _gioHangService.XoaKhoiGioAsync(user, maSanPham);
            }


            try
            {
                await _context.SaveChangesAsync();

                // Xóa session
                HttpContext.Session.Remove("danhSachThanhToan");
                HttpContext.Session.Remove("soLuongThanhToan");
                HttpContext.Session.Remove("MuaNgaySP");
                HttpContext.Session.Remove("MuaNgaySoLuong");

                TempData["Message"] = "Đặt hàng thành công! Mã đơn: " + donHang.MaDH;
                TempData["OpenLichSu"] = true;

                return RedirectToAction("Index", "DongHoes");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi xử lý đơn hàng: " + ex.Message;
                return RedirectToAction("MuaNgay");
            }
        }

        // Xử lý thanh toán giỏ hàng từ danh sách chọn
        [HttpPost]
        public async Task<IActionResult> Index(List<string> selectedSP)
        {
            string user = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(user))
            {
                TempData["Error"] = "Vui lòng đăng nhập!";
                return RedirectToAction("Index", "DongHoes");
            }

            if (selectedSP == null || !selectedSP.Any())
            {
                TempData["Error"] = "Vui lòng chọn sản phẩm để thanh toán!";
                return RedirectToAction("Index", "GioHang");
            }

            // Lấy số lượng từ form
            var soLuongThanhToan = new Dictionary<string, int>();
            foreach (var maSP in selectedSP)
            {
                int sl = int.Parse(Request.Form["soLuong_" + maSP]);
                soLuongThanhToan.Add(maSP, sl);
            }

            var danhSachSP = await _context.DongHos
                .Where(sp => selectedSP.Contains(sp.MaSP))
                .ToListAsync();

            decimal tongTien = 0m;
            var chiTietDonHang = new List<object>();
            foreach (var sp in danhSachSP)
            {
                int sl = soLuongThanhToan[sp.MaSP];
                decimal thanhTien = sl * (decimal)sp.DonGia;
                tongTien += thanhTien;

                chiTietDonHang.Add(new
                {
                    sp.MaSP,
                    sp.TenSP,
                    sp.Anh,
                    DonGia = (decimal)sp.DonGia,
                    SoLuong = sl,
                    ThanhTien = thanhTien
                });
            }

            // Lưu session
            HttpContext.Session.SetString("danhSachThanhToan", JsonSerializer.Serialize(selectedSP));
            HttpContext.Session.SetString("soLuongThanhToan", JsonSerializer.Serialize(soLuongThanhToan));

            ViewBag.ChiTietDonHang = chiTietDonHang;
            ViewBag.TongTien = tongTien;

            return View("MuaNgay");
        }

        // Tạo mã đơn kiểu DH1, DH2...
        private string GenerateMaDH_Small()
        {
            var allDH = _context.DonHangs.Select(d => d.MaDH).AsEnumerable();
            int maxId = 0;

            if (allDH.Any())
            {
                maxId = allDH.Select(dh =>
                {
                    int n;
                    return int.TryParse(dh.Replace("DH", ""), out n) ? n : 0;
                }).Max();
            }

            return "DH" + (maxId + 1);
        }


        // Thêm method này vào ThanhToanController.cs

        [HttpGet]
        public async Task<IActionResult> GetChiTietDonHang(string maDH)
        {
            if (string.IsNullOrEmpty(maDH))
                return BadRequest(new { error = "Mã đơn hàng không hợp lệ" });

            var donHang = await _context.DonHangs
                .FirstOrDefaultAsync(d => d.MaDH == maDH);

            if (donHang == null)
                return NotFound(new { error = "Không tìm thấy đơn hàng" });

            var chiTietDonHang = await _context.ChiTietDonHangs
                .Where(ct => ct.MaDH == maDH)
                .Include(ct => ct.DongHo)
                .ToListAsync();

            var result = new
            {
                maDH = donHang.MaDH,
                ngayLap = donHang.NgayLap,
                tongTien = donHang.TongThanhTien,
                trangThaiText = donHang.TrangThai == 0 ? "Chờ xử lý" :
                               donHang.TrangThai == 1 ? "Đã xử lý" : "Đã hủy",
                chiTiet = chiTietDonHang.Select(ct => new
                {
                    maSP = ct.MaSP,
                    tenSP = ct.DongHo?.TenSP ?? "",
                    donGia = ct.DongHo?.DonGia ?? 0,
                    anh = ct.DongHo?.Anh ?? "noimage.png",
                    soLuong = ct.SoLuong,
                    thanhTien = ct.ThanhTien
                }).ToList()
            };

            return Json(result);
        }

    }
}