using UnityEngine;
using TMPro;

public class HighlightManager : MonoBehaviour
{
    public OVRHand leftHand;
    public OVRHand rightHand;
    public TextMeshProUGUI debugText; // Assign this in the Inspector to display debug info
    public TextMeshProUGUI inputText; // Assign the input text component in the Inspector
    public TextMeshProUGUI outputText; // Assign the output text component in the Inspector
    public LayerMask canvasLayer; // Set to only interact with the canvas layer
    public GameObject contextMenu; // Assign the context menu in the Inspector

    private RectTransform rectTransform;
    private LineRenderer lineRenderer; // LineRenderer for ray visualization
    private Vector3 smoothedPosition;

    public float minBoxHeight = 50f; // Minimum height for clamping
    public float maxBoxHeight = 200f; // Maximum height for clamping
    public float minEditorHeight = -200f; // Minimum height boundary for positioning
    public float maxEditorHeight = 50f; // Maximum height boundary for positioning
    public float pinchThreshold = 0.7f; // Threshold to determine if fingers are pinched

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        smoothedPosition = rightHand.transform.position;

        // Ensure the context menu is initially hidden
        if (contextMenu != null)
        {
            contextMenu.SetActive(false);
        }
        else
        {
            Debug.LogWarning("ContextMenu not assigned in the Inspector.");
        }

        // Initialize the LineRenderer
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }
        lineRenderer.positionCount = 2; // Start and end points
        lineRenderer.startWidth = 0.01f;
        lineRenderer.endWidth = 0.01f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;
    }

    void Update()
    {
        // Check if either hand is making a pinch gesture
        bool leftPinch = IsPinch(leftHand);
        bool rightPinch = IsPinch(rightHand);

        if (leftPinch || rightPinch)
        {
            Debug.Log("Pinch gesture detected");

            // Position and activate the context menu
            if (contextMenu != null && !contextMenu.activeSelf)
            {
                PositionContextMenu();
                contextMenu.SetActive(true);
            }
        }
        else
        {
            Debug.Log("Pinch gesture not detected");

            // Deactivate the context menu if neither hand is pinching
            if (contextMenu != null && contextMenu.activeSelf)
            {
                contextMenu.SetActive(false);
            }
        }
    }

    // Method to check if thumb and middle finger are pinching
    bool IsPinch(OVRHand hand)
    {
        if (hand.IsTracked)
        {
            float pinchStrength = hand.GetFingerPinchStrength(OVRHand.HandFinger.Middle);
            return pinchStrength > pinchThreshold;
        }
        return false;
    }

    void PositionContextMenu()
    {
        // Set the context menu's Y position to match the Y position of this component
        if (contextMenu != null)
        {
            Vector3 newPosition = contextMenu.transform.position;
            newPosition.y = transform.position.y;
            contextMenu.transform.position = newPosition;
        }
    }

    string ExtractTextSection(float yAxis, float height)
    {
        if (inputText == null) return "";

        // Calculate start line and number of lines to extract
        int startLine = Mathf.FloorToInt(yAxis / 28);
        int numLines = Mathf.CeilToInt(height / 28);

        // Split the input text by lines
        string[] lines = inputText.text.Split('\n');

        // Clamp the start line and number of lines to prevent out-of-bounds access
        startLine = Mathf.Clamp(startLine, 0, lines.Length - 1);
        numLines = Mathf.Clamp(numLines, 1, lines.Length - startLine);

        // Extract the specified range of lines
        string extractedText = string.Join("\n", lines, startLine, numLines);
        return extractedText;
    }


    void LateUpdate()
    {
        // Smooth the hand's position for more stable raycasting
        smoothedPosition = Vector3.Lerp(smoothedPosition, rightHand.transform.position, 0.1f);

        // Calculate new size delta based on pinch strength
        float newHeight = 200 * (1 - rightHand.GetFingerPinchStrength(OVRHand.HandFinger.Index));
        newHeight = Mathf.Clamp(newHeight, minBoxHeight, maxBoxHeight);
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, newHeight);

        // Perform raycast with smoothed position
        Ray ray = new Ray(smoothedPosition, rightHand.PointerPose.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, 10.0f, canvasLayer)) // Limit raycast distance
        {
            Vector3 hitPoint = hit.point;

            // Set the line positions for the ray visualization
            lineRenderer.SetPosition(0, smoothedPosition);
            lineRenderer.SetPosition(1, hitPoint);

            // Convert hitPoint to local canvas coordinates
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform.parent as RectTransform, 
                Camera.main.WorldToScreenPoint(hitPoint), 
                Camera.main, 
                out Vector2 localPoint
            );

            float clampedY = Mathf.Clamp(localPoint.y, minEditorHeight, maxEditorHeight);
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, clampedY);

            // Update both debugText and outputText with the extracted section
            string extractedText = ExtractTextSection(localPoint.y, newHeight);
            
            // Keep previous debug information and add extracted text
            if (debugText != null)
            {
                debugText.text = "Pointer Ray Hit Position: " + hitPoint.ToString("F2") + "\n" +
                                "Local Position on Canvas: " + localPoint.ToString("F2") + "\n" +
                                "Clamped Y Position: " + clampedY.ToString("F2") + "\n" +
                                "Box Height (clamped): " + newHeight.ToString("F2") + "\n" +
                                "Extracted Text:\n" + extractedText;
            }
            
            if (outputText != null)
            {
                outputText.text = extractedText;
            }
        }
        else
        {
            // If no hit, make the line just show a short ray pointing outwards
            lineRenderer.SetPosition(0, smoothedPosition);
            lineRenderer.SetPosition(1, smoothedPosition + rightHand.PointerPose.forward * 0.5f);

            // Add fallback information in debugText if there's no hit
            if (debugText != null)
            {
                float pointerY = smoothedPosition.y;
                float clampedY = (Mathf.Abs(pointerY - minEditorHeight) < Mathf.Abs(pointerY - maxEditorHeight)) ? minEditorHeight : maxEditorHeight;
                debugText.text = "No Hit Detected\n" +
                                "Using Boundary Y Position: " + clampedY.ToString("F2") + "\n" +
                                "Box Height (clamped): " + newHeight.ToString("F2") + "\n";
            }
        }
    }
}