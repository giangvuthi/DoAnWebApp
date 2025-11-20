using DoAnWebApp.Models;
using Microsoft.EntityFrameworkCore;
using DoAnWebApp.Data;

namespace DoAnWebApp.Service
{
    public class DongHoService
    {
        private readonly DoAnWebAppContext _context;

        public DongHoService(DoAnWebAppContext context)
        {
            _context = context;
        }

        // Lấy tất cả sản phẩm
        public async Task<List<DongHo>> GetAllAsync()
        {
            return await _context.DongHos.Include(s => s.loaiDH).ToListAsync();
        }

        // Lấy tất cả ID
        public async Task<List<string>> GetAllIDAsync()
        {
            return await _context.DongHos.Select(s => s.MaSP).ToListAsync();
        }

        // Lấy sản phẩm theo ID
        public async Task<DongHo> GetByIDAsync(string ma)
        {
            return await _context.DongHos
                                 .Include(s => s.loaiDH)
                                 .FirstOrDefaultAsync(s => s.MaSP == ma);
        }

        // Thêm sản phẩm
        public async Task InsertAsync(DongHo sp)
        {
            _context.DongHos.Add(sp);
            await _context.SaveChangesAsync();
        }

        // Cập nhật sản phẩm
        public async Task UpdateAsync(DongHo sp)
        {
            _context.DongHos.Update(sp);
            await _context.SaveChangesAsync();
        }

        // Xóa sản phẩm
        public async Task DeleteAsync(string ma)
        {
            var sp = await _context.DongHos.FindAsync(ma);
            if (sp != null)
            {
                _context.DongHos.Remove(sp);
                await _context.SaveChangesAsync();
            }
        }

        // Tìm kiếm theo tên
        public async Task<List<DongHo>> SearchByNameAsync(string keyword)
        {
            return await _context.DongHos
                                 .Where(s => s.TenSP.Contains(keyword))
                                 .Include(s => s.loaiDH)
                                 .ToListAsync();
        }

        // Tìm kiếm theo loại
        public async Task<List<DongHo>> SearchByLoaiAsync(int maLoai)
        {
            return await _context.DongHos
                                 .Where(s => s.MaLoai == maLoai)
                                 .Include(s => s.loaiDH)
                                 .ToListAsync();
        }
    }
}
