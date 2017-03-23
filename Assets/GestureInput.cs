using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestureInput : MonoBehaviour {

    private Vector3 position;
    public GameObject line;
    public GameObject ball;
    private bool finishedUpdate;
    private Vector3 forceVector;

    // Use this for initialization
    void Start() {
        finishedUpdate = false;
    }

    // Update is called once per frame
    void Update() {
        LineRenderer lineRenderer= line.GetComponent<LineRenderer>();
        lineRenderer.SetPosition(1, position*3);
        AnimationCurve curve = new AnimationCurve();
        curve.AddKey(0.0f, 0.0f);
        curve.AddKey(1.0f, 1.0f);
        lineRenderer.widthCurve = curve;
        lineRenderer.widthMultiplier = 1.0f;
    }

    public void updatePosition(Vector3 newPosition) {
        position = newPosition;
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
        position = newPosition;
        ball.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        finishedUpdate = true;
        forceVector = new Vector3(position.x * 100, position.y * 100, 100 * Mathf.Abs(position.z));
        //forceVector = new Vector3(0, 0, 10);
    }
}
