using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;

namespace TestProject
{
    public partial class frmMain : Form
    {
        //===== variable
        private SerialPort _comPLC;

        //===== constructor
        public frmMain()
        {
            InitializeComponent();
            _comPLC = new SerialPort("COM1", 115200, Parity.Even, 8, StopBits.None);

        }
        //===== member function
        public void PLCInitial(int nStationNo)
        {
            Send(0x00, 0x01);
            byte byteData;
            List<byte> lstReciveDatas = new List<byte>();

            while (true)
            {
                byteData = (byte)_comPLC.ReadByte();
                lstReciveDatas.Add(byteData);
                
            }
        }
       
        public void Open() //step 1. 開啟通訊
        {
            _comPLC.Open();
        }
        public void Close()
        {
            _comPLC.Close();
        }
        public void Send(params byte[] datas)
        {
            _comPLC.Write(datas, 0, datas.Length);
        }
        public string Display(params byte[] datas)
        {
            string strResult = ""; 
            for (int nIdx = 0; nIdx < datas.Length; nIdx++)
            {
                strResult += datas[nIdx].ToString() + ",";
            }
            return strResult;
        }

        private void frmMain_Load(object sender, EventArgs e)
        {

        }
    }
}
