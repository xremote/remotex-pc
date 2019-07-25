using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;

namespace RemoteX
{
    public partial class MainWindow
    {
        //special controls 
        String G_timedelay = "10"; // for shutdown restart cmd commands
        public Dictionary<string, string> G_SystemInfo = new Dictionary<string, string>();

        public void specialaction(String action)
        {
            int volume = 50; //default value
            String[] _action = action.Split(';');
            if (_action[0] == "setvolume")
            {
                action = _action[0];
                volume = Convert.ToInt32(_action[1]);
            }
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
                case "setvolume":
                    setvolume(volume);
                    break;
            }

        }

        [DllImport("user32.dll")]
        public static extern bool LockWorkStation();
        public void lockuser()
        {
            LockWorkStation();
        }
        
        public void setvolume(int value)
        {
            G_defaultPlaybackDevice.Volume = value;            
        }

        public void shutdown()
        {
            var psi = new ProcessStartInfo("shutdown", "/s /t " + G_timedelay);
            psi.CreateNoWindow = true;
            psi.UseShellExecute = false;
            Process.Start(psi);
        }

        public void restart()
        {
            var psi = new ProcessStartInfo("shutdown", "/r /t " + G_timedelay);
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
            const string message = "42";
            const string caption = "Wait";
            var result = System.Windows.MessageBox.Show(message, caption, MessageBoxButton.OK);
        }


        public void shortcut(String input)
        {
            Debug.WriteLine(input);
            Debug.WriteLine(input.Length);
            Debug.WriteLine(input.GetType());
            Debug.WriteLine(input.Clone());
            String tmp = "";
            for(int i = 0; i < input.Length; i++)
            {
                Debug.WriteLine(i);
                Debug.WriteLine(input[i]);
            }
            

            Debug.WriteLine(input.IsNormalized() + " " + "d ".IsNormalized() + input.Length);
            //shortcut keys......................

            if (input == "close")
            {
                extractkey("1Lmenu;1F4;2F4;2Lmenu;#");
            }
            else if (input == "switch1")
            {
                extractkey("1Lmenu;1Tab;2Tab;2Lmenu;#");
            }
            else if (input == "search")
            {
                extractkey("1F3;2F3;#");
            }
            else if (input == "help")
            {
                extractkey("1F1;2F1;#");
            }
            else if (input == "explorer")
            {
                extractkey("1Lwin;1E;2Lwin;2E;#");
            }
            else if (input == "utility")
            {
                extractkey("1Lwin;1U;2Lwin;2U;#");
            }
            else if (input == "sysinfo")
            {
                extractkey("1F3;2F3;#");
            }

            //media keys ............................

            else if (input == "playlist")
            {
                extractkey("1ControlKey;1Lmenu;1Q;2Q;2Lmenu;2ControlKey;#");
            }
            else if (input == "media")
            {
                extractkey("1Selectmedia;2Selectmedia;#");
            }

            else if (input == "mute")
            {
                extractkey("1volumemute;2volumemute;#");
            }
            else if (input == "plus")
            {
                extractkey("1volumeup;2volumeup;#");
            }
            else if (input == "minus")
            {
                extractkey("1volumedown;2volumedown;#");
            }

            else if (input == "play")
            {
                extractkey("1mediaplaypause;2mediaplaypause;#");
            }
            else if (input == "stop")
            {
                extractkey("1mediastop;2mediastop;#");
            }
            else if (input == "next")
            {
                extractkey("1medianexttrack;2medianexttrack;#");
            }
            else if (input == "previous")
            {
                extractkey("1mediaprevioustrack;2mediaprevioustrack;#");
            }

            //browser keys...................

            else if (input == "chrome")
            {
                extractkey("1BrowserHome;2BrowserHome;#");
            }
            else if (input == "chromeincognito")
            {
                extractkey("1ControlKey;1LShiftKey;1N;2N;2ControlKey;2LShiftKey;#");
            }

            else if (input == "history")
            {
                extractkey("1ControlKey;1H;2H;2ControlKey;#");
            }
            else if (input == "downloads")
            {
                extractkey("1ControlKey;1J;2J;2ControlKey;#");
            }


            else if (input == "newtab")
            {
                extractkey("1ControlKey;1T;2T;2ControlKey;#");
            }
            else if (input == "closetab")
            {
                extractkey("1ControlKey;1W;2W;2ControlKey;#");
            }

            else if (input == "newwindow")
            {
                extractkey("1ControlKey;1N;2N;2ControlKey;#");
            }
            else if (input == "search")
            {
                extractkey("1ControlKey;1H;2H;2ControlKey;#");
            }
            else if (input == "nexttab")
            {
                extractkey("1ControlKey;1H;2H;2ControlKey;#");
            }
            else if (input == "previoustab")
            {
                extractkey("1ControlKey;1H;2H;2ControlKey;#");
            }
        }




        //system info

        static ulong GetTotalMemoryInBytes()
        {
            return new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory;
        }

        private void refreshSystemInfo()
        {
            G_SystemInfo["volume"]  = G_defaultPlaybackDevice.Volume.ToString();
            G_SystemInfo["ipaddress"]  =  G_pcIP;
        }

        private void initializeSystemInfo()
        {
            //store system info for future use
            try
            {

                G_SystemInfo.Add("username", SystemInformation.UserName);
                G_SystemInfo.Add("computername", SystemInformation.ComputerName);
                G_SystemInfo.Add("monitorcount", SystemInformation.MonitorCount.ToString());
                G_SystemInfo.Add("monitorheight", SystemInformation.PrimaryMonitorSize.Height.ToString());
                G_SystemInfo.Add("monitorwidth", SystemInformation.PrimaryMonitorSize.Width.ToString());
                G_SystemInfo.Add("batterylife", (SystemInformation.PowerStatus.BatteryLifePercent).ToString());
                G_SystemInfo.Add("batterystatus", (SystemInformation.PowerStatus.BatteryChargeStatus).ToString());
                G_SystemInfo.Add("volume", G_defaultPlaybackDevice.Volume.ToString());                
            }
            catch (Exception e)
            {
                Debug.WriteLine("Unable to get System Info : " + e.ToString());
            }



            try
            {

                if (Environment.Is64BitOperatingSystem)
                {
                    G_SystemInfo.Add("osbit", "64 bit");
                }
                else
                {
                    G_SystemInfo.Add("osbit", "32 bit");
                }
                G_SystemInfo.Add("processorcount", Environment.ProcessorCount.ToString());
                G_SystemInfo.Add("osversion", Environment.OSVersion.ToString());
            }
            catch (Exception e)
            {
                Debug.WriteLine("Unable to get System Info : " + e.ToString());
            }



            try
            {
                G_SystemInfo.Add("osname", new Microsoft.VisualBasic.Devices.ComputerInfo().OSFullName);
                G_SystemInfo.Add("totalmemory", (new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory / (1024.0 * 1024 * 1024)).ToString().Substring(0, 4) + " GB");
            }
            catch (Exception e)
            {
                Debug.WriteLine("Unable to get System Info : " + e.ToString());
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

                G_SystemInfo.Add("HDD-SIZE", totalhddsize.ToString() + " GB");
            }
            catch (Exception e)
            {
                Debug.WriteLine("Unable to get System Info : " + e.ToString());
            }


            G_SystemInfo.Add("ipaddress", G_pcIP);

            Debug.WriteLine("complete systeminfo " + MyDictionaryToJson(G_SystemInfo));

        }



        string MyDictionaryToJson(Dictionary<string, string> dict)
        {
            //convert dictionary data into json and return json as string
            String Jsondata = "{";
            foreach (KeyValuePair<string, string> entry in dict)
            {
                Jsondata += "\"" + entry.Key + "\":" + "\"" + entry.Value + "\"" + ",";
            }
            Jsondata = Jsondata.Substring(0, Jsondata.Length - 1);
            Jsondata += "}";
            return Jsondata;
        }

        //send system info to android for main screen

        public void sendsysinfo(String infotype)
        {
            refreshSystemInfo();
            String json_sys_info = MyDictionaryToJson(G_SystemInfo);
            if (json_sys_info != null)
            {
                crypt_WriteLine(json_sys_info);
            }

        }


        //end of special controls
        //------------------------------------------------------------------------------------------------------------------

    }
}
