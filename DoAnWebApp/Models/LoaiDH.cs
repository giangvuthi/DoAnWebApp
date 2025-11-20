using System.ComponentModel.DataAnnotations;

namespace DoAnWebApp.Models
{
    public class LoaiDH
    {
        [Key]
        public int MaLoai {  get; set; }
        public String TenLoai { get; set; } = String.Empty;
        public ICollection<DongHo> DongHos { get; set; }
    }
}
