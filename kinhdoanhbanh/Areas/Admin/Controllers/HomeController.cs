using kinhdoanhbanh.Areas.Admin.Controllers;
using kinhdoanhbanh.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace kinhdoanhbanh.Areas.Admin.Controllers
{
    public class HomeController : BaseController
    {
        private KinhDoanhBanhEntities db = new KinhDoanhBanhEntities();

        // GET: Admin/Home
        public ActionResult Index()
        {
            ViewBag.TongDoanhThuThang = TongDoanhThuThang();
            ViewBag.TongSoLuongSanPhamBanTrongThang = TongSoLuongSanPhamTrongThang();
            ViewBag.TongSoLuongDonChoXuLy = ListHoaDonNote("Chờ xử lý").Count();
            ViewBag.TongSoLuongDonDangGiaoHang = ListHoaDonNote("Đang giao hàng").Count();


            ViewBag.TotalAmountOfFourWeek = GetListDoanhThu4Tuan();


            ViewBag.TotalAmountOfYear = GetListDoanhThuTungThangNamHienTai();
            //truyen qua so luong thang de loop
            ViewBag.SoLuongThang = ((ViewBag.TotalAmountOfYear) as List<double>).Count;

            Session["listContact"] = GetListConTact() as List<Contact>;

            return View();
        }



        // tong doanh thu 4 tuan
        public List<double> GetListDoanhThu4Tuan()
        {
            // xu li datetime trong thang hien tai
            DateTime firstDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime lastDate = firstDate.AddMonths(1).AddDays(-1);

            List<double> list = new List<double>();

            //xu li ngay bat dau cua tuan hien tai 
            int seed = firstDate.DayOfWeek - DayOfWeek.Monday;
            // start trừ đi thứ hiện tại để lấy đc thứ 2 của tuần đầu tiên của 4 tuần gần nhất
            DateTime start = firstDate.AddDays(-1 * seed);
            DateTime end = start.AddDays(6);
            while (start.Month < firstDate.Month)
            {
                start = start.AddDays(1);
            }

            List<ChiTietHoaDon> orderDetails = db.ChiTietHoaDons
                .Where(m => m.HoaDon.NgayXuatHD >= firstDate
                            && m.HoaDon.NgayXuatHD <= lastDate
                            && m.HoaDon.Note.Equals("Giao hàng thành công"))
                .ToList();

            for (int i = 0; i < 4; i++)
            {
                double total = 0;
                foreach (var item in orderDetails)
                {

                    if (item.HoaDon.NgayXuatHD >= start.Date
                        && item.HoaDon.NgayXuatHD <= end.Date)
                    {
                        total += (double)item.DonGiaBan;
                    }

                }

                list.Add(total);
                start = end.AddDays(1);
                end = start.AddDays(6);
            }

            return list;
        }

        // tong doanh thu nam hien tai
        public List<double> GetListDoanhThuTungThangNamHienTai()
        {
            int firstMonth = 1;
            int lastMonth = DateTime.Now.Month;

            List<double> list = new List<double>();
            for (int i = firstMonth; i <= lastMonth; i++)
            {
                list.Add(DoanhThu1Thang(i));
            }

            return list;
        }

        public double DoanhThu1Thang(int thang)
        {
            // xu li ngay trong 1 thang
            DateTime firstDate = new DateTime(DateTime.Now.Year, thang, 1);
            DateTime lastDate = firstDate.AddMonths(1).AddDays(-1);

            double result = 0d;

            List<ChiTietHoaDon> orderDetails = db.ChiTietHoaDons
                .Where(m => m.HoaDon.NgayXuatHD >= firstDate
                            && m.HoaDon.NgayXuatHD <= lastDate
                            && m.HoaDon.Note.Equals("Giao hàng thành công"))
                .ToList();

            foreach (var item in orderDetails)
            {
                result += (double)item.DonGiaBan;
            }
            return result;
        }

        public double TongDoanhThuThang()
        {
            DateTime firstDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime lastDate = firstDate.AddMonths(1).AddDays(-1);

            double total = 0d;

            foreach (HoaDon o in db.HoaDons.ToList())
            {
                if (o.Note.Equals("Giao hàng thành công"))
                {
                    if (o.NgayXuatHD >= firstDate.Date && o.NgayXuatHD <= lastDate.Date)
                    {
                        total += (double)o.TongTien;
                    }
                }
            }

            return total;
        }

        public int TongSoLuongSanPhamTrongThang()
        {
            int total = 0;
            DateTime firstDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime lastDate = firstDate.AddMonths(1).AddDays(-1);

            foreach (HoaDon o in db.HoaDons.ToList())
            {
                if (o.Note.Equals("Giao hàng thành công"))
                {
                    if (o.NgayXuatHD >= firstDate.Date && o.NgayXuatHD <= lastDate.Date)
                    {
                        total += (int)o.SoLuong;
                    }
                }
            }

            return total;
        }

        public List<HoaDon> ListHoaDonNote(string Note)
        {

            return db.HoaDons.Where(m => m.Note.Equals(Note)).ToList();
        }


        public List<Contact> GetListConTact()
        {
            List<Contact> list = db.Contacts.ToList();
            return list;
        }

    }
}