using System;
using System.Collections.Generic;
using System.Threading;
using System.IO.Ports;

namespace demo3
{
    class Program
    {
        //宣告SerialPort to _comPLC
        private static SerialPort _comPLC;
        private static string _strLastData;
        private const string _cstrTrail = "\r\n";
        //========== check sum
        public static void GetCRC(out byte Lsb, out byte Hsb, params byte[] data)
        {
            int wResult = 0xFFFF;
            for (int nIdx = 0; nIdx < data.Length; nIdx++)
            {
                wResult ^= data[nIdx];
                for (int j = 0; j < 8; j++)
                {
                    if ((wResult & 0x01) > 0) wResult = (wResult >> 1) ^ 0xA001;
                    else wResult = wResult >> 1;
                }
            }
            Lsb = (byte)(wResult & 0xFF); //Lsb
            Hsb = (byte)(wResult >> 0x8); //Hsb
        }
        public static byte HSB(int Word) { return (byte)(Word >> 8); }
        public static byte LSB(int Word) { return (byte)(Word & 0xFF); }

        public static void Read()
        {
            _strLastData = string.Empty;
            //宣告bytes 陣列
            List<byte> _lInget = new List<byte>();
            Console.WriteLine("Read get count {0}", _comPLC.BytesToRead);
            try
            { 
                if (!_comPLC.IsOpen) return;

                while (_comPLC.BytesToRead > 0)
                {
                    char rec = (char)_comPLC.ReadChar();
                    _strLastData += rec;

                    int _iGet = Convert.ToInt32(rec);
                    byte _bGet = Convert.ToByte(_iGet);
                    _lInget.Add(_bGet);

                    if (_strLastData.EndsWith(_cstrTrail))
                    {
                        _strLastData = string.Empty;
                    }
                }
                if (_lInget.Count == 0) return;
                Console.WriteLine("OUT DATA : ");
                for (int i = 0; i < _lInget.Count; i++)
                {
                    Console.Write(_lInget[i]);
                    Console.Write(",");
                }
                byte byteCRCL, byteCRCH;
                GetCRC(out byteCRCL, out byteCRCH, _lInget.ToArray());
                Console.WriteLine("{0},{1}", Convert.ToString(byteCRCL, 16), Convert.ToString(byteCRCH, 16));
                //Console.WriteLine("out2: {0}", _strLastData);
                //Console.ReadKey();

            }
            catch (Exception e) { Console.WriteLine(e.ToString()); }
        }
        static void Main(string[] args)
        {
            //宣告bytes 陣列
            List<byte> _lInput = new List<byte>();
            //宣告COM1 (COM1,115200,偶數,8,Stopbite 2)
            _comPLC = new SerialPort("COM4", 115200, Parity.Even, 8, StopBits.Two);

            //new readThread
            Thread readThread = new Thread(Read);

            try
            {
                ////宣告COM1 (COM1,115200,偶數,8,Stopbite 2)
                //_comPLC = new SerialPort("COM4", 115200, Parity.Even, 8, StopBits.Two);

                while (true)
                {
                    Console.WriteLine("Input number (input -1 and complete):");

                    int _iPut = Convert.ToInt32(Console.ReadLine());

                    if (_iPut < 0 || _iPut > 255) break;

                    byte _bPut = Convert.ToByte(_iPut);

                    _lInput.Add(_bPut);
                }

                Console.WriteLine("Send and CRC DATA : ");
                for (int i = 0; i < _lInput.Count; i++)
                {
                    Console.Write(_lInput[i]);
                    Console.Write(",");
                }
                byte byteCRCL, byteCRCH;
                GetCRC(out byteCRCL, out byteCRCH, _lInput.ToArray());
                Console.WriteLine("{0},{1}", Convert.ToString(byteCRCL, 16), Convert.ToString(byteCRCH, 16));

                
                //input data to send
                byte[] data = _lInput.ToArray();

                // Set the read/write timeouts
                _comPLC.ReadTimeout = 500;
                _comPLC.WriteTimeout = 500;
                // open connect ports
                _comPLC.Open();

                _comPLC.Write(data, 0, _lInput.Count);
                Console.ReadKey();
                //_comPLC.WriteLine("test by rychenga");
                Console.WriteLine("Send: {0}", string.Join(",", data));
                Console.WriteLine("Send _lInput count {0}", _lInput.Count);
                Console.WriteLine("Send get count {0}", _comPLC.BytesToRead);
                //Read();
                readThread.Start();

                readThread.Join();
                _comPLC.Close();

                Console.ReadKey();

            }
            catch (Exception e)
            {
                Console.Write(e.ToString());
                Console.ReadKey();
            }
        }

    }
}
