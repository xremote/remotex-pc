using FirstFloor.ModernUI.Windows.Controls;
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Threading;
using System.ComponentModel;
using System.Collections.Generic;
using Microsoft.VisualBasic;
using AudioSwitcher.AudioApi.CoreAudio;

namespace RemoteX
{
  
    public partial class MainWindow : ModernWindow
    {
         
        SolidColorBrush red = new SolidColorBrush(System.Windows.Media.Color.FromRgb(197, 19, 19)); 
        SolidColorBrush green = new SolidColorBrush(System.Windows.Media.Color.FromRgb(19, 147, 43));
        public Boolean disconnect = false;
        NotifyIcon nIcon = new NotifyIcon();
        private Boolean stopclose = true;
        private System.Windows.Forms.ContextMenu contextMenu1;
        private System.Windows.Forms.MenuItem menuItem1;
        private System.Windows.Forms.MenuItem menuItem2;
        private System.ComponentModel.IContainer components;
        public Boolean threadrunning = false;
        public string pcname = "",pcIP="",remotename="",remoteip="";

        CoreAudioDevice defaultPlaybackDevice = new CoreAudioController().DefaultPlaybackDevice;
        // Create a new dictionary of strings, with string keys.
        //
        public Dictionary<string, string> SystemInfo =   new Dictionary<string, string>();        


        Thread thread1;

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);

        [DllImport("User32.dll")]
        private static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetCursorPos(out POINT lpPoint);

        static TcpListener listener;
        public NetworkStream s;
        public StreamReader sr;
        public StreamWriter sw;
        Socket soc = null;

        int curposx = 0;
        int curposy = 0;
      
        int screenwidth = Screen.PrimaryScreen.Bounds.Width;
        int screenheight = Screen.PrimaryScreen.Bounds.Height;
      
        


        public MainWindow()
        {
            InitializeComponent();
           
            nIcon.MouseDoubleClick += NIcon_MouseDoubleClick;
            this.components = new System.ComponentModel.Container();
            this.contextMenu1 = new System.Windows.Forms.ContextMenu();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();

            this.contextMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] { this.menuItem1,this.menuItem2 });


            this.menuItem2.Index = 0;    
            this.menuItem2.Click += MenuItem2_Click;
            this.menuItem2.Text = "Open RemoteX";


            this.menuItem1.Index = 1;
            this.menuItem1.Text = "Exit";
            this.menuItem1.Click += MenuItem1_Click;

            nIcon.ContextMenu = this.contextMenu1;
            nIcon.Text = "RemoteX";
            Connected_Status_text.Foreground = red;
            this.nIcon.Icon = new Icon("icon_small.ico");
            nIcon.Visible = true;

            listener = new TcpListener(IPAddress.Any, 2055);
            listener.Start();
            

            refreshUI();
            initializeSystemInfo();

        }
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            if (stopclose)
            {
                minimize_tray();
            }
            else
            {
                nIcon.Visible = false;
                closeall();
            }

            e.Cancel = stopclose;
        }

        





        

        /////////////END of CODE//////////////////

    }
}   
