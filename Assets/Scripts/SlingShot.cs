using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlingShot : MonoBehaviour {
    
    // Use this for initialization
    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public Vector3 calculatePosition()
    {
        Camera camera = GetComponent<Camera>();
        //calculate postion at lower right corner of camera in world space
        var lowerRightPos = new Vector3(1,-1,1);
        Vector3 p = camera.ViewportToWorldPoint(lowerRightPos);
        return p;
    }
}
 