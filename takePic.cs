/// attach this script on any object, usually main camera
/// press s to start screenshot, press s again to stop, press again to restart...






using UnityEngine;
using System.IO;
 using System.Collections;
 using System.Threading.Tasks;
using System.Threading;
 
 public class takePic : MonoBehaviour {
     public int resWidth = 600; 
     public int resHeight = 800;

	 

	 private int frameRate = 30;

	 

	 public int numberOfPic = 0;
 
    

	 

	 private string timeNow;

	 private string AppPath;

	 

	 private int record = 0;

	 private bool count;
 

 //creating no-replying png file name
     public static string ScreenShotName(string dir,string camName,int index) {
         string dir2 = dir +camName+"/";

            if (!Directory.Exists(dir2))

                {Directory.CreateDirectory(dir2);}

			return dir2+"pic"+index+".png";

     }
 
     
	 void Start(){
		 
		 Application.targetFrameRate = frameRate;
		 AppPath = Application.dataPath;
		 timeNow = System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
		 Debug.Log("start ");
		 
		 

	 }

	 void shotting(string dir){


	Debug.Log("go ");
	//looping for every camera
	foreach(Camera cam in Camera.allCameras){
             RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
             cam.targetTexture = rt;
             Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
             cam.Render();
             RenderTexture.active = rt;
             screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
             cam.targetTexture = null;
             RenderTexture.active = null; 
             Destroy(rt);
             byte[] bytes = screenShot.EncodeToPNG();
             string filename = ScreenShotName(dir,cam.name,numberOfPic);
             System.IO.File.WriteAllBytes(filename, bytes);
             
			 numberOfPic++;
			 
	
}


	 }

	
 
     async void Update() {
		 count |=Input.GetKeyDown("s");
		 if(count){
record++;
Debug.Log("record="+record);
count = !count;
		 }
		 

		  

		if (record%2 == 1) {
			
			string dir = AppPath + "/screenshots/"+timeNow+"/";

            if (!Directory.Exists(dir))

                {Directory.CreateDirectory(dir);}
			
			shotting(dir);
			
            
		}

		
		
 
 }
 }