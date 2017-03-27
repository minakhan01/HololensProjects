using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GestureInput : MonoBehaviour {

    private Vector3 arrowOrigin;
    private Vector3 arrowTarget;
    public GameObject line;
    public GameObject ball;//big ball
    public GameObject smallBall;
    private bool finishedUpdate;
    private Vector3 forceVector;
    public GameObject forceText;
    public GameObject finalForceText;

    // Use this for initialization
    void Start() {
        finishedUpdate = false;
        arrowOrigin = smallBall.transform.position;
        forceText.GetComponent<Text>().text = "updated";
        finalForceText.GetComponent<Text>().text = "final";
        Debug.Log("updated");
    }

    void Update() {

    }

    void drawArrow(Vector3 change)
    {
        Vector3 ArrowOrigin = transform.position;
        Vector3 ArrowTarget = transform.position + new Vector3(change.x, change.y, 0);
        LineRenderer cachedLineRenderer = line.GetComponent<LineRenderer>();
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

        //arrowOrigin = smallBall.transform.position;
        //arrowTarget = arrowOrigin + change;

        //LineRenderer lineRenderer = line.GetComponent<LineRenderer>();
        //AnimationCurve arrowShapedCurve = new AnimationCurve(new Keyframe(0, 0.4f),
        //    new Keyframe(0.9f, 0.4f),
        //    new Keyframe(0.91f, 1f),
        //    new Keyframe(1, 0f)
        //);
        //lineRenderer.widthCurve = arrowShapedCurve;
        //Vector3[] arrowpositions = new Vector3[] {
        //    arrowOrigin,
        //    Vector3.Lerp(arrowOrigin,arrowTarget,0.9f),
        //    Vector3.Lerp(arrowOrigin,arrowTarget,0.91f),
        //    arrowTarget };
        //lineRenderer.numPositions = arrowpositions.Length;
        //lineRenderer.SetPositions(arrowpositions);
    }

    public void updatePosition(Vector3 newPosition) {
        drawArrow(newPosition);
        forceText.GetComponent<Text>().text = newPosition.ToString();
        Debug.Log("update position");
    }

    private void FixedUpdate()
    {
        if (finishedUpdate)
        {
            if (Time.timeScale == 0) {
                Time.timeScale = 1;
                return;
            }
            // ball.GetComponent<Rigidbody>().AddForce(forceVector, ForceMode.Force);
            finishedUpdate = false;
        }
    }

    public void updateFinished(Vector3 newPosition)
    {
        ball.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
        }
        finishedUpdate = true;
        finalForceText.GetComponent<Text>().text = newPosition.ToString();
        forceVector = new Vector3(newPosition.x * 2, newPosition.y * 2, 2 * Mathf.Abs(newPosition.z));
        ball.GetComponent<Rigidbody>().AddForce(Camera.main.transform.forward * forceVector.z + Camera.main.transform.up * forceVector.y + 
            Camera.main.transform.right * forceVector.x, ForceMode.Impulse);
    }
}
