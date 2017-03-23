using UnityEngine;
using System.Collections;

/// <summary>
/// Key control to roate camera boom using Arrow keys for rotation and < > keys for zoom.
///
/// Assumes the Main Camara is a child of the object holding this script with a local position offset
/// (the boom length) and oriented to point at this object. Then pressing the keys will spin the camera
/// around the object this script is attached to.
/// </summary>
public class CameraSpin : MonoBehaviour {

	//! Rate of spin (degrees per Update)
	public float spinRate = 1f; 
	public float zoomSize = 1f; 

	private Vector3 initialBoom; 
	// factor by which zoom is changed 
	private float zoomStep = 0.02f;
	private Camera boomCamera;

	// Use this for initialization
	void Start () {
		boomCamera = GetComponentInChildren<Camera>();
		if (boomCamera != null) {
			initialBoom = boomCamera.transform.localPosition;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey(KeyCode.UpArrow)) {
			transform.rotation *= Quaternion.AngleAxis( spinRate, Vector3.right);
		} else if (Input.GetKey(KeyCode.DownArrow)) {
			transform.rotation *= Quaternion.AngleAxis( -spinRate, Vector3.right);
		} else if (Input.GetKey(KeyCode.RightArrow)) {
			transform.rotation *= Quaternion.AngleAxis( spinRate, Vector3.up);
		} else if (Input.GetKey(KeyCode.LeftArrow)) {
			transform.rotation *= Quaternion.AngleAxis( -spinRate, Vector3.up);
		} else if (Input.GetKey(KeyCode.Comma)) {
			// change boom length
			zoomSize += zoomStep; 
			boomCamera.transform.localPosition = zoomSize * initialBoom;
		} else if (Input.GetKey(KeyCode.Period)) {
			// change boom lenght
			// change boom length
			zoomSize -= zoomStep; 
			if (zoomSize < 0.1f)
				zoomSize = 0.1f;
			boomCamera.transform.localPosition = zoomSize * initialBoom;
		}		

	}
}
