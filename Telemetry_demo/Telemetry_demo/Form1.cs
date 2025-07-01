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
        private UserControl1 inputPage;
        private UserControl3 plottingPage;
        private SettingsPage settingsPage;
        public Telemetry()
        {
            InitializeComponent();
            InitializeNavigationControl();
        }

        public void InitializeNavigationControl()
        {
            inputPage = new UserControl1();
            plottingPage = new UserControl3();
            settingsPage = new SettingsPage();
            List<UserControl> userControls = new List<UserControl>() { inputPage, plottingPage, settingsPage };
            navigationControl = new NavigationControl(userControls, panelMain);
            navigationControl.Display(0);
        }
        

        private void btnWifi_Click(object sender, EventArgs e)
        {
            navigationControl.Display(1);
            plottingPage.LoadSavedConfigs();
        }

        private void btnUART_Click(object sender, EventArgs e)
        {
            navigationControl.Display(0);
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

        private void btnSettings_Click(object sender, EventArgs e)
        {
            navigationControl.Display(2);
        }

        private void panelMain_Paint_1(object sender, PaintEventArgs e)
        {

        }
    }
}
