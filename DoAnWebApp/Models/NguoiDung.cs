using System.ComponentModel.DataAnnotations;

namespace DoAnWebApp.Models
{
    public class NguoiDung
    {
        [Key]
        public string MaDangNhap { get; set; } = string.Empty;
        public string TenND { get; set; } = string.Empty;
        public string MatKhau { get; set; } = string.Empty;
        public int Quyen { get; set; }

        public ICollection<DonHang> DonHangs { get; set; }
    }
}
