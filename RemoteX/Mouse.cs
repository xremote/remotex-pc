using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace RemoteX
{
    public partial class MainWindow
    {
        //Mouse
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);

        [DllImport("User32.dll")]
        private static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetCursorPos(out POINT lpPoint);

        private const int MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const int MOUSEEVENTF_LEFTUP = 0x0004;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x0008;
        private const int MOUSEEVENTF_RIGHTUP = 0x0010;
        private const int MOUSEEVENTF_MIDDLEDOWN = 0X0020;
        private const int MOUSEEVENTF_MIDDLEUP = 0X0040;


        private const int LEFT_CLICK_DOWN = 0;
        private const int LEFT_CLICK_UP = 1;
        private const int RIGHT_CLICK_DOWN = 2;
        private const int RIGHT_CLICK_UP = 3;
        private const int MIDDLE_CLICK_DOWN = 4;
        private const int MIDDLE_CLICK_UP = 5;
        private const int LEFT_SINGLE_CLICK = 6;
        private const int LEFT_DOUBLE_CLICK = 7;


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



        private POINT GetMousePos()
        {
            POINT cursorpos = new POINT { X = 0, Y = 0 }; //default cursor positions
            GetCursorPos(out cursorpos);
            return cursorpos;
        }



        public void SetCursor(int x1, int y1)
        {
            SetCursorPos(x1, y1);
        }




        public void DoMouseClick(uint X, uint Y, int operation)
        {
            switch (operation)
            {
                case LEFT_CLICK_DOWN:
                    {
                        mouse_event(MOUSEEVENTF_LEFTDOWN, X, Y, 0, 0);
                        break;
                    }
                case LEFT_CLICK_UP:
                    {
                        mouse_event(MOUSEEVENTF_LEFTUP, X, Y, 0, 0);
                        break;
                    }
                case RIGHT_CLICK_DOWN:
                    {
                        mouse_event(MOUSEEVENTF_RIGHTDOWN, X, Y, 0, 0);
                        break;
                    }
                case RIGHT_CLICK_UP:
                    {
                        mouse_event(MOUSEEVENTF_RIGHTUP, X, Y, 0, 0);
                        break;
                    }
                case MIDDLE_CLICK_DOWN:
                    {
                        mouse_event(MOUSEEVENTF_MIDDLEDOWN, X, Y, 0, 0);
                        break;
                    }
                case MIDDLE_CLICK_UP:
                    {
                        mouse_event(MOUSEEVENTF_MIDDLEUP, X, Y, 0, 0);
                        break;
                    }
                case LEFT_SINGLE_CLICK:
                    {
                        mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, X, Y, 0, 0);
                        break;
                    }
                case LEFT_DOUBLE_CLICK:
                    {
                        mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, X, Y, 0, 0);
                        mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, X, Y, 0, 0);
                        break;
                    }
                default:
                    {
                        break;
                    }
            }

        }


        public void movemouse(String mouseinput)
        {

            if (mouseinput != null)
            {
                POINT cursorpos = GetMousePos();
                string[] m_input = mouseinput.Split(';');

                int x = int.Parse(m_input[0]);
                int y = int.Parse(m_input[1]);
                int click_operation = int.Parse(m_input[2]);

                cursorpos.X += x; cursorpos.Y += y;

                if (cursorpos.X >= G_screenwidth)
                {
                    cursorpos.X = G_screenwidth - 1;
                }

                if (cursorpos.Y >= G_screenheight)
                {
                    cursorpos.Y = G_screenheight - 1;
                }

                if (cursorpos.X < 0)
                {
                    cursorpos.X = 0;
                }

                if (cursorpos.Y < 0)
                {
                    cursorpos.Y = 0;
                }

                SetCursorPos(cursorpos.X, cursorpos.Y);
                DoMouseClick((uint)cursorpos.X, (uint)cursorpos.Y, click_operation);
            }


        }


        public void screen_mouse(String mouseinput)
        {

            if (mouseinput != null)
            {
                POINT cursorpos = GetMousePos();
                string[] m_input = mouseinput.Split(';');

                int x = int.Parse(m_input[0]);
                int y = int.Parse(m_input[1]);
                int click_operation = int.Parse(m_input[2]);
                int enter_key = int.Parse(m_input[3]);

                if (enter_key == 1 || click_operation == 2)
                {
                    if ((x == -1 && y == -1))
                    { x = -3; y = -3; }
                }


                if (enter_key == 1)
                {
                    KeyboardSend.KeyDown(Keys.Enter);
                    KeyboardSend.KeyUp(Keys.Enter);
                }

                if (!(x == -1 && y == -1))
                {
                    if (x != -3)
                    {
                        cursorpos.X = (x * G_screenwidth) / 2000;
                        cursorpos.Y = (y * G_screenheight) / 2000;

                        if (cursorpos.X >= G_screenwidth)
                        {
                            cursorpos.X = G_screenwidth - 1;
                        }

                        if (cursorpos.Y >= G_screenheight)
                        {
                            cursorpos.Y = G_screenheight - 1;
                        }

                        if (cursorpos.X < 0)
                        {
                            cursorpos.X = 0;
                        }

                        if (cursorpos.Y < 0)
                        {
                            cursorpos.Y = 0;
                        }

                        SetCursorPos(cursorpos.X, cursorpos.Y);
                    }

                    if (click_operation == 2)
                    {
                        DoMouseClick((uint)cursorpos.X, (uint)cursorpos.Y, RIGHT_CLICK_DOWN);
                        DoMouseClick((uint)cursorpos.X, (uint)cursorpos.Y, RIGHT_CLICK_UP);
                    }
                    else
                    {
                        DoMouseClick((uint)cursorpos.X, (uint)cursorpos.Y, LEFT_SINGLE_CLICK);
                    }
                }

            }



        }

    }
}
