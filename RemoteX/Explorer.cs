using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace RemoteX
{
    public partial class MainWindow
    {
        private const int EXPLORER_COMPUTER = 0;
        private const int EXPLORER_DRIVE = 1;
        private const int EXPLORER_FILE = 2;

        public void explorer_actions(String _input)
        {
            String drive_info = null;

            if (_input != null)
            {
                int explorer_case = _input[0] - '0';
                _input = _input.Substring(1, _input.Length - 1);

                switch (explorer_case)
                {
                    case EXPLORER_COMPUTER:
                        {
                            foreach (var drive in DriveInfo.GetDrives())
                            {
                                try
                                {
                                    if (drive.TotalSize == 0 || drive.Name == null || drive.Name == "")
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
                                {
                                    drive_info += EXPLORER_DRIVE + drive.Name + ";";
                                }
                            }

                            if (drive_info == null)
                            {
                                drive_info = "";
                            }
                            else
                            {
                                drive_info = drive_info.Substring(0, drive_info.Length - 1); //remove last symbol ';'
                            }

                            G_streamwriter.WriteLine(drive_info);
                            break;
                        }

                    case EXPLORER_DRIVE:
                        {
                            try
                            {

                                string[] fileEntries = Directory.GetFiles(_input);
                                string[] folderentries = Directory.GetDirectories(_input);

                                foreach (string folder in folderentries)
                                {

                                    drive_info += EXPLORER_DRIVE + Path.GetFileName(folder) + ";";
                                }



                                foreach (string file in fileEntries)
                                {
                                    if (isreadable(file))
                                    {
                                        drive_info += EXPLORER_FILE + Path.GetFileName(file) + ";";
                                    }
                                }

                            }
                            catch (Exception e)
                            {
                                Debug.WriteLine(e.Message);
                            }




                            if (drive_info == null)
                            {
                                drive_info = "";
                            }
                            else
                            {
                                drive_info = drive_info.Substring(0, drive_info.Length - 1); //remove last ;
                            }

                            G_streamwriter.WriteLine(drive_info);
                            break;
                        }
                    case EXPLORER_FILE:
                        {
                            try
                            {
                                G_sendfile_thread = new Thread(() => sendfile(_input));
                                if (File.Exists(_input) && G_sendfile_thread.ThreadState!=0) // not running
                                {
                                    //send file from a thread
                                    try
                                    {                                           
                                        G_sendfile_thread.Start();
                                    }
                                    catch (Exception f)
                                    {
                                        Debug.WriteLine("catch " + f.Message);
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                Debug.WriteLine("file not sent : " + e.ToString());
                            }

                            break;
                        }
                    default:
                        {
                            G_streamwriter.WriteLine(drive_info);
                            break;
                        }


                }
            }

        }


        public Boolean isreadable(string filename)
        {
            try
            {
                using (FileStream stream = File.Open(filename, FileMode.Open, FileAccess.Read))
                {
                    stream.Close();
                    return true;
                }
            }
            catch (IOException)
            {
                return false; // <- File failed to open
            }
        }

        private void sendfile(string filename)
        {
            MemoryStream memory_stream = new MemoryStream();
            System.IO.FileStream file_stream=null;

            try
            {

                int bytes_read = 0;
                Int64 numberOfBytes = 0, bytesReceived = 0;
                 memory_stream = new MemoryStream();
                file_stream = new FileStream(filename, FileMode.Open, FileAccess.Read);
                var buffer = new byte[1024 * 128];

                memory_stream.Position = 0;
                file_stream.Position = 0;

                file_stream.CopyTo(memory_stream);
                file_stream.Close();

                G_stream.Write(BitConverter.GetBytes(memory_stream.Length), 0, 8);

                if (memory_stream.Length > 0)
                {
                    Debug.WriteLine("send file " + memory_stream.Length);
                    memory_stream.Position = 0;
                    numberOfBytes = memory_stream.Length;
                    G_stream.WriteTimeout = 1500;
                    while (bytesReceived < numberOfBytes && (bytes_read = memory_stream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        try
                        {
                            G_stream.Write(buffer, 0, bytes_read);
                            Debug.WriteLine("Sending");

                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine(e.ToString());
                        }
                        bytesReceived += bytes_read;
                        Debug.WriteLine("sent " + bytesReceived);
                    }
                    G_stream.WriteTimeout = System.Threading.Timeout.Infinite;
                }
            }
            finally
            {
                //free all memory leakages
                Debug.WriteLine("clear sent file memory leakages");
                long ln = -1;
                try
                {
                    G_stream.Write(BitConverter.GetBytes(ln), 0, 8);
                }
                catch(Exception e)
                {
                    Debug.WriteLine(e);
                }
                
                memory_stream.Close();
                memory_stream.Dispose();
                memory_stream = null;
                if (file_stream != null)
                {
                    file_stream.Close();
                    file_stream.Dispose();
                    file_stream = null;
                }
            }
        }
    }
}
