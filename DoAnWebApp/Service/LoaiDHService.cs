using DoAnWebApp.Data;
using DoAnWebApp.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace DoAnWebApp.Service
{
    public class LoaiDHService
    {
        private readonly DoAnWebAppContext _context;

        public LoaiDHService(DoAnWebAppContext context)
        {
            _context = context;
        }

        // Lấy tất cả loại
        public async Task<List<LoaiDH>> GetAllAsync()
        {
            return await _context.Loais.ToListAsync();
        }

        // Lấy loại theo ID
        public async Task<LoaiDH?> GetByIDAsync(int ma)
        {
            return await _context.Loais
                                 .FirstOrDefaultAsync(l => l.MaLoai == ma);
        }

        // Thêm loại
        public async Task InsertAsync(LoaiDH loai)
        {
            _context.Loais.Add(loai);
            await _context.SaveChangesAsync();
        }

        // Cập nhật loại
        public async Task UpdateAsync(LoaiDH loai)
        {
            _context.Loais.Update(loai);
            await _context.SaveChangesAsync();
        }

        // Xóa loại
        public async Task DeleteAsync(int ma)
        {
            var loai = await _context.Loais.FindAsync(ma);
            if (loai != null)
            {
                _context.Loais.Remove(loai);
                await _context.SaveChangesAsync();
            }
        }

        // Tìm kiếm theo tên
        public async Task<List<LoaiDH>> SearchByNameAsync(string keyword)
        {
            return await _context.Loais
                                 .Where(l => l.TenLoai.Contains(keyword))
                                 .ToListAsync();
        }

        // Đếm số sản phẩm thuộc loại này
        public async Task<int> CountProductThisTypeAsync(int ma)
        {
            return await _context.DongHos
                                 .CountAsync(sp => sp.MaLoai == ma);
        }
    }
}
