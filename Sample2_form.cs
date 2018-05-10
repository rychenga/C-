using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Data;
using System.Drawing;
using System.Windows.Forms.DataVisualization.Charting;
using Jeff.DB;

namespace sample
{
    public partial class Form1 : Form
    {
        //宣告Jeff.DB dll
        private MDB _smdb;

        public Form1()
        {
            InitializeComponent();
        }

        // Select MDB File buttion
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

        //Query buttion
        private void bQuery_Click(object sender, EventArgs e)
        {
            try
            {
                _smdb = new MDB();

                this.chart1.Series.Clear();//清楚CHART 值
                this.chart1.ChartAreas[0].RecalculateAxesScale();//重新計算的圖表區域屬性


                Series series1 = new Series("total count"); //初始totail 名稱
                series1.Color = Color.Blue; //設定線條顏色
                series1.Font = new System.Drawing.Font("新細明體", 10); //設定字型
                series1.ChartType = SeriesChartType.Column; //設定線條種類
                series1.MarkerSize = 1; //顯示值的點大小
                series1.MarkerStyle = MarkerStyle.Circle; //顯示值的點種類

                //清空DataGrivew row data(DataSource = null)
                dgvTable.DataSource = null;
                //SQL1
                string sql = "select * from " + cbox_SheetName.Text;
                //SQL2
                string sql2 = "select count(*) as cnt from " + cbox_SheetName.Text;

                if (txt_Path.Text != "")
                {
                    //DataGridView 顯示 row data
                    DataTable dt = _smdb.GetOleDbDataTable(txt_Path.Text, sql);
                    dgvTable.DataSource = dt;
                    dt = null;

                    //Chart 顯示 統計結果
                    DataTable dt2 = _smdb.GetOleDbDataTable(txt_Path.Text, sql2);
                    series1.Points.AddXY(dt2.Columns[0].ToString(), dt2.Rows[0].ItemArray[0]);                    
                    this.chart1.Series.Add(series1);//將線畫在圖上
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
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
