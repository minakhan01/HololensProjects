using UnityEngine;
using UnityEngine.VR.WSA.Input;

namespace Academy.HoloToolkit.Unity
{
    /// <summary>
    /// GestureManager contains event handlers for subscribed gestures.
    /// </summary>
    public class GestureMain : MonoBehaviour
    {
        private GestureRecognizer gestureRecognizer;

        void Start()
        {
            Debug.Log("Start");
            gestureRecognizer = new GestureRecognizer();
            gestureRecognizer.SetRecognizableGestures(GestureSettings.Tap);

            gestureRecognizer.TappedEvent += (source, tapCount, ray) =>
            {
                Debug.Log("gestureRecognizer");
            };

            gestureRecognizer.StartCapturingGestures();
        }

        void OnDestroy()
        {
            gestureRecognizer.StopCapturingGestures();
        }
    }
}