using System;
using System.Net;

namespace PhoneEmulator
{
    class Program
    {
        static string callId = "1348";
        static string callDate = new DateTime(2020, 12, 16).ToString();
        static string completeDate = new DateTime(2020, 12, 16).ToString();
        static string caller = "%2B7(495)133-33-37";
        static string reason = "Easy reason";
        static string userShortNumber = "13377";

        static void Main(string[] args)
        {
            string result = "";
            Console.WriteLine("IncomingCall() -> press 1");
            Console.WriteLine("CompleteCall() -> press 2");
            Console.WriteLine("Answer() -> press 3");
            //Console.WriteLine("Deny() -> press 4");
            Console.WriteLine("Summary() -> press 5");
            Console.WriteLine("LogIn() -> press 6");
            bool reload = true;
            while (reload)
            {
                string num = Console.ReadLine();
                if (num.Equals("1"))
                {
                    result = IncomingCall();
                    Console.WriteLine(result);
                }
                else if (num.Equals("2"))
                {
                    result = CompleteCall();
                    Console.WriteLine(result);
                }
                else if (num.Equals("3"))
                {
                    result = Answer();
                    Console.WriteLine(result);
                }
                else if (num.Equals("4"))
                {
                    //result = Deny();
                    //Console.WriteLine(result);
                }
                else if (num.Equals("5"))
                {
                    result = Summary();
                    Console.WriteLine(result);
                }
                else if (num.Equals("6"))
                {
                    result = LogIn();
                    Console.WriteLine(result);
                }
                if (num.Equals("end")) reload = false;
            }
        }
        static string IncomingCall()
        {
            var httpRequest = (HttpWebRequest)WebRequest.Create($"http://localhost:56623/cti/incomingcall?callId={callId}&callDate={callDate}&caller={caller}&userShortNumber={userShortNumber}");
            httpRequest.Method = "POST";
            httpRequest.ContentLength = 0;
            httpRequest.ContentType = "application/json";
            try
            {
                using (HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse())
                {
                    return httpResponse.StatusCode.ToString();
                }
            }
            catch (WebException e)
            {
                return ((HttpWebResponse)e.Response).StatusDescription;
            }
        }
        static string CompleteCall()
        {
            var httpRequest = (HttpWebRequest)WebRequest.Create($"http://localhost:56623/cti/completecall?callId={callId}&completeDate={completeDate}&reason={reason}");
            httpRequest.Method = "POST";
            httpRequest.ContentLength = 0;
            httpRequest.ContentType = "application/json";
            try
            {
                using (HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse())
                {
                    return httpResponse.StatusCode.ToString();
                }
            }
            catch (WebException e)
            {
                return ((HttpWebResponse)e.Response).StatusDescription;
            }
        }
        static string Answer()
        {
            var httpRequest = (HttpWebRequest)WebRequest.Create($"http://localhost:56623/cti/answer?callId={callId}");
            httpRequest.Method = "POST";
            httpRequest.ContentLength = 0;
            httpRequest.ContentType = "application/json";
            try
            {
                using (HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse())
                {
                    return httpResponse.StatusCode.ToString();
                }
            }
            catch (WebException e)
            {
                return ((HttpWebResponse)e.Response).StatusDescription;
            }
        }
        //static string Deny()
        //{
        //    var httpRequest = (HttpWebRequest)WebRequest.Create($"http://localhost:56623/cti/deny?callId={callId}");
        //    httpRequest.Method = "POST";
        //    httpRequest.ContentLength = 0;
        //    httpRequest.ContentType = "application/json";
        //    try
        //    {
        //        using (HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse())
        //        {
        //            return httpResponse.StatusCode.ToString();
        //        }
        //    }
        //    catch (WebException e)
        //    {
        //        return ((HttpWebResponse)e.Response).StatusDescription;
        //    }
        //}
        static string LogIn()
        {
            var httpRequest = (HttpWebRequest)WebRequest.Create($"http://localhost:56623/cti/login?inputNumber={userShortNumber}");
            httpRequest.Method = "POST";
            httpRequest.ContentLength = 0;
            httpRequest.ContentType = "application/json";
            try
            {
                using (HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse())
                {
                    return httpResponse.StatusCode.ToString();
                }
            } catch (WebException e)
            {
                return ((HttpWebResponse)e.Response).StatusDescription;
            }
        }
        static string Summary()
        {
            using (var webClient = new WebClient())
            {
                string result = webClient.DownloadString($"http://localhost:56623/cti/summary?callId={callId}");
                return result;
            }
        }
    }
}
