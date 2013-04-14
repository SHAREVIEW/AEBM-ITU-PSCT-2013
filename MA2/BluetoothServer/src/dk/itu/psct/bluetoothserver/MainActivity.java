package dk.itu.psct.bluetoothserver;

import android.os.Bundle;
import android.app.Activity;
import android.bluetooth.BluetoothAdapter;
import android.bluetooth.BluetoothServerSocket;
import android.bluetooth.BluetoothSocket;
import android.content.Intent;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.util.Log;
import android.view.Menu;
import android.widget.ImageView;

import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.util.UUID;


public class MainActivity extends Activity {

	BluetoothAdapter myBt;

	private UUID uuid = UUID.fromString("a60f35f0-b93a-11de-8a39-08002009c666");
	private static int REQUEST_ENABLE_BT = 0;

	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_main);
		
		findMyBT();
		enableMyBT();
		beVisible();
		new Thread()
		{
			public void run() {
				while (true)
				{
					listenForConnection();
				}
			}
		}.start();
		
	}

	@Override
	public boolean onCreateOptionsMenu(Menu menu) {
		// Inflate the menu; this adds items to the action bar if it is present.
		getMenuInflater().inflate(R.menu.main, menu);
		return true;
	}
	
	//Main necessary methoeds
	// get the bluetooth device reference
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
			this.startActivityForResult(enableBtIntent, REQUEST_ENABLE_BT);
		}	
	}
	
	//ask to be visible for <visibility> seconds.
	public void beVisible() {
		Log.i("BTDEV","Let me be visible. thanks.");
		Intent discoverableIntent = new Intent(BluetoothAdapter.ACTION_REQUEST_DISCOVERABLE);
		discoverableIntent.putExtra(BluetoothAdapter.EXTRA_DISCOVERABLE_DURATION, 120);
		this.startActivity(discoverableIntent);
	}
	
	private byte[] extract(InputStream inputStream) throws IOException {	
		ByteArrayOutputStream baos = new ByteArrayOutputStream();				
		byte[] buffer = new byte[277577];
		int read = 0;
		try {
		while ((read = inputStream.read(buffer, 0, buffer.length)) != -1) {
			baos.write(buffer, 0, read);
		}		
		} catch (Exception e)
		{
			// done reading - LOL fuckedupway.
		}
		baos.flush();		
		return baos.toByteArray();
	}
	 
	///start listening for incoming data
	//this method must run inside a thread.
	Bitmap mutableBitmap;
	private void listenForConnection() {
		try {
			BluetoothServerSocket bss = myBt.listenUsingRfcommWithServiceRecord("SURFACEIMGSHARE", uuid);
			
			BluetoothSocket bs = bss.accept();
			InputStream is = bs.getInputStream();
			byte[] data = extract(is);
			

			Bitmap bmp;
			bmp = BitmapFactory.decodeByteArray(data, 0, data.length);
			mutableBitmap = bmp.copy(Bitmap.Config.ARGB_8888, true);
			

			try {

				this.runOnUiThread(new Runnable() { 
					@Override 
					public void run() { 
						ImageView image = (ImageView) findViewById(R.id.imageView1);
						image.setImageBitmap(mutableBitmap);
					}
				});

			} catch (Exception e)
			{

			}
			is.close();
			bs.close();
		} catch (IOException e) {
			e.printStackTrace();
		}
	}

}
