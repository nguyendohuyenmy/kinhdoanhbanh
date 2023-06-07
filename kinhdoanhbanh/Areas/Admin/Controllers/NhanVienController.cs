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
    public class NhanVienController : BaseController
    {
        KinhDoanhBanhEntities db = new KinhDoanhBanhEntities();
        // GET: Admin/NhanVien
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
                var dsNhanVien = db.NhanViens.Where(m => (m.isAdmin == 0 || m.isAdmin == null) && m.DaXoa != 1).ToList();


                // phan trang
                var pageSize = 6;
                var pageNumber = dsNhanVien.Count() % pageSize == 0 ? dsNhanVien.Count() / pageSize : (dsNhanVien.Count() / pageSize) + 1;

                var dsHienThi = dsNhanVien.Skip((trang - 1) * pageSize).Take(pageSize).ToList();
                return Json(new { trangthai = 0, data = dsHienThi, soTrang = pageNumber }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { trangthai = 1, msg = ex.Message }, JsonRequestBehavior.AllowGet);

            }
        }

        [HttpPost]
        public ActionResult Create(string HoTen, string NgaySinh, string DiaChi, string GioiTinh, string SDT, string Email, string MatKhau)
        {
            try
            {
                NhanVien nv = new NhanVien();
                nv.HoTen = HoTen;
                if (NgaySinh != "")
                {
                    nv.NgaySinh = DateTime.Parse(NgaySinh);

                }
                nv.DiaChi = DiaChi;
                if (GioiTinh != "")
                {
                    nv.GioiTinh = int.Parse(GioiTinh) == 1 ? true : false;

                }
                nv.SDT = SDT;
                nv.Email = Email;
                nv.MatKhau = GetMD5(MatKhau);


                db.NhanViens.Add(nv);
                db.SaveChanges();
                return Json(new { trangthai = true, msg = "Thêm mới thành công !!" }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { trangthai = true, msg = "Thêm mới Faillll !!" }, JsonRequestBehavior.AllowGet);

            }


        }

        [HttpPost]
        public JsonResult Update(int MaNV, string HoTen, string NgaySinh, string DiaChi, string GioiTinh, string SDT, string Email, string MatKhau)
        {
            try
            {
                var nv = db.NhanViens.FirstOrDefault(x => x.MaNV == MaNV);

                nv.HoTen = HoTen;
                nv.NgaySinh = DateTime.Parse(NgaySinh);
                nv.DiaChi = DiaChi;
                if (GioiTinh != "")
                {
                    nv.GioiTinh = int.Parse(GioiTinh) == 1 ? true : false;

                }
                nv.SDT = SDT;
                nv.Email = Email;
                nv.MatKhau = GetMD5(MatKhau);

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
                var nv = db.NhanViens.Find(id);

                return Json(new { trangthai = 0, data = nv, msg = "Lấy thông tin thành công !" }, JsonRequestBehavior.AllowGet);
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
                var nv = db.NhanViens.FirstOrDefault(x => x.MaNV == id);

                nv.DaXoa = 1;
                db.SaveChanges();
                return Json(new { trangthai = 0, msg = "Xóa Nhan Vien có tên : " + nv.HoTen + " thành công!" }, JsonRequestBehavior.AllowGet);


            }
            catch (Exception ex)
            {
                return Json(new { trangthai = 1, msg = "Xóa thất bại Failll!:" + ex.Message }, JsonRequestBehavior.AllowGet);

            }
        }



        //public ActionResult Create()
        //{
        //    return View();
        //}
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "HoTen,NgaySinh,DiaChi,GioiTinh,SDT,Email,MatKhau")] NhanVien nhanVien)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.NhanViens.Add(nhanVien);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    return View();
        //}

        //public ActionResult Edit(int id)
        //{
        //    var NhanVien = db.NhanViens.Find(id);
        //    return View(NhanVien);
        //}

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

        public ActionResult NhanVienDaXoa()
        {
            List<NhanVien> lstNV = db.NhanViens.Where(x => x.DaXoa == 1).ToList();
            return View(lstNV);
        }

        public ActionResult HoanTac(int MaNV)
        {
            NhanVien nv = db.NhanViens.FirstOrDefault(x => x.MaNV == MaNV);
            nv.DaXoa = 0;
            db.SaveChanges();
            return Redirect("NhanVienDaXoa");
        }
    }
}