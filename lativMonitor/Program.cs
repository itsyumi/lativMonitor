using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace lativMonitor
{
    class Program
    {
        static HttpWebRequest request;
        static HttpWebResponse response;
        static string url = "https://www.lativ.com.tw/Detail/52203014";

        static async Task Main(string[] args)
        {
            //Notify();
            RunHttp(url, false);
            Console.WriteLine($"Press any key to Exist...{Environment.NewLine}");
            Console.ReadKey();

        }

        static async void RunHttp(string url, bool isInStock)
        {
            if (!isInStock)
            {
                request = WebRequest.Create(url) as HttpWebRequest;
                request.AllowAutoRedirect = false;
                request.Method = "GET";
                try
                {
                    response = await request.GetResponseAsync() as HttpWebResponse;
                    //Console.WriteLine(response.StatusCode);
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        Notify("6.5有貨啦！！！！\n"+url);
                        return;
                    }
                }
                catch (WebException e)
                {
                    if (e.Message.Contains("302"))
                    {
                        RunHttp(url, false);
                    }
                }
            }

        }

        static async void Notify(string message)
        {
            request = WebRequest.Create("https://notify-api.line.me/api/notify") as HttpWebRequest;
            request.AllowAutoRedirect = true;
            request.Method = "POST";
            request.Headers.Add("Authorization", "Bearer {authorization_code}");
            request.ContentType = "application/x-www-form-urlencoded";
            NameValueCollection postParams = System.Web.HttpUtility.ParseQueryString(string.Empty);
            postParams.Add("message", message);
            //要發送的字串轉為byte[] 
            byte[] byteArray = Encoding.UTF8.GetBytes(postParams.ToString());
            using (Stream reqStream = await request.GetRequestStreamAsync())
            {
                reqStream.Write(byteArray, 0, byteArray.Length);
            }
            try
            {
                response = await request.GetResponseAsync() as HttpWebResponse;
                Console.WriteLine(message);
            }
            catch (WebException e)
            {
                Console.WriteLine(e.Message);
            }

        }
    }
}
