using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Jeff.DB;

namespace demo2
{
    public partial class Form1 : Form
    {
        //宣告oracle dll
        private ODBC _sSqlcmd;

        public Form1()
        {
            InitializeComponent();
        }

        private void bImput_Click(object sender, EventArgs e)
        {
            try
            {                
                //new _sSqlcmd
                _sSqlcmd = new ODBC();
                //宣告sqlcmd
                //_sSqlcmd.Ssqlcmd = "select * from LEGAL t where t.day = to_date('2014/05/06','yyyy/mm/dd')";
                _sSqlcmd.Ssqlcmd = textBox1.Text;
                //宣告SsPath
                //_sSqlcmd.SsPath = "D:\\demo.ini";
                _sSqlcmd.SsPath = textBox2.Text;

                //執行oracle dll
                _sSqlcmd.ConnectDB();

                dgvTable.DataSource = _sSqlcmd.GetDataTable;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        //Add row number
        private void dgvTable_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            Rectangle rectangle = new Rectangle(e.RowBounds.Location.X, e.RowBounds.Location.Y, dgvTable.RowHeadersWidth - 4, e.RowBounds.Height);
            TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(),
                dgvTable.RowHeadersDefaultCellStyle.Font,
                rectangle,
                dgvTable.RowHeadersDefaultCellStyle.ForeColor,
                TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }
    }
}
