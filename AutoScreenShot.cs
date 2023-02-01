// attach this script on main camera, and set variable "player" to the character object.



using System;
using UnityEngine;
using System.IO;
using System.Collections;
using System.Threading.Tasks;
using System.Threading;

// This class corresponds to the 3rd person camera features.
public class AutoScreenShot : MonoBehaviour 
{
	public Transform player;                                           // Player's reference.
	public Vector3 pivotOffset = new Vector3(0.0f, 1.7f,  0.0f);       // Offset to repoint the camera.
	public Vector3 camOffset   = new Vector3(0.0f, 0.0f, -3.0f);       // Offset to relocate the camera related to the player position.
	public float smooth = 15f;                                         // Speed of camera responsiveness.
	public float horizontalAimingSpeed = 0.2f;                           // Horizontal turn speed.
	public float verticalAimingSpeed = 10f;                             // Vertical turn speed.
	public float maxVerticalAngle = 30f;                               // Camera max clamp angle. 
	public float minVerticalAngle = -60f;                              // Camera min clamp angle.
	
                                  // The default vertical axis input name.

	private float angleH = 0;                                          // Float to store camera horizontal angle related to mouse movement.
	private float angleV = 0;                                          // Float to store camera vertical angle related to mouse movement.
	private Transform cam;                                             // This transform.
	private Vector3 smoothPivotOffset;                                 // Camera current pivot offset on interpolation.
	private Vector3 smoothCamOffset;                                   // Camera current offset on interpolation.
	private Vector3 targetPivotOffset;                                 // Camera pivot offset target to iterpolate.
	private Vector3 targetCamOffset;                                   // Camera offset target to interpolate.
	private float defaultFOV;                                          // Default camera Field of View.
	private float targetFOV;                                           // Target camera Field of View.
	private float targetMaxVerticalAngle;                              // Custom camera max vertical clamp angle.
	private bool isCustomOffset;      
      

    
    //
    private int camWidth ;
    private int camHeight ;//= GetComponent<Camera>().pixelHeight;
    //

    
	async void Awake()
	{
        //
        // Create a new Texture2D with the width and height of the screen, and cache it for reuse
        camWidth = GetComponent<Camera>().pixelWidth;
        camHeight = GetComponent<Camera>().pixelHeight;

		setUpWhiteRoom(player);
		//
		
     
     //cube1.renderer.material.color = Color.white;
     


		//
		
        
		// Reference to the camera transform.
		cam = transform;

		// Set camera default position.
		cam.position = player.position + Quaternion.identity * pivotOffset + Quaternion.identity * camOffset;
		cam.rotation = Quaternion.identity;

		// Set up references and default values.
		smoothPivotOffset = pivotOffset;
		smoothCamOffset = camOffset;
		defaultFOV = cam.GetComponent<Camera>().fieldOfView;
		angleH = player.eulerAngles.y;

		ResetTargetOffsets ();
		ResetFOV ();
		
        for(int i = 0; i < 36; i++){
        if(i<12){SetCamera(i,0);}
        
        else if(i< 24){SetCamera(i,Convert.ToSingle(-0.7));}

        else{SetCamera(i,Convert.ToSingle(0.7));}
        }
		// Check for no vertical offset.
		if (camOffset.y > 0)
			Debug.LogWarning("Vertical Cam Offset (Y) will be ignored during collisions!\n" +
				"It is recommended to set all vertical offset in Pivot Offset.");
	}

	void setUpWhiteRoom(Transform target){
var cube1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
     cube1.name = "cube1";
	 cube1.transform.position = new Vector3(player.transform.position.x, player.transform.position.y-13, player.transform.position.z);
	 cube1.transform.localScale += new Vector3(68, -1, 70);
	 var cubeRenderer1 = cube1.GetComponent<Renderer>();
	 cubeRenderer1.material.SetColor("_Color", Color.white);

	 var cube2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
     cube2.name = "cube2";
	 cube2.transform.position = new Vector3(player.transform.position.x, player.transform.position.y+13, player.transform.position.z);
	 cube2.transform.localScale += new Vector3(68, -1, 70);
	 var cubeRenderer2 = cube2.GetComponent<Renderer>();
	 cubeRenderer2.material.SetColor("_Color", Color.white);

	 var cube3 = GameObject.CreatePrimitive(PrimitiveType.Cube);
     cube3.name = "cube3";
	 cube3.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z-35);
	 cube3.transform.localScale += new Vector3(68, 27, -1);
	 var cubeRenderer3 = cube3.GetComponent<Renderer>();
	 cubeRenderer3.material.SetColor("_Color", Color.white);

	 var cube4 = GameObject.CreatePrimitive(PrimitiveType.Cube);
     cube4.name = "cube4";
	 cube4.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z+35);
	 cube4.transform.localScale += new Vector3(68, 27, -1);
	  var cubeRenderer4 = cube4.GetComponent<Renderer>();
	 cubeRenderer4.material.SetColor("_Color", Color.white);

	 var cube5 = GameObject.CreatePrimitive(PrimitiveType.Cube);
     cube5.name = "cube5";
	 cube5.transform.position = new Vector3(player.transform.position.x+34, player.transform.position.y, player.transform.position.z);
	 cube5.transform.localScale += new Vector3(-1, 27, 70);
	  var cubeRenderer5 = cube5.GetComponent<Renderer>();
	 cubeRenderer5.material.SetColor("_Color", Color.white);

	 var cube6 = GameObject.CreatePrimitive(PrimitiveType.Cube);
     cube6.name = "cube6";
	 cube6.transform.position = new Vector3(player.transform.position.x-34, player.transform.position.y, player.transform.position.z);
	 cube6.transform.localScale += new Vector3(-1, 27, 70);
	  var cubeRenderer6 = cube6.GetComponent<Renderer>();
	 cubeRenderer6.material.SetColor("_Color", Color.white);
	 

	}

    
	async void Update()
	{

	}


   
    public void SetCamera(int numberOfCamera, float adjustment){

        
        for(int i = 0; i< 120; i++){
       // System.Threading.Thread.Sleep(500);
        angleH += Mathf.Clamp(5, -1, 1) * horizontalAimingSpeed;
        

        Debug.Log("i=" + i);
		

		// Set vertical movement limit.
		angleV = Mathf.Clamp(angleV, minVerticalAngle, targetMaxVerticalAngle);

		// Set camera orientation.
		Quaternion camYRotation = Quaternion.Euler(0, angleH, 0);
		Quaternion aimRotation = Quaternion.Euler(-angleV, angleH, 0);
		cam.rotation = aimRotation;

		// Set FOV.
		cam.GetComponent<Camera>().fieldOfView = Mathf.Lerp (cam.GetComponent<Camera>().fieldOfView, targetFOV,  Time.deltaTime);

		
		//Repostition the camera.
		smoothPivotOffset = Vector3.Lerp(smoothPivotOffset, targetPivotOffset, smooth * Time.deltaTime);
		smoothCamOffset = Vector3.Lerp(smoothCamOffset, targetCamOffset, smooth * Time.deltaTime);

		cam.position =  player.position + camYRotation * smoothPivotOffset + aimRotation * smoothCamOffset;
        
        }
        //Debug.Log(cam.transform.position);
        //Debug.Log(cam.transform.eulerAngles);
        
        var cameraGameObject = new GameObject("camera"+numberOfCamera);
        var camera = cameraGameObject.AddComponent<Camera>();
        cameraGameObject.transform.position = cam.transform.position;
        cameraGameObject.transform.eulerAngles = cam.transform.eulerAngles;
        cameraGameObject.transform.position += new Vector3(0, adjustment, 0);
        if(adjustment != 0){
            cameraGameObject.transform.position = Vector3.MoveTowards(cameraGameObject.transform.position, player.transform.position, Convert.ToSingle(0.2));
        
            cameraGameObject.transform.LookAt(player);
            }
        

    }

    

       
        
            
           



	// Set camera offsets to custom values.
	/*	public void SetTargetOffsets(Vector3 newPivotOffset, Vector3 newCamOffset)
	{
		targetPivotOffset = newPivotOffset;
		targetCamOffset = newCamOffset;
		isCustomOffset = true;
	}*/

	// Reset camera offsets to default values.
	public void ResetTargetOffsets()
	{
		targetPivotOffset = pivotOffset;
		targetCamOffset = camOffset;
		isCustomOffset = false;
	}

	// Reset the camera vertical offset.
	/*public void ResetYCamOffset()
	{
		targetCamOffset.y = camOffset.y;
	}

	// Set camera vertical offset.
	public void SetYCamOffset(float y)
	{
		targetCamOffset.y = y;
	}

	// Set camera horizontal offset.
	public void SetXCamOffset(float x)
	{
		targetCamOffset.x = x;
	}

	// Set custom Field of View.
	public void SetFOV(float customFOV)
	{
		this.targetFOV = customFOV;
	}*/

	// Reset Field of View to default value.
	public void ResetFOV()
	{
		this.targetFOV = defaultFOV;
	}

	// Set max vertical camera rotation angle.
	/*public void SetMaxVerticalAngle(float angle)
	{
		this.targetMaxVerticalAngle = angle;
	}

	// Reset max vertical camera rotation angle to default value.
	public void ResetMaxVerticalAngle()
	{
		this.targetMaxVerticalAngle = maxVerticalAngle;
	}

	// Double check for collisions: concave objects doesn't detect hit from outside, so cast in both directions.
	bool DoubleViewingPosCheck(Vector3 checkPos)
	{
		return ViewingPosCheck (checkPos) && ReverseViewingPosCheck (checkPos);
	}*/

	// Check for collision from camera to player.
	/*bool ViewingPosCheck (Vector3 checkPos)
	{
		// Cast target and direction.
		Vector3 target = player.position + pivotOffset;
		Vector3 direction = target - checkPos;
		// If a raycast from the check position to the player hits something...
		if (Physics.SphereCast(checkPos, 0.2f, direction, out RaycastHit hit, direction.magnitude))
		{
			// ... if it is not the player...
			if(hit.transform != player && !hit.transform.GetComponent<Collider>().isTrigger)
			{
				// This position isn't appropriate.
				return false;
			}
		}
		// If we haven't hit anything or we've hit the player, this is an appropriate position.
		return true;
	}*/

	// Check for collision from player to camera.
	/*bool ReverseViewingPosCheck(Vector3 checkPos)
	{
		// Cast origin and direction.
		Vector3 origin = player.position + pivotOffset;
		Vector3 direction = checkPos - origin;
		if (Physics.SphereCast(origin, 0.2f, direction, out RaycastHit hit, direction.magnitude))
		{
			if(hit.transform != player && hit.transform != transform && !hit.transform.GetComponent<Collider>().isTrigger)
			{
				return false;
			}
		}
		return true;
	}

	// Get camera magnitude.
	public float GetCurrentPivotMagnitude(Vector3 finalPivotOffset)
	{
		return Mathf.Abs ((finalPivotOffset - smoothPivotOffset).magnitude);
	}*/
}
