//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace kinhdoanhbanh.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class HoaDon
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public HoaDon()
        {
            this.ChiTietHoaDons = new HashSet<ChiTietHoaDon>();
        }
    
        public int MaHD { get; set; }
        public Nullable<System.DateTime> NgayXuatHD { get; set; }
        public string HinhThucThanhToan { get; set; }
        public Nullable<double> TongTien { get; set; }
        public Nullable<int> MaKH { get; set; }
        public Nullable<int> MaNV { get; set; }
        public Nullable<byte> DaXoa { get; set; }
        public string Note { get; set; }
        public Nullable<int> SoLuong { get; set; }
        public string DiaChiGiaoHang { get; set; }
    
        public virtual KhachHang KhachHang { get; set; }
        public virtual NhanVien NhanVien { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ChiTietHoaDon> ChiTietHoaDons { get; set; }
    }
}
