using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RemoteX
{
    public partial class MainWindow
    {

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
            NetworkStream clientStream = null;
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

                    if (disconnect)
                    {
                        disconnect = false;
                        break;
                    }
                    str2 = sr.ReadLine();


                    if (str2 != null)
                    {
                        Debug.WriteLine(str2);
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
    }
}
