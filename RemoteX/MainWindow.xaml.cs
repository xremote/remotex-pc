using AudioSwitcher.AudioApi.CoreAudio;
using FirstFloor.ModernUI.Windows.Controls;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Threading;

namespace RemoteX
{

    public partial class MainWindow : ModernWindow
    {
        //G_global_variables...

        SolidColorBrush G_red = new SolidColorBrush(System.Windows.Media.Color.FromRgb(197, 19, 19));
        SolidColorBrush G_green = new SolidColorBrush(System.Windows.Media.Color.FromRgb(19, 147, 43));

        NotifyIcon G_nIcon = new NotifyIcon();
        private System.Windows.Forms.ContextMenu G_traymenu;
        private System.Windows.Forms.MenuItem G_traymennu_exit;
        private System.Windows.Forms.MenuItem G_traymenu_open;
        private System.ComponentModel.IContainer G_components;
        private Boolean G_stopclose = true; // do not close.. minimize to tray

        CoreAudioDevice G_defaultPlaybackDevice = new CoreAudioController().DefaultPlaybackDevice;

        int G_screenwidth = Screen.PrimaryScreen.Bounds.Width;
        int G_screenheight = Screen.PrimaryScreen.Bounds.Height;

        public MainWindow()
        {
            InitializeComponent();

            G_nIcon.MouseDoubleClick += NIcon_MouseDoubleClick;
            this.G_components = new System.ComponentModel.Container();
            this.G_traymenu = new System.Windows.Forms.ContextMenu();
            this.G_traymennu_exit = new System.Windows.Forms.MenuItem();
            this.G_traymenu_open = new System.Windows.Forms.MenuItem();

            this.G_traymenu.MenuItems.AddRange
                (new System.Windows.Forms.MenuItem[] { this.G_traymennu_exit, this.G_traymenu_open });


            this.G_traymenu_open.Index = 0;
            this.G_traymenu_open.Click += traymenu_openclick;
            this.G_traymenu_open.Text = "Open RemoteX";


            this.G_traymennu_exit.Index = 1;
            this.G_traymennu_exit.Text = "Exit";
            this.G_traymennu_exit.Click += traymenu_exitclick;

            G_nIcon.ContextMenu = this.G_traymenu;
            G_nIcon.Text = "RemoteX";
            Connected_Status_text.Foreground = G_red;
            this.G_nIcon.Icon = new Icon("icon_small.ico");
            G_nIcon.Visible = true;

            G_listener = new TcpListener(IPAddress.Any, 2055);
            G_listener.Start();


            refreshUI();
            initializeSystemInfo();

        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            if (G_stopclose)
            {
                minimize_to_tray();
            }
            else
            {
                G_nIcon.Visible = false;
                G_nIcon.Dispose();
                closeall();
            }
            e.Cancel = G_stopclose;
        }

        private void traymenu_openclick(object sender, EventArgs e)
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



        private void traymenu_exitclick(object sender, EventArgs e)
        {
            G_nIcon.Visible = false;
            G_stopclose = false;
            this.Close();
        }

        private void Refresh_button_Click(object sender, RoutedEventArgs e)
        {
            //refreshUI();         
            // it is automatic after every 2 seconds
        }


        private void maximize_app()
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



        private void minimize_to_tray()
        {
            this.WindowState = System.Windows.WindowState.Minimized;
            this.Hide();
            this.G_nIcon.Icon = new Icon("icon_small.ico");
            G_nIcon.Visible = true;
            this.G_nIcon.ShowBalloonTip(1000, "RemoteX", "Working in Background...", ToolTipIcon.Info);
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
                if (G_disconnect)
                {
                    disconnect_network();
                }
                if (!G_threadrunning)
                {
                    G_threadrunning = true;
                    start_thread();
                }
                G_pcname = Dns.GetHostName();
                G_pcIP = getlocalip();
                PCName_text.Text = "PC Name : " + G_pcname;
                hostIP_text.Text = "IP Address : " + G_pcIP;

                try
                {

                    if (G_socket != null)
                    {
                        IPEndPoint socket_endpoint = (IPEndPoint)G_socket.RemoteEndPoint;
                        G_remoteip = socket_endpoint.ToString();
                        RemoteIP_Text.Text = "Remote IP Address : " + G_remoteip;
                        Connected_Status_text.Text = "Connected";
                        Connected_Status_text.Foreground = G_green;
                    }
                    else
                    {
                        G_remoteip = "";
                        RemoteIP_Text.Text = "Remote IP Address : " + G_remoteip;
                        Connected_Status_text.Text = "No Device Connected";
                        Connected_Status_text.Foreground = G_red;
                    }

                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                    G_remoteip = "";
                    RemoteIP_Text.Text = "Remote IP Address : " + G_remoteip;

                    Connected_Status_text.Text = "No Device Connected";
                    Connected_Status_text.Foreground = G_green;

                }
                //Debug.WriteLine("Refreshed");
                await Task.Delay(2000);
            }
        }

        private void Disconnect_button_Click(object sender, RoutedEventArgs e)
        {
            G_disconnect = true;
            disconnect_network();
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
