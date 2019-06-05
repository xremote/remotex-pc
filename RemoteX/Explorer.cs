using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RemoteX
{
    public partial class MainWindow
    {


        static ulong GetTotalMemoryInBytes()
        {
            return new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory;
        }

        private void initializeSystemInfo()
        {
            try
            {

                SystemInfo.Add("username", SystemInformation.UserName);
                SystemInfo.Add("computername", SystemInformation.ComputerName);
                SystemInfo.Add("monitorcount", SystemInformation.MonitorCount.ToString());
                SystemInfo.Add("monitorheight", SystemInformation.PrimaryMonitorSize.Height.ToString());
                SystemInfo.Add("monitorwidth", SystemInformation.PrimaryMonitorSize.Width.ToString());

                PowerStatus pw = SystemInformation.PowerStatus;

                SystemInfo.Add("batterylife", (pw.BatteryLifePercent).ToString());
                SystemInfo.Add("batterystatus", (pw.BatteryChargeStatus).ToString());
            }
            catch (Exception e)
            {

            }



            try
            {

                if (Environment.Is64BitOperatingSystem)
                {
                    SystemInfo.Add("osbit", "64 bit");
                }
                else
                {
                    SystemInfo.Add("osbit", "32 bit");
                }
                SystemInfo.Add("processorcount", Environment.ProcessorCount.ToString());
                SystemInfo.Add("osversion", Environment.OSVersion.ToString());
            }
            catch (Exception e)
            {

            }



            try
            {
                SystemInfo.Add("osname", new Microsoft.VisualBasic.Devices.ComputerInfo().OSFullName);
                SystemInfo.Add("totalmemory", (new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory / (1024.0 * 1024 * 1024)).ToString().Substring(0, 4) + " GB");
            }
            catch (Exception e)
            {

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

                SystemInfo.Add("HDD-SIZE", totalhddsize.ToString() + " GB");
            }
            catch (Exception e)
            {

            }



            SystemInfo.Add("ipaddress", pcIP);

            Debug.WriteLine("sys " + MyDictionaryToJson(SystemInfo));


        }



        string MyDictionaryToJson(Dictionary<string, string> dict)
        {
            String Jsondata = "{";
            foreach (KeyValuePair<string, string> entry in dict)
            {
                Jsondata += "\"" + entry.Key + "\":" + "\"" + entry.Value + "\"" + ",";
            }
            Jsondata = Jsondata.Substring(0, Jsondata.Length - 1);
            Jsondata += "}";
            return Jsondata;
        }




        private void refreshSystemInfo()
        {

            //SystemInfo.Add("batterypercentage", );

        }






        public void getdriveinfo(String input)
        {
            String result = null;
            Debug.WriteLine("input: " + input);
            if (input != null)
            {
                int i = input[0] - '0';
                input = input.Substring(1, input.Length - 1);
                if (i == 0)
                {
                    try
                    {
                        foreach (var drive in DriveInfo.GetDrives())
                        {
                            long temp = 0;

                            try
                            {
                                temp = drive.TotalSize;
                                if (temp == 0 || drive.Name == null || drive.Name == "")
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
                                result = result + "1" + drive.Name + ";";

                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message);
                    }
                    if (result == null)
                        result = "";
                    else
                        result = result.Substring(0, result.Length - 1);


                    sw.WriteLine(result);

                }
                else if (i == 1)
                {
                    try
                    {

                        string[] fileEntries = Directory.GetFiles(input);
                        string[] folderentries = Directory.GetDirectories(input);

                        foreach (string folder in folderentries)
                        {

                            result = result + "1" + Path.GetFileName(folder) + ";";
                        }



                        foreach (string file in fileEntries)
                        {
                            if (isreadable(file))
                                result = result + "2" + Path.GetFileName(file) + ";";
                        }

                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message);
                    }



                    if (result != null)
                        result = result.Substring(0, result.Length - 1);
                    else
                        result = "empty";


                    sw.WriteLine(result);


                }
                else if (i == 2)
                {

                    if (File.Exists(input))
                    { //sw.WriteLine("catch"); 
                        sendfile(input);
                    }



                }
                else
                { sw.WriteLine(result); }

                Debug.WriteLine(result);
            }

        }


        //send system info to android for main screen

        public void sendsysinfo(String infotype)
        {
            String result = MyDictionaryToJson(SystemInfo);
            if (result != null)
            {
                sw.WriteLine(result);
                Debug.WriteLine(result);
            }

        }




        public Boolean isreadable(string filename)
        {
            try
            {
                using (FileStream stream = File.Open(filename, FileMode.Open, FileAccess.Read))
                {

                    return true;
                }
            }
            catch (IOException)
            {
                return false; // <- File failed to open
            }

        }

        public void sendfile(string input)
        {
            Debug.WriteLine(input);
            int count = 0;
            Int64 numberOfBytes = 0, bytesReceived = 0;
            MemoryStream ms = new MemoryStream();
            System.IO.FileStream fs = new FileStream(input, FileMode.Open, FileAccess.Read);
            var buffer = new byte[1024 * 128];
            ms.Position = 0;
            fs.Position = 0;

            fs.CopyTo(ms);

            fs.Close();

            Debug.WriteLine(ms.Length);
            s.Write(BitConverter.GetBytes(ms.Length), 0, 8);

            if (ms.Length > 0)
            {
                ms.Position = 0;
                numberOfBytes = ms.Length;

                while (bytesReceived < numberOfBytes && (count = ms.Read(buffer, 0, buffer.Length)) > 0)
                {

                    s.Write(buffer, 0, count);

                    bytesReceived += count;
                    //Debug.WriteLine(bytesReceived);
                }



            }
        }
    }
}
