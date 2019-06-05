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


    }
}
