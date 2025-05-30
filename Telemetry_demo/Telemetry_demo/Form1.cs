using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Telemetry_demo
{
    public partial class Telemetry : Form
    {
        NavigationControl navigationControl;
        private int sidebarExpandedWidth = 150;
        private int sidebarCollapsedWidth = 40;
        private bool sidebarCollapsed = false;
        public Telemetry()
        {
            InitializeComponent();
            InitializeNavigationControl();
        }

        public void InitializeNavigationControl()
        {
            List<UserControl> userControls = new List<UserControl>() { new UserControl1(),new UserControl3()};
            navigationControl = new NavigationControl(userControls,panelMain);
            navigationControl.Display(0);
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

       

        private void btnWifi_Click(object sender, EventArgs e)
        {
            navigationControl.Display(1);
            
        }

        private void btnUART_Click(object sender, EventArgs e)
        {
            navigationControl.Display(0);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            navigationControl.Display(2);
        }

        private void panelMain_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnCollapseSidebar_Click(object sender, EventArgs e)
        {
            if (!sidebarCollapsed)
            {
                panel1.Width = sidebarCollapsedWidth;
                btnCollapseSidebar.Text = ">>";
                sidebarCollapsed = true;
            }
            else
            {
                panel1.Width = sidebarExpandedWidth;
                btnCollapseSidebar.Text = "☰";
                sidebarCollapsed = false;
            }
        }
    }
}
