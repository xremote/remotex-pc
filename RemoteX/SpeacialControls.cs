using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RemoteX
{
    public partial class MainWindow
    {


        //special controls 

        public void specialaction(String action)
        {
            int value = 50;
            String[] _action = action.Split(';');
            if (_action[0] == "setvolume")
            {
                action = _action[0];
                value = Convert.ToInt32(_action[1]);
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
                    setvolume(value);
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

        public void setvolume(int value)
        {
            defaultPlaybackDevice.Volume = value;

        }



        public void shutdown()
        {
            var psi = new ProcessStartInfo("shutdown", "/s /t " + timedelay);
            psi.CreateNoWindow = true;
            psi.UseShellExecute = false;
            Process.Start(psi);
        }

        public void restart()
        {
            var psi = new ProcessStartInfo("shutdown", "/r /t " + timedelay);
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


        //end of special controls
        //------------------------------------------------------------------------------------------------------------------

    }
}
