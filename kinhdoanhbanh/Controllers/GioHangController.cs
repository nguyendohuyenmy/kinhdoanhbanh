using kinhdoanhbanh.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace kinhdoanhbanh.Controllers
{
    public class GioHangController : Controller
    {
        KinhDoanhBanhEntities db = new KinhDoanhBanhEntities();
        List<string> listTenTinhTP = new List<string> { "An Giang", "Bà Rịa - Vũng Tàu", "Bạc Liêu", "Bắc Kạn", "Bắc Giang", "Bắc Ninh", "Bến Tre", "Bình Dương", "Bình Định", "Bình Phước", "Bình Thuận", "Cà Mau", "Cao Bằng", "Cần Thơ", "Đà Nẵng", "Đắk Lắk", "Đắk Nông", "Đồng Nai", "Đồng Tháp", "Điện Biên", "Gia Lai", "Hà Giang", "Hà Nam", "Hà Nội", "Hà Tĩnh", "Hải Dương", "Hải Phòng", "Hòa Bình", "Hậu Giang", "Hưng Yên", "Thành phố Hồ Chí Minh", "Khánh Hòa", "Kiên Giang", "Kon Tum", "Lai Châu", "Lào Cai", "Lạng Sơn", "Lâm Đồng", "Long An", "Nam Định", "Nghệ An", "Ninh Bình", "Ninh Thuận", "Phú Thọ", "Phú Yên", "Quảng Bình", "Quảng Nam", "Quảng Ngãi", "Quảng Ninh", "Quảng Trị", "Sóc Trăng", "Sơn La", "Tây Ninh", "Thái Bình", "Thái Nguyên", "Thanh Hóa", "Thừa Thiên - Huế", "Tiền Giang", "Trà Vinh", "Tuyên Quang", "Vĩnh Long", "Vĩnh Phúc", "Yên Bái" };

        private string root = System.Web.HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);//lay root goc dia chi trang wweb


        // bien de luu tru string thông báo
        public static string alert = null;

        // GET: Giohang
        public ActionResult Index()
        {
            List<Cartltem> giohang = Session["giohang"] as List<Cartltem>;
            ViewBag.DanhSach = giohang;

            ViewBag.alert = alert;
            if (Session["user"] != null)
            {
                var toEmailKH = (Models.KhachHang)Session["user"];
                ViewBag.toEmail = toEmailKH.Email;
            }

            return View();
        }

        [HttpGet]
        // khai bao phương thức thêm sản phẩm vào giỏ hang
        public ActionResult AddToCart(int MaSP, int soluong)
        {

            if (Session["giohang"] == null)
            {
                Session["giohang"] = new List<Cartltem>();
            }
            List<Cartltem> giohang = Session["giohang"] as List<Cartltem>;
            //kiem tra san pham co chua
            if (giohang.FirstOrDefault(m => m.MaSP == MaSP) == null)// chua có san pham
            {
                SanPham sp = db.SanPhams.Find(MaSP);
                Cartltem newItem = new Cartltem();
                newItem.MaSP = MaSP;
                newItem.TenSP = sp.TenSP;
                newItem.SoLuong = soluong;
                newItem.DonGia = Convert.ToDouble(sp.DonGiaBan);
                giohang.Add(newItem);
            }
            else//san pham da co trong gio hàng
            {
                Cartltem cardltem = giohang.FirstOrDefault(m => m.MaSP == MaSP);
                cardltem.SoLuong += soluong;
            }
            Session["giohang"] = giohang;
            return RedirectToAction("Index");
        }

        //cap nhat so luong san pham trong gio hang
        public ActionResult Update(int MaSP, int txtSoLuong)
        {
            alert = null;

            List<Cartltem> giohang = Session["giohang"] as List<Cartltem>;
            Cartltem item = giohang.FirstOrDefault(m => m.MaSP == MaSP);

            if (item != null)
            {
                if ((Int32)txtSoLuong > db.SanPhams.Where(x => x.MaSP == item.MaSP).FirstOrDefault().SoLuong)
                {
                    alert = "So Luong San Pham Con Lai Khong Du ";
                    return RedirectToAction("Index");
                }

                item.SoLuong = txtSoLuong;
                Session["giohang"] = giohang;
            }
            return RedirectToAction("Index");
        }
        //xoa gio hang
        public ActionResult DelCartItem(int MaSP)
        {
            List<Cartltem> giohang = Session["giohang"] as List<Cartltem>;
            Cartltem item = giohang.FirstOrDefault(m => m.MaSP == MaSP);
            if (item != null)
            {
                giohang.Remove(item);
                if (giohang.Count == 0)
                {
                    giohang = null;

                }
                Session["giohang"] = giohang;
            }
            return RedirectToAction("Index");
        }

        //Thanh toans bang tien mat
        public ActionResult Order(string Email, string DiaChiDatHangChiTiet, string calc_shipping_provinces, string calc_shipping_district)
        {
            alert = null;

            if (Session["user"] == null)
            {
                return Redirect("/Account/Login");
            }



            List<Cartltem> giohang = Session["giohang"] as List<Cartltem>;
            string sMsg = "<html><body><table border='1'><caption> Thông Tin đạt hàng</caption><tr><th>STT</th><th>Tên Hàng</th><th>Số Lượng</th><th> Đơn giá</th><th>Thanh Tiền</th></tr>";
            int i = 0;
            float tongtien = 0;
            int TongSoLuongDonHang = 0;
            foreach (Cartltem item in giohang)
            {
                i++;
                sMsg += "<tr>";
                sMsg += "<td>" + i.ToString() + "</td>";
                sMsg += "<td>" + item.TenSP + "</td>";
                sMsg += "<td>" + item.SoLuong.ToString() + "</td>";
                sMsg += "<td>" + item.DonGia.ToString() + "</td>";
                sMsg += "<td>" + string.Format("{0:#,###}", item.SoLuong * item.DonGia) + "</td>";
                sMsg += "<tr>";
                tongtien += item.SoLuong * (int)item.DonGia;
                TongSoLuongDonHang += item.SoLuong;
            }

            sMsg += "<tr><th_colspan='5'>Tõng cộng:"
               + string.Format("{0:#,###}", tongtien) + "</th></tr></table>";
            sMsg += "<tr><th_colspan='5'>Hình thức thanh toán:"
               + "Thanh toán khi nhận hàng" + "</th></tr></table>";
            //MailMessage mail = new MailMessage("", Email, "thong tin đơn hàng", sMsg);
            // mail.IsBodyHtml = true;

            //WebMail.Send(Email, "Thông tin đơn đặt hàng", sMsg, null, null, null, true, null, null, null, null, null, null);


            HoaDon hoadon = new HoaDon();
            var kh = Session["user"] as KhachHang;
            hoadon.MaKH = kh.MaKH;
            hoadon.NgayXuatHD = DateTime.Now;
            hoadon.TongTien = tongtien;
            hoadon.SoLuong = TongSoLuongDonHang;

            // vi danh sach tinh , TP tra ve so nen phai map du lieu sang string , con distric tra ve san kieu string neen k can 
            hoadon.DiaChiGiaoHang = DiaChiDatHangChiTiet + " ," + calc_shipping_district + " ," + (listTenTinhTP[Int32.Parse(calc_shipping_provinces) - 1]).ToString();
            hoadon.Note = "Chờ xử lý";
            hoadon.HinhThucThanhToan = "Thanh toán khi nhận hàng";
            db.HoaDons.Add(hoadon);
            db.SaveChanges();
            ChiTietHoaDon cthd = new ChiTietHoaDon();
            foreach (var item in Session["giohang"] as List<Cartltem>)
            {

                cthd.MaHD = db.HoaDons.FirstOrDefault(m => m.MaHD == hoadon.MaHD).MaHD;
                cthd.MaSP = item.MaSP;
                cthd.SoLuong = int.Parse(item.SoLuong.ToString());
                cthd.DonGiaBan = float.Parse(item.ThanhTien.ToString());
                //db.SanPhams.Where(x => x.MaSP == cthd.MaSP).FirstOrDefault().SoLuongDaBan += (int)cthd.Soluong;
                db.SanPhams.Where(x => x.MaSP == cthd.MaSP).FirstOrDefault().SoLuong -= (int)cthd.SoLuong;


                Session["giohang"] = null;
                alert = "Dat Hang Thanh Cong ! Successfull";
            }
            db.ChiTietHoaDons.Add(cthd);
            db.SaveChanges();

            return RedirectToAction("Index");

        }

        public ActionResult MomoPayment(string Email, string DiaChiDatHangChiTiet, string calc_shipping_provinces, string calc_shipping_district)
        {

            alert = null;

            if (Session["user"] == null)
            {
                return Redirect("/Account/Login");
            }

            #region lay tong tien thanh toan momo
            List<Cartltem> giohang = Session["giohang"] as List<Cartltem>;
            float tongtien = 0;

            foreach (Cartltem item in giohang)
            {
                tongtien += item.SoLuong * (int)item.DonGia;
            }
            #endregion



            string endpoint = "https://test-payment.momo.vn/gw_payment/transactionProcessor";
            string partnerCode = "MOMOOJOI20210710";
            string accessKey = "iPXneGmrJH0G8FOP";
            string serectkey = "sFcbSGRSJjwGxwhhcEktCHWYUuTuPNDB";
            string orderInfo = "Long Sơn Cake";
            string returnUrl = $"{root}/GioHang/ConfirmOrderMomo?Email={Email}&DiaChiDatHangChiTiet={DiaChiDatHangChiTiet}&calc_shipping_provinces={calc_shipping_provinces}&calc_shipping_district={calc_shipping_district}";
            string notifyurl = "https://momo.vn/notify";// notifyurl không được sử dụng localhost
            string amount = tongtien + "";
            string orderid = DateTime.Now.Ticks.ToString();
            string requestId = DateTime.Now.Ticks.ToString();
            string extraData = "";

           
          
            //Before sign HMAC SHA256 signature
            string rawHash = "partnerCode=" +
                partnerCode + "&accessKey=" +
                accessKey + "&requestId=" +
                requestId + "&amount=" +
                amount + "&orderId=" +
                orderid + "&orderInfo=" +
                orderInfo + "&returnUrl=" +
                returnUrl + "&notifyUrl=" +
                notifyurl + "&extraData=" +
                extraData;

            MoMoSecurity crypto = new MoMoSecurity();
            //sign signature SHA256
            string signature = crypto.signSHA256(rawHash, serectkey);

            //build body json request
            JObject message = new JObject
            {
                { "partnerCode", partnerCode },
                { "accessKey", accessKey },
                { "requestId", requestId },
                { "amount", amount },
                { "orderId", orderid },
                { "orderInfo", orderInfo },
                { "returnUrl", returnUrl },
                { "notifyUrl", notifyurl },
                { "extraData", extraData },
                { "requestType", "captureMoMoWallet"},
                { "signature", signature }

            };

            string responseFromMomo = PaymentRequest.sendPaymentRequest(endpoint, message.ToString());

            JObject jmessage = JObject.Parse(responseFromMomo);

            return Redirect(jmessage.GetValue("payUrl").ToString());
        }

        public ActionResult ConfirmOrderMomo(string Email, string DiaChiDatHangChiTiet, string calc_shipping_provinces, string calc_shipping_district)
        {
            string result = Request.QueryString["errorCode"];
            if (result.Equals("0")) // neu thanh cong
            {
                alert = null;



                List<Cartltem> giohang = Session["giohang"] as List<Cartltem>;
                string sMsg = "<html><body><table border='1'><caption> Thông Tin đạt hàng</caption><tr><th>STT</th><th>Tên Hàng</th><th>Số Lượng</th><th> Đơn giá</th><th>Thanh Tiền</th></tr>";
                int i = 0;
                float tongtien = 0;
                int TongSoLuongDonHang = 0;
                foreach (Cartltem item in giohang)
                {
                    i++;
                    sMsg += "<tr>";
                    sMsg += "<td>" + i.ToString() + "</td>";
                    sMsg += "<td>" + item.TenSP + "</td>";
                    sMsg += "<td>" + item.SoLuong.ToString() + "</td>";
                    sMsg += "<td>" + item.DonGia.ToString() + "</td>";
                    sMsg += "<td>" + string.Format("{0:#,###}", item.SoLuong * item.DonGia) + "</td>";
                    sMsg += "<tr>";
                    tongtien += item.SoLuong * (int)item.DonGia;
                    TongSoLuongDonHang += item.SoLuong;
                }

                sMsg += "<tr><th_colspan='5'>Tõng cộng:"
                   + string.Format("{0:#,###}", tongtien) + "</th></tr></table>";
                sMsg += "<tr><th_colspan='5'>Hình thức thanh toán:"
                   + "MoMo" + "</th></tr></table>";
                //MailMessage mail = new MailMessage("", Email, "thong tin đơn hàng", sMsg);
                // mail.IsBodyHtml = true;

                //WebMail.Send(Email, "Thông tin đơn đặt hàng", sMsg, null, null, null, true, null, null, null, null, null, null);


                HoaDon hoadon = new HoaDon();
                var kh = Session["user"] as KhachHang;
                hoadon.MaKH = kh.MaKH;
                hoadon.NgayXuatHD = DateTime.Now;
                hoadon.TongTien = tongtien;
                hoadon.SoLuong = TongSoLuongDonHang;

                // vi danh sach tinh , TP tra ve so nen phai map du lieu sang string , con distric tra ve san kieu string neen k can 
                hoadon.DiaChiGiaoHang = DiaChiDatHangChiTiet + " ," + calc_shipping_district + " ," + (listTenTinhTP[Int32.Parse(calc_shipping_provinces) - 1]).ToString();
                hoadon.Note = "Chờ xử lý";
                hoadon.HinhThucThanhToan = "MoMo";
                db.HoaDons.Add(hoadon);
                db.SaveChanges();
                ChiTietHoaDon cthd = new ChiTietHoaDon();
                foreach (var item in Session["giohang"] as List<Cartltem>)
                {

                    cthd.MaHD = db.HoaDons.FirstOrDefault(m => m.MaHD == hoadon.MaHD).MaHD;
                    cthd.MaSP = item.MaSP;
                    cthd.SoLuong = int.Parse(item.SoLuong.ToString());
                    cthd.DonGiaBan = float.Parse(item.ThanhTien.ToString());
                    //db.SanPhams.Where(x => x.MaSP == cthd.MaSP).FirstOrDefault().SoLuongDaBan += (int)cthd.Soluong;
                    db.SanPhams.Where(x => x.MaSP == cthd.MaSP).FirstOrDefault().SoLuong -= (int)cthd.SoLuong;


                    Session["giohang"] = null;
                    alert = "Dat Hang Thanh Cong ! Successfull";
                }
                db.ChiTietHoaDons.Add(cthd);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }


    }
}