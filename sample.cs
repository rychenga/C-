using System;
using System.Collections.Generic;
using System.Text;
using System.Management; //參考要另外加入System.Management;
using System.Security.Cryptography;
using System.IO;

namespace demo
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
        //MD5 32位元 加密為大寫
        public static string UserMd5(string str)
        {
            string cl = str;
            string pwd = "";
            MD5 md5 = MD5.Create();//實例化一個md5對像
            // 加密後是一個字節類型的數組，這裡要注意編碼UTF8/Unicode等的選擇
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(cl));
            // 通過使用循環，將字節類型的數組轉換為字符串，此字符串是常規字符格式化所得
            for (int i = 0; i < s.Length; i++)
            {
                // 將得到的字符串使用十六進制類型格式。格式後的字符是小寫的字母，如果使用大寫（X）則格式後的字符是大寫字符 
                pwd = pwd + s[i].ToString("X");
            }
            return pwd;
        }
        // DES EncryptStringToBytes(TripleDESCryptoServiceProvider)
        static byte[] EncryptStringToBytes(string plainText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("Key");
            byte[] encrypted;
            // Create an TripleDESCryptoServiceProvider object
            // with the specified key and IV.
            using (TripleDESCryptoServiceProvider tdsAlg = new TripleDESCryptoServiceProvider())
            {
                tdsAlg.Key = Key;
                tdsAlg.IV = IV;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform encryptor = tdsAlg.CreateEncryptor(tdsAlg.Key, tdsAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {

                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }


            // Return the encrypted bytes from the memory stream.
            return encrypted;

        }

        //DES DecryptStringFromBytes(TripleDESCryptoServiceProvider)
        static string DecryptStringFromBytes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("Key");

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an TripleDESCryptoServiceProvider object
            // with the specified key and IV.
            using (TripleDESCryptoServiceProvider tdsAlg = new TripleDESCryptoServiceProvider())
            {
                tdsAlg.Key = Key;
                tdsAlg.IV = IV;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptor = tdsAlg.CreateDecryptor(tdsAlg.Key, tdsAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }

            }

            return plaintext;

        }

        //DES EncryptString (DESCryptoServiceProvider)
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

        //DES DecryptString(DESCryptoServiceProvider)
        public static string Decrypt(string pToDecrypt, string sKey)
        {
            byte[] inputByteArray = Convert.FromBase64String(pToDecrypt);
            using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
            {
                des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
                des.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                using (CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(inputByteArray, 0, inputByteArray.Length);
                    cs.FlushFinalBlock();
                    cs.Close();
                }
                string str = Encoding.UTF8.GetString(ms.ToArray());
                ms.Close();
                return str;
            }
        }        


        static void Main(string[] args)
        {
            ManagementObjectSearcher wmiSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");
            int i = 0;
            string original="";
            // 使用 ManagementObjectSearcher 的 Get 方法取得所有集合

            foreach (ManagementObject obj in wmiSearcher.Get())
            {

                // 取得CPU 序號
                original = obj["ProcessorId"].ToString();
                Console.WriteLine("CPU {0} ID:\t{1}", i++, obj["ProcessorId"].ToString());
                Console.WriteLine("32 bit MD5 crypt: {0}", UserMd5(obj["ProcessorId"].ToString()));
                Console.WriteLine("16 bit MD5 crypt: {0}", GetMd5Str(obj["ProcessorId"].ToString()));
            }

            using (TripleDESCryptoServiceProvider myTripleDES = new TripleDESCryptoServiceProvider())
            {
                // Encrypt the string to an array of bytes.
                byte[] encrypted = EncryptStringToBytes(original, myTripleDES.Key, myTripleDES.IV);

                // Decrypt the bytes to a string.
                string roundtrip = DecryptStringFromBytes(encrypted, myTripleDES.Key, myTripleDES.IV);

                //Display the original data and the decrypted data.
                Console.WriteLine("Original:   {0}", original);
                Console.WriteLine("DES PASSWORD  :   {0}", Encoding.UTF8.GetString(encrypted,0,encrypted.Length));
                Console.WriteLine("DES PASSWORD2 :  {0}", Convert.ToBase64String(encrypted, 0, encrypted.Length));
                Console.WriteLine("Round Trip: {0}", roundtrip);
                Console.WriteLine("16 bit MD5 crypt: {0}", GetMd5Str(Convert.ToBase64String(encrypted, 0, encrypted.Length)));
            }

            Console.WriteLine("方法2加密: {0}", Encrypt(original,"AAAAAAAA"));
            Console.WriteLine("方法2解密: {0}", Decrypt(Encrypt(original, "AAAAAAAA"), "AAAAAAAA"));
            Console.WriteLine("16 bit MD5 crypt: {0}", GetMd5Str(Encrypt(original, "AAAAAAAA")));
            //// 或透過 ManagementObject 類別直接存取特定 CPU 序號

            ////ManagementObject wmiObj = new ManagementObject("Win32_Processor.DeviceID='CPU0'");

            ////Console.WriteLine("CPU{0} ID:\t{1}", 0, wmiObj.GetPropertyValue("ProcessorId").ToString());
            //Console.WriteLine("***** Get CPU ID complete *****");

            //string normalTxt = "ABCDEFGH999999G1";
            //////var bytes = Encoding.Default.GetBytes(normalTxt);//求Byte[]数组  
            ////var bytes = Encoding.UTF8.GetBytes(normalTxt);
            ////var Md5 = new MD5CryptoServiceProvider().ComputeHash(bytes);//求哈希值
            ////Console.WriteLine("password :\t{0}", Convert.ToBase64String(Md5));
            //Console.WriteLine("32 bit MD5 crypt: {0}",UserMd5(normalTxt));
            //Console.WriteLine("16 bit MD5 crypt: {0}",GetMd5Str(normalTxt));

            //Console.WriteLine("***** Get password complete *****");
            Console.ReadKey();

        }
    }
}
