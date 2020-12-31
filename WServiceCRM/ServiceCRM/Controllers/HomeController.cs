using ServiceCRM.Helpers;
using System;
using System.Net;
using System.Web.Mvc;

namespace ServiceCRM.Controllers
{
    public class HomeController : Controller
    {
        [HttpPost]
        public ActionResult LogIn(string inputNumber)
        {
            CrmHelper crm = new CrmHelper();
            ResponseHelper response = crm.LogIn(inputNumber);
            if (response.IsError)
            {
                if (response.Code == 500)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
                }
                else if (response.Code == 404)
                {
                    return new HttpNotFoundResult("Not Found");
                }
            }
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        [HttpPost]
        public ActionResult LogOff(string inputNumber)
        {
            CrmHelper crm = new CrmHelper();
            ResponseHelper response = crm.LogOff(inputNumber);
            if (response.IsError)
            {
                if (response.Code == 500)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
                }
                else if (response.Code == 404)
                {
                    return new HttpNotFoundResult("Not Found");
                }
            }
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        [HttpPost]
        public ActionResult IncomingCall(string callId, string callDate, string caller)
        {
            DateTime date = DateTime.Parse(callDate);
            CrmHelper crm = new CrmHelper();
            CallerHepler callerHelper = crm.IncomingCall(callId, date, caller);
            if (callerHelper.Code == 200)
            {
                CrmHub signalRUser = new CrmHub();
                ResponseHelper response = signalRUser.IncomingCall(callId, date, callerHelper.PhoneOfCaller, callerHelper.FullName, callerHelper.DateOfBirth);
                if (response.IsError)
                {
                    if (response.Code == 500)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
                    }
                    else if (response.Code == 404)
                    {
                        return new HttpNotFoundResult("Not Found");
                    }
                }
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
        [HttpPost]
        public ActionResult CompleteCall(string callId, string completeDate, string reason)
        {
            DateTime date = DateTime.Parse(completeDate);
            CrmHelper crm = new CrmHelper();
            ResponseHelper response = crm.CompleteCall(callId, date, reason);
            if (response.IsError)
            {
                if (response.Code == 500)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
                }
                else if (response.Code == 404)
                {
                    return new HttpNotFoundResult("Not Found");
                }
            }
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
        [HttpGet]
        public string Summary(string callId)
        {
            CrmHelper crm = new CrmHelper();
            string result = crm.Summary(callId);
            return result;
        }
        [HttpPost]
        public ActionResult Answer(string callId)
        {
            CrmHelper crm = new CrmHelper();
            ResponseHelper response = crm.Answer(callId);
            if (response.IsError)
            {
                if (response.Code == 500)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
                }
                else if (response.Code == 404)
                {
                    return new HttpNotFoundResult("Not Found");
                }
            }
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
        [HttpPost]
        public JsonResult Deny(string callId)
        {
            return Json("Вызов сброшен без ответа");
        }
    }
}