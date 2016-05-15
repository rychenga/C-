using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection; // 用來描述組件、模組和型別。

namespace GetAppPath
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string _sPath1= System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);
                string _sPath2= System.IO.Path.GetDirectoryName(Assembly.GetAssembly(typeof(Program)).Location);
                string _sPath3= System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

                Console.WriteLine("path1:'{0}'\t path2:'{1}'\t path3:'{2}'\t",_sPath1,_sPath2,_sPath3);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            Console.ReadKey();
        }
    }
}
//path1:'file:\D:\WORK\20160515\GetAppPath\GetAppPath\bin\Debug'   
//path2:'D:\WORK\20160515\GetAppPath\GetAppPath\bin\Debug'        
//path3:'D:\WORK\20160515\GetAppPath\GetAppPath\bin\Debug'
