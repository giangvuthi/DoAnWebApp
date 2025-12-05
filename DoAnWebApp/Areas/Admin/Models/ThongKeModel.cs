namespace DoAnWebApp.Areas.Admin.Models
{
    public class ThongKeModel
    {
        public int Thang { get; set; }
        public decimal DoanhThu { get; set; }
        public int SoDonHang { get; set; }
    }

    public class ThongKeMatHangModel
    {
        public string MaDongHo { get; set; }  // Đổi từ int sang string để khớp với MaSP
        public string TenDongHo { get; set; }
        public int SoLuongTonKho { get; set; }
        public int SoLuongDaBan { get; set; }
        public int TongSoLuong { get; set; }
        public decimal DoanhThu { get; set; }
        public decimal GiaBan { get; set; }
        public decimal TyLeDaBan { get; set; }
    }
}