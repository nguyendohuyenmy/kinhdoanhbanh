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
    public class AccountController : Controller
    {
        private KinhDoanhBanhEntities db = new KinhDoanhBanhEntities();
        // GET: Admin/Account
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(FormCollection collection)
        {
            var email = collection["InputEmail"];
            var pass = GetMD5(collection["InputPassword"]);
            NhanVien nv = db.NhanViens.SingleOrDefault(n => n.Email == email && n.MatKhau == pass);
            if (nv != null)
            {
                if (nv.Email == email && nv.MatKhau == pass)
                {
                    ViewBag.Message = " Đăng Nhập Thành Công";
                    Session["userAdmin"] = nv;
                    View();
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.Message = "Mật Khẩu Đăng Nhập Không Đúng !";
                }
            }
            else
            {
                ViewBag.Message = "Email Sai Hoặc Chưa Tồn Tại ! ";
            }

            return View();
        }
        public ActionResult Logout()
        {
            Session["userAdmin"] = null;
            return RedirectToAction("Login");
        }

        public ActionResult Create()
        {
            return View();
        }

        //POST: Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(NhanVien nhanVien)
        {
            if (ModelState.IsValid)
            {

                var _NhanVien = nhanVien;
                _NhanVien.MatKhau = GetMD5(_NhanVien.MatKhau);

                var check = db.NhanViens.FirstOrDefault(s => s.Email == _NhanVien.Email);
                if (check == null)
                {

                    db.Configuration.ValidateOnSaveEnabled = false;
                    db.NhanViens.Add(_NhanVien);
                    db.SaveChanges();

                    Response.Write(@"<script language='javascript'>alert('Message: " + "Register Success!!!!!!" + " .');</script>");
                    return RedirectToAction("Login");
                }
                else
                {
                    ViewBag.error = "Email already exists";
                    return this.Create();
                }
            }
            return this.Create();

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
    }
}