using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Appium
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DeviceEntity d = new DeviceEntity();
            d.Name = "33008dbca2a22489";  // Device
            d.Port = 4723; // Port // cmd appium --port 4723
            d.Version = "9";
            var start = AppHelper.StartServer(d.Name, d.Port);
            if (start.is_started)
            {
                var phone = AppHelper.StartPhone(d);
                if (phone != null)
                    AppHelper.ExpandNotification(d); // phone.FindElementByXPath("//android.widget.ScrollView[@resource-id=\"com.android.systemui:id/notification_stack_scroller\"]");
                var scroler =  phone.FindElementByXPath("/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout[2]/android.widget.LinearLayout/android.widget.RelativeLayout/android.widget.LinearLayout/android.widget.LinearLayout/android.widget.EditText");
                scroler.Click();
            }
        }
    }
}
