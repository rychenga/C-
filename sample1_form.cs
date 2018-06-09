using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Data;
using System.Drawing;
using System.Windows.Forms.DataVisualization.Charting;
using Jeff.DB;
using System.Data.OleDb;

namespace sample
{
    public partial class Form1 : Form
    {
        //宣告Jeff.DB dll
        private MDB _smdb;

        //宣告Jeff.ODBC dll
        private ODBC _odbc;

        //DataGridView的BindingSource資料結構複製到我們準備的DataTable
        private BindingSource bds = new BindingSource();

        public Form1()
        {
            InitializeComponent();
            //將AllowNew設為true, 允許可新增資料 
            bds.AllowNew = true;
            //將AllowEdit設為true, 允許可編輯資料 
            //bds.AllowEdit = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //DataGridView的BindingSource資料結構複製到我們準備的DataTable
            dgvTable.DataSource = bds;

            //DataGridView選中儲存格時整個行背景變色
            //設定如何選擇儲存格
            dgvTable.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            //選中儲存格的前景色
            dgvTable.DefaultCellStyle.SelectionForeColor = Color.Blue;
            //選中儲存格的背景色
            dgvTable.DefaultCellStyle.SelectionBackColor = Color.LightBlue;


            //根據桌面大小調整視窗大小
            int DeskWidth = Screen.PrimaryScreen.WorkingArea.Width;//取得桌面寬度
            int DeskHeight = Screen.PrimaryScreen.WorkingArea.Height;//取得桌面高度
            this.Width = Convert.ToInt32(DeskWidth * 0.8);//設定視窗寬度
            this.Height = Convert.ToInt32(DeskHeight * 0.8);//設定視窗高度
        }


        //ODBC Get list
        private void bOlist_Click(object sender, EventArgs e)
        {
            //input User/Pwd@DB
            string _sDB = "sample";
            string _sUser = "rychenga";
            string _sPwd = "rychenga";

            //宣告connectString 
            string _sConnectString = "DSN=" + _sDB + ";UID=" + _sUser + ";PWD=" + _sPwd + ";";

            _odbc = new ODBC();

            try
            {
                cbox_SheetName.Items.Clear();//清空下拉列表項
                List<string> tablelist = new List<string>(); // table list 
                tablelist = _odbc.GetTableList(_sConnectString); //取得MDB FILE中所有TABLE LIST

                for (int i = 0; i < tablelist.Count; i++)
                {
                    cbox_SheetName.Items.Add(tablelist[i]); //加入COMBO BOX中
                }
                cbox_SheetName.SelectedIndex = 0;//設定下拉列表預設選項為第一項

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        // ODBC Select Query
        private void bOQry_Click(object sender, EventArgs e)
        {
            _odbc = new ODBC();//宣告Jeff.DB.ODBC

            //input User/Pwd@DB
            string _sDB = "sample";
            string _sUser = "rychenga";
            string _sPwd = "rychenga";

            //宣告connectString 
            string _sConnectString = "DSN=" + _sDB + ";UID=" + _sUser + ";PWD=" + _sPwd + ";";

            //宣告sqlcmd
            string _sSqlcmd = "SELECT * from " + cbox_SheetName.Text;

            //DataTable Getdata = new DataTable();//取得ODBC 回來的資料          

            try
            {
                //清空DataGrivew row data(DataSource = null)
                //dgvTable.DataSource = null;
                //bds = null;//初始化BindingSource
                DataTable Getdata = _odbc.GetOdbcDataTable(_sConnectString, _sSqlcmd);//取得ODBC 回來的資料      
                //dgvTable.DataSource = Getdata;
                bds.DataSource = Getdata;

                Getdata.Dispose();//釋放Getdata (DataTable)

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        //ODBC Sync(Insert/Update/delete) to ACCESS(DB)
        private void bOUpd_Click(object sender, EventArgs e)
        {
            DataTable dtAl = new DataTable();

            //input User/Pwd@DB
            string _sDB = "sample";
            string _sUser = "rychenga";
            string _sPwd = "rychenga";

            //宣告connectString 
            string _sConnectString = "DSN=" + _sDB + ";UID=" + _sUser + ";PWD=" + _sPwd + ";";

            //強轉DataGridView 轉成 DataTable 並複製資料結構
            //dtAl = ((DataTable)dgvTable.DataSource).Clone();//所有筆
            dtAl = ((DataTable)bds.DataSource).Clone();//所有筆

            //DataGridView data to DataTable
            //dtAl = (DataTable)(dgvTable.DataSource);
            dtAl = (DataTable)(bds.DataSource);
            //dtAl.AcceptChanges(); //AcceptChanges這道會把那Flag都清掉，因此對Adapter來說 那個DatatTable就視為沒變動過

            try
            {
                //_smdb.OleDbSyncData(txt_Path.Text, cbox_SheetName.Text, dtAl);
                _odbc.ODBCSyncData(_sConnectString,cbox_SheetName.Text,dtAl);
                dtAl.Dispose(); //釋放dtAl (DataTable)
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        //(OLE) Select MDB File buttion
        private void bSelect_Click(object sender, EventArgs e)
        {
            try
            {
                cbox_SheetName.Items.Clear();//清空下拉列表項
                List<string> tablelist = new List<string>(); // table list 

                //連接 access file
                openFileDialog1.Filter = "Access文件|*.mdb"; //設定打開文件篩選器
                openFileDialog1.Title = "選擇Access文件(限定*.mdb)";//設定打開對話框標題
                openFileDialog1.Multiselect = false; //設定打開對話框中只能單擇

                if (openFileDialog1.ShowDialog() == DialogResult.OK)//判斷是否選擇了文件
                {
                    txt_Path.Text = openFileDialog1.FileName;//文字框中顯示MDB文件名

                    _smdb = new MDB();
                    tablelist = _smdb.GetTableList(txt_Path.Text); //取得MDB FILE中所有TABLE LIST

                    for (int i = 0; i < tablelist.Count; i++)
                    {
                        cbox_SheetName.Items.Add(tablelist[i]); //加入COMBO BOX中
                    }
                    cbox_SheetName.SelectedIndex = 0;//設定下拉列表預設選項為第一項
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        //(OLE) Query buttion(Data) from Access DB
        private void bQuery_Click(object sender, EventArgs e)
        {
            try
            {
                _smdb = new MDB();//宣告Jeff.DB.MDB

                this.chart1.Series.Clear();//清除 CHART 值
                this.chart1.ChartAreas[0].RecalculateAxesScale();//重新計算的圖表區域屬性


                Series series1 = new Series("total count"); //初始totail 名稱
                series1.Color = Color.Blue; //設定線條顏色
                series1.Font = new System.Drawing.Font("新細明體", 10); //設定字型
                series1.ChartType = SeriesChartType.Column; //設定線條種類
                series1.MarkerSize = 1; //顯示值的點大小
                series1.MarkerStyle = MarkerStyle.Circle; //顯示值的點種類

                //清空DataGrivew row data(DataSource = null)
                //dgvTable.DataSource = null;
                //bds.DataSource = null;

                //SQL1
                string sql = "select * from " + cbox_SheetName.Text;
                //SQL2
                string sql2 = "select count(*) as cnt from " + cbox_SheetName.Text;

                if (txt_Path.Text != "")
                {
                    //DataGridView 顯示 row data
                    DataTable dt = _smdb.GetOleDbDataTable(txt_Path.Text, sql);
                    //dgvTable.DataSource = dt;
                    bds.DataSource = dt;
                    //dt = null;
                    dt.Dispose();//釋放dt (DataTable)

                    //Chart 顯示 統計結果
                    DataTable dt2 = _smdb.GetOleDbDataTable(txt_Path.Text, sql2);
                    series1.Points.AddXY(dt2.Columns[0].ToString(), dt2.Rows[0].ItemArray[0]);
                    this.chart1.Series.Add(series1);//將線畫在圖上
                    //dt2 = null;
                    dt2.Dispose();//釋放dt2 (DataTable)
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }


        //delete DataGridView Rows
        private void bDel_Click(object sender, EventArgs e)
        {
            ////方法一
            //foreach (DataGridViewRow item in this.dgvTable.SelectedRows)
            //{
            //    dgvTable.Rows.RemoveAt(item.Index);
            //}

            //方法二
            foreach (DataGridViewCell oneCell in dgvTable.SelectedCells)
            {
                if (oneCell.Selected)
                    dgvTable.Rows.RemoveAt(oneCell.RowIndex);
            }
        }


        //Copy Select DataGridView Rows
        private void bCopy_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable get = new DataTable();
                DataTable dtAl = new DataTable();

                //強轉DataGridView 轉成 DataTable 並複製資料結構
                get = ((DataTable)bds.DataSource).Clone();
                dtAl = ((DataTable)bds.DataSource).Clone();

                ////DataGridView data to DataTable
                dtAl = (DataTable)(bds.DataSource);

                //取得已選取的行列資料
                foreach (DataGridViewRow row in dgvTable.SelectedRows)
                {
                    get.ImportRow(((DataTable)bds.DataSource).Rows[row.Index]);
                }
                get.AcceptChanges();//AcceptChanges這道會把那Flag都清掉，因此對Adapter來說 那個DatatTable就視為沒變動過

                //將取得的行列資料Add 回DataGridView(BindingSource)中
                foreach (System.Data.DataRow pri in get.Rows)
                {
                    dtAl.Rows.Add(pri.ItemArray);
                }

                dtAl.Dispose();//釋放dtAl (DataTable)
                get.Dispose();//釋放dtAl (DataTable)

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        //(OLE) 將DataGridView Data sync to Access
        private void bUpDB_Click(object sender, EventArgs e)
        {
            DataTable dtAl = new DataTable();

            //強轉DataGridView 轉成 DataTable 並複製資料結構
            //dtAl = ((DataTable)dgvTable.DataSource).Clone();//所有筆
            dtAl = ((DataTable)bds.DataSource).Clone();//所有筆

            //DataGridView data to DataTable
            //dtAl = (DataTable)(dgvTable.DataSource);
            dtAl = (DataTable)(bds.DataSource);
            //dtAl.AcceptChanges(); //AcceptChanges這道會把那Flag都清掉，因此對Adapter來說 那個DatatTable就視為沒變動過

            try
            {
                _smdb.OleDbSyncData(txt_Path.Text, cbox_SheetName.Text, dtAl);
                dtAl.Dispose(); //釋放dtAl (DataTable)

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        //Add row number to DataGridView
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
