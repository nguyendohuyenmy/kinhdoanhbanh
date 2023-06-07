using Facebook;
using kinhdoanhbanh.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace kinhdoanhbanh.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        KinhDoanhBanhEntities db = new KinhDoanhBanhEntities();

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(FormCollection collection)
        {
            var email = collection["InputEmail"];
            var pass = GetMD5(collection["InputPassword"]);
            KhachHang kh = db.KhachHangs.SingleOrDefault(k => k.Email == email && k.MatKhau == pass);
            if (kh != null)
            {
                if (kh.Email == email && kh.MatKhau == pass && kh.TrangThaiTaiKhoan != false)
                {
                    ViewBag.Message = " Đăng Nhập Thành Công";
                    Session["user"] = kh;
                    View();
                    return Redirect("../Home/Index");
                }
                else
                {
                    ViewBag.Message = "Mật Khẩu Đăng Nhập Không Đúng !";
                }
            }
            else
            {
                ViewBag.Message = "Email Đăng Nhập Sai Hoặc Chưa Được Đăng Ký ";
            }

            return View();
        }

        public ActionResult Logout()
        {
            Session["user"] = null;
            return Redirect("../Home/Index");
        }


        //GET: Register

        public ActionResult Register()
        {
            return View();
        }

        //POST: Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(FormCollection collection)
        {
            if (ModelState.IsValid)
            {

                var TenKH = collection["FirstName"] + " " + collection["LastName"];
                var Email = collection["InputEmail"];
                var MatKhau = collection["InputPassword"];
                var MatKhauXacNhan = collection["RepeatPassword"];

                var check = db.KhachHangs.FirstOrDefault(s => s.Email == Email);
                if (check == null)
                {
                    if (MatKhauXacNhan != MatKhau)
                    {

                        ViewBag.error = "Nhập sai mật khẩu xác nhận!";
                        return this.Register();
                    }

                    else
                    {
                        KhachHang kh = new KhachHang();
                        kh.Email = Email;
                        kh.MatKhau = GetMD5(MatKhau);
                        kh.TenKH = TenKH;

                        db.Configuration.ValidateOnSaveEnabled = false;
                        db.KhachHangs.Add(kh);
                        db.SaveChanges();

                        //show thoong baos
                        Response.Write(@"<script language='javascript'>alert('Message: " + "Register Success!!!!!!" + " .');</script>");
                        return RedirectToAction("Login");
                    }
                }
                else
                {
                    ViewBag.error = "Email already exists";
                    return this.Register();
                }
            }
            return this.Register();

        }

        //create a string MD5
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



        //login with FB
        private Uri RedirectUri
        {
            get
            {
                var uriBuilder = new UriBuilder(Request.Url);
                uriBuilder.Query = null;
                uriBuilder.Fragment = null;
                uriBuilder.Path = Url.Action("FacebookCallback");
                return uriBuilder.Uri;
            }
        }
        public ActionResult LoginFacebook()
        {
            var fb = new FacebookClient();
            var loginUrl = fb.GetLoginUrl(new
            {
                client_id = ConfigurationManager.AppSettings["FbAppId"],
                client_secret = ConfigurationManager.AppSettings["FbAppSecret"],
                redirect_uri = RedirectUri.AbsoluteUri,
                response_type = "code",
                scope = "email"
            });

            return Redirect(loginUrl.AbsoluteUri);

        }

        public long InsertForFacebook(KhachHang kh)
        {
            var user = db.KhachHangs.SingleOrDefault(x => x.Email == kh.Email);
            if (user == null)
            {
                db.KhachHangs.Add(kh);
                db.SaveChanges();
                return kh.MaKH;
            }
            else
            {
                return user.MaKH;
            }
        }

        public ActionResult FacebookCallback(string code)
        {
            var fb = new FacebookClient();
            dynamic result = fb.Post("oauth/access_token", new
            {
                client_id = ConfigurationManager.AppSettings["FbAppId"],
                client_secret = ConfigurationManager.AppSettings["FbAppSecret"],
                redirect_uri = RedirectUri.AbsoluteUri,
                code = code
            });

            var accessToken = result.access_token;
            if (!string.IsNullOrEmpty(accessToken))
            {
                fb.AccessToken = accessToken;


                // lay ra thong tin cua user fb
                dynamic me = fb.Get("me?fields=first_name,middle_name,last_name,id,email");
                string email = me.email;
                string userName = me.email;
                string firstname = me.first_name;
                string middlename = me.middle_name;
                string lastname = me.last_name;


                var kh = new KhachHang();
                kh.Email = email;
                kh.TenKH = firstname + " " + middlename + " " + lastname;

                var resultInsert = InsertForFacebook(kh);

                if (resultInsert > 0) // neu thanh cong
                {
                    Session["user"] = kh;
                }
            }
            return Redirect("/");


        }

        //view quen MK
        public ActionResult ForgotPass(string alert = "")
        {
            ViewBag.Message = alert;
            return View();
        }

        //an nut gui quen mat khau
        public ActionResult SubmitForgotPassWork(string EmailNGuoiNhan)
        {

            string root = System.Web.HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

            //truong hop email khong ton tai 
            var check = db.KhachHangs.FirstOrDefault(x => x.Email == EmailNGuoiNhan);
            if (check == null)
            {
                ViewBag.Message = "test ne";
                return RedirectToAction("ForgotPass", new { alert = "Email Nhập Không Tồn Tại Hoặc Chưa Được Đăng Ký" });
            }


            //cap phat mat khau reset
            string MatKhauMoi = RandomString(8);

            try
            {
                var senderEmail = new MailAddress("ntctv04042001@gmail.com", "Nguyễn Thành Công");
                var receiverEmail = new MailAddress(EmailNGuoiNhan, "Người nhận");

                var sub = "Xác thực Quên mật Khẩu Website Hải Sản!!";
                var body = $"<p> Mật khẩu mới của bạn là : {MatKhauMoi} </p><br/>";
                body += $"<p> Vui lòng ấn vào link bên dưới để xác nhận Reset Mật Khẩu </p><br/>";
                body += $"<a href='{root}/Account/KichHoatMKMoi?Email={EmailNGuoiNhan}&MatKhauMoi={MatKhauMoi}' style='font-size: 20px; padding: 5px 10px; background-color: green; color: white; border-radius: 5px; text-decoration: none'>Kích hoạt</a>";

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(senderEmail.Address, "uhznwxvkomyubhev")
                };

                using (var mess = new MailMessage(senderEmail, receiverEmail)
                {
                    Subject = sub,
                    Body = body.ToString()
                })
                {
                    mess.IsBodyHtml = true;
                    smtp.Send(mess);
                }

            }
            catch (Exception)
            {
                ViewBag.Error = "Có lỗi xảy ra";
            }
            return View();
        }



        //ham xu li kich hoat mat khau moi sau khi nguoi dung` da dien email va mk moi cua forgot passwork
        public ActionResult KichHoatMKMoi(string Email, string MatKhauMoi)
        {
            KhachHang kh = db.KhachHangs.FirstOrDefault(h => h.Email == Email);

            if (kh != null)
            {
                kh.MatKhau = GetMD5(MatKhauMoi);
                db.SaveChanges();
            }

            return View();
        }





        //Lay danh sach don hang tai khoan da dat
        public ActionResult DonHangDaDat()
        {
            if (Session["user"] == null)
            {
                return Redirect("Login");
            }

            KhachHang kh = Session["user"] as KhachHang;

            List<HoaDon> lstHoaDonDaDat = db.HoaDons.Where(x => x.MaKH == kh.MaKH).ToList();


            return View(lstHoaDonDaDat);
        }

        //View Thay doi thong tin tai khoan 
        public ActionResult CapNhatThongTinTaiKhoan()
        {
            if (Session["user"] == null)
            {
                return Redirect("Login");
            }
            KhachHang kh = Session["user"] as KhachHang;

            return View(kh);
        }

        [HttpPost]
        public ActionResult CapNhatThongTinTaiKhoan(string TenKH, string DiaChi, string SDT, string Email, string MatKhauCu, string MatKhauMoi, string XacNhanMatKhauMoi)
        {
            if (Session["user"] == null)
            {
                return Redirect("Login");
            }
            KhachHang _kh = Session["user"] as KhachHang; // lay khach hang tu session
            KhachHang kh = db.KhachHangs.FirstOrDefault(x => x.MaKH == _kh.MaKH); // lay khach hang tu database de cap nhat

            if (MatKhauCu == "" || MatKhauMoi == "") // neu truong hop khong nhap MK Cu hoac khong Nhap Mk Moi <=> chi thay doi thong tin ca nhan
            {
                kh.TenKH = TenKH;
                kh.DiaChi = DiaChi;
                kh.SDT = SDT;
                kh.Email = Email;
                db.SaveChanges();
                Response.Write(@"<script language='javascript'>alert('Message: " + "CAP NHAT THONG TIN TAI KHOAN THANH CONG!!!!!!" + " .');</script>");
            }

            else if ((MatKhauCu != null || MatKhauCu != "") && (MatKhauMoi != null || MatKhauMoi != ""))
            {
                kh.TenKH = TenKH;
                kh.DiaChi = DiaChi;
                kh.SDT = SDT;
                kh.Email = Email;

                if (GetMD5(MatKhauCu) == kh.MatKhau)
                {
                    if (MatKhauMoi == XacNhanMatKhauMoi)
                    {
                        kh.MatKhau = GetMD5(MatKhauMoi);
                        db.SaveChanges();
                        Response.Write(@"<script language='javascript'>alert('Message: " + "CAP NHAT THONG TIN TAI KHOAN THANH CONG!!!!!!" + " .');</script>");

                    }
                    else
                    {
                        Response.Write(@"<script language='javascript'>alert('Message: " + "MAT KHAU XAC NHAN KHONG DUNG ! FAILLL" + " .');</script>");

                    }

                }
                else
                {
                    Response.Write(@"<script language='javascript'>alert('Message: " + "NHAP SAI MAT KHAU CU ! VUI LONG NHAP LAI MK ! FAILL" + " .');</script>");

                }
            }

            Session["user"] = kh;

            return View(kh);
        }

        // ham rand 
        public static string RandomString(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

    }
}