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

namespace devices
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
            this.menuItem2.Text = "Open Remote Devices";


            this.menuItem1.Index = 1;
            this.menuItem1.Text = "Exit";
            this.menuItem1.Click += MenuItem1_Click;

            nIcon.ContextMenu = this.contextMenu1;
            nIcon.Text = "Remote Devices";
            Connected_Status_text.Foreground = red;
            this.nIcon.Icon = new Icon("icon_small.ico");
            nIcon.Visible = true;

            listener = new TcpListener(IPAddress.Any, 2055);
            listener.Start();
            

            refreshUI();
            initializeSystemInfo();

        }

        static ulong GetTotalMemoryInBytes()
        {
            return new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory;
        }

        private void initializeSystemInfo()
        {
            try
            {

                SystemInfo.Add("username", SystemInformation.UserName);
                SystemInfo.Add("computername", SystemInformation.ComputerName);
                SystemInfo.Add("monitorcount", SystemInformation.MonitorCount.ToString());                
                SystemInfo.Add("monitorheight", SystemInformation.PrimaryMonitorSize.Height.ToString());
                SystemInfo.Add("monitorwidth", SystemInformation.PrimaryMonitorSize.Width.ToString());

                PowerStatus pw = SystemInformation.PowerStatus;
                
                SystemInfo.Add("batterylife", (pw.BatteryLifePercent).ToString());
                SystemInfo.Add("batterystatus", (pw.BatteryChargeStatus).ToString());
            }
            catch (Exception e)
            {

            }



            try
            {                
                
                if (Environment.Is64BitOperatingSystem)
                {
                    SystemInfo.Add("osbit", "64 bit");
                }
                else
                {
                    SystemInfo.Add("osbit", "32 bit");
                }
                SystemInfo.Add("processorcount", Environment.ProcessorCount.ToString());
                SystemInfo.Add("osversion", Environment.OSVersion.ToString());
            }
            catch (Exception e)
            {
            
            }



            try
            {
                SystemInfo.Add("osname", new Microsoft.VisualBasic.Devices.ComputerInfo().OSFullName);
                SystemInfo.Add("totalmemory", (new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory / (1024.0 * 1024 * 1024)).ToString().Substring(0, 4) + " GB");
            }
            catch (Exception e)
            {

            }


            try
            {
                double totalhddsize = 0;
                foreach (DriveInfo info in DriveInfo.GetDrives())
                {
                    if (info.IsReady && info.DriveType == DriveType.Fixed)
                    {
                        totalhddsize += info.TotalSize;
                    }
                }

                totalhddsize = totalhddsize / (1024.0 * 1024 * 1024);

                SystemInfo.Add("HDD-SIZE", totalhddsize.ToString() + " GB");
            }
            catch (Exception e)
            {

            }

          

            SystemInfo.Add("ipaddress", pcIP);
            
         Debug.WriteLine("sys " + MyDictionaryToJson(SystemInfo));
           
            
        }



        string MyDictionaryToJson(Dictionary<string, string> dict)
        {
            String Jsondata = "{";
            foreach (KeyValuePair<string, string> entry in dict)
            {
                Jsondata += "\"" + entry.Key + "\":" + "\"" +  entry.Value + "\"" + ",";
            }
            Jsondata = Jsondata.Substring(0,Jsondata.Length-1);
            Jsondata += "}";
            return Jsondata;
        }




        private void refreshSystemInfo()
        {
           
            //SystemInfo.Add("batterypercentage", );

        }




        private void createthread()
        {

            try
            {


                thread1 = new Thread(new ThreadStart(acceptingsockets));
                
                thread1.Start();

            }
            catch (Exception f)
            {
                Debug.WriteLine(f.Message);
            }
            

        }

        private void acceptingsockets()
        {   

            try
            {
                //       IPEndPoint c_endPoint = (IPEndPoint)soc.RemoteEndPoint;
                soc = listener.AcceptSocket();
            }
            catch (Exception f)
            {
                Debug.WriteLine(f.Message);

            }
            NetworkStream clientStream =null;
            int t = 1000;
            try
            {
                clientStream = new NetworkStream(soc);


                s = new NetworkStream(soc);
                sr = new StreamReader(s);
                sw = new StreamWriter(s);
                sw.AutoFlush = true;
                sw.Flush();
                string str2 = "";

                //string str2 = sr.ReadLine();

                //if(str2 != "password" + Properties.Settings.Default.Password)
                {
                  //  disconnect = true;
                }

                while (t > 0)
                {
                    
                    if(disconnect)
                    {
                        disconnect = false;
                        break;
                    }
                     str2 = sr.ReadLine();


                    if (str2 != null)
                    {
                        //  Debug.WriteLine(str2);
                        if (str2[0] == '!')
                        {
                            movemouse2(str2.Substring(1, str2.Length - 1) + ";#");  //screensharing
                            sendscreen();
                        }
                        else if (str2[0] == '*')
                            movemouse(str2.Substring(1, str2.Length - 1) + "#");   //mouse 

                        else if (str2[0] == '@')
                            extractkey(str2.Substring(1, str2.Length - 1) + "#");    //keyboard
                        else if (str2[0] == '%')
                            shortcut(str2.Substring(1, str2.Length - 1));
                        else if (str2[0] == '$')
                            sendsysinfo(str2.Substring(1, str2.Length - 1));    //system info
                        else if (str2[0] == '&')
                            getdriveinfo(str2.Substring(1, str2.Length - 1));   //explorer
                        else if (str2[0] == '^')
                            specialaction(str2.Substring(1, str2.Length - 1));

                    }
                    else
                        break;
                    //t--;
                }

                
            }
            catch (Exception e)
            {
                Debug.WriteLine("every thing ends: " + e.Message);
            }

            try
            {

                sw.Close();
                sr.Close();
                s.Close();
                sw = null;
                sr = null;
                s = null;
                clientStream.Close();
                soc.Shutdown(SocketShutdown.Both);
                soc.Close();
                soc = null;

            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            threadrunning = false;
        }
        


        private void MenuItem2_Click(object sender, EventArgs e)
        {
            maximize_app();
        }

        private void NIcon_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            maximize_app();
        }

        
        private void minimize_taskbar(){
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

            this.nIcon.ShowBalloonTip(500, "Remote Devices", "Working in Background...", ToolTipIcon.Info);

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

        private void closeall()
        {
            String pname = Process.GetCurrentProcess().ProcessName;
            Debug.WriteLine(pname);
            foreach (var process in Process.GetProcessesByName(pname) )
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
                        Connected_Status_text.Text = "You Are Not Connected To Any Network";
                        Connected_Status_text.Foreground = red;
                    }

                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                    remoteip = "";
                    RemoteIP_Text.Text = "Remote IP Address : " + remoteip;

                    Connected_Status_text.Text = "You Are Not Connected To Any Network";
                    Connected_Status_text.Foreground = green;

                }
                await Task.Delay(2000);
            }


        }

     
        //special controls 

        public void specialaction(String action)
        {

            switch (action)
            {
                case "lockuser":
                    lockuser();
                    break;
                case "logoff":
                    logoff();
                    break;
                case "screenshot":
                    screenshot();
                    break;
                case "taskmanager":
                    taskmanager();
                    break;
                case "magnify+":
                    magnifyinc();
                    break;
                case "magnify-":
                    magnifydec();
                    break;
                case "shutdown":
                    shutdown();
                    break;
                case "restart":
                    restart();
                    break;
                case "previousslide":
                    prevslide();
                    break;
                case "nextslide":
                    nextslide();
                    break;
                case "pauseplay":
                    pauseplayslide();
                    break;
                case "clearsins":
                    clearsins();
                    break;
            }

        }

        [DllImport("user32.dll")]
        public static extern bool LockWorkStation();
        public void lockuser()
        {
            LockWorkStation();
        }
        String timedelay = "10";

        public void shutdown()
        {
            var psi = new ProcessStartInfo("shutdown", "/s /t "+timedelay);
            psi.CreateNoWindow = true;
            psi.UseShellExecute = false;
            Process.Start(psi);
        }

        public void restart()
        {
            var psi = new ProcessStartInfo("shutdown", "/r /t "+timedelay);
            psi.CreateNoWindow = true;
            psi.UseShellExecute = false;
            Process.Start(psi);
        }

        public void logoff()
        {
            var psi = new ProcessStartInfo("shutdown", "/L");
            psi.CreateNoWindow = true;
            psi.UseShellExecute = false;
            Process.Start(psi);
        }

        public void screenshot()
        {           
            using (Bitmap bitmap = new Bitmap(screenwidth, screenheight))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen(0, 0, 0, 0, new System.Drawing.Size(screenwidth, screenheight));
                }
                var desktopFolder = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                var screenshotpath = Path.Combine(desktopFolder, "Screenshot_RD_" + DateTime.Now.ToString("yyyyMMddHHmmssffff")+".jpg");                
                bitmap.Save(screenshotpath, ImageFormat.Jpeg);
            }
        }

        public void taskmanager()
        {
            var psi = new ProcessStartInfo("TASKMGR");
            psi.CreateNoWindow = true;
            psi.UseShellExecute = false;
            Process.Start(psi);
        }
        public void magnifyinc()
        {
            extractkey("1LWin;1Oemplus;2Oemplus;2LWin;#");
        }
        public void magnifydec()
        {
            extractkey("1LWin;1Oemminus;2Oemminus;2LWin;#");
        }

        public void pauseplayslide()
        {
            extractkey("1S;2S;#");            
        }
        public void nextslide()
        {
            extractkey("1Right;2Right;#");            
        }
        public void prevslide()
        {
            extractkey("1Left;2Left;#");
        }

        public void clearsins()
        {
            const string message ="42";
            const string caption = "Wait";
            var result = System.Windows.MessageBox.Show(message, caption, MessageBoxButton.OK);
        }



        //end of special ontrols
        //------------------------------------------------------------------------------------------------------------------

        //get local ip address
        public string getlocalip()
        {
            IPHostEntry host;
            string localIP = "?";
            host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (IPAddress ip in host.AddressList)
            {
                
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                }
            }
            return localIP;


        }





        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public POINT(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }
        }



        private void GetPoint()
        {
            POINT p;
            if (GetCursorPos(out p))
            {

                curposx = p.X;
                curposy = p.Y;
            }

        }



        public void SetCursor(int x1, int y1)
        {

            SetCursorPos(x1, y1);
        }



        private const int MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const int MOUSEEVENTF_LEFTUP = 0x0004;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x0008;
        private const int MOUSEEVENTF_RIGHTUP = 0x0010;
        private const int MOUSEEVENTF_MIDDLEDOWN = 0X0020;
        private const int MOUSEEVENTF_MIDDLEUP = 0X0040;

        public void DoMouseClick(uint X, uint Y, int z)
        {



            if (z == 0)
                mouse_event(MOUSEEVENTF_LEFTDOWN, X, Y, 0, 0);
            else if (z == 1)
                mouse_event(MOUSEEVENTF_LEFTUP, X, Y, 0, 0);
            else if (z == 2)
                mouse_event(MOUSEEVENTF_RIGHTDOWN, X, Y, 0, 0);
            else if (z == 3)
                mouse_event(MOUSEEVENTF_RIGHTUP, X, Y, 0, 0);
            else if (z == 4)
                mouse_event(MOUSEEVENTF_MIDDLEDOWN, X, Y, 0, 0);
            else if (z == 5)
                mouse_event(MOUSEEVENTF_MIDDLEUP, X, Y, 0, 0);
            else if (z == 6)
            { mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, X, Y, 0, 0); }
            else if (z == 7)
            {
                mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, X, Y, 0, 0);
                mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, X, Y, 0, 0);
            }

        }






        public void movemouse(String mouseinput)
        {

            GetPoint();
          
            int x1, y1, z1;
            if (mouseinput != null)
            {
                int tempx = 0, tempy = 0;

                string[] keys = mouseinput.Split(';');

                x1 = int.Parse(keys[0]);
                y1 = int.Parse(keys[1]);
                z1 = int.Parse(keys[2]);


                //       curposx += (int)(x1/1.5);   curposy += (int)(y1/1.5);


                tempx = curposx + x1; tempy = curposy + y1;


                curposx += x1; curposy += y1;


                if (curposx >= screenwidth)
                    curposx = screenwidth - 1;

                if (curposy >= screenheight)
                    curposy = screenheight - 1;


                if (curposx < 0)
                    curposx = 0;

                if (curposy < 0)
                    curposy = 0;

                SetCursorPos(curposx, curposy);
                if (z1 != 9)
                    DoMouseClick((uint)curposx, (uint)curposy, z1);

            }


        }




        public void movemouse2(String mouseinput)
        {

            GetPoint();
            
            int x1, y1, z1,r1;
            if (mouseinput != null)
            {


                string[] keys = mouseinput.Split(';');

                x1 = int.Parse(keys[0]);
                y1 = int.Parse(keys[1]);
                z1 = int.Parse(keys[2]);
                r1 = int.Parse(keys[3]);
                //Debug.WriteLine(z1 + " " + r1);
                if (r1 == 1 || z1 == 2)
                {
                    if ((x1 == -1 && y1 == -1))
                    { x1 = -3; y1 = -3; }
                }
                

                if (r1 == 1)
                {
                    KeyboardSend.KeyDown(Keys.Enter);     KeyboardSend.KeyUp(Keys.Enter);

                }



                if (!(x1 == -1 && y1 == -1))
                {


                    if (x1 != -3)
                    {

                        curposx = (int)((x1 * screenwidth) / 2000);
                        curposy = (int)((y1 * screenheight) / 2000);

                        Debug.WriteLine(curposx + " get " + curposy);
                        if (curposx >= screenwidth)
                            curposx = screenwidth - 1;

                        if (curposy >= screenheight)
                            curposy = screenheight - 1;


                        if (curposx < 0)
                            curposx = 0;

                        if (curposy < 0)
                            curposy = 0;

                        SetCursorPos(curposx, curposy);
                        //if (z1 != 9)

                    }

                    if (z1 == 2)
                    {
                        DoMouseClick((uint)curposx, (uint)curposy, 2);
                        DoMouseClick((uint)curposx, (uint)curposy, 3);
                    }
                    else
                        DoMouseClick((uint)curposx, (uint)curposy, 6);
                }

            }



        }




        // Keyboard movements

        public void extractkey(string keyinput)
        {
            //Details.Text = "";
            int i;

            if (keyinput != null)
            {


                string[] keys = keyinput.Split(';');

                foreach (string key in keys)
                {
                    if (key == null || key == "#")
                        break;
                    try
                    {
                        i = key[0] - '0';
                        typethis(key.Substring(1, key.Length - 1), i);
                        //Details.Text += "\n" + key + " " + i + " " + key.Substring(1, key.Length - 1);
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message);
                    }
                }



            }
        }





        public void typethis(string key, int i)
        {
            Keys mykey;
            mykey = (Keys)Enum.Parse(typeof(Keys), key, true);

            if (i == 1)
                KeyboardSend.KeyDown(mykey);
            else if (i == 2)
                KeyboardSend.KeyUp(mykey);




        }




        static class KeyboardSend
        {
            [DllImport("user32.dll")]
            private static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

            private const int KEYEVENTF_EXTENDEDKEY = 1;
            private const int KEYEVENTF_KEYUP = 2;

            public static void KeyDown(Keys vKey)
            {
                keybd_event((byte)vKey, 0, KEYEVENTF_EXTENDEDKEY, 0);
            }

            public static void KeyUp(Keys vKey)
            {
                keybd_event((byte)vKey, 0, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0);
            }
        }



        public void getdriveinfo(String input)
        {
            String result = null;
            Debug.WriteLine("input: "+input);
            if (input != null)
            {
                int i = input[0] - '0';
                input = input.Substring(1, input.Length - 1);
                if (i == 0)
                {
                    try
                    {
                        foreach (var drive in DriveInfo.GetDrives())
                        {
                            long temp = 0;

                            try
                            {
                                temp = drive.TotalSize;
                                if (temp == 0 || drive.Name == null || drive.Name == "")
                                {
                                    continue;
                                }

                            }
                            catch (Exception e)
                            {
                                Debug.WriteLine(e.Message);
                                continue;
                            }


                            if (drive.IsReady)
                                result = result + "1" + drive.Name + ";";

                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message);
                    }
                    if (result == null)
                        result = "";
                    else
                        result = result.Substring(0, result.Length - 1);


                    sw.WriteLine(result);

                }
                else if (i == 1)
                {
                    try
                    {

                        string[] fileEntries = Directory.GetFiles(input);
                        string[] folderentries = Directory.GetDirectories(input);

                        foreach (string folder in folderentries)
                        {

                            result = result + "1" + Path.GetFileName(folder) + ";";
                        }



                        foreach (string file in fileEntries)
                        {
                            if (isreadable(file))
                                result = result + "2" + Path.GetFileName(file) + ";";
                        }

                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message);
                    }



                    if (result != null)
                        result = result.Substring(0, result.Length - 1);
                    else
                        result = "empty";


                    sw.WriteLine(result);


                }
                else if (i == 2)
                {

                    if (File.Exists(input))
                    { //sw.WriteLine("catch"); 
                        sendfile(input);
                    }



                }
                else
                { sw.WriteLine(result); }

                Debug.WriteLine(result);
            }

        }


        //send system info to android for main screen

        public void sendsysinfo(String infotype)
        {
            String result = MyDictionaryToJson(SystemInfo);
            if (result != null)
            {
                sw.WriteLine(result);
                Debug.WriteLine(result);
            } 
            
        }



        
        public Boolean isreadable(string filename)
        {
            try
            {
                using (FileStream stream = File.Open(filename, FileMode.Open, FileAccess.Read))
                {

                    return true;
                }
            }
            catch (IOException)
            {
                return false; // <- File failed to open
            }

        }

        public void sendfile(string input)
        {
            Debug.WriteLine(input);
            int count = 0;
            Int64 numberOfBytes = 0, bytesReceived = 0;
            MemoryStream ms = new MemoryStream();
            System.IO.FileStream fs = new FileStream(input, FileMode.Open, FileAccess.Read);
            var buffer = new byte[1024 * 128];
            ms.Position = 0;
            fs.Position = 0;

            fs.CopyTo(ms);

            fs.Close();

            Debug.WriteLine(ms.Length);
            s.Write(BitConverter.GetBytes(ms.Length), 0, 8);

            if (ms.Length > 0)
            {
                ms.Position = 0;
                numberOfBytes = ms.Length;

                while (bytesReceived < numberOfBytes && (count = ms.Read(buffer, 0, buffer.Length)) > 0)
                {

                    s.Write(buffer, 0, count);

                    bytesReceived += count;
                    //Debug.WriteLine(bytesReceived);
                }



            }
        }

        private void Disconnect_button_Click(object sender, RoutedEventArgs e)
        {
            disconnect = true;
        }

        private void Heading_text_Click(object sender, RoutedEventArgs e)
        {
            if(Properties.Settings.Default.settingwindows == 0)
            {
                show_settings_wndw();
            }
            
        }

        private void show_settings_wndw()
        {
            
            Settings subWindow = new Settings();
            subWindow.Show();
        }

        public void shortcut(String input)
        {
            //shortcut keys......................

             if (input == "close")
                extractkey("1Lmenu;1F4;2F4;2Lmenu;#");

            else if (input == "switch1")
                extractkey("1Lmenu;1Tab;2Tab;2Lmenu;#");

            else if (input == "search")
                extractkey("1F3;2F3;#");

            else if (input == "help")
                extractkey("1F1;2F1;#");

            else if (input == "explorer")
                extractkey("1Lwin;1E;2Lwin;2E;#");

            else if (input == "utility")
                extractkey("1Lwin;1U;2Lwin;2U;#");
            
            else if (input == "sysinfo")
                extractkey("1F3;2F3;#");

             //media keys ............................

            else if (input == "playlist")
                extractkey("1ControlKey;1Lmenu;1Q;2Q;2Lmenu;2ControlKey;#");

            else if (input == "media")
                extractkey("1Selectmedia;2Selectmedia;#");


            else if (input == "mute")
                extractkey("1volumemute;2volumemute;#");

            else if (input == "plus")
                extractkey("1volumeup;2volumeup;#");

            else if (input == "minus")
                extractkey("1volumedown;2volumedown;#");


            else if (input == "play")
                extractkey("1mediaplaypause;2mediaplaypause;#");

            else if (input == "stop")
                extractkey("1mediastop;2mediastop;#");

            else if (input == "next")
                extractkey("1medianexttrack;2medianexttrack;#");

            else if (input == "previous")
                extractkey("1mediaprevioustrack;2mediaprevioustrack;#");

             //browser keys...................

            else if (input == "chrome")
                extractkey("1BrowserHome;2BrowserHome;#");
            
            else if (input == "chromeincognito")
                extractkey("1ControlKey;1LShiftKey;1N;2N;2ControlKey;2LShiftKey;#");
           

            else if (input == "history")
                extractkey("1ControlKey;1H;2H;2ControlKey;#");

            else if (input == "downloads")
                extractkey("1ControlKey;1J;2J;2ControlKey;#");



            else if (input == "newtab")
                extractkey("1ControlKey;1T;2T;2ControlKey;#");

            else if (input == "closetab")
                extractkey("1ControlKey;1W;2W;2ControlKey;#");            

             
            else if (input == "newwindow")
                extractkey("1ControlKey;1N;2N;2ControlKey;#");

            else if (input == "search")
                extractkey("1ControlKey;1H;2H;2ControlKey;#");
           
            else if (input == "nexttab")
                extractkey("1ControlKey;1H;2H;2ControlKey;#");

            else if (input == "previoustab")
                extractkey("1ControlKey;1H;2H;2ControlKey;#");

        }


        private ImageCodecInfo GetEncoder(ImageFormat format)
        {

            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }





        private void sendscreen()
        {



            ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);

            System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
            EncoderParameters myEncoderParameters = new EncoderParameters(1);

            EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 50L);
            myEncoderParameters.Param[0] = myEncoderParameter;
            var buffer = new byte[1024 * 8];
            var screenBmp = new Bitmap(screenwidth, screenheight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            var bmpGraphics = Graphics.FromImage(screenBmp);


            int count = 1;

            using (MemoryStream ms = new MemoryStream())
            {
                bmpGraphics.CopyFromScreen(0, 0, 0, 0, new System.Drawing.Size(screenwidth, screenheight));

                screenBmp.Save(ms, jpgEncoder, myEncoderParameters);



                s.Write(BitConverter.GetBytes(ms.Length), 0, 8);
                long numberOfBytes = ms.Length;
                long bytesReceived = 0;
                ms.Position = 0;

                while (bytesReceived < numberOfBytes && (count = ms.Read(buffer, 0, buffer.Length)) > 0)
                {

                    s.Write(buffer, 0, count);
                    bytesReceived += count;
                }

                //   Debug.WriteLine(ms.Length + " : "+ bytesReceived);
                // sw.WriteLine("sent");
            }
            //  sw.WriteLine("!bro");
        }

        /////////////END of CODE//////////////////

    }
}   
