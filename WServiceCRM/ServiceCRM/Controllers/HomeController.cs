using ServiceCRM.Helpers;
using System;
using System.Web.Mvc;

namespace ServiceCRM.Controllers
{
    public class HomeController : Controller
    {
        public JsonResult LogIn(string inputNumber)
        {
            CrmHelper crm = new CrmHelper();
            string result = crm.LogIn(inputNumber);
            return Json(result);
        }
        public JsonResult LogOff(string inputNumber)
        {
            CrmHelper crm = new CrmHelper();
            string result = crm.LogOff(inputNumber);
            return Json(result);
        }
        public JsonResult IncomingCall(string callId, string callDate, string caller)
        {
            DateTime date = DateTime.Parse(callDate);
            CrmHelper crm = new CrmHelper();
            string result = crm.IncommingCall(callId, date, caller);
            return Json(result);
        }
        public JsonResult CompleteCall(string callId, string completeDate, string reason)
        {
            DateTime date = DateTime.Parse(completeDate);
            CrmHelper crm = new CrmHelper();
            string result = crm.CompleteCall(callId, date, reason);
            return Json(result);
        }
        public string Summary(string callId)
        {
            CrmHelper crm = new CrmHelper();
            string result = crm.Summary(callId);
            return result;
        }
        public JsonResult Answer(string callId)
        {
            CrmHelper crm = new CrmHelper();
            string result = crm.Answer(callId);
            return Json(result);
        }
        public JsonResult Deny(string callId)
        {
            return Json("Вызов сброшен без ответа");
        }
    }
}
