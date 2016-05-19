using System;
using System.IO;
using System.Net;

namespace GetHtml
{
    class Program
    {
        static void Main(string[] args)
        {
            //將要取得HTML原如碼的網頁放在WebRequest.Create(@”網址” )
            WebRequest myRequest = WebRequest.Create(@"https://www.google.com/finance?q=USDTWD");

            //Method選擇GET
            myRequest.Method = "GET";

            //取得WebRequest的回覆
            WebResponse myResponse = myRequest.GetResponse();

            //Streamreader讀取回覆
            StreamReader sr = new StreamReader(myResponse.GetResponseStream());

            //將全文轉成string
            string result = sr.ReadToEnd();

            //關掉StreamReader
            sr.Close();

            //關掉WebResponse
            myResponse.Close();

            //搜尋頭尾關鍵字, 
            int first = result.IndexOf("1 USD = <span class=bld>");
            int last = result.LastIndexOf("TWD</span>");


            //減去頭尾不要的字元或數字, 並將結果轉為string
            string HTMLCut = result.Substring(first + 25, last - first - 25);
            string txtRate;
            txtRate = HTMLCut;

            Console.WriteLine(txtRate);
            Console.ReadKey();
            
        }
    }
}
