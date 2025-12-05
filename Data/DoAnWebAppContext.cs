using Microsoft.EntityFrameworkCore;
using DoAnWebApp.Models;

namespace DoAnWebApp.Data
{
    public class DoAnWebAppContext : DbContext
    {
        public DoAnWebAppContext(DbContextOptions<DoAnWebAppContext> options)
            : base(options)
        {
        }

        public DbSet<DongHo> DongHos { get; set; }
        public DbSet<LoaiDH> Loais { get; set; }
        public DbSet<NguoiDung> NguoiDungs { get; set; }
        public DbSet<DonHang> DonHangs { get; set; }
        public DbSet<ChiTietDonHang> ChiTietDonHangs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DongHo>().ToTable("SanPham");
            modelBuilder.Entity<LoaiDH>().ToTable("Loai");
            modelBuilder.Entity<NguoiDung>().ToTable("NguoiDung");
            modelBuilder.Entity<DonHang>().ToTable("DonHang");
            modelBuilder.Entity<ChiTietDonHang>().ToTable("ChiTietDonHang");

            modelBuilder.Entity<DongHo>().HasKey(s => s.MaSP);
            modelBuilder.Entity<LoaiDH>().HasKey(l => l.MaLoai);
            modelBuilder.Entity<DonHang>().HasKey(d => d.MaDH);
            modelBuilder.Entity<NguoiDung>().HasKey(u => u.MaDangNhap);
            modelBuilder.Entity<ChiTietDonHang>().HasKey(c => new { c.MaDH, c.MaSP });

            // Quan hệ
            modelBuilder.Entity<DongHo>()
                .HasOne(s => s.loaiDH)
                .WithMany(l => l.DongHos)
                .HasForeignKey(s => s.MaLoai);

            modelBuilder.Entity<DonHang>()
                .HasOne(d => d.NguoiDung)
                .WithMany(u => u.DonHangs)
                .HasForeignKey(d => d.MaDangNhap);

            modelBuilder.Entity<ChiTietDonHang>()
                .HasOne(c => c.DonHang)
                .WithMany(d => d.ChiTietDonHangs)
                .HasForeignKey(c => c.MaDH);

            modelBuilder.Entity<ChiTietDonHang>()
                .HasOne(c => c.DongHo)
                .WithMany(s => s.ChiTietDonHangs)
                .HasForeignKey(c => c.MaSP);
        }
    }
}
