using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.IO;

namespace WarmUpIIS
{
    class Program
    {
        static void Main(string[] args)
        {
            long interval = 1000 * 60 * 3;
            Timer t = new Timer(TimerCallback,null,0, interval) ;
            // Wait for the user to hit <Enter>
            Console.ReadLine();
        }

        private static void TimerCallback(Object o)
        {
            Console.WriteLine("In TimerCallback: " + DateTime.Now);
            //call url:
            //https://myfileit.net/MyFileItService/MyFileItAppService.svc/rest/InitService
            var URL = "https://myfileit.net/MyFileItService/MyFileItAppService.svc/rest/InitService";
            
            WebRequest wrGETURL;
            wrGETURL = WebRequest.Create(URL);

           // WebProxy myProxy = new WebProxy("myproxy", 80);
            //myProxy.BypassProxyOnLocal = true;

            //wrGETURL.Proxy = WebProxy.GetDefaultProxy();

            Stream objStream;
            objStream = wrGETURL.GetResponse().GetResponseStream();

            StreamReader objReader = new StreamReader(objStream);

			string sLine = "";
			int i = 0;

			while (sLine!=null)
			{
				i++;
				sLine = objReader.ReadLine();
				if (sLine!=null)
					Console.WriteLine("{0}:{1}",i,sLine);
			}
             //Console.ReadLine();

            
            GC.Collect();
        }
    }
}
