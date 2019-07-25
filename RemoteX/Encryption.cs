using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace RemoteX
{
    public partial class MainWindow
    {
        String EncryptData(String Plaintext, String Encryptionkey)
        {

            RijndaelManaged objrij = new RijndaelManaged();
            //set the mode for operation of the algorithm   
            objrij.Mode = CipherMode.CBC;
            //set the padding mode used in the algorithm.   
            objrij.Padding = PaddingMode.ISO10126;
            //set the size, in bits, for the secret key.   
            objrij.KeySize = 0x80;
            //set the block size in bits for the cryptographic operation.    
            objrij.BlockSize = 0x80;

            //set the symmetric key that is used for encryption & decryption.    
            byte[] passBytes = Encoding.UTF8.GetBytes(Encryptionkey);
            //set the initialization vector (IV) for the symmetric algorithm    
            byte[] EncryptionkeyBytes = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

            int len = passBytes.Length;
            if (len > EncryptionkeyBytes.Length)
            {
                len = EncryptionkeyBytes.Length;
            }
            Array.Copy(passBytes, EncryptionkeyBytes, len);

            objrij.Key = EncryptionkeyBytes;
            objrij.IV = EncryptionkeyBytes;

            //Creates symmetric AES object with the current key and initialization vector IV.    
            ICryptoTransform objtransform = objrij.CreateEncryptor();
            byte[] textDataByte = Encoding.UTF8.GetBytes(Plaintext);
            byte[] EncryptedBytes = objtransform.TransformFinalBlock(textDataByte, 0, textDataByte.Length);
            return Convert.ToBase64String(EncryptedBytes);
        }


        string DecryptData(string EncryptedText, string Encryptionkey)
        {
            Debug.WriteLine(EncryptedText + " enc");
            RijndaelManaged objrij = new RijndaelManaged();
            objrij.Mode = CipherMode.CBC;
            objrij.Padding = PaddingMode.ISO10126;

            objrij.KeySize = 0x80;
            objrij.BlockSize = 0x80;
            byte[] encryptedTextByte = Convert.FromBase64String(EncryptedText);
            byte[] passBytes = Encoding.UTF8.GetBytes(Encryptionkey);
            byte[] EncryptionkeyBytes = new byte[] 
            { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            int len = passBytes.Length;
            if (len > EncryptionkeyBytes.Length)
            {
                len = EncryptionkeyBytes.Length;
            }
            Array.Copy(passBytes, EncryptionkeyBytes, len);
            objrij.Key = EncryptionkeyBytes;
            objrij.IV = EncryptionkeyBytes;
            byte[] TextByte = objrij.CreateDecryptor().TransformFinalBlock(encryptedTextByte, 0, encryptedTextByte.Length);
            
            return Encoding.UTF8.GetString(TextByte);  //it will return readable string  
        }


        public void crypt_WriteLine(String plainString)
        {
            Debug.WriteLine("sendmsg " + plainString);
            String EncryptedData = EncryptData(plainString, Properties.Settings.Default.Password);
            Debug.WriteLine("sendmsg enc " + EncryptedData);
            G_streamwriter.WriteLine(EncryptedData);
        }


        public string crypt_ReadLine()
        {
            string e_msg = "";
            while(e_msg==null || e_msg.Length < 1)
            {
                e_msg = G_streamreader.ReadLine();
            }
            Debug.WriteLine("recieved " + e_msg);            
            string d_msg = DecryptData(e_msg, Properties.Settings.Default.Password);
            Debug.WriteLine("recieved dec " + d_msg);
            return d_msg;
        }

        public void crypt_Write(byte[] buffer, int offset, int len)
        {
            G_stream.Write(buffer, offset, len);
        }


    }
}
