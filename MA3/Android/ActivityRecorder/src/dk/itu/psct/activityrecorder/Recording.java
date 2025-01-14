package dk.itu.psct.activityrecorder;

import java.io.IOException;

import org.apache.http.HttpResponse;
import org.apache.http.HttpStatus;
import org.apache.http.StatusLine;
import org.apache.http.client.HttpClient;
import org.apache.http.client.methods.HttpGet;
import org.apache.http.impl.client.DefaultHttpClient;

public class Recording {
	private String name, timeStamp;
	private float x,y,z;
	public String getName() {
		return name;
	}
	public void setName(String name) {
		this.name = name;
	}
	public float getX() {
		return x;
	}
	public void setX(float x) {
		this.x = x;
	}
	public float getY() {
		return y;
	}
	public void setY(float y) {
		this.y = y;
	}
	public float getZ() {
		return z;
	}
	public void setZ(float z) {
		this.z = z;
	}
	
	public void setTimeStamp(String timeStamp)
	{
		this.timeStamp = timeStamp;
	}
	
	public String getTimeStamp()
	{
		return timeStamp;
	}
	
	public Recording(String name, String timeStamp, float x, float y, float z)
	{
		this.name = name;
		this.timeStamp = timeStamp;
		this.x = x;
		this.y = y;
		this.z = z;
	}
	
	public void sendRecording()
	{
		String url = "http://psctacttracker.appspot.com/recordings/save?name=" + name + "&timestamp=" + timeStamp + "&x=" + x  + "&y=" + y + "&z=" + z;
		url = url.replace(" ", "%20");
		
		boolean sent = false;
		while (!sent)
		{
			try {
			    HttpClient httpclient = new DefaultHttpClient();
			    HttpResponse response = httpclient.execute(new HttpGet(url));
			    StatusLine statusLine = response.getStatusLine();
			    if(statusLine.getStatusCode() == HttpStatus.SC_OK){
			    	sent = true;
			    }
			} catch (IOException e)
			{
				// keep trying..
			}
		}
	}
}
