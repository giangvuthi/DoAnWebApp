using DoAnWebApp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using DoAnWebApp.Areas.Admin.Models; // Đảm bảo namespace này khớp với ThongKeModel

// Thay thế "Admin" bằng tên Area thực tế của bạn
[Area("Admin")]
public class ThongKeController : Controller
{
    private readonly DoAnWebAppContext _context;

    public ThongKeController(DoAnWebAppContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        // =================================================================
        // 1. CHỈ SỐ TỔNG QUAN VÀ TỒN KHO/ĐÃ BÁN
        // =================================================================

        // 1.1. Tính Tổng Doanh Thu (từ các đơn hàng thành công: TrangThai == 1)
        var tongDoanhThu = await _context.DonHangs
            .Where(dh => dh.TrangThai == 1)
            .SumAsync(dh => (decimal?)dh.TongThanhTien) ?? 0; // Dùng decimal? để Sum an toàn

        // 1.2. Đếm tổng số đơn hàng & đơn hàng thành công
        var tongSoDonHang = await _context.DonHangs.CountAsync();
        var tongSoDonHangThanhCong = await _context.DonHangs
            .CountAsync(dh => dh.TrangThai == 1);

        // 1.3. Tính tổng số lượng tồn kho (Tổng SoLuongTon của tất cả Đồng Hồ)
        // Dựa trên DbSet: _context.DongHos
        var tongSoLuongTonKho = await _context.DongHos
            .SumAsync(dh => (decimal?)dh.SoLuongTon) ?? 0;

        // 1.4. Tính tổng số lượng đã bán (Tổng SoLuong trong ChiTietDonHang của đơn hàng thành công)
        // Dựa trên DbSet: _context.ChiTietDonHangs
        var tongSoLuongDaBan = await _context.ChiTietDonHangs
            // Chỉ tính số lượng của các ChiTietDonHang thuộc Đơn Hàng thành công
            .Where(ct => ct.DonHang.TrangThai == 1)
            .SumAsync(ct => (decimal?)ct.SoLuong) ?? 0;

        // =================================================================
        // 2. THỐNG KÊ THEO THÁNG (Cho biểu đồ và bảng chi tiết)
        // =================================================================

        var thongKeTheoThang = await _context.DonHangs
            .Where(dh => dh.TrangThai == 1 && dh.NgayLap.Year == DateTime.Now.Year)
            .GroupBy(dh => new { Month = dh.NgayLap.Month })
            .Select(g => new ThongKeModel
            {
                Thang = g.Key.Month,
                // Ép kiểu rõ ràng cho DoanhThu (đã sửa lỗi CS0266)
                DoanhThu = (decimal)g.Sum(dh => dh.TongThanhTien),
                SoDonHang = g.Count()
            })
            .OrderBy(t => t.Thang)
            .ToListAsync();

        // =================================================================
        // 3. TRUYỀN DỮ LIỆU SANG VIEW
        // =================================================================

        ViewBag.TongDoanhThu = tongDoanhThu;
        ViewBag.TongSoDonHang = tongSoDonHang;
        ViewBag.TongSoDonHangThanhCong = tongSoDonHangThanhCong;
        ViewBag.ThongKeTheoThang = thongKeTheoThang;

        ViewBag.TongSoLuongTonKho = tongSoLuongTonKho;
        ViewBag.TongSoLuongDaBan = tongSoLuongDaBan;

        return View();
    }
}