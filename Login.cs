using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace POS
{
    public partial class Login : Form
    {

        public Login()
        {
            InitializeComponent();
        }

        private void Login_Load(object sender, EventArgs e)
        {
            // 找出字體大小,並算出比例
            float dpiX, dpiY;
            Graphics graphics = this.CreateGraphics();
            dpiX = graphics.DpiX;
            dpiY = graphics.DpiY;
            //int intPercent = (dpiX == 96) ? 100 : (dpiX == 120) ? 125 : (dpiX == 144) ? 150:200;
            float floatPercent = (dpiX == 96) ? 100 : (dpiX == 120) ? 125 : (dpiX == 144) ? 150 : 200;
        }

        private void bQuery_Click(object sender, EventArgs e)
        {
            this.Hide();
            Query Query = new Query();
            Query.Show();
        }

        private void bOrder_Click(object sender, EventArgs e)
        {
            this.Hide();
            Order order = new Order();
            order.Show();
        }

        private void Login_FormClosed(object sender, FormClosedEventArgs e)
        {
            //離開程式
            this.Close();
            Environment.Exit(Environment.ExitCode);       
        }
    }
}
