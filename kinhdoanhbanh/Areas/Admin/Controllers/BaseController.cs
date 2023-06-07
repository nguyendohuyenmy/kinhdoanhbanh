using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace kinhdoanhbanh.Areas.Admin.Controllers
{
    public class BaseController : Controller
    {
        // Validate Tat ca cac  form neu chua dc dang nhap thi tra ve form login. ghi de 

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Session["userAdmin"] == null)
            {
                filterContext.Result = new RedirectToRouteResult(
                    new System.Web.Routing.RouteValueDictionary(new { Controller = "Account", Action = "Login" })
                    );
            }
            base.OnActionExecuting(filterContext);
        }


    }
}