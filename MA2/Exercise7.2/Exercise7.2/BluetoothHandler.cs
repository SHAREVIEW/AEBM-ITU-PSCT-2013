using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InTheHand.Net;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;
using System.Net.Sockets;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Exercise7._2
{
    class BluetoothHandler
    {
        static Guid serviceGuid = new Guid("86C189CD-4E28-4CB2-B556-360F3E8E261B");
        public static byte[] BmSourceToByteArr(BitmapSource bms)
        {
            BitmapEncoder encoder = new JpegBitmapEncoder();
            using (MemoryStream stream = new MemoryStream())
            {
                byte[] bit = new byte[0];
                encoder.Frames.Add(BitmapFrame.Create(bms));
                encoder.Save(stream);
                bit = stream.ToArray();
                stream.Close();
                return bit;
            }
        }

        public static void SendBluetooth(BluetoothEndPoint endpoint, BitmapSource bms)
        {
            InTheHand.Net.Sockets.BluetoothClient btc = new InTheHand.Net.Sockets.BluetoothClient();
            btc.Connect(endpoint);
            byte[] img = BmSourceToByteArr(bms);
            var nws = btc.GetStream();
            byte[] imgSize = BitConverter.GetBytes(img.Length);
            if (BitConverter.IsLittleEndian) Array.Reverse(imgSize);
            nws.Write(imgSize, 0, imgSize.Length); // write image size
            nws.Write(img, 0, img.Length); // Write image
            nws.Flush();
        }

        public static void GetImages(BluetoothEndPoint endpoint, Color tagColor)
        {
            InTheHand.Net.Sockets.BluetoothClient btc = new InTheHand.Net.Sockets.BluetoothClient();
            btc.Connect(endpoint);
            var nws = btc.GetStream();
            byte[] emptySize = BitConverter.GetBytes(0); //
            if (BitConverter.IsLittleEndian) Array.Reverse(emptySize); // redundant but usefull in case the number changes later..
            nws.Write(emptySize, 0, emptySize.Length); // write image size
            int imgCount = GetImgSize(nws);
            nws = btc.GetStream();
            for (int i = 0; i < imgCount; i++)
            {
                MemoryStream ms = new MemoryStream();
                int size = GetImgSize(nws);
                if (size == 0) continue;
                byte[] buffer = new byte[size];
                int read = 0;

                while ((read = nws.Read(buffer, 0, buffer.Length)) != 0)
                {
                    ms.Write(buffer, 0, read);
                }
                SurfaceWindow1.AddImage(System.Drawing.Image.FromStream(ms), tagColor);
            }
        }

        static int GetImgSize(NetworkStream nws)
        {
            byte[] buffer = new byte[4];
            int i = 4;
            while (i > 0)
            {
                int read = nws.Read(buffer, 0, i);
                i = i - read;
            }
            
            if (BitConverter.IsLittleEndian) Array.Reverse(buffer);
            int ret = BitConverter.ToInt32(buffer, 0);

            return ret;
        }
    }
}
