using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VectorDrawing : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Debug.DrawLine(new Vector3(0, 0, 0), new Vector3(1,0,0) , Color.red);
    }
}
