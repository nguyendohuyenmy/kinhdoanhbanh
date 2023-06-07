using kinhdoanhbanh.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace kinhdoanhbanh.Controllers
{
    public class HomeController : Controller
    {
        KinhDoanhBanhEntities mydata = new KinhDoanhBanhEntities();



        public ActionResult Index(int? page, string SeachString = "", int filterLoai = 0)
        {
            Session["DanhMucHome"] = mydata.Loais.Where(l => l.DaXoa != 1).ToList();

            if (page == null) page = 1;
            var all_sanPham = (from s in mydata.SanPhams where s.TenSP.Contains(SeachString) && s.DaXoa != 1 select s).OrderBy(m => m.MaSP);
            int pageSize = 6;
            int pageNum = page ?? 1;

            //loc danh muc menu
        if (filterLoai > 0)
            {

                all_sanPham = (from s in mydata.SanPhams where s.Loai.MaLoai == filterLoai select s).OrderBy(m => m.MaSP);

            }
            return View(all_sanPham.ToPagedList(pageNum, pageSize));
        }

        public ActionResult About()
        {
            // Lay ra 3 maSP ban chay
            var TopBanChay = (from item in mydata.ChiTietHoaDons
                              group item.SoLuong by item.MaSP into g
                              orderby g.Sum() descending
                              select g.Key).Take(3).ToList();
            List<SanPham> Top3SpBanChay = new List<SanPham>();
            foreach (var item in TopBanChay)
            {
                Top3SpBanChay.Add(mydata.SanPhams.FirstOrDefault(x => x.MaSP == item));
            }

            ViewBag.TopBanChay = Top3SpBanChay;

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.TenQuan = mydata.CauHinhs.Single(x => x.Key == "TenQuan").Value;
            ViewBag.DiaChi = mydata.CauHinhs.Single(x => x.Key == "DiaChi").Value;
            ViewBag.SDT = mydata.CauHinhs.Single(x => x.Key == "SDT").Value;


            return View();
        }

        //ham` gui du lieu lien he
        public ActionResult SubmitContact(string name, string email, string subject, string message)
        {
            Contact contact = new Contact();
            contact.HoTen = name;
            contact.Email = email;
            contact.TieuDe = subject;
            contact.NoiDung = message;
            contact.NgayGui = DateTime.Now;
            mydata.Contacts.Add(contact);
            mydata.SaveChanges();

            return RedirectToAction("index");
        }

        public ActionResult Details(int id)
        {
            var sanpham = mydata.SanPhams.FirstOrDefault(x => x.MaSP == id);
            return View(sanpham);
        }


        //xu li munu danh muc layout
        [ChildActionOnly]
        public ActionResult RenderMenu()
        {
            var ds = mydata.Loais.Where(l => l.DaXoa != 1).ToList();
            ViewBag.DanhMucHome = ds;

            //set list lay du lieu autocomplete cho search
            var sp = (from x in mydata.SanPhams
                      where x.DaXoa != 1
                      select x.TenSP).ToList();
            Session["ListSanPham"] = sp;


            return PartialView("_MenuDanhMucHome");
        }




    }
}