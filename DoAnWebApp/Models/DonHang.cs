using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnWebApp.Models
{
    public class DonHang
    {
        [Key]
        public string MaDH { get; set; } = String.Empty;
        [ForeignKey("NguoiDung")]
        public string MaDangNhap { get; set; }
        public DateTime NgayLap { get; set; }
        public int TrangThai { get; set; }
        public double TongThanhTien { get; set; }

        public NguoiDung NguoiDung { get; set; }
        public ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; }
    }
}
