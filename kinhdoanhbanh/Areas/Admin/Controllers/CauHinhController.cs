using kinhdoanhbanh.Areas.Admin.Controllers;
using kinhdoanhbanh.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace kinhdoanhbanh.Areas.Admin.Controllers
{
    public class CauHinhController : BaseController
    {
        private KinhDoanhBanhEntities db = new KinhDoanhBanhEntities();

        // GET: Admin/CauHinh
        public ActionResult Index()
        {
            List<CauHinh> lst = db.CauHinhs.ToList();
            return View(lst);
        }

        public ActionResult Edit(string id)
        {
            CauHinh ch = db.CauHinhs.Where(x => x.Key == id).FirstOrDefault();
            return View(ch);
        }

        [HttpPost]
        public ActionResult Edit(string Key, string Value)
        {
            var ch = db.CauHinhs.FirstOrDefault(x => x.Key == Key);
            ch.Value = Value;
            db.SaveChanges();

            //show thoong baos
            Response.Write(@"<script language='javascript'>alert('Message: " + "Update Success!!!!!!" + " .');</script>");

            return RedirectToAction("Index", "CauHinh");
        }

    }
}