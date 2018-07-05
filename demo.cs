using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;

namespace get
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("請輸入第一個進行運算數值 :");
            string key = Console.ReadLine();

            try
            {
                //將要取得HTML原如碼的網頁放在WebRequest.Create(@”網址” )
                //WebRequest myRequest = WebRequest.Create(@"https://tw.stock.yahoo.com/q/q?s=3088");
                WebRequest myRequest = WebRequest.Create(@"https://tw.stock.yahoo.com/q/q?s=" + key);

                //Method選擇GET
                myRequest.Method = "GET";

                //取得WebRequest的回覆
                WebResponse myResponse = myRequest.GetResponse();

                //Streamreader讀取回覆
                //StreamReader sr = new StreamReader(myResponse.GetResponseStream()); //中文會亂碼
                StreamReader sr = new StreamReader(myResponse.GetResponseStream(), System.Text.Encoding.Default); //顯示中文字           

                //將全文轉成string
                string result = sr.ReadToEnd();

                //關掉StreamReader
                sr.Close();

                //關掉WebResponse
                myResponse.Close();

                ////搜尋頭尾關鍵字, 搜尋方法見後記(1)
                //範例：
                //      <td align=center width=105><a
                //  href="/q/bc?s=3088">3088艾訊</a><br><a href="/pf/pfsel?stocklist=3088;"><font size=-1>加到投資組合</font><br></a></td>
                //            <td align="center" bgcolor="#FFFfff" nowrap>13:30</td>
                //            <td align="center" bgcolor="#FFFfff" nowrap><b>62.0</b></td>
                //            <td align="center" bgcolor="#FFFfff" nowrap>61.9</td>
                //            <td align="center" bgcolor="#FFFfff" nowrap>62.0</td>
                //            <td align="center" bgcolor="#FFFfff" nowrap><font color=#009900>▽0.5
                //            <td align="center" bgcolor="#FFFfff" nowrap>430</td>
                //            <td align="center" bgcolor="#FFFfff" nowrap>62.5</td>
                //            <td align="center" bgcolor="#FFFfff" nowrap>62.7</td>
                //            <td align="center" bgcolor="#FFFfff" nowrap>63.1</td>
                //            <td align="center" bgcolor="#FFFfff" nowrap>61.8</td>
                //      <td align=center width=137 class="tt">
                //<a href="/q/ts?s=3088">成交明細</a><br><a href="/q/ta?s=3088">技術</a>　<a href='/q/h?s=3088'>新聞</a><a href='/d/s/company_3088.html'><br>基本</a>　<a href='/d/s/credit_3088.html'>籌碼</a><br><a target='_blank' style='color:red' href='https://tw.rd.yahoo.com/referurl/stock/other/SIG=125v47s73/**https://tw.screener.finance.yahoo.net/screener/check.html?symid=3088'>個股健診</a></font></td>        </tr>

                int first = result.IndexOf("<td align=center width=105><a");
                int last = result.LastIndexOf("</font><br></a></td>");
                ////減去頭尾不要的字元或數字, 並將結果轉為string. 計算方式見後記(2)
                string HTMLCut = result.Substring(first , last - first);
                Console.WriteLine("name {0}", HTMLCut);
                Console.ReadKey();

                first = result.IndexOf("nowrap><b>");
                last = result.LastIndexOf("</b></td>");
                ////減去頭尾不要的字元或數字, 並將結果轉為string. 計算方式見後記(2)
                HTMLCut = result.Substring(first + 10, last - first - 10);
                Console.WriteLine("resule {0}", HTMLCut);
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.ReadKey();
            }
        }
    }
}
