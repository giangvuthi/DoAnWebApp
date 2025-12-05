using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnWebApp.Models
{
    public class DongHo
    {
        [Key]
        public String MaSP {  get; set; } = String.Empty;
        public String TenSP {  get; set; } = String.Empty;
        [ForeignKey("LoaiDH")]
        public int MaLoai {  get; set; }
        public Double DonGia {  get; set; }
        public String Anh {  get; set; } = String.Empty;
        public int SoLuongTon {  get; set; }
        public String MoTa {  get; set; } = String.Empty;
        public String MoTaChiTiet {  get; set; } = String.Empty;

        public LoaiDH loaiDH { get; set; }
        public ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; }
    }
}
