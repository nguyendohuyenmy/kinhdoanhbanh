using kinhdoanhbanh.Areas.Admin.Controllers;
using kinhdoanhbanh.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace kinhdoanhbanh.Areas.Admin.Controllers
{
    public class SanPhamController : BaseController
    {
        KinhDoanhBanhEntities db = new KinhDoanhBanhEntities();
        // GET: Admin/SanPham
        public ActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public JsonResult DsSanPham(int trang = 1)
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false; // cau hinh proxy cho database 
                var dsSanPham = (from k in db.SanPhams where k.DaXoa != 1 select new { k.MaSP, k.TenSP, k.HinhAnh, k.DVT, k.SoLuong, /*k.DonGiaNhap,*/ k.DonGiaBan, /*k.NhaCungCap,*/ k.Loai }).ToList();
                //var lstNhaCungCap = db.NhaCungCaps.Where(x => x.DaXoa != 1).ToList();
                var lstLoaiSP = db.Loais.Where(x => x.DaXoa != 1).ToList();



                // phan trang
                var pageSize = 4;
                var pageNumber = dsSanPham.Count() % pageSize == 0 ? dsSanPham.Count() / pageSize : (dsSanPham.Count() / pageSize) + 1;

                var dsHienThi = dsSanPham.Skip((trang - 1) * pageSize).Take(pageSize).ToList();
                return Json(new { trangthai = true, dsSanPham = dsHienThi,/* selectListNCC = lstNhaCungCap, */selectListLoai = lstLoaiSP, soTrang = pageNumber, msg = " Lấy danh sách sản phẩm thành công!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { trangthai = false, msg = " Lấy danh sách sản phẩm thất bại ! Loi : " + ex.Message }, JsonRequestBehavior.AllowGet);

            }
        }

        //////
        [HttpPost]
        public JsonResult CreateUpdate(string MaSp, string tenSP, HttpPostedFileBase HinhAnh, string DVT, string SoLuongSP, string DonGiaBan, string idNCC, string idLoai, string MoTa)
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false; // cau hinh proxy cho database 

                if (MaSp == "") // truong hop them moi
                {
                    var sp = new SanPham();
                    if (HinhAnh != null && HinhAnh.ContentLength > 0)
                    {
                        string filename = Path.GetFileNameWithoutExtension(HinhAnh.FileName);
                        string extension = Path.GetExtension(HinhAnh.FileName);
                        filename = filename + "_" + long.Parse(DateTime.Now.ToString("yyyyMMddhhmmss")) + extension;
                        sp.HinhAnh = filename;
                        HinhAnh.SaveAs(Path.Combine(Server.MapPath("../../Content/Images/"), filename));
                    }

                    sp.TenSP = tenSP;
                    sp.DVT = DVT;
                    sp.SoLuong = int.Parse(SoLuongSP);
                    sp.DonGiaBan = double.Parse(DonGiaBan);
                    sp.MaLoai = int.Parse(idLoai);
                    sp.MoTa = MoTa;


                    db.SanPhams.Add(sp);
                    db.SaveChanges();

                    return Json(new { trangthai = 0, msg = "Thêm mới thành công !" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var _maSpParse = int.Parse(MaSp);
                    var sp = db.SanPhams.FirstOrDefault(x => x.MaSP == _maSpParse);

                    //Lấy ra ảnh cũ
                    var PathAnhCu = (Server.MapPath("../../Content/Images/") + sp.HinhAnh);
                    FileInfo FileAnhCu = new FileInfo(PathAnhCu.ToString());

                    if (HinhAnh != null && HinhAnh.ContentLength > 0)
                    {

                        //delete anh cu  neu co
                        //if (FileAnhCu.Exists)
                        //{
                        //    FileAnhCu.Delete();
                        //}

                        // add lai anh moi
                        string filename = Path.GetFileNameWithoutExtension(HinhAnh.FileName);
                        string extension = Path.GetExtension(HinhAnh.FileName);
                        filename = filename + "_" + long.Parse(DateTime.Now.ToString("yyyyMMddhhmmss")) + extension;
                        sp.HinhAnh = filename;
                        HinhAnh.SaveAs(Path.Combine(Server.MapPath("../../Content/Images/"), filename));
                    }

                    sp.TenSP = tenSP;
                    sp.DVT = DVT;
                    sp.SoLuong = int.Parse(SoLuongSP);
                    sp.DonGiaBan = double.Parse(DonGiaBan);
                    sp.MaLoai = int.Parse(idLoai);
                    sp.MoTa = MoTa;

                    db.SaveChanges();
                    return Json(new { trangthai = 0, msg = "Cập nhật dữ liệu thành công !" }, JsonRequestBehavior.AllowGet);

                }

            }
            catch (Exception ex)
            {
                return Json(new { trangthai = 1, msg = "Thất bại ! . Lỗi : " + ex.Message }, JsonRequestBehavior.AllowGet);

            }
        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            try
            {
                var sp = db.SanPhams.FirstOrDefault(x => x.MaSP == id);
                //// Lấy ra file ảnh neu co
                // var PathAnhCu = (Server.MapPath("../../Content/Images/") + sp.HinhAnh);
                // FileInfo FileAnh = new FileInfo(PathAnhCu.ToString());
                // //delete anh  neu co
                // if (FileAnh.Exists)
                // {
                //     FileAnh.Delete();
                //     sp.HinhAnh = null;
                // }
                sp.DaXoa = 1;
                db.SaveChanges();
                return Json(new { trangthai = 0, msg = "Xóa San Pham có tên : " + sp.TenSP + " thành công!" }, JsonRequestBehavior.AllowGet);


            }
            catch (Exception ex)
            {
                return Json(new { trangthai = 1, msg = "Xóa thất bại Failll!:" + ex.Message }, JsonRequestBehavior.AllowGet);

            }
        }


        [HttpGet]
        public JsonResult GetById(int id)
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false; // cau hinh proxy cho database //fix loi server 500
                var sp = db.SanPhams.Find(id);

                return Json(new { trangthai = 0, data = sp, msg = "Lấy thông tin thành công !" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { trangthai = 1, msg = "Lấy thông tin Failll :  !" + ex.Message }, JsonRequestBehavior.AllowGet);

            }
        }


        // // GET: SanPhams/Create
        // public ActionResult Create()
        // {
        //     ViewBag.MaLoai = new SelectList(db.Loais, "MaLoai", "TenLoai");
        //     ViewBag.MaNCC = new SelectList(db.NhaCungCaps,"MaNCC","TenNCC");
        //     return View();
        // }

        // //POST: SanPhams/Create
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        // public ActionResult Create([Bind(Include = "MaSP,TenSanPham,DVT,SoLuong,DonGiaNhap,MaLoai,MaNCC,MoTa,HinhAnh,DonGiaBan")] SanPham sanPham, HttpPostedFileBase HinhAnh)
        // {
        //     if (ModelState.IsValid)
        //     {
        //         if (HinhAnh != null && HinhAnh.ContentLength > 0)
        //         {
        //             string filename = Path.GetFileNameWithoutExtension(HinhAnh.FileName);
        //             string extension = Path.GetExtension(HinhAnh.FileName);
        //             filename = filename +"_"+ long.Parse(DateTime.Now.ToString("yyyyMMddhhmmss")) + extension;
        //             sanPham.HinhAnh = filename;
        //             HinhAnh.SaveAs(Path.Combine(Server.MapPath("../../Content/Images/"),filename));
        //         }
        //         db.SanPhams.Add(sanPham);
        //         db.SaveChanges();
        //         return RedirectToAction("Index");
        //     }

        //     return View(sanPham);
        // }
        public ActionResult Edit(int id)
        {
            var sanPham = db.SanPhams.Find(id);
            ViewBag.Loai = new SelectList(db.Loais, "MaLoai", "TenLoai", sanPham.Loai.MaLoai);
            //ViewBag.NCC = new SelectList(db.NhaCungCaps, "MaNCC", "TenNCC", sanPham.NhaCungCap);
            return View(sanPham);
        }

        [HttpPost]
        public ActionResult Edit(SanPham sanPham, HttpPostedFileBase HinhAnh)
        {
            if (ModelState.IsValid)
            {
                var sp = db.SanPhams.FirstOrDefault(x => x.MaSP == sanPham.MaSP);
                sp.TenSP = sanPham.TenSP;
                sp.DVT = sanPham.DVT;
                sp.MoTa = sanPham.MoTa;
                //sp.NhaCungCap = sanPham.NhaCungCap;
                sp.SoLuong = sanPham.SoLuong;
                //sp.DonGiaNhap = sanPham.DonGiaNhap;
                sp.Loai = sanPham.Loai;
                sp.DonGiaBan = sanPham.DonGiaBan;

                //Lấy ra ảnh cũ
                var PathAnhCu = (Server.MapPath("../../../Content/Images/") + sp.HinhAnh);
                FileInfo FileAnhCu = new FileInfo(PathAnhCu.ToString());

                if (HinhAnh != null && HinhAnh.ContentLength > 0)
                {

                    ////delete anh cu  neu co
                    //if (FileAnhCu.Exists)
                    //{
                    //    FileAnhCu.Delete();
                    //}

                    // add lai anh moi
                    string filename = Path.GetFileNameWithoutExtension(HinhAnh.FileName);
                    string extension = Path.GetExtension(HinhAnh.FileName);
                    filename = filename + "_" + long.Parse(DateTime.Now.ToString("yyyyMMddhhmmss")) + extension;
                    sp.HinhAnh = filename;
                    HinhAnh.SaveAs(Path.Combine(Server.MapPath("../../../Content/Images/"), filename));
                }


                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }


        //[HttpPost]
        //public ActionResult Delete(int id)
        //{
        //    var sp = db.SanPhams.Find(id);
        //    //Lấy ra file ảnh neu co
        //    var PathAnhCu = (Server.MapPath("../../Content/Images/") + sp.HinhAnh);
        //    FileInfo FileAnh = new FileInfo(PathAnhCu.ToString());
        //    //delete anh  neu co
        //    if (FileAnh.Exists)
        //    {
        //        FileAnh.Delete();
        //    }

        //    sp.DaXoa = 1;

        //    //db.SanPhams.Remove(sp);
        //    db.SaveChanges();
        //    return View("Index");
        //}

        public ActionResult SanPhamDaXoa()
        {
            List<SanPham> sanPhamList = db.SanPhams.Where(x => x.DaXoa == 1).ToList();

            return View(sanPhamList);
        }
        //Hoàn tác sản phẩm 
        public ActionResult HoanTac(int MaSP)
        {
            SanPham sp = db.SanPhams.FirstOrDefault(x => x.MaSP == MaSP);
            sp.DaXoa = 0;
            db.SaveChanges();
            return Redirect("SanPhamDaXoa");
        }

    }
}