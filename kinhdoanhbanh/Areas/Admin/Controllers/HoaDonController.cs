using kinhdoanhbanh.Areas.Admin.Controllers;
using kinhdoanhbanh.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace kinhdoanhbanh.Areas.Admin.Controllers
{
    public class HoaDonController : BaseController
    {
        private KinhDoanhBanhEntities db = new KinhDoanhBanhEntities();
        // GET: Admin/HoaDon
        public ActionResult Index(string SeachString = "")
        {
            var hoadon = db.HoaDons.Where(x => x.MaHD.ToString().Contains(SeachString) || x.DiaChiGiaoHang.Contains(SeachString)).ToList();
            //truyen gia tri cho autocomplete goi y 
            Session["listAutoComplete"] = (from x in db.HoaDons
                                           select x.DiaChiGiaoHang).ToList();

            return View(hoadon);
        }

        public ActionResult Details(int id)
        {
            List<ChiTietHoaDon> lstCTHD = db.ChiTietHoaDons.Where(x => x.MaHD == id).ToList();
            return View(lstCTHD);
        }

        //dung ham tra ve json de xu li ajax lay tinh trang don hang
        public JsonResult LayThongTinByID(int id)
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false; // cau hinh proxy cho database //fix loi server 500
                var hd = db.HoaDons.FirstOrDefault(x => x.MaHD == id);

                return Json(new { trangthai = 0, data = hd, msg = "Lấy thông tin thành công !" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { trangthai = 1, msg = "Lấy thông tin Failll :  !" + ex.Message }, JsonRequestBehavior.AllowGet);

            }
        }

        // Cap nhat tinh trang don hang
        public JsonResult UpdateStatus(int MaHD, string Note)
        {
            HoaDon hd = db.HoaDons.FirstOrDefault(x => x.MaHD == MaHD);
            hd.Note = Note;
            db.SaveChanges();
            return Json(new { trangthai = 0, msg = "Cập Nhật thành công !" }, JsonRequestBehavior.AllowGet);

        }
    }
}