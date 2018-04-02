using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Jeff.DB;

namespace demo
{
    public partial class Form1 : Form
    {
        //宣告oracle dll
        private ODBC _sSqlcmd;

        public Form1()
        {
            InitializeComponent();
        }

        private void bQuery_Click(object sender, EventArgs e)
        {
            try
            {
                DdatTable.Columns.Clear();
                DdatTable.Rows.Clear();
                //DdatTable.Rows.SharedRow(1);

                _sSqlcmd = new ODBC();
                //宣告sqlcmd
                //_sSqlcmd.Ssqlcmd = "select * from LEGAL t where t.day = to_date('2014/05/06','yyyy/mm/dd')";
                _sSqlcmd.Ssqlcmd = textBox1.Text;
                //宣告SsPath
                //_sSqlcmd.SsPath = "D:\\demo.ini";
                _sSqlcmd.SsPath = textBox2.Text;

                //執行oracle dll
                _sSqlcmd.ConnectDB();

                DdatTable.ColumnCount = _sSqlcmd.columns.Count;
                for (int i = 0; i < _sSqlcmd.columns.Count; i++)
                {
                    DdatTable.Columns[i].Name = _sSqlcmd.columns[i];
                }

                //DdatTable.Columns

                for (int i = 0; i < _sSqlcmd.rows.Count; i++)
                {
                    DdatTable.Rows.Add(_sSqlcmd.rows[i].ToArray());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        //Add row number
        private void DdatTable_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            Rectangle rectangle = new Rectangle(e.RowBounds.Location.X, e.RowBounds.Location.Y, DdatTable.RowHeadersWidth - 4, e.RowBounds.Height);
            TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(),
                DdatTable.RowHeadersDefaultCellStyle.Font,
                rectangle,
                DdatTable.RowHeadersDefaultCellStyle.ForeColor,
                TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }
       

    }
}
