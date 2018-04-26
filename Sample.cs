using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;


namespace chart
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            chart1.Series[0].Points.Clear();
            chart1.Series[1].Points.Clear();
            chart1.Series[2].Points.Clear();
            //chart1.Series.Clear();

            Series series1 = new Series("test4"); //eries series1= new Series("Di0", 500); //初始畫線條(名稱，最大值)
            series1.Color = Color.Blue; //設定線條顏色
            series1.Font = new System.Drawing.Font("新細明體", 10); //設定字型
            series1.ChartType = SeriesChartType.Spline; //設定線條種類
            //series1.ChartType = SeriesChartType.FastLine; //設定線條種類
            //series1.ChartType = SeriesChartType.Spline; //設定線條種類
            //chart1.ChartAreas[0].AxisY.Minimum = 0;//設定Y軸最小值
            //chart1.ChartAreas[0].AxisY.Maximum = 500;//設定Y軸最大值
            //chart1.ChartAreas[0].AxisY.Enabled= AxisEnabled.False; //隱藏Y 軸標示
            //chart1.ChartAreas[0].AxisY.MajorGrid.Enabled= true;  //隱藏Y軸標線
            series1.IsValueShownAsLabel = true; //是否把數值顯示在線上
            

            //chart1.Series[0].Name = "test1";
            //chart1.Series[1].Name = "test2";
            //chart1.Series[2].Name = "test3";

            //double[] data1 = new double[360];
            //double[] data2 = new double[360];
            //double[] data3 = new double[360];
            //double[] data4 = new double[360];

            //for (int i = 0; i < 360; i++)
            //{
            //    data1[i] = Math.Sin(i * 2 * Math.PI / 360);
            //    data2[i] = Math.Cos(i * 2 * Math.PI / 360);
            //    data3[i] = data1[i] + data2[i];
            //    data4[i] = data1[i] - data2[i];
            //}
            //// 建立好資料

            //// 匯入Chart1
            //for (int i = 0; i < 360; i++)
            //{
            //    chart1.Series[0].Points.AddXY(i, data1[i]);
            //    chart1.Series[1].Points.AddXY(i, data2[i]);
            //    chart1.Series[2].Points.AddXY(i, data3[i]);

            //    series1.Points.AddXY(i, data3[i]); //把值加入X 軸Y 軸
            //}
            chart1.Series.Clear();
            //把值加入X 軸Y 軸
            //chart1.Series[0].MarkerSize = 8;
            series1.MarkerSize = 8;
            // chart1.Series[0].MarkerStyle = MarkerStyle.Circle;   
            series1.MarkerStyle = MarkerStyle.Circle;
            series1.Points.AddXY("A", 200);
            series1.Points.AddXY("B", 100);
            series1.Points.AddXY("C", 100);
            series1.Points.AddXY("D", 30);
            series1.Points.AddXY("E", 300);
            series1.Points.AddXY("F", 70);
            this.chart1.Series.Add(series1);//將線畫在圖上
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }
    }
}
