using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices; //DllImport need

namespace Actini
{
    class Program
    {
        //import kernel32 for writer ini.
        [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern bool WritePrivateProfileString(string lpAppName, string lpKeyName, string lpString, string lpFileName);
        //import kernel32 for read ini.
        [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern int GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault, StringBuilder lpReturnedString,int nSize, string lpFileName);

        public static void WriteIni(string section, string key, string input, string Path)
        {
            WritePrivateProfileString(section, key, input, Path);

        }
        public static string ReadIni(string section, string key,string Path)
        {
            string sRec;
            StringBuilder record = new StringBuilder();
            GetPrivateProfileString(section,key, "", record, 255, Path);
            sRec = record.ToString();
            return sRec;
        }
    }
}
