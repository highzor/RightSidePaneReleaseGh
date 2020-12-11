using Microsoft.AspNet.SignalR;
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
        public void Crm(string name, string message)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<CrmHub>();

            //context.Clients.All.addNewMessageToPage(name, message);
            // or
            //context.Clients.Group("groupname").methodInJavascript("hello world");
        }
        public JsonResult IncomingCall(string callId, DateTime callDate, string caller)
        {
            using (CrmHelper crm = new CrmHelper())
            {
                string result = crm.IncommingCall(callId, callDate, caller);
                return Json(result);
            }
        }
        public void CompleteCall(string callId, DateTime completeDate, string reason)
        {
            using (CrmHelper crm = new CrmHelper())
            {
                crm.CompleteCall(callId, completeDate, reason);
            }
        }
        public JsonResult Summary(string callId)
        {
            using (CrmHelper crm = new CrmHelper())
            {

                return Json(crm.Summary(callId), JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult Answer(string callId)
        {
            using (CrmHelper crm = new CrmHelper())
            {
                string result = crm.Answer(callId);
                return Json(result);
            }
        }
        public JsonResult Deny(string callId)
        {
            return Json("Вызов сброшен без ответа");
        }
    }
}