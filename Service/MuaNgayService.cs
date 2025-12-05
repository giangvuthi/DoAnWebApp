using DoAnWebApp.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Text.Json;

namespace DoAnWebApp.Service
{
    public class MuaNgayService
    {
        private readonly DoAnWebAppContext _context;
       
        public MuaNgayService(DoAnWebAppContext context)
        {
            _context = context;
        }


        // Thực hiện Mua Ngay
        public async Task<bool> ThucHienMuaNgay(ISession session, string maSP, int soLuong)
        {
            if (string.IsNullOrEmpty(maSP) || soLuong < 1)
                return false;

            // Lấy sản phẩm từ DB
            var sp = await _context.DongHos.FirstOrDefaultAsync(s => s.MaSP == maSP);
            if (sp == null) return false;

            if (soLuong > sp.SoLuongTon)
            {
                // Lưu thông báo lỗi vào session
                session.SetString("errorMsg", $"Số lượng vượt quá tồn kho! Chỉ còn {sp.SoLuongTon} sản phẩm.");
                return false;
            }

            // Lưu thông tin mua ngay vào session
            var danhSachThanhToan = new List<string> { maSP };
            var soLuongMap = new Dictionary<string, int> { { maSP, soLuong } };

            session.SetString("isMuaNgay", "true");
            session.SetString("danhSachThanhToan", JsonSerializer.Serialize(danhSachThanhToan));
            session.SetString("soLuongThanhToan", JsonSerializer.Serialize(soLuongMap));

            return true;
        }
    }
}
