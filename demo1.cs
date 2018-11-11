using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Calendar;
using System.Xml.Serialization;
using System.IO;
using CalendarDemo;

namespace demo
{
    public partial class Form1 : Form
    {
        List<CalendarItem> _items = new List<CalendarItem>();
        CalendarItem contextItem = null;

        public Form1()
        {
            InitializeComponent();

            //Monthview colors
            monthView1.MonthTitleColor = monthView1.MonthTitleColorInactive = CalendarColorTable.FromHex("#C2DAFC");
            monthView1.ArrowsColor = CalendarColorTable.FromHex("#77A1D3");
            monthView1.DaySelectedBackgroundColor = CalendarColorTable.FromHex("#F4CC52");
            monthView1.DaySelectedTextColor = monthView1.ForeColor;
        }

        //取得xml file 路徑
        public FileInfo ItemsFile
        {
            get
            {
                return new FileInfo(Path.Combine(Application.StartupPath, "items.xml"));
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //預設calendar 今天+6天(共7天)
            calendar1.SetViewRange(DateTime.Now, DateTime.Now.AddDays(6));

            // 找出字體大小,並算出比例
            float dpiX, dpiY;
            Graphics graphics = this.CreateGraphics();
            dpiX = graphics.DpiX;
            dpiY = graphics.DpiY;
            //int intPercent = (dpiX == 96) ? 100 : (dpiX == 120) ? 125 : (dpiX == 144) ? 150:200;
            float floatPercent = (dpiX == 96) ? 100 : (dpiX == 120) ? 125 : (dpiX == 144) ? 150 : 200;

            float fontRatio = 70 / floatPercent; //縮小70%
            //calendar1.Font = new Font(f.FontFamily, calOriginal * 0.59F, f.Style);
            //calendar 字型縮小0.6
            calendar1.Font = new Font(calendar1.Font.FontFamily, calendar1.Font.Size * fontRatio, calendar1.Font.Style);

            this.Tag = this.Height + "|" + this.Width;
            foreach (Control o in this.Controls)
            {
                o.Tag = o.Top + "|" + o.Left + "|" + o.Height + "|" + o.Width;
            }

            //取得xml file(暫存) 內容
            if (ItemsFile.Exists)
            {
                List<ItemInfo> lst = new List<ItemInfo>();

                XmlSerializer xml = new XmlSerializer(lst.GetType());

                using (Stream s = ItemsFile.OpenRead())
                {
                    lst = xml.Deserialize(s) as List<ItemInfo>;
                }

                foreach (ItemInfo item in lst)
                {
                    CalendarItem cal = new CalendarItem(calendar1, item.StartTime, item.EndTime, item.Text);

                    if (!(item.R == 0 && item.G == 0 && item.B == 0))
                    {
                        cal.ApplyColor(Color.FromArgb(item.A, item.R, item.G, item.B));
                    }

                    _items.Add(cal);
                }

                PlaceItems();//讀取calendar 項目
            }

        }

        //當拉扯GUI 大小時，重新計算元件大小
        private void Form1_Resize(object sender, EventArgs e)
        {
            foreach (Control o in this.Controls)
            {
                o.Width = (int)(double.Parse(o.Tag.ToString().Split('|')[3]) * (this.Width / double.Parse(this.Tag.ToString().Split('|')[1])));
                o.Height = (int)(double.Parse(o.Tag.ToString().Split('|')[2]) * (this.Height / double.Parse(this.Tag.ToString().Split('|')[0])));
                o.Left = (int)(double.Parse(o.Tag.ToString().Split('|')[1]) * (this.Width / double.Parse(this.Tag.ToString().Split('|')[1])));
                o.Top = (int)(double.Parse(o.Tag.ToString().Split('|')[0]) * (this.Height / double.Parse(this.Tag.ToString().Split('|')[0])));

                calendar1.Width = (int)(double.Parse(o.Tag.ToString().Split('|')[3]) * (this.Width / double.Parse(this.Tag.ToString().Split('|')[1])));
                calendar1.Height = (int)(double.Parse(o.Tag.ToString().Split('|')[2]) * (this.Height / double.Parse(this.Tag.ToString().Split('|')[0])));
                calendar1.Left = (int)(double.Parse(o.Tag.ToString().Split('|')[1]) * (this.Width / double.Parse(this.Tag.ToString().Split('|')[1])));
                calendar1.Top = (int)(double.Parse(o.Tag.ToString().Split('|')[0]) * (this.Height / double.Parse(this.Tag.ToString().Split('|')[0])));
            }
        }

        //選取日期時，變更calendar內容
        private void monthView1_SelectionChanged_1(object sender, EventArgs e)
        {
            calendar1.SetViewRange(monthView1.SelectionStart, monthView1.SelectionEnd);
        }

        //當檔案關閉時，存檔
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            List<ItemInfo> lst = new List<ItemInfo>();

            foreach (CalendarItem item in _items)
            {
                lst.Add(new ItemInfo(item.StartDate, item.EndDate, item.Text, item.BackgroundColor));
            }

            XmlSerializer xmls = new XmlSerializer(lst.GetType());

            if (ItemsFile.Exists)
            {
                ItemsFile.Delete();
            }

            using (Stream s = ItemsFile.OpenWrite())
            {
                xmls.Serialize(s, lst);
                s.Close();
            }
        }

        #region calendar 設定        
        private void calendar1_LoadItems(object sender, CalendarLoadEventArgs e)
        {
            PlaceItems(); //讀取calendar 項目
        }

        //讀取calendar 項目
        private void PlaceItems()
        {
            foreach (CalendarItem item in _items)
            {
                if (calendar1.ViewIntersects(item))
                {
                    calendar1.Items.Add(item);
                }
            }
        }

        //建立新的項目
        private void calendar1_ItemCreated(object sender, CalendarItemCancelEventArgs e)
        {
            _items.Add(e.Item);
        }

        //移動即有項目
        private void calendar1_ItemMouseHover(object sender, CalendarItemEventArgs e)
        {
            Text = e.Item.Text;
        }

        //當在既有項目，滑鼠點兩下，重新編輯內容
        private void calendar1_ItemDoubleClick(object sender, CalendarItemEventArgs e)
        {
            //MessageBox.Show("Double click: " + e.Item.Text);
            calendar1.ActivateEditMode();
        }

        //刪除即有項目
        private void calendar1_ItemDeleted(object sender, CalendarItemEventArgs e)
        {
            _items.Remove(e.Item);
        }

        //選擇日期標題時(星期xxx)，會變更calendar內容
        private void calendar1_DayHeaderClick(object sender, CalendarDayEventArgs e)
        {
            calendar1.SetViewRange(e.CalendarDay.Date, e.CalendarDay.Date);
        }
        #endregion


        #region ContexMenu_Colors
        //滑鼠右鍵，選取顏色的功能
        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            contextItem = calendar1.ItemAt(contextMenuStrip1.Bounds.Location);
        }

        private void redTagToolStripMenuItem_Click(object sender, EventArgs e) //紅色
        {
            foreach (CalendarItem item in calendar1.GetSelectedItems())
            {
                item.ApplyColor(Color.Red);
                calendar1.Invalidate(item);
            }
        }
        private void yellowTagToolStripMenuItem_Click(object sender, EventArgs e)//黃色
        {
            foreach (CalendarItem item in calendar1.GetSelectedItems())
            {
                item.ApplyColor(Color.Gold);
                calendar1.Invalidate(item);
            }

        }
        private void greenTagToolStripMenuItem_Click(object sender, EventArgs e)//綠色
        {
            foreach (CalendarItem item in calendar1.GetSelectedItems())
            {
                item.ApplyColor(Color.Green);
                calendar1.Invalidate(item);
            }
        }
        private void blueTagToolStripMenuItem_Click(object sender, EventArgs e)//藍色
        {
            foreach (CalendarItem item in calendar1.GetSelectedItems())
            {
                item.ApplyColor(Color.DarkBlue);
                calendar1.Invalidate(item);
            }
        }
        private void otherColorTagToolStripMenuItem_Click(object sender, EventArgs e)//其它顏色
        {
            using (ColorDialog dlg = new ColorDialog())
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    foreach (CalendarItem item in calendar1.GetSelectedItems())
                    {
                        item.ApplyColor(dlg.Color);
                        calendar1.Invalidate(item);
                    }
                }
            }
        }
        #endregion ContexMenu_Colors

        #region calendar_Y軸格式
        //滑鼠右鍵，選取日曆時間格式的功能
        private void hourToolStripMenuItem_Click(object sender, EventArgs e)
        {
            calendar1.TimeScale = CalendarTimeScale.SixtyMinutes;//一個小時
        }
        private void minutesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            calendar1.TimeScale = CalendarTimeScale.ThirtyMinutes;//半個小時
        }

        #endregion calendar_Y軸格式






    }
}
