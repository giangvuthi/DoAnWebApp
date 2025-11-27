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

        //public async Task<string> ThemVaoGioAsync(string maDangNhap, string maSP, int soLuong)
        //{
        //    var gioHang = await GetGioHangAsync(maDangNhap)
        //                  ?? await TaoGioHangMoiAsync(maDangNhap);

        //    var sp = await _context.DongHos.FirstOrDefaultAsync(x => x.MaSP == maSP);
        //    if (sp == null) return "Sản phẩm không tồn tại.";

        //    int soLuongTon = sp.SoLuongTon;

        //    // Hết hàng
        //    if (soLuongTon <= 0)
        //        return "Sản phẩm trong kho đã hết.";

        //    var ct = await _context.ChiTietDonHangs
        //        .FirstOrDefaultAsync(x => x.MaDH == gioHang.MaDH && x.MaSP == maSP);

        //    // Nếu đã có trong giỏ → kiểm tra cộng dồn
        //    if (ct != null)
        //    {
        //        if (ct.SoLuong + soLuong > soLuongTon)
        //            return $"Chỉ còn {soLuongTon} sản phẩm trong kho.";

        //        ct.SoLuong += soLuong;
        //        ct.ThanhTien = ct.SoLuong * sp.DonGia;
        //    }
        //    else
        //    {
        //        if (soLuong > soLuongTon)
        //            return $"Chỉ còn {soLuongTon} sản phẩm trong kho.";

        //        ct = new ChiTietDonHang
        //        {
        //            MaDH = gioHang.MaDH,
        //            MaSP = maSP,
        //            SoLuong = soLuong,
        //            ThanhTien = sp.DonGia * soLuong
        //        };
        //        _context.ChiTietDonHangs.Add(ct);
        //    }

        //    // Cập nhật tổng tiền
        //    gioHang.TongThanhTien = await _context.ChiTietDonHangs
        //        .Where(x => x.MaDH == gioHang.MaDH)
        //        .SumAsync(x => x.ThanhTien);

        //    await _context.SaveChangesAsync();

        //    return "OK";
        //}



        // 3️⃣ Thêm vào giỏ
        public async Task<string> ThemVaoGioAsync(string maDangNhap, string maSP, int soLuong)
        {
            var gioHang = await GetGioHangAsync(maDangNhap)
                          ?? await TaoGioHangMoiAsync(maDangNhap);

            var sp = await _context.DongHos.FirstOrDefaultAsync(x => x.MaSP == maSP);
            if (sp == null) return "Sản phẩm không tồn tại.";

            int soLuongTon = sp.SoLuongTon;

            // Hết hàng
            if (soLuongTon <= 0)
                return "Sản phẩm trong kho đã hết hàng.";

            var ct = await _context.ChiTietDonHangs
                .FirstOrDefaultAsync(x => x.MaDH == gioHang.MaDH && x.MaSP == maSP);

            // Nếu đã có trong giỏ → kiểm tra cộng dồn
            if (ct != null)
            {
                int soLuongDangCo = ct.SoLuong;
                int tongSoLuongSau = soLuongDangCo + soLuong;

                if (tongSoLuongSau > soLuongTon)
                {
                    return $"Bạn đã có {soLuongDangCo} sản phẩm trong giỏ. Kho chỉ còn {soLuongTon} sản phẩm. Không thể thêm {soLuong} sản phẩm nữa.";
                }

                ct.SoLuong = tongSoLuongSau;
                ct.ThanhTien = ct.SoLuong * sp.DonGia;
            }
            else
            {
                // Chưa có trong giỏ
                if (soLuong > soLuongTon)
                    return $"Kho chỉ còn {soLuongTon} sản phẩm. Không thể thêm {soLuong} sản phẩm.";

                ct = new ChiTietDonHang
                {
                    MaDH = gioHang.MaDH,
                    MaSP = maSP,
                    SoLuong = soLuong,
                    ThanhTien = sp.DonGia * soLuong
                };
                _context.ChiTietDonHangs.Add(ct);
            }

            // Cập nhật tổng tiền
            gioHang.TongThanhTien = await _context.ChiTietDonHangs
                .Where(x => x.MaDH == gioHang.MaDH)
                .SumAsync(x => x.ThanhTien);

            await _context.SaveChangesAsync();

            return "OK";
        }



        public async Task<string> CapNhatSoLuongAsync(string maDangNhap, string maSP, int soLuong)
        {
            var gioHang = await GetGioHangAsync(maDangNhap);
            if (gioHang == null) return "Giỏ hàng không tồn tại.";

            var ct = await _context.ChiTietDonHangs
                .FirstOrDefaultAsync(x => x.MaDH == gioHang.MaDH && x.MaSP == maSP);

            if (ct == null) return "Sản phẩm không có trong giỏ.";

            var sp = await _context.DongHos.FirstOrDefaultAsync(x => x.MaSP == maSP);
            if (sp == null) return "Sản phẩm không tồn tại.";

            if (soLuong > sp.SoLuongTon)
                return $"Chỉ còn {sp.SoLuongTon} sản phẩm trong kho.";

            ct.SoLuong = soLuong;
            ct.ThanhTien = ct.SoLuong * sp.DonGia;

            gioHang.TongThanhTien = await _context.ChiTietDonHangs
                .Where(x => x.MaDH == gioHang.MaDH)
                .SumAsync(x => x.ThanhTien);

            await _context.SaveChangesAsync();

            return "OK";
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

