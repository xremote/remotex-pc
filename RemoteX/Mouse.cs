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



        //mouse

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

            int x1, y1, z1, r1;
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
                    KeyboardSend.KeyDown(Keys.Enter); KeyboardSend.KeyUp(Keys.Enter);

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

    }
}
