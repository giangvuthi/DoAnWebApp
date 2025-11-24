using DoAnWebApp.Data;
using DoAnWebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace DoAnWebApp.Service
{
    public class GioHangService
    {
        private readonly DoAnWebAppContext _context;

        public GioHangService(DoAnWebAppContext context)
        {
            _context = context;
        }

        // 1️⃣ Lấy giỏ hàng đang mở của user (TrangThai = 0)
        public async Task<DonHang?> GetGioHangAsync(string maDangNhap)
        {
            return await _context.DonHangs
                .Include(d => d.ChiTietDonHangs)
                .ThenInclude(ct => ct.DongHo)
                .FirstOrDefaultAsync(d => d.MaDangNhap == maDangNhap && d.TrangThai == 0);
        }

        // 2️⃣ Tạo giỏ hàng mới nếu chưa có
        private async Task<DonHang> TaoGioHangMoiAsync(string maDangNhap)
        {
            var gh = new DonHang
            {
                MaDH = "GH" + DateTime.Now.Ticks,
                MaDangNhap = maDangNhap,
                NgayLap = DateTime.Now,
                TrangThai = 0,
                TongThanhTien = 0
            };

            _context.DonHangs.Add(gh);
            await _context.SaveChangesAsync();

            return gh;
        }

        // 3️⃣ Thêm vào giỏ
        public async Task ThemVaoGioAsync(string maDangNhap, string maSP, int soLuong)
        {
            var gioHang = await GetGioHangAsync(maDangNhap)
                          ?? await TaoGioHangMoiAsync(maDangNhap);

            var ct = await _context.ChiTietDonHangs
                .FirstOrDefaultAsync(x => x.MaDH == gioHang.MaDH && x.MaSP == maSP);

            double donGia = await _context.DongHos
                                .Where(x => x.MaSP == maSP)
                                .Select(x => x.DonGia)
                                .FirstAsync();

            if (ct != null)
            {
                // giống Java: cộng dồn
                ct.SoLuong += soLuong;
                ct.ThanhTien = ct.SoLuong * donGia;
            }
            else
            {
                ct = new ChiTietDonHang
                {
                    MaDH = gioHang.MaDH,
                    MaSP = maSP,
                    SoLuong = soLuong,
                    ThanhTien = donGia * soLuong
                };
                _context.ChiTietDonHangs.Add(ct);
            }

            // cập nhật tổng tiền
            gioHang.TongThanhTien = await _context.ChiTietDonHangs
                .Where(x => x.MaDH == gioHang.MaDH)
                .SumAsync(x => x.ThanhTien);

            await _context.SaveChangesAsync();
        }

        // 4️⃣ Cập nhật số lượng
        public async Task CapNhatSoLuongAsync(string maDangNhap, string maSP, int soLuong)
        {
            var gioHang = await GetGioHangAsync(maDangNhap);
            if (gioHang == null) return;

            var ct = await _context.ChiTietDonHangs
                .FirstOrDefaultAsync(x => x.MaDH == gioHang.MaDH && x.MaSP == maSP);

            if (ct == null) return;

            double donGia = await _context.DongHos
                                .Where(x => x.MaSP == maSP)
                                .Select(x => x.DonGia)
                                .FirstAsync();

            ct.SoLuong = soLuong;
            ct.ThanhTien = soLuong * donGia;

            gioHang.TongThanhTien = await _context.ChiTietDonHangs
                .Where(x => x.MaDH == gioHang.MaDH)
                .SumAsync(x => x.ThanhTien);

            await _context.SaveChangesAsync();
        }

        // 5️⃣ Xóa khỏi giỏ
        public async Task XoaKhoiGioAsync(string maDangNhap, string maSP)
        {
            var gioHang = await GetGioHangAsync(maDangNhap);
            if (gioHang == null) return;

            var ct = await _context.ChiTietDonHangs
                .FirstOrDefaultAsync(x => x.MaDH == gioHang.MaDH && x.MaSP == maSP);

            if (ct != null)
            {
                _context.ChiTietDonHangs.Remove(ct);
            }

            gioHang.TongThanhTien = await _context.ChiTietDonHangs
                .Where(x => x.MaDH == gioHang.MaDH)
                .SumAsync(x => x.ThanhTien);

            await _context.SaveChangesAsync();
        }
    }
}

