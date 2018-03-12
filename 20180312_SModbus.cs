using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Threading;
using Allen.IO;
using Allen.Threading;

namespace AGVController
{
    /// <summary>
    /// Modbus回覆封包解析事件參數類別 
    /// Design by Allen 2015.10.30
    /// </summary>
    public class ModBusEventArgs : EventArgs
    {
        /// <summary>
        /// Modbus站號
        /// </summary>
        public int Station { get; set; }
        /// <summary>
        /// Modbus功能碼
        /// </summary>
        public int Function { get; set; }
        /// <summary>
        /// Modbus暫存器位址
        /// </summary>
        public int Address { get; set; }
        /// <summary>
        /// Modbus資料長度 (byte)
        /// </summary>
        public int Length { get; set; }
        /// <summary>
        /// Modbus暫存器資料文本
        /// </summary>
        public int[] Context { get; set; }
        /// <summary>
        /// 建構Modbus回覆封包解析物件
        /// </summary>
        /// <param name="frames">完整RTU封包資料陣列</param>
        public ModBusEventArgs(params byte[] frames)
        {
            Station = frames[0];
            Function = frames[1];
            Length = frames[2];
            List<int> lstContext = new List<int>();
            for (int idx = 0; idx <= Length - 1; idx += 2)
                lstContext.Add((frames[3 + idx] << 8) + frames[4 + idx]);
            Context = lstContext.ToArray();
        }
    }
    /// <summary>
    /// Modbus封包處理事件委派
    /// </summary>
    /// <param name="sender">modbus obj</param>
    /// <param name="e">event args obj</param>
    public delegate void ModBusEventHandler(object sender, ModBusEventArgs e);

    /// <summary>
    /// Modbus Rtu 串列埠通訊類別
    /// Design by Allen 2016.10.30
    /// </summary>
    public class AAGVModbus : SerialPort
    {
        //========== variable
        private ALogger _logger = ALogger.GetLogger("ModBus");
        private APolling _pollingRead;
        private string _strLastData;
        private List<byte> _lstLastData;

        //========== event
        /// <summary>
        /// Modbus封包接收事件
        /// </summary>
        public event ModBusEventHandler OnModBusReceived;
        //========== constructor
        /// <summary>
        /// 建構Modbus Rtu 串列埠通訊類別
        /// </summary>
        /// <param name="nPortNo">COM port number</param>
        /// <param name="baudRate">Baudrate</param>
        /// <param name="parity">Parity</param>
        /// <param name="dataBits">Data length</param>
        /// <param name="stopBits">Stop bits</param>
        public AAGVModbus(int nPortNo, int baudRate, Parity parity, int dataBits, StopBits stopBits)
            : base(string.Format("COM{0}", nPortNo), baudRate, parity, dataBits, stopBits)
        {
            //this.Encoding = Encoding.Unicode;
            _lstLastData = new List<byte>();
            _strLastData = string.Empty;
            _pollingRead = new APolling(10);
            _pollingRead.DoPolling += new Allen.Threading.dlgv_v(RunReadBus);
            _pollingRead.Set();
        }
        //========== event handler
        void RunReadBus()
        {
            try
            {
                if (!this.IsOpen) return;

                bool bDebugFlag = false;
                while (this.BytesToRead > 0)
                {
                    bDebugFlag = true;
                    int nByteData = this.ReadByte();
                    _lstLastData.Add((byte)nByteData);

                    if (CheckCRC(_lstLastData.ToArray()))
                    {
                        string astr = "";
                        for (int idx = 0; idx < _lstLastData.Count; idx++)
                        {
                            astr += _lstLastData[idx].ToString("X2") + ",";
                        }
                        Console.WriteLine("Recevie: {0}", astr);

                        if ((OnModBusReceived != null))
                            //&& (_lstLastData[1] == 0x03)) //event be used and frame is read register
                            OnModBusReceived(this, new ModBusEventArgs(_lstLastData.ToArray()));
                        _lstLastData.Clear(); //clear receive buffer when crc is correct. 
                    }
                }

                if (!bDebugFlag) return;
                if (_lstLastData.Count <= 0) return;

                //string astr = "";
                //for (int idx = 0; idx < _lstLastData.Count; idx++)
                //{
                //    astr += _lstLastData[idx].ToString("X2") + ",";
                //}
                //Console.WriteLine("Recevie: {0}", astr);
            }
            catch (Exception ex)
            {
                _logger.WriteLog(ex);
            }
        }
        //========== member function
        /// <summary>
        /// send modbus message by RTU
        /// </summary>
        /// <param name="station">modbus station</param>
        /// <param name="function">modbus function code</param>
        /// <param name="address">modbus register address</param>
        /// <param name="data">data context</param>
        public void WriteRtuCrc(int station, int function, int address, int data)
        {
            //Calculate CRC
            int crc = GetCRC(
                (byte)station,
                (byte)function,
                HSB(address),
                LSB(address),
                HSB(data),
                LSB(data));
            //Send message by RTU (byte array)
            Send(
                (byte)station,
                (byte)function,
                HSB(address),
                LSB(address),
                HSB(data),
                LSB(data),
                LSB(crc),
                HSB(crc)
                );
        }
        /// <summary>
        /// send modbus message by RTU
        /// </summary>
        /// <param name="station">modbus station</param>
        /// <param name="function">modbus function code</param>
        /// <param name="address">modbus device register address</param>
        /// <param name="args">modbus data array</param>
        public void WriteRtuCrc(int station, int function, int address, params int[] args)
        {
            //Calculate CRC
            List<byte> lstByte = new List<byte>();
            lstByte.Add((byte)station);
            lstByte.Add((byte)function);

            lstByte.Add(HSB(address));
            lstByte.Add(LSB(address));
            lstByte.Add(HSB(args.Length));
            lstByte.Add(LSB(args.Length));

            lstByte.Add((byte)(args.Length * 2));
            foreach (int data in args)
            {
                lstByte.Add(HSB(data));
                lstByte.Add(LSB(data));
            }
            int crc = GetCRC(lstByte.ToArray());
            lstByte.Add(LSB(crc));
            lstByte.Add(HSB(crc));
            Send(lstByte.ToArray());

        }
        /// <summary>
        /// send modbus message
        /// </summary>
        /// <param name="data">byte array</param>
        public void Send(params byte[] data)
        {
            _lstLastData.Clear();
            this.Write(data, 0, data.Count());
            string[] astr = new string[data.Length];
            for (int idx = 0; idx < data.Length; idx++)
                astr[idx] = string.Format("{0:X2}", data[idx]);

            Console.WriteLine("Send: {0}", string.Join(",", astr));
        }
        /// <summary>
        /// check the crc of received message 
        /// </summary>
        /// <param name="datas">received message (RTU)</param>
        /// <returns>pass</returns>
        private bool CheckCRC(params byte[] datas)
        {
            try
            {
                if (datas.Length < 3) return false; //data with check sum should be more than 3 bytes.

                //get receive data context
                byte[] context = new byte[datas.Length - 2];
                for (int idx = 0; idx < context.Length; idx++)
                    context[idx] = datas[idx];

                //check the CRC
                byte byteHSB;
                byte byteLSB;
                GetCRC(out byteLSB, out byteHSB, context);
                if (byteLSB != datas[datas.Length - 2]) return false; //crc Hi byte error
                if (byteHSB != datas[datas.Length - 1]) return false; //crc Lo byte error
                return true;
            }
            catch (Exception ex)
            {
                _logger.WriteLog(ex);
            }
            return false;
        }
        //========== check sum
        /// <summary>
        /// calculate CRC (Cyclic redundancy check)
        /// </summary>
        /// <param name="data">context</param>
        /// <returns>CRC result</returns>
        public int GetCRC(params byte[] data)
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
            return wResult;
        }
        /// <summary>
        /// calculate CRC (Cyclic redundancy check)
        /// </summary>
        /// <param name="Lsb">LSB of CRC</param>
        /// <param name="Hsb">HSB of CRC</param>
        /// <param name="data">context</param>
        public void GetCRC(out byte Lsb, out byte Hsb, params byte[] data)
        {
            int wResult = GetCRC(data);
            Lsb = (byte)(wResult & 0xFF); //Lsb
            Hsb = (byte)(wResult >> 0x8); //Hsb
        }
        //========== static member
        /// <summary>
        /// Get HSB
        /// </summary>
        /// <param name="Word">word (two byte)</param>
        /// <returns>HSB</returns>
        public static byte HSB(int Word) { return (byte)(Word >> 8); }
        /// <summary>
        /// Get LSB
        /// </summary>
        /// <param name="Word">word (two byte)</param>
        /// <returns>LSB</returns>
        public static byte LSB(int Word) { return (byte)(Word & 0xFF); }
    }
}
