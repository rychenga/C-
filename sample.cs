using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;
using System.IO;

namespace ERP
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                //清空DataGrivew row data(DataSource = null)
                dgvTable.DataSource = null;

                //string sql = "select * from location" + cbox_SheetName.Text;
                string sql = "select * from " + cbox_SheetName.Text;
                //執行Access connect
                //DataTable dt = GetOleDbDataTable("new.mdb", sql);
                //dgvTable.DataSource = dt;
                if (txt_Path.Text != "")
                {
                    DataTable dt = GetOleDbDataTable(txt_Path.Text, sql);
                    dgvTable.DataSource = dt;
                    dt = null;
                }
                	
                	
     

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        //連線Access MDB
        public static OleDbConnection OleDbOpenConn(string Database)
        {
            string cnstr = string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + Database);
            OleDbConnection icn = new OleDbConnection();
            icn.ConnectionString = cnstr;
            if (icn.State == ConnectionState.Open) icn.Close();
            icn.Open();
            return icn;
        }
        //Query Access MDB
        public static DataTable GetOleDbDataTable(string Database, string OleDbString)
        {
            DataTable myDataTable = new DataTable();
            OleDbConnection icn = OleDbOpenConn(Database);
            OleDbDataAdapter da = new OleDbDataAdapter(OleDbString, icn);
            DataSet ds = new DataSet();
            ds.Clear();
            da.Fill(ds);
            myDataTable = ds.Tables[0];
            if (icn.State == ConnectionState.Open) icn.Close();
            return myDataTable;
        }

        //Add row number
        private void dgvTable_RowPostPaint_1(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            Rectangle rectangle = new Rectangle(e.RowBounds.Location.X, e.RowBounds.Location.Y, dgvTable.RowHeadersWidth - 4, e.RowBounds.Height);
            TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(),
                dgvTable.RowHeadersDefaultCellStyle.Font,
                rectangle,
                dgvTable.RowHeadersDefaultCellStyle.ForeColor,
                TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }

        private void btn_Select_Click(object sender, EventArgs e)
        {
            //連接Excel
            //openFileDialog1.Filter = "Excel文件|*.xls";//設定打開文件篩選器
            //openFileDialog1.Title = "選擇Excel文件";//設定打開對話框標題
            
            //連接Access
            openFileDialog1.Filter = "Access文件|*.mdb";//設定打開文件篩選器
            openFileDialog1.Title = "選擇Access文件";//設定打開對話框標題
            
            openFileDialog1.Multiselect = false;//設定打開對話框中只能單選
            if (openFileDialog1.ShowDialog() == DialogResult.OK)//判斷是否選擇了文件
            {
                txt_Path.Text = openFileDialog1.FileName;//在文字框中顯示Excel文件名
                CBoxBind();//對下拉列表進行資料繫結
            }
        }

        private void CBoxBind()//對下拉列表進行資料繫結
        {
            cbox_SheetName.Items.Clear();//清空下拉列表項
            //連接Excel資料庫
            //OleDbConnection olecon = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + txt_Path.Text + ";Extended Properties=Excel 8.0");
            //連接Access資料庫
            OleDbConnection olecon = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + txt_Path.Text);
            olecon.Open();//打開資料庫連接
            //連接Excel資料庫
            //System.Data.DataTable DTable = olecon.GetSchema("Tables");//實例化表對像
            //連接Access資料庫
            System.Data.DataTable DTable = olecon.GetOleDbSchemaTable(OleDbSchemaGuid.Tables,new object[] { null, null, null, "TABLE" });//實例化表對像
            
            DataTableReader DTReader = new DataTableReader(DTable);//實例化表讀取對像
            while (DTReader.Read())//循環讀取
            {
                string P_str_Name = DTReader["Table_Name"].ToString().Replace('$', ' ').Trim();//記錄工作表名稱
                if (!cbox_SheetName.Items.Contains(P_str_Name))//判斷下拉列表中是否已經存在該工作表名稱
                    cbox_SheetName.Items.Add(P_str_Name);//將工作表名新增到下拉列表中
            }
            DTable = null;//清空表對像
            DTReader = null;//清空表讀取對像
            olecon.Close();//關閉資料庫連接
            cbox_SheetName.SelectedIndex = 0;//設定下拉列表預設選項為第一項
        }

        private void button1_Click_1(object sender, EventArgs e)//選取datagrview data
        {
            for (int i = 0; i < dgvTable.SelectedCells.Count; i++)
            {
                MessageBox.Show(dgvTable.SelectedCells[i].Value.ToString());
            }
        }
    }
}
