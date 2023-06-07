using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace kinhdoanhbanh.Models
{
    public partial class KinhDoanhBanhEntities : DbContext
    {
        public KinhDoanhBanhEntities()
            : base("name=KinhDoanhBanhEntities")
        {
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }


        public virtual DbSet<CauHinh> CauHinhs { get; set; }
        public virtual DbSet<ChiTietHoaDon> ChiTietHoaDons { get; set; }
        public virtual DbSet<Contact> Contacts { get; set; }
        public virtual DbSet<HoaDon> HoaDons { get; set; }
        public virtual DbSet<KhachHang> KhachHangs { get; set; }
        public virtual DbSet<Loai> Loais { get; set; }
        //public virtual DbSet<NhaCungCap> NhaCungCaps { get; set; }
        public virtual DbSet<NhanVien> NhanViens { get; set; }
        public virtual DbSet<SanPham> SanPhams { get; set; }

        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<HoaDon>()
        //        .HasMany(e => e.ChiTietHoaDons)
        //        .WithRequired(e => e.HoaDon)
        //        .WillCascadeOnDelete(false);

        //    modelBuilder.Entity<KhachHang>()
        //        .Property(e => e.SDT)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<NhaCungCap>()
        //        .Property(e => e.SDT)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<NhanVien>()
        //        .Property(e => e.SDT)
        //        .IsFixedLength();

        //    modelBuilder.Entity<NhanVien>()
        //        .Property(e => e.MatKhau)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<SanPham>()
        //        .HasMany(e => e.ChiTietHoaDons)
        //        .WithRequired(e => e.SanPham)
        //        .WillCascadeOnDelete(false);
        //}
    }
}
