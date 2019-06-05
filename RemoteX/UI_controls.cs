using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;

namespace RemoteX
{
    public partial class MainWindow
    {

        private void MenuItem2_Click(object sender, EventArgs e)
        {
            maximize_app();
        }

        private void NIcon_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            maximize_app();
        }


        private void minimize_taskbar()
        {
            this.WindowState = WindowState.Minimized;
        }



        private void MenuItem1_Click(object sender, EventArgs e)
        {
            nIcon.Visible = false;
            stopclose = false;
            this.Close();

        }

        private void Refresh_button_Click(object sender, RoutedEventArgs e)
        {
            //closeall();
            refreshUI();
            //Debug.WriteLine(totalhddsize/(1024*1024*1024));
            //Debug.WriteLine(cpuCounter.NextValue() + "%" + ramCounter.NextValue() + "MB");


        }


        private void maximize_app()
        {
            if (this.Visibility == Visibility.Hidden)
            {
                this.Visibility = Visibility.Visible;
                System.Windows.Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                    new Action(delegate ()
                    {
                        this.Show();
                        this.WindowState = WindowState.Normal;
                        this.Activate();
                    })
                );
            }


        }



        private void minimize_tray()
        {

            this.WindowState = System.Windows.WindowState.Minimized;
            this.Hide();
            this.nIcon.Icon = new Icon("icon_small.ico");
            nIcon.Visible = true;

            this.nIcon.ShowBalloonTip(500, "RemoteX", "Working in Background...", ToolTipIcon.Info);

        }

        

        private void closeall()
        {
            String pname = Process.GetCurrentProcess().ProcessName;
            Debug.WriteLine(pname);
            foreach (var process in Process.GetProcessesByName(pname))
            {
                process.Kill();
            }
        }

        private async void refreshUI()
        {

            while (true)
            {
                if (!threadrunning)
                {
                    threadrunning = true; createthread();
                }
                pcname = Dns.GetHostName();
                pcIP = getlocalip();
                PCName_text.Text = "PC Name : " + pcname;
                hostIP_text.Text = "IP Address : " + pcIP;

                try
                {

                    if (soc != null)
                    {
                        IPEndPoint c_endPoint = (IPEndPoint)soc.RemoteEndPoint;
                        remoteip = c_endPoint.ToString();

                        RemoteIP_Text.Text = "Remote IP Address : " + remoteip;
                        Connected_Status_text.Text = "Connected";
                        Connected_Status_text.Foreground = green;
                    }
                    else
                    {
                        remoteip = "";
                        RemoteIP_Text.Text = "Remote IP Address : " + remoteip;
                        Connected_Status_text.Text = "No Device Connected";
                        Connected_Status_text.Foreground = red;
                    }

                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                    remoteip = "";
                    RemoteIP_Text.Text = "Remote IP Address : " + remoteip;

                    Connected_Status_text.Text = "No Device Connected";
                    Connected_Status_text.Foreground = green;

                }
                await Task.Delay(2000);
            }


        }



        private void Disconnect_button_Click(object sender, RoutedEventArgs e)
        {
            disconnect = true;
        }

        private void Heading_text_Click(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.settingwindows == 0)
            {
                show_settings_wndw();
            }

        }

        private void show_settings_wndw()
        {

            Settings subWindow = new Settings();
            subWindow.Show();
        }

    }
}
