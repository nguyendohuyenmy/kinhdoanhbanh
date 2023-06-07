using kinhdoanhbanh.Areas.Admin.Controllers;
using kinhdoanhbanh.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace kinhdoanhbanh.Areas.Admin.Controllers
{
    public class LoaiSPController : BaseController
    {
        private KinhDoanhBanhEntities db = new KinhDoanhBanhEntities();
        // GET: Admin/LoaiSP
        public ActionResult Index()
        {
            //var dsLoai = db.Loais.Where(x => x.DaXoa != 1).ToList();

            //// phan trang
            //var pageSize = 6;
            //var pageNumber = dsLoai.Count() % pageSize == 0 ? dsLoai.Count()/pageSize : dsLoai.Count() / pageSize  +1;
            //ViewBag.PageNumber = pageNumber;

            return View();
        }

        [HttpGet]
        public JsonResult LoadDs(int trang = 1)
        {
            try
            {


                var dsLoai = (from l in db.Loais
                              where l.DaXoa != 1
                              select new { l.MaLoai, l.TenLoai }).ToList();


                // phan trang
                var pageSize = 6;
                var pageNumber = dsLoai.Count() % pageSize == 0 ? dsLoai.Count() / pageSize : (dsLoai.Count() / pageSize) + 1;

                var dsHienThi = dsLoai.Skip((trang - 1) * pageSize).Take(pageSize).ToList();
                return Json(new { trangthai = 0, data = dsHienThi, soTrang = pageNumber }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { trangthai = 1, msg = ex.Message }, JsonRequestBehavior.AllowGet);

            }
        }



        public JsonResult Create(string tenLoaiSP)
        {
            try
            {
                var l = new Loai();
                l.TenLoai = tenLoaiSP;
                db.Loais.Add(l);
                db.SaveChanges();

                return Json(new { trangthai = 0, msg = "Thêm mới thành công !" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { trangthai = 1, msg = "Thêm mới thất bại ! . Lỗi : " + ex.Message }, JsonRequestBehavior.AllowGet);

            }
        }

        [HttpGet]
        public JsonResult details(int id)
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false; // cau hinh proxy cho database //fix loi server 500
                var loai = db.Loais.Find(id);

                return Json(new { trangthai = 0, data = loai, msg = "Lấy thông tin thành công !" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { trangthai = 1, msg = "Lấy thông tin Failll :  !" + ex.Message }, JsonRequestBehavior.AllowGet);

            }
        }

        [HttpPost]
        public JsonResult Update(int id, string tenLoai)
        {
            try
            {
                var l = db.Loais.FirstOrDefault(x => x.MaLoai == id);

                l.TenLoai = tenLoai;
                db.SaveChanges();
                return Json(new { trangthai = 0, msg = "Cập nhật thành công!" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { trangthai = 1, msg = "Cập nhật Failll! :" + ex }, JsonRequestBehavior.AllowGet);

            }
        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            try
            {
                var l = db.Loais.FirstOrDefault(x => x.MaLoai == id);
                l.DaXoa = 1;
                db.SaveChanges();
                return Json(new { trangthai = 0, msg = "Xóa loại có tên : " + l.TenLoai + " thành công!" }, JsonRequestBehavior.AllowGet);


            }
            catch (Exception ex)
            {
                return Json(new { trangthai = 1, msg = "Xóa thất bại Failll!:" + ex.Message }, JsonRequestBehavior.AllowGet);

            }
        }


        public ActionResult LoaiSPDaXoa()
        {
            List<Loai> lstLoai = db.Loais.Where(x => x.DaXoa == 1).ToList();
            return View(lstLoai);
        }

        public ActionResult HoanTac(int MaLoai)
        {
            Loai l = db.Loais.FirstOrDefault(x => x.MaLoai == MaLoai);
            l.DaXoa = 0;
            db.SaveChanges();
            return Redirect("LoaiSPDaXoa");
        }

    }
}