using UnityEngine;
using Oculus.Interaction; // For Oculus and Meta SDK-specific components
using UnityEngine.XR; // For XR-specific types (optional depending on the setup)
using UnityEngine.XR.Interaction.Toolkit; // For XR Interaction Toolkit (if used)

public class ZoomHandler : MonoBehaviour
{
    public CodeZoomManager codeZoomManager; // Link this to CodeZoomManager
    private int zoomLevel = 0;

    public void ZoomIn()
    {
        if (zoomLevel < 3)
        {
            zoomLevel++;
            codeZoomManager.SetZoomLevel(zoomLevel);
        }
    }

    public void ZoomOut()
    {
        if (zoomLevel > 0)
        {
            zoomLevel--;
            codeZoomManager.SetZoomLevel(zoomLevel);
        }
    }
}
