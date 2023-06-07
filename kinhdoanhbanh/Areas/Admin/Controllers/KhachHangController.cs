using kinhdoanhbanh.Areas.Admin.Controllers;
using kinhdoanhbanh.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;


namespace kinhdoanhbanh.Areas.Admin.Controllers
{
    public class KhachHangController : BaseController
    {
        KinhDoanhBanhEntities db = new KinhDoanhBanhEntities();
        // GET: Admin/KhachHang
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult LoadDs(int trang = 1)
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                var dsKhachHang = db.KhachHangs.Where(m => m.TrangThaiTaiKhoan != false).ToList();


                // phan trang
                var pageSize = 6;
                var pageNumber = dsKhachHang.Count() % pageSize == 0 ? dsKhachHang.Count() / pageSize : (dsKhachHang.Count() / pageSize) + 1;

                var dsHienThi = dsKhachHang.Skip((trang - 1) * pageSize).Take(pageSize).ToList();
                return Json(new { trangthai = 0, data = dsHienThi, soTrang = pageNumber }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { trangthai = 1, msg = ex.Message }, JsonRequestBehavior.AllowGet);

            }
        }

        [HttpPost]
        public ActionResult Create(string TenKH, string DiaChi, string SDT, string Email, string MatKhau)
        {
            try
            {
                KhachHang kh = new KhachHang();
                kh.TenKH = TenKH;

                kh.DiaChi = DiaChi;

                kh.SDT = SDT;
                kh.Email = Email;
                kh.MatKhau = GetMD5(MatKhau);


                db.KhachHangs.Add(kh);
                db.SaveChanges();
                return Json(new { trangthai = true, msg = "Thêm mới thành công !!" }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { trangthai = true, msg = "Thêm mới Faillll !!" }, JsonRequestBehavior.AllowGet);

            }


        }
        public static string GetMD5(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = Encoding.UTF8.GetBytes(str);
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;

            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String += targetData[i].ToString("x2");

            }
            return byte2String;
        }

        [HttpPost]
        public JsonResult Update(int MaKH, string TenKH, string DiaChi, string SDT, string Email, string MatKhau)
        {
            try
            {
                var kh = db.KhachHangs.FirstOrDefault(x => x.MaKH == MaKH);

                kh.TenKH = TenKH;
                kh.DiaChi = DiaChi;

                kh.SDT = SDT;
                kh.Email = Email;
                kh.MatKhau = GetMD5(MatKhau);

                db.SaveChanges();
                return Json(new { trangthai = true, msg = "Cập nhật thành công!" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { trangthai = false, msg = "Cập nhật Failll! :" + ex }, JsonRequestBehavior.AllowGet);

            }
        }

        [HttpGet]
        public JsonResult GetById(int id)
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false; // cau hinh proxy cho database //fix loi server 500
                var kh = db.KhachHangs.Find(id);

                return Json(new { trangthai = 0, data = kh, msg = "Lấy thông tin thành công !" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { trangthai = 1, msg = "Lấy thông tin Failll :  !" + ex.Message }, JsonRequestBehavior.AllowGet);

            }
        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            try
            {
                var nv = db.KhachHangs.FirstOrDefault(x => x.MaKH == id);

                nv.TrangThaiTaiKhoan = false;
                db.SaveChanges();
                return Json(new { trangthai = 0, msg = "Xóa Khách Hàng có tên : " + nv.TenKH + " thành công!" }, JsonRequestBehavior.AllowGet);


            }
            catch (Exception ex)
            {
                return Json(new { trangthai = 1, msg = "Xóa thất bại Failll!:" + ex.Message }, JsonRequestBehavior.AllowGet);

            }
        }

        public ActionResult KhachHangDaXoa()
        {
            List<KhachHang> lst = db.KhachHangs.Where(x => x.TrangThaiTaiKhoan == false).ToList();
            return View(lst);
        }

        public ActionResult HoanTac(int MaKH)
        {
            KhachHang kh = db.KhachHangs.FirstOrDefault(x => x.MaKH == MaKH);
            kh.TrangThaiTaiKhoan = true;
            db.SaveChanges();
            return Redirect("KhachHangDaXoa");
        }
    }
}