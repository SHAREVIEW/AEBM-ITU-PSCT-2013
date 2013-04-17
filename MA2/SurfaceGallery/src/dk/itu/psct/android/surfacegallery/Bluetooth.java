package dk.itu.psct.android.surfacegallery;

import java.io.ByteArrayOutputStream;
import java.io.DataOutputStream;
import java.io.File;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.lang.reflect.InvocationTargetException;
import java.lang.reflect.Method;
import java.nio.ByteBuffer;
import java.util.ArrayList;
import java.util.UUID;

import android.bluetooth.BluetoothAdapter;
import android.bluetooth.BluetoothDevice;
import android.bluetooth.BluetoothServerSocket;
import android.bluetooth.BluetoothSocket;

import android.content.Intent;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.os.Environment;
import android.provider.MediaStore;
import android.util.Log;

public class Bluetooth {
	private static int REQUEST_ENABLE_BT = 0;
	private UUID uuid = UUID.fromString("a60f35f0-b93a-11de-8a39-08002009c666");
	private UUID surfaceUUID = UUID.fromString("86C189CD-4E28-4CB2-B556-360F3E8E261B");
	BluetoothAdapter myBt;
	BluetoothServerSocket bss;
	MainActivity a;
	//Main necessary methods
	// get the bluetooth device reference


	public Bluetooth(MainActivity a)
	{
		this.a = a;
	}
	
	
	public BluetoothAdapter findMyBT(){
		myBt = BluetoothAdapter.getDefaultAdapter();
		if (myBt!= null) {
			Log.i("BTDEV","BT Device Found!");
			Log.i("BTDEV","" + myBt.getName());
			return myBt;
		} else {
			Log.i("BTDEV","No BT Device Found!");
			return null;
		}
	}
	
	//ask to enable BT to user
	public void enableMyBT() {
		if ( ! myBt.isEnabled()) {
			Intent enableBtIntent = new Intent(BluetoothAdapter.ACTION_REQUEST_ENABLE);
			a.startActivityForResult(enableBtIntent, REQUEST_ENABLE_BT);
		}	
	}
	
	//ask to be visible for <visibility> seconds.
	public void beVisible() {
		Log.i("BTDEV","Let me be visible. thanks.");
		Intent discoverableIntent = new Intent(BluetoothAdapter.ACTION_REQUEST_DISCOVERABLE);
		discoverableIntent.putExtra(BluetoothAdapter.EXTRA_DISCOVERABLE_DURATION, 360);
		a.startActivity(discoverableIntent);
	}
	
	private byte[] extractImageAsByteArr(InputStream inputStream, int size) throws IOException {	
		ByteArrayOutputStream baos = new ByteArrayOutputStream();				
		byte[] buffer = new byte[size];
		int read = 0;
		try {
		while ((read = inputStream.read(buffer, 0, buffer.length)) != -1) {
			baos.write(buffer, 0, read);
		}		
		} catch (Exception e)
		{
			// done reading - LOL fuckedupway.
			inputStream.close();
		}
		baos.flush();		
		byte[] retValue = baos.toByteArray();
		baos.close();
		return retValue;
	}
	 
	///start listening for incoming data
	//this method must run inside a thread.
	Bitmap mutableBitmap;
	void listenForConnection() {
		try {
			if (bss == null)
				bss = myBt.listenUsingRfcommWithServiceRecord("SURFACEIMGSHARE", uuid);
			
			BluetoothSocket bs = bss.accept();
			InputStream is = bs.getInputStream();
			int imgSize = extractImageSize(is);
			if (imgSize > 0)
			{
				byte[] data = extractImageAsByteArr(is, imgSize);

				Bitmap bmp;
				bmp = BitmapFactory.decodeByteArray(data, 0, data.length);
				saveImage(bmp);

			} else if(imgSize == 0) {
				try {
				sendImages(new DataOutputStream(bs.getOutputStream()));
				bs.getOutputStream().close();
				} catch (Exception e)
				{
				}
			}
		} catch (IOException e) {
			e.printStackTrace();
		}
	}
	
	void sendImages(DataOutputStream os)
	{
//		BluetoothSocket btSocket =  null;
		try {
//			BluetoothDevice device = myBt.getRemoteDevice(address);
//
//			Method m = device.getClass().getMethod("createRfcommSocket", new Class[] {int.class});
//			btSocket = (BluetoothSocket) m.invoke(device, 1); // Apparently this is a workaround if UUID does not work.. Reflection.. eurgh.
//			myBt.cancelDiscovery(); // Expensive and not needed.
//			btSocket.connect();
		
					
			ArrayList<String> imgPaths = a.getImagePaths();

			os.writeInt(imgPaths.size());
			
			for (String imgPath : imgPaths)
			{
				try {
					Bitmap img = ImgUtil.decodeSampledBitmapFromResource(imgPath, 200, 200);
					if (img == null) continue;
					ByteArrayOutputStream baos = new ByteArrayOutputStream();
					img.compress(Bitmap.CompressFormat.JPEG, 100, baos);
	
					byte[] imgAsByte = baos.toByteArray();
					Log.w("SURFACELOL", "WRITING TO STREAM");
					os.writeInt(imgAsByte.length);
					os.write(imgAsByte);
				} catch (Exception e)
				{
					
				} 

			}
		} catch (IOException e) {
			e.printStackTrace();
		} finally {

		}
	}
	
	
	/**
	 * Reads the type and returns size of image if it is an image.
	 * @param stream
	 * @return returns the size of the image. If there is no image (it is a send-images-request), it returns 0.
	 * @throws IOException
	 */
	int extractImageSize(InputStream stream)
	{
		try {
			return readInt(stream);
		} catch (Exception e)
		{
			return -1;
		}
	}
	
	int readInt(InputStream stream) throws IOException
	{				
		byte[] buffer = new byte[4];
		int read = 0;
		read = stream.read(buffer, 0, buffer.length);
		ByteBuffer wrapped = ByteBuffer.wrap(buffer); 
		int num = wrapped.getInt();
		return num;
	}
	
	void saveImage(Bitmap img) {
		if (img == null) return;
        File filename;
        try {
            String path = Environment.getExternalStorageDirectory().toString();

            new File(path + "/DCIM/100MEDIA/").mkdirs();
            filename = new File(path + "/DCIM/100MEDIA/" + UUID.randomUUID() +".jpg");
            filename.createNewFile();
            FileOutputStream out = new FileOutputStream(filename);

            img.compress(Bitmap.CompressFormat.JPEG, 90, out);
            out.flush();
            out.close();

            MediaStore.Images.Media.insertImage(a.getContentResolver(),
                    filename.getAbsolutePath(), filename.getName(),
                    filename.getName());
            a.addImage(filename.getAbsolutePath());
        } catch (Exception e) {
            e.printStackTrace();
        }

    }
	
}
