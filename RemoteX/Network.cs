using System;
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
        public StreamWriter G_streamwriter;
        Socket G_socket = null;
        Thread G_Listener_thread;
        Thread G_sendfile_thread;

        private void start_thread()
        {
            try
            {
                G_Listener_thread = new Thread(new ThreadStart(acceptingsockets));
                G_Listener_thread.Start();
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
                Debug.WriteLine("Listening......");
                G_socket = G_listener.AcceptSocket();
            }
            catch (Exception f)
            {
                Debug.WriteLine(f.Message);
            }



            try
            {
                G_stream = new NetworkStream(G_socket);
                G_streamreader = new StreamReader(G_stream);
                G_streamwriter = new StreamWriter(G_stream);
                G_streamwriter.AutoFlush = true;
                G_streamwriter.Flush();
                string networkmessage = "";

                while (true) // keep listening for messages until disconnects
                {

                    if (G_disconnect)
                    {
                        disconnect_network();
                        break;
                    }
                    Debug.WriteLine("Waiting for msg....");
                    
                    networkmessage = crypt_ReadLine();
                    
                    Debug.WriteLine(networkmessage);
                    if (networkmessage != null && networkmessage.Length>0)
                    {
                        if (networkmessage.Equals("syncback"))
                        {
                            if (G_sendfile_thread!=null &&  G_sendfile_thread.IsAlive)
                            {
                                Debug.WriteLine("closed file thread");
                                G_sendfile_thread.Abort();
                                G_sendfile_thread = null;
                            }
                        }
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
                            explorer_actions(networkmessage.Substring(1, networkmessage.Length - 1));   //explorer
                        }
                        else if (networkmessage[0] == '^')
                        {
                            specialaction(networkmessage.Substring(1, networkmessage.Length - 1)); //special actions
                        }
                    }
                    else
                    {
                        Debug.WriteLine("what");
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
                G_disconnect = true;
                disconnect_network();

            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

        }


        public void disconnect_network()
        {

            try
            {
                G_streamwriter.Close();
                G_streamwriter = null;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }

            try
            {
                G_streamreader.Close();
                G_streamreader = null;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }

            try
            {
                G_stream.Close();
                G_stream = null;

            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
            
            try
            {
                G_socket.Shutdown(SocketShutdown.Both);
                G_socket.Close();
                G_socket = null;


            }
            catch (Exception e)
            {
                Debug.WriteLine("socket shutdown " + e);
            }

            try
            {
                G_Listener_thread.Abort();

            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }

            G_threadrunning = false;
            G_disconnect = false;
            Debug.WriteLine("Disconneted");
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
