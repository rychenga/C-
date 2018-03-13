//first include IO.dll & Threading.dll
using System;
using System.Collections.Generic;
using System.IO.Ports;

namespace demo1
{
    class Program
    {
        static void Main(string[] args)
        {
            //宣告bytes 陣列
            List<byte> _lInput = new List<byte>();
            //宣告PLC
            //_comPLC = new SerialPort("COM4", 115200, Parity.Even, 8, StopBits.Two);
            AGVController.AAGVModbus _comPLC = new AGVController.AAGVModbus(4, 115200, Parity.Even, 8, StopBits.Two);

            try
            {

                Console.WriteLine("Input Station:");
                int _iStation = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine("Input Function:");
                int _iFunction = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine("Input Address:");
                int _iAddress = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine("Input data(data count):");
                int _iData = Convert.ToInt32(Console.ReadLine());
                    
                _comPLC.Open();

                for (int i = 0; i < _iData; i++)
                {
                    //_comPLC.WriteRtuCrc(01,03,000000,01);
                    _comPLC.WriteRtuCrc(_iStation, _iFunction, _iAddress, _iData);
                    _iAddress ++;
                }

                Console.ReadKey();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.ReadKey();
            }

        }
    }
}
