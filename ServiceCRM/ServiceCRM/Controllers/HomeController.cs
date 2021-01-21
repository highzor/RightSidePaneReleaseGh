using ServiceCRM.Helpers;
using System;
using System.Net;
using System.Web.Mvc;

namespace ServiceCRM.Controllers
{
    public class HomeController : Controller
    {
        /// <summary>
        /// Авторизация extension
        /// </summary>
        /// <param name="inputNumber">Короткий номер пользователя</param>
        /// <returns>status code</returns>
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
        /// <summary>
        /// Выход из extension
        /// </summary>
        /// <param name="inputNumber">Короткий номер пользователя</param>
        /// <returns>status code</returns>
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
        /// <summary>
        /// Поступающий выов от ATC
        /// </summary>
        /// <param name="callId">ID звонка, который будет создан</param>
        /// <param name="callDate">Дата события звонка</param>
        /// <param name="caller">№Телефона вызывающего абонента от АТС</param>
        /// <param name="userShortNumber">Короткий номер пользователя, который вызывают от АТС</param>
        /// <returns>status code</returns>
        [HttpPost]
        public ActionResult IncomingCall(string callId, string callDate, string caller, string userShortNumber)
        {
            DateTime date = DateTime.Parse(callDate);
            CrmHelper crm = new CrmHelper();
            CallerHepler callerHelper = crm.CreatePhoneCall(callId, date, caller, userShortNumber);
            if (callerHelper.Code == 200)
            {
                CrmHub signalRUser = new CrmHub();
                ResponseHelper response = signalRUser.ExecuteSignalRIncomingCall(callerHelper);
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
        /// <summary>
        /// Завершение звонка
        /// </summary>
        /// <param name="callId">ID звонка, который создан</param>
        /// <param name="completeDate">Дата завершения события звонка</param>
        /// <param name="reason">Описание(причина) завершения</param>
        /// <returns>status code</returns>
        [HttpPost]
        public ActionResult CompleteCall(string callId, string completeDate, string reason)
        {
            DateTime date = DateTime.Parse(completeDate);
            CrmHelper crm = new CrmHelper();
            ResponseHelper response = crm.SetAttrsCompleteCall(callId, date, reason);
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
        /// <summary>
        /// Получения полей сущности 'Звонок'
        /// </summary>
        /// <param name="callId">ID звонка, который создан</param>
        /// <returns>status code</returns>
        [HttpGet]
        public string Summary(string callId)
        {
            CrmHelper crm = new CrmHelper();
            string result = crm.GetSummaryFields(callId);
            return result;
        }
        /// <summary>
        /// Ответ(снятие трубки) на звонок
        /// </summary>
        /// <param name="callId">ID звонка, который создан</param>
        /// <returns>status code</returns>
        [HttpPost]
        public ActionResult Answer(string callId)
        {
            CrmHelper crm = new CrmHelper();
            ResponseHelper response = crm.SetAttrsAnswer(callId);
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
        //[HttpPost]
        //public JsonResult Deny(string callId)
        //{
        //    return Json("Вызов сброшен без ответа");
        //}
    }
}