using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using Scientech.Threading;

namespace Scientech.IO.Ports
{
    public class ModBusEventArgs : EventArgs
    {
        public int Station { get; set; }
        public int Function { get; set; }
        public int Address { get; set; }
        public int Length { get; set; }
        public int[] Context { get; set; }
        public ModBusEventArgs(string strMessage, string strHead, string strTrail)
        {
            if (!strMessage.StartsWith(strHead))
                throw new Exception(string.Format("Modbus Head Code Error. Message = [{0}]", strMessage));
            if (!strMessage.EndsWith(strTrail))
                throw new Exception(string.Format("Modbus Trail Code Error. Message = [{0}]", strMessage));
            string strFrame = strMessage.Trim(':', '\n', '\r');

            Station = Convert.ToInt32(strFrame.Substring(0, 2), 16); 
            strFrame = strFrame.Remove(0, 2);

            Function = Convert.ToInt32(strFrame.Substring(0, 2), 16); 
            strFrame = strFrame.Remove(0, 2);

            if (Function == 3)
            {
                Length = Convert.ToInt32(strFrame.Substring(0, 2), 16);
                strFrame = strFrame.Remove(0, 2);
                Address = -1;
            }
            else if (Function == 6)
            {
                Length = 2;
                Address = Convert.ToInt32(strFrame.Substring(0, 4), 16);
                strFrame = strFrame.Remove(0, 4);
            }
            else
            {
                throw new Exception(string.Format("Modbus anaylsis frame error. dont support function code [{0}], Message = [{1}]", Function, strMessage));
            }

            Context = new int[Length / 2]; // 1 interger = 2 bytes
            for (int idx = 0; idx < Context.Length; idx++)
            {
                Context[idx] = Convert.ToInt32(strFrame.Substring(0, 4), 16);
                strFrame = strFrame.Remove(0, 4);
            }
        }
    }
    public delegate void ModBusEventHandler(object sender, ModBusEventArgs e);

    public class SModbus : SerialPort
    {
        //========== variable
        private const string _cstrHead = ":";
        private const string _cstrTrail = "\r\n";
        private const char _ccSTX = (char)2;
        private const char _ccETX = (char)3;

        private SLogger _logger = SLogger.GetLogger("ModBus");
        private SPollingThread _pollingRead;
        private string _strLastData;
        //========== event
        public event ModBusEventHandler OnModBusReceived;
        //========== constructor
        public SModbus(int nPortNo, int baudRate, Parity parity, int dataBits, StopBits stopBits)
            : base(string.Format("COM{0}", nPortNo), baudRate, parity, dataBits, stopBits)
        {
            _strLastData = string.Empty;
            _pollingRead = new SPollingThread(10);
            _pollingRead.DoPolling += new dlgv_v(RunReadBus);
            _pollingRead.Set();
        }
        //========== event handler
        void RunReadBus()
        {
            try
            {
                if (!this.IsOpen) return;

                while (this.BytesToRead > 0)
                {
                    char c = (char)this.ReadChar();
                    _strLastData += c;
                    if (_strLastData.EndsWith(_cstrTrail))
                    {
                        if (OnModBusReceived != null)
                            OnModBusReceived(this, new ModBusEventArgs(_strLastData, _cstrHead, _cstrTrail));
                        _strLastData = string.Empty;
                    }
                
                }
            }
            catch (Exception ex)
            {
                _logger.WriteLog(ex);
            }
        }
        //========== member function
        public void WriteAsciiLrc(int station, int function, int address, int data)
        {
            try
            {
                byte lrc = GetLRC(
                    (byte)station,
                    (byte)function,
                    HSB(address),
                    LSB(address),
                    HSB(data),
                    LSB(data));
                string strSend = string.Join(string.Empty,
                    _cstrHead,
                    station.ToString("X2"),
                    function.ToString("X2"),
                    address.ToString("X4"),
                    data.ToString("X4"),
                    lrc.ToString("X2"),
                    _cstrTrail);
                //this.WriteLine(strSend);
                this.Write(strSend);
            }
            catch (Exception ex)
            {
                _logger.WriteLog(ex);
            }
        }
        public void WriteAsciiCrc(int station, int function, int address, int data)
        {
            byte byteCRCL, byteCRCH;
            GetCRC(out byteCRCL, out byteCRCH, 
                (byte)station,
                (byte)function,
                HSB(address),
                LSB(address),
                HSB(data),
                LSB(data));
            Send(
                (byte)station,
                (byte)function,
                HSB(address),
                LSB(address),
                HSB(data),
                LSB(data),
                byteCRCL,
                byteCRCH
                );
            //string strSend = string.Join(string.Empty,
            //    _cstrHead,
            //    station.ToString("X2"),
            //    function.ToString("X2"),
            //    address.ToString("X4"),
            //    data.ToString("X4"),
            //    byteCRCH.ToString("X2"),
            //    byteCRCL.ToString("X2"),
            //    _cstrTrail);
            ////this.WriteLine(strSend);
            //this.Write(strSend);
        }
        public void Send(params byte[] data)
        {
            this.Write(data, 0, data.Count());
            Console.WriteLine("Send: {0}", string.Join(",", data));
        }
        //========== check sum
        private byte GetLRC(params byte[] datas)
        {
            byte nSum = 0;
            foreach (byte data in datas)
                nSum += data;
            nSum =(byte) ~nSum;
            nSum++;
            nSum &= 0xFF;
            return nSum;
        }
        public void GetCRC(out byte Lsb, out byte Hsb, params byte[] data)
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
    }



}
