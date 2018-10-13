using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace demo
{
    //Console.WriteLine("輸入工號");
    //string _Snumber = Console.ReadLine();
    //Console.WriteLine("輸入姓名");
    //string _Sname = Console.ReadLine();
    //Console.WriteLine("輸入到職日");
    //DateTime _Dcheckin = DateTime.Parse(Console.ReadLine());
    //Console.WriteLine("輸入留職停薪日");
    //DateTime _DLwp = DateTime.Parse(Console.ReadLine());
    //Console.WriteLine("輸入復職日期");
    //DateTime _DRd = DateTime.Parse(Console.ReadLine());
    //Console.WriteLine("工號: {0};姓名: {1};到職日: {2};留職日: {3};復職日: {4}",_Snumber,_Sname,_Dcheckin.ToString(),_DLwp.ToString(),_DRd.ToString());
    class Program
    {

        static void Main(string[] args)
        {
            try
            {
                DateTime _Dnow = DateTime.Now; //取得目前日期時間                
                DateTime dt = DateTime.Parse(Console.ReadLine()); //輸入日期
                Console.WriteLine(_Dnow.ToShortDateString());
                ExtendMethod.Age myAge = ExtendMethod.CalculateAge(dt, _Dnow); //計算相差多少、年、日、月

                Console.WriteLine("{0} 年; {1} 月; {2} 天;",myAge.Years.ToString(),myAge.Months.ToString(),myAge.Days.ToString());


            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.ToString());
            }

            Console.ReadKey();
        }
    }
}
