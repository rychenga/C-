using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace Crawler
{
    class Program
    {
        static void Main(string[] args)
        {
            //將要取得HTML原如碼的網頁放在WebRequest.Create(@”網址” )
            System.Net.WebRequest MyRequest = System.Net.WebRequest.Create(@"http://jdata.yuanta.com.tw/Z/zg/zgk_d.djhtm");

            //Method選擇GET
            MyRequest.Method="GET";

            //取得WebRequest的回覆
            System.Net.WebResponse MyResponse = MyRequest.GetResponse();

            //取的繁體(BIG5)
            System.Text.Encoding enc = System.Text.Encoding.GetEncoding("BIG5");

            //Streamreader讀取回覆(依BIG5 格式)
            System.IO.StreamReader sr = new System.IO.StreamReader(MyResponse.GetResponseStream(),enc);

            //將全文轉成string
            string strResult = sr.ReadToEnd();

            //關掉StreamReader
            sr.Close();

            //關掉WebResponse
            MyResponse.Close();

            //Console.WriteLine(strResult);

            //搜尋頭尾關鍵字
            int iBodyStart = strResult.IndexOf("<body", 0);
            Console.WriteLine(strResult.Substring(iBodyStart,  8));
            Console.ReadKey();
            int iStart = strResult.IndexOf("買超", iBodyStart);
            int iTableStart = strResult.IndexOf("<table", iStart);
            int iTableEnd = strResult.IndexOf("</table>", iTableStart);
    


            Console.ReadKey();

        }
    }
}
