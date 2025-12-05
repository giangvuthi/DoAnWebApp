using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnWebApp.Models
{
    public class ChiTietDonHang
    {
        [Key]
        [Column(Order = 0)]
        [ForeignKey("DonHang")]
        public string MaDH { get; set; } = string.Empty;
        [Key]
        [Column(Order = 1)]
        [ForeignKey("DongHo")]
        public string MaSP { get; set; }
        public int SoLuong { get; set; }
        public double ThanhTien { get; set; }

        public DonHang DonHang { get; set; }
        public DongHo DongHo { get; set; }
        
    }
}
