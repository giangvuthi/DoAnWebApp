using DoAnWebApp.Data;
using DoAnWebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace DoAnWebApp.Service
{
    public class NguoiDungService
    {
        private readonly DoAnWebAppContext _context;

        public NguoiDungService(DoAnWebAppContext context)
        {
            _context = context;
        }

        // Lấy tất cả người dùng
        public async Task<List<NguoiDung>> GetAllAsync()
        {
            return await _context.NguoiDungs.ToListAsync();
        }

        // Lấy tất cả ID
        public async Task<List<string>> GetAllIDAsync()
        {
            return await _context.NguoiDungs.Select(u => u.MaDangNhap).ToListAsync();
        }

        // Lấy người dùng theo ID
        public async Task<NguoiDung> GetByIDAsync(string ma)
        {
            return await _context.NguoiDungs
                                 .FirstOrDefaultAsync(u => u.MaDangNhap == ma);
        }

        // Thêm người dùng
        public async Task InsertAsync(NguoiDung ng)
        {
            _context.NguoiDungs.Add(ng);
            await _context.SaveChangesAsync();
        }

        // Cập nhật người dùng
        public async Task UpdateAsync(NguoiDung ng)
        {
            _context.NguoiDungs.Update(ng);
            await _context.SaveChangesAsync();
        }

        // Xóa người dùng
        public async Task DeleteAsync(string ma)
        {
            var ng = await _context.NguoiDungs.FindAsync(ma);
            if (ng != null)
            {
                _context.NguoiDungs.Remove(ng);
                await _context.SaveChangesAsync();
            }
        }

        // Tìm kiếm theo tên
        public async Task<List<NguoiDung>> SearchByNameAsync(string keyword)
        {
            return await _context.NguoiDungs
                                 .Where(u => u.TenND.Contains(keyword))
                                 .ToListAsync();
        }
    }
}
