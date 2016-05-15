using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace readini
{
    class Run
    {
        static void Main(string[] args)
        {
            try
            {
                string _sDB;
                string _sUser;
                string _sPwd;
                string _sPath= "D:\\WORK\\20160515\\demo.ini";
                string _sTime = "2016-05-15 14:50:00";

                //讀取ini by Source for DB
                _sDB=Actini.Program.ReadIni("Source","DB",_sPath);
                //讀取ini by Source for User
                _sUser= Actini.Program.ReadIni("Source", "user", _sPath);
                //讀取ini by Source for Pwd""
                _sPwd = Actini.Program.ReadIni("Source","pwd",_sPath);

                Console.WriteLine("DB_NAME:{0} User_Name:{1} Password:{2}",_sDB,_sUser,_sPwd);

                //寫入新的時間給ini
                Actini.Program.WriteIni("Source","time",_sTime,_sPath);


            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            Console.ReadKey();         
        }
    }
}
