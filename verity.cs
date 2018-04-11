using System;
using System.Collections.Generic;
using System.Text;
using System.Management; //參考要另外加入
using System.Security.Cryptography;
using System.IO;

namespace verity
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
            string _sLicense ="";
            string _sKey = "";
            string _sRecord = "";

            while (_sLicense == "")
            {
                Console.WriteLine("Input License key:");
                _sLicense = Console.ReadLine().Replace("-","");
            }
            while (true)
            {
                Console.WriteLine("Input Key  (只能輸入英文字8碼，不能含有數字):");
                string _sPut = Console.ReadLine();
                _sKey = _sPut.ToUpper();
                if (_sKey.Length == 8) break;
            }

            ManagementObjectSearcher wmiSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");
            int i = 0;
            string original = "";
            // 使用 ManagementObjectSearcher 的 Get 方法取得所有集合

            foreach (ManagementObject obj in wmiSearcher.Get())
            {

                // 取得CPU 序號
                original = obj["ProcessorId"].ToString();
                Console.WriteLine("CPU {0} ID:\t{1}", i++, obj["ProcessorId"].ToString());
                //Console.WriteLine("DES Password: {0}", Encrypt(original, _sKey));
                _sRecord = GetMd5Str(Encrypt(original, _sKey));
                Console.WriteLine("Verity license key: {0}-{1}-{2}-{3}", _sRecord.Substring(0, 4), _sRecord.Substring(4, 4), _sRecord.Substring(8, 4), _sRecord.Substring(12, 4));
                //Console.WriteLine("16 bit MD5 crypt: {0}", _sRecord);
            }

            if (_sLicense == _sRecord)
            {
                Console.WriteLine("Lincese Verity Successy !!!!");
            }
            else
            {
                Console.WriteLine("Lincese Verity Fail !!!!");
            }
            Console.ReadKey();
        }
    }
}
