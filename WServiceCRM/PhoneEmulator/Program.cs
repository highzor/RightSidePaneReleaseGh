using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PhoneEmulator
{
    class Program
    {
        static string callId = "1343";
        static string callDate = "14.12.2020";
        static string completeDate = "14.12.2020";
        static string caller = "+7(495)133-33-37";
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
            string postParameters = "{'callId' : '"+callId+"', 'callDate' : '"+callDate+"', 'caller' : '"+caller+"'}";
            var httpRequest = (HttpWebRequest)WebRequest.Create("http://localhost:56623/home/incomingcall");
            httpRequest.Method = "POST";
            httpRequest.ContentType = "application/json";
            using (var requestStream = httpRequest.GetRequestStream())
            using (var writer = new StreamWriter(requestStream))
            {
                writer.Write(postParameters);
            }
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
            string postParameters = "{'callId' : '"+callId+"', 'completeDate' : '"+completeDate+"', 'reason' : '"+reason+"'}";
            var httpRequest = (HttpWebRequest)WebRequest.Create("http://localhost:56623/home/completecall");
            httpRequest.Method = "POST";
            httpRequest.ContentType = "application/json";
            using (var requestStream = httpRequest.GetRequestStream())
            using (var writer = new StreamWriter(requestStream))
            {
                writer.Write(postParameters);
            }
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
            string postParameters = "{'callId' : '"+callId+"'}";
            var httpRequest = (HttpWebRequest)WebRequest.Create("http://localhost:56623/home/answer");
            httpRequest.Method = "POST";
            httpRequest.ContentType = "application/json";
            using (var requestStream = httpRequest.GetRequestStream())
            using (var writer = new StreamWriter(requestStream))
            {
                writer.Write(postParameters);
            }
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
            string postParameters = "{'callId' : '" + callId + "'}";
            var httpRequest = (HttpWebRequest)WebRequest.Create("http://localhost:56623/home/deny");
            httpRequest.Method = "POST";
            httpRequest.ContentType = "application/json";
            using (var requestStream = httpRequest.GetRequestStream())
            using (var writer = new StreamWriter(requestStream))
            {
                writer.Write(postParameters);
            }
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
            string result = "";
            string postParameters = "{'callId' : '" + callId + "'}";
            var httpRequest = (HttpWebRequest)WebRequest.Create("http://localhost:56623/home/summary");
            httpRequest.Method = "POST";
            httpRequest.ContentType = "application/json";
            using (var requestStream = httpRequest.GetRequestStream())
            using (var writer = new StreamWriter(requestStream))
            {
                writer.Write(postParameters);
            }
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
    }
}
