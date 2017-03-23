using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedPosition : MonoBehaviour {

    private Interpolator interpolator;
    private float Distance = 4.0f;

    private void Start()
    {
        if (interpolator == null)
        {
            interpolator = gameObject.AddComponent<Interpolator>();
        }

        // Screen-lock the Fitbox to match the OOBE Fitbox experience
        interpolator.PositionPerSecond = 0.0f;
    }

    int foo = 0;
    private void LateUpdate()
    {
        foo++;
        if (foo < 2) return;

        Transform cameraTransform = Camera.main.transform;

        interpolator.SetTargetPosition(cameraTransform.position + (cameraTransform.forward * Distance) + cameraTransform.up * 0.4f);
        interpolator.SetTargetRotation(Quaternion.LookRotation(-cameraTransform.forward, -cameraTransform.up));
    }

    private void Update()
    {
        
    }
}
