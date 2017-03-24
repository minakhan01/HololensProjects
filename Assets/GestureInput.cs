using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestureInput : MonoBehaviour {

    private Vector3 arrowOrigin;
    private Vector3 arrowTarget;
    public GameObject line;
    public GameObject ball;//big ball
    public GameObject smallBall;
    private bool finishedUpdate;
    private Vector3 forceVector;

    // Use this for initialization
    void Start() {
        finishedUpdate = false;
        arrowOrigin = smallBall.transform.position;
    }

    void Update() {

    }

    void drawArrow(Vector3 change)
    {
        arrowOrigin = smallBall.transform.position;
        arrowTarget = arrowOrigin + change;

        LineRenderer lineRenderer = line.GetComponent<LineRenderer>();
        AnimationCurve arrowShapedCurve = new AnimationCurve(new Keyframe(0, 0.4f),
            new Keyframe(0.9f, 0.4f),
            new Keyframe(0.91f, 1f),
            new Keyframe(1, 0f)
        );
        lineRenderer.widthCurve = arrowShapedCurve;
        Vector3[] arrowpositions = new Vector3[] {
            arrowOrigin,
            Vector3.Lerp(arrowOrigin,arrowTarget,0.9f),
            Vector3.Lerp(arrowOrigin,arrowTarget,0.91f),
            arrowTarget };
        lineRenderer.numPositions = arrowpositions.Length;
        lineRenderer.SetPositions(arrowpositions);
    }

    public void updatePosition(Vector3 newPosition) {
        drawArrow(newPosition);
    }

    private void FixedUpdate()
    {
        if (finishedUpdate)
        {
            ball.GetComponent<Rigidbody>().AddForce(forceVector, ForceMode.Force);
            finishedUpdate = false;
        }
    }

    public void updateFinished(Vector3 newPosition)
    {
        //arrowTarget = newPosition;
        ball.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        finishedUpdate = true;
        forceVector = new Vector3(arrowTarget.x * 100, arrowTarget.y * 100, 100 * Mathf.Abs(arrowTarget.z));
    }
}
