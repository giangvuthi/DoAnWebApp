using DoAnWebApp.Data;
using DoAnWebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace DoAnWebApp.Service
{
    public class ChiTietDonHangService
    {
        private readonly DoAnWebAppContext _context;

        public ChiTietDonHangService(DoAnWebAppContext context)
        {
            _context = context;
        }

        /// Lấy toàn bộ chi tiết theo mã đơn hàng
        public async Task<List<ChiTietDonHang>> GetAllByDonHangAsync(string maDH)
        {
            return await _context.ChiTietDonHangs
                                 .Where(ct => ct.MaDH == maDH)
                                 .Include(ct => ct.DongHo)   // load thông tin sản phẩm nếu cần
                                 .ToListAsync();
        }

        /// Tìm kiếm chi tiết trong 1 đơn hàng theo Mã SP hoặc Tên SP
        public async Task<List<ChiTietDonHang>> SearchAsync(string maDH, string? keyword)
        {
            var query = _context.ChiTietDonHangs
                                .Where(ct => ct.MaDH == maDH)
                                .Include(ct => ct.DongHo)     // Join sang bảng Đồng Hồ
                                .AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = keyword.Trim().ToLower();

                query = query.Where(ct =>
                    ct.MaSP.ToLower().Contains(keyword) ||
                    ct.DongHo.TenSP.ToLower().Contains(keyword)
                );
            }

            return await query.ToListAsync();
        }
    }
}