﻿using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace RemoteX
{
    public partial class MainWindow
    {
        public Boolean G_threadrunning = false;
        public string G_pcname = "", G_pcIP = "", G_remotename = "", G_remoteip = "";

        public Boolean G_disconnect = false;
        static TcpListener G_listener;
        public NetworkStream G_stream;
        public StreamReader G_streamreader;
        public StreamWriter streamwriter;
        Socket G_socket = null;

        private void start_thread()
        {
            try
            {
                Thread Listener_thread = new Thread(new ThreadStart(acceptingsockets));
                Listener_thread.Start();
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
                G_socket = G_listener.AcceptSocket();
            }
            catch (Exception f)
            {
                Debug.WriteLine(f.Message);
            }


            NetworkStream clientStream = null;
            try
            {
                clientStream = new NetworkStream(G_socket);
                G_stream = new NetworkStream(G_socket);
                G_streamreader = new StreamReader(G_stream);
                streamwriter = new StreamWriter(G_stream);
                streamwriter.AutoFlush = true;
                streamwriter.Flush();
                string networkmessage = "";

                while (true) // keep listening for messages until disconnects
                {

                    if (G_disconnect)
                    {
                        G_disconnect = false;
                        break;
                    }
                    networkmessage = G_streamreader.ReadLine();


                    if (networkmessage != null)
                    {
                        Debug.WriteLine(networkmessage);
                        if (networkmessage[0] == '!')
                        {
                            screen_mouse(networkmessage.Substring(1, networkmessage.Length - 1) + ";#");  //screensharing
                            sendscreen();
                        }
                        else if (networkmessage[0] == '*')
                        {
                            movemouse(networkmessage.Substring(1, networkmessage.Length - 1) + "#");   //mouse 
                        }
                        else if (networkmessage[0] == '@')
                        {
                            extractkey(networkmessage.Substring(1, networkmessage.Length - 1) + "#");    //keyboard
                        }
                        else if (networkmessage[0] == '%')
                        {
                            shortcut(networkmessage.Substring(1, networkmessage.Length - 1));        // shortcutkeys
                        }
                        else if (networkmessage[0] == '$')
                        {
                            sendsysinfo(networkmessage.Substring(1, networkmessage.Length - 1));    //system info
                        }
                        else if (networkmessage[0] == '&')
                        {
                            getdriveinfo(networkmessage.Substring(1, networkmessage.Length - 1));   //explorer
                        }
                        else if (networkmessage[0] == '^')
                        {
                            specialaction(networkmessage.Substring(1, networkmessage.Length - 1)); //special actions
                        }
                    }
                    else
                    {
                        break;
                    }
                }


            }
            catch (Exception e)
            {
                Debug.WriteLine("every thing ends: " + e.Message);
            }

            try
            {

                streamwriter.Close();
                G_streamreader.Close();
                G_stream.Close();
                streamwriter = null;
                G_streamreader = null;
                G_stream = null;
                clientStream.Close();
                G_socket.Shutdown(SocketShutdown.Both);
                G_socket.Close();
                G_socket = null;

            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            G_threadrunning = false;
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
