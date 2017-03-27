using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class linTestScript : MonoBehaviour {

    public Vector3 ArrowOrigin;
    public Vector3 ArrowTarget;
    private LineRenderer cachedLineRenderer;

    // Use this for initialization
    void Update () {
		
	}
	
	// Update is called once per frame
	void Start () {
        Vector3 ArrowOrigin = transform.position;
        Vector3 ArrowTarget = transform.position + new Vector3(0.1f,0.1f,0 );
        cachedLineRenderer = this.GetComponent<LineRenderer>();
        cachedLineRenderer.widthCurve = new AnimationCurve(
            new Keyframe(0, 0.1f)
            , new Keyframe(0.4f, 0.1f) // neck of arrow
            , new Keyframe(0.41f, 1f)  // max width of arrow head
            , new Keyframe(1, 0f));  // tip of arrow
        cachedLineRenderer.SetPositions(new Vector3[] {
             ArrowOrigin
             , Vector3.Lerp(ArrowOrigin, ArrowTarget, 0.5f)
             , Vector3.Lerp(ArrowOrigin, ArrowTarget, 0.5f)
             , ArrowTarget });
    }
}
