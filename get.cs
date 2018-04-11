using System;
using System.Collections.Generic;
using System.Text;
using System.Management; //參考要另外加入
using System.Security.Cryptography;
using System.IO;

namespace GET
{
    class Program
    {
        //MD5 16位元 加密為大寫
        public static string GetMd5Str(string ConvertString)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            string t2 = BitConverter.ToString(md5.ComputeHash(UTF8Encoding.Default.GetBytes(ConvertString)), 4, 8);
            t2 = t2.Replace("-", "");
            //t2 = t2.ToLower(); //加這一行就變小寫
            return t2;
        }
        //DES 加密
        public static string Encrypt(string pToEncrypt, string sKey)
        {
            using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
            {
                byte[] inputByteArray = Encoding.UTF8.GetBytes(pToEncrypt);
                des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
                des.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                using (CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(inputByteArray, 0, inputByteArray.Length);
                    cs.FlushFinalBlock();
                    cs.Close();
                }
                string str = Convert.ToBase64String(ms.ToArray());
                ms.Close();
                return str;
            }
        }

        static void Main(string[] args)
        {
            string _sKey="AAAAAAAA";
            string _sOut = "";
            Console.WriteLine("Input CPUID: ");
            string _sCPUID = Console.ReadLine();

            while (true)
            {
                Console.WriteLine("Input Key  (只能輸入英文字8碼，不能含有數字):");
                string _sPut = Console.ReadLine();
                //if (_sPut.Length < 0 || _sPut.Length > 9 ||_sPut == "") break;
                _sKey = _sPut.ToUpper();
                if (_sKey.Length == 8) break;
            }
            Console.WriteLine("-----------[OUT PUT Record]-------------");
            Console.WriteLine("CPU0 ID: {0} ", _sCPUID);
            //Console.WriteLine("DES PASSWORD: {0}", Encrypt(_sCPUID, _sKey));
            //Console.WriteLine("16 bit MD5 crypt: {0}", GetMd5Str(Encrypt(_sCPUID, _sKey)));
            _sOut = GetMd5Str(Encrypt(_sCPUID, _sKey));
            Console.WriteLine("License KEY: {0}-{1}-{2}-{3}", _sOut.Substring(0, 4), _sOut.Substring(4, 4), _sOut.Substring(8, 4), _sOut.Substring(12, 4));
            Console.ReadKey();
        }
    }
}
