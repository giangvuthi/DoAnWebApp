using DoAnWebApp.Data;
using DoAnWebApp.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace DoAnWebApp.Service
{
    public class DonHangService
    {
        private readonly DoAnWebAppContext _context;

        public DonHangService(DoAnWebAppContext context)
        {
            _context = context;
        }

        public async Task<List<DonHang>> GetAllAsync()
        {
            return await _context.DonHangs.ToListAsync();
        }

        public async Task<List<DonHang>> GetByUserAsync(string maDangNhap)
        {
            return await _context.DonHangs
                                 .Include(d => d.NguoiDung) // load thông tin người dùng
                                 .Include(d => d.ChiTietDonHangs) // nếu cần
                                 .Where(d => d.MaDangNhap == maDangNhap)
                                 .OrderByDescending(d => d.NgayLap)
                                 .ToListAsync();
        }

        // Lấy tất cả đơn hàng + chi tiết
        public async Task<List<DonHang>> GetAllAdminAsync()
        {
            return await _context.DonHangs
                .Include(d => d.ChiTietDonHangs)
                .ToListAsync();
        }

        // Lấy đơn hàng theo ID
        public async Task<DonHang?> GetByIDAsync(string ma)
        {
            return await _context.DonHangs
                .Include(d => d.ChiTietDonHangs)
                .FirstOrDefaultAsync(d => d.MaDH == ma);
        }

        // Thêm đơn hàng
        public async Task InsertAsync(DonHang dh)
        {
            _context.DonHangs.Add(dh);
            await _context.SaveChangesAsync();
        }

        // Cập nhật đơn hàng
        public async Task UpdateAsync(DonHang dh)
        {
            // Tính lại tổng tiền dựa trên chi tiết
            double tong = await _context.ChiTietDonHangs
                .Where(c => c.MaDH == dh.MaDH)
                .SumAsync(c => c.ThanhTien);

            dh.TongThanhTien = tong;

            _context.DonHangs.Update(dh);
            await _context.SaveChangesAsync();
        }

        // Xóa đơn hàng
        public async Task DeleteAsync(string ma)
        {
            var dh = await _context.DonHangs
                .Include(d => d.ChiTietDonHangs)
                .FirstOrDefaultAsync(d => d.MaDH == ma);

            if (dh != null)
            {
                // Xóa chi tiết đơn hàng trước
                _context.ChiTietDonHangs.RemoveRange(dh.ChiTietDonHangs);

                // Xóa đơn hàng
                _context.DonHangs.Remove(dh);

                await _context.SaveChangesAsync();
            }
        }

        // Tìm kiếm theo mã đơn hàng (LIKE %keyword%)
        public async Task<List<DonHang>> SearchByIDAsync(string keyword)
        {
            return await _context.DonHangs
                .Where(d => d.MaDH.Contains(keyword))
                .Include(d => d.ChiTietDonHangs)
                .ToListAsync();
        }
    }
}
