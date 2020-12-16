using System;
using System.IO;
using System.Net;


namespace PhoneEmulator
{
    class Program
    {
        static string callId = "1344";
        static string callDate = new DateTime(2020, 12, 16).ToString();
        static string completeDate = new DateTime(2020, 12, 16).ToString();
        static string caller = "%2B7(495)133-33-37";
        static string reason = "Easy reason";

        static void Main(string[] args)
        {
            string result = "";
            Console.WriteLine("IncommingCall() -> press 1");
            Console.WriteLine("CompleteCall() -> press 2");
            Console.WriteLine("Answer() -> press 3");
            Console.WriteLine("Deny() -> press 4");
            Console.WriteLine("Summary() -> press 5");
            bool reload = true;
            while (reload)
            {
                string num = Console.ReadLine();
                if (num.Equals("1"))
                {
                    result = IncommingCall();
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
                    result = Deny();
                    Console.WriteLine(result);
                }
                else if (num.Equals("5"))
                {
                    result = Summary();
                    Console.WriteLine(result);
                }
                if (num.Equals("end")) reload = false;
            }
        }
        static string IncommingCall()
        {
            string result = "";
            var httpRequest = (HttpWebRequest)WebRequest.Create($"http://localhost:56623/home/incomingcall?callId={callId}&callDate={callDate}&caller={caller}");
            httpRequest.Method = "POST";
            httpRequest.ContentLength = 0;
            httpRequest.ContentType = "application/json";
            using (var httpResponse = httpRequest.GetResponse())
            {
                using (Stream stream = httpResponse.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        result = reader.ReadToEnd();
                    }
                }
            }
            return result;
        }
        static string CompleteCall()
        {
            string result = "";
            var httpRequest = (HttpWebRequest)WebRequest.Create($"http://localhost:56623/home/completecall?callId={callId}&completeDate={completeDate}&reason={reason}");
            httpRequest.Method = "POST";
            httpRequest.ContentLength = 0;
            httpRequest.ContentType = "application/json";
            using (var httpResponse = httpRequest.GetResponse())
            {
                using (Stream stream = httpResponse.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        result = reader.ReadToEnd();
                    }
                }
            }
            return result;
        }
        static string Answer()
        {
            string result = "";
            var httpRequest = (HttpWebRequest)WebRequest.Create($"http://localhost:56623/home/answer?callId={callId}");
            httpRequest.Method = "POST";
            httpRequest.ContentLength = 0;
            httpRequest.ContentType = "application/json";
            using (var httpResponse = httpRequest.GetResponse())
            {
                using (Stream stream = httpResponse.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        result = reader.ReadToEnd();
                    }
                }
            }
            return result;
        }
        static string Deny()
        {
            string result = "";
            var httpRequest = (HttpWebRequest)WebRequest.Create($"http://localhost:56623/home/deny?callId={callId}");
            httpRequest.Method = "POST";
            httpRequest.ContentLength = 0;
            httpRequest.ContentType = "application/json";
            using (var httpResponse = httpRequest.GetResponse())
            {
                using (Stream stream = httpResponse.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        result = reader.ReadToEnd();
                    }
                }
            }
            return result;
        }
        static string Summary()
        {
            using (var webClient = new WebClient())
            {
                string result = webClient.DownloadString($"http://localhost:56623/home/summary?callId={callId}");
                return result;
            }
        }
    }
}
