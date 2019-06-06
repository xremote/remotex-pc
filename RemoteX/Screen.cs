using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteX
{
    public partial class MainWindow
    {
        public void screenshot()
        {
            using (Bitmap bitmap = new Bitmap(G_screenwidth, G_screenheight))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen(0, 0, 0, 0, new System.Drawing.Size(G_screenwidth, G_screenheight));
                }
                var desktopFolder = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                var screenshotpath = Path.Combine(desktopFolder, "Screenshot_RD_" + DateTime.Now.ToString("yyyyMMddHHmmssffff") + ".jpg");
                bitmap.Save(screenshotpath, ImageFormat.Jpeg);
            }
        }


        private ImageCodecInfo GetEncoder(ImageFormat format)
        {

            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }
        
        private void sendscreen()
        {
            
            ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);

            System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
            EncoderParameters myEncoderParameters = new EncoderParameters(1);

            EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 50L);
            myEncoderParameters.Param[0] = myEncoderParameter;
            var buffer = new byte[1024 * 8];
            var screenBmp = new Bitmap(G_screenwidth, G_screenheight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            var bmpGraphics = Graphics.FromImage(screenBmp);


            int bytes_read = 1;

            using (MemoryStream ms = new MemoryStream())
            {
                bmpGraphics.CopyFromScreen(0, 0, 0, 0, new System.Drawing.Size(G_screenwidth, G_screenheight));

                screenBmp.Save(ms, jpgEncoder, myEncoderParameters);
                
                G_stream.Write(BitConverter.GetBytes(ms.Length), 0, 8);
                long numberOfBytes = ms.Length;
                long bytesReceived = 0;
                ms.Position = 0;

                while (bytesReceived < numberOfBytes && (bytes_read = ms.Read(buffer, 0, buffer.Length)) > 0)
                {
                    G_stream.Write(buffer, 0, bytes_read);
                    bytesReceived += bytes_read;
                }
                
            }
        }



    }
}
