using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RemoteX
{
    public partial class MainWindow
    {
        // Keyboard movements


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

        public void extractkey(string keyinput)
        {
            int key_status; // down/up

            if (keyinput != null)
            {
                string[] k_input = keyinput.Split(';');

                foreach (string key in k_input)
                {
                    if (key == null || key == "#")
                        break;
                    try
                    {
                        key_status = key[0] - '0';
                        typethis(key.Substring(1, key.Length - 1), key_status);
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message);
                    }
                }



            }
        }
                          
        public void typethis(string key, int key_status)
        {
            Keys mykey;
            mykey = (Keys)Enum.Parse(typeof(Keys), key, true);

            if (key_status == 1)
                KeyboardSend.KeyDown(mykey);
            else if (key_status == 2)
                KeyboardSend.KeyUp(mykey);
        }

    }
}
