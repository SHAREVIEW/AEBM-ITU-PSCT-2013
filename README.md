AEBM-ITU-PSCT-2013
==================

Pervasive Computing  2013 projects for aebm@itu.dk at the IT-University of Copenhagen


File: BluetoothHandler.cs  Project: EmilBechMadsen/AEBM-ITU-PSCT-2013
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
 
 
 
 https://csharp.hotexamples.com/examples/InTheHand.Net.Sockets/BluetoothClient/Connect/php-bluetoothclient-connect-method-examples.html
 
 EXAMPLE #7
