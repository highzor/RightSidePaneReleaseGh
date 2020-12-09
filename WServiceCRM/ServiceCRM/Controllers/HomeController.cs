using ServiceCRM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ServiceCRM.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public JsonResult SignIn(string inputNumber)
        {
            string result = "";
                using (CrmHelper crm = new CrmHelper())
                {
                    result = crm.StartsWork(inputNumber);
                    return Json(result);
                }
        }
        public JsonResult SignOut(string shortNumber)
        {
            string result = "";
            using (CrmHelper crm = new CrmHelper())
            {
                result = crm.StopsWork(shortNumber);
                return Json(result);
            }
        }
        public ActionResult Crm()
        {
            return View();
        }
    }
}