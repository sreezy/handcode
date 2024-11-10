using UnityEngine;
using Oculus.Interaction;

public class PinchDetector : MonoBehaviour
{
    public OVRHand ovrHand; // Reference to the OVRHand component for hand tracking
    public ZoomHandler zoomHandler; // Reference to the ZoomHandler script
    private float pinchThreshold = 0.7f; // Adjust sensitivity as needed
    private bool wasPinching = false;

    void Update()
    {
        // Check if ovrHand is set and index finger pinch strength is above threshold
        if (ovrHand != null && ovrHand.GetFingerPinchStrength(OVRHand.HandFinger.Index) > pinchThreshold)
        {
            if (!wasPinching)
            {
                zoomHandler.ZoomIn(); // Trigger zoom in action
                wasPinching = true;
            }
        }
        else
        {
            if (wasPinching)
            {
                zoomHandler.ZoomOut(); // Trigger zoom out action
                wasPinching = false;
            }
        }
    }
}