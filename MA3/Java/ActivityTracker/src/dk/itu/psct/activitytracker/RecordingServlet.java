package dk.itu.psct.activitytracker;

import java.io.IOException;
import java.util.ArrayList;
import java.util.List;

import javax.servlet.http.HttpServlet;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import com.google.appengine.api.datastore.DatastoreService;
import com.google.appengine.api.datastore.DatastoreServiceFactory;
import com.google.appengine.api.datastore.Entity;
import com.google.appengine.api.datastore.FetchOptions;
import com.google.appengine.api.datastore.Key;
import com.google.appengine.api.datastore.KeyFactory;
import com.google.appengine.api.datastore.Query;

public class RecordingServlet extends HttpServlet {
	private static final long serialVersionUID = -7149249106914008638L;

	public void doGet(HttpServletRequest req, HttpServletResponse resp) throws IOException
	  {
  		String name = req.getParameter("name");
  		String timestamp = req.getParameter("timestamp");
		String xString = req.getParameter("x");
		String yString = req.getParameter("y");
		String zString = req.getParameter("z");
		float x,y,z;
		
		x = xString != null ? Float.parseFloat(xString) : 0;
		y = yString != null ? Float.parseFloat(yString) : 0;
		z = zString != null ? Float.parseFloat(zString) : 0;
  		
		
		Entity rec = saveRecording(name, timestamp, x, y, z);
		
		
  		resp.setContentType("text/plain");
  		resp.getWriter().println(rec);
	  }
	  
	  public Entity saveRecording(String name, String timestamp, float x, float y, float z)
	  {
	        Key recordingKey = KeyFactory.createKey("Recording", name);
	        
	        Entity recording = new Entity("Recording", recordingKey);
	        recording.setProperty("timestamp", timestamp);
	        recording.setProperty("x", x);
	        recording.setProperty("y", y);
	        recording.setProperty("z", z);
	        
	        
	        DatastoreService datastore = DatastoreServiceFactory.getDatastoreService();
	        datastore.put(recording);
	        return recording;
	  }
	  
	  public List<Entity> getRecordings(String name)
	  {
		  Key recordingKey = KeyFactory.createKey("Recording", name);
		  DatastoreService datastore = DatastoreServiceFactory.getDatastoreService();
		  Query query = new Query("Recording", recordingKey);
		  List<Entity> recordings = datastore.prepare(query).asList(FetchOptions.Builder.withDefaults());
		  
		  return recordings;
	  }
}
