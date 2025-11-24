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
    }
}
