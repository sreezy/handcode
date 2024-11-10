using UnityEngine;

public class ContextMenuController : MonoBehaviour
{
    public GameObject contextMenu;         // Assign the context menu GameObject in the Inspector
    public Transform targetComponent;       // Assign the target component here
    public float heightTolerance = 0.1f;    // Tolerance range around the target height
    public bool showMenuFlag = false;       // Boolean flag to control menu visibility
    private float targetHeight;             // Target height will be set dynamically

    private void Start()
    {
        // Hide the context menu at the start
        if (contextMenu != null)
        {
            contextMenu.SetActive(false);
        }

        // Set the initial target height based on the target component's position
        if (targetComponent != null)
        {
            targetHeight = targetComponent.position.y;
        }
    }

    private void Update()
    {
        // Update targetHeight to follow the target component's height in real time
        if (targetComponent != null)
        {
            targetHeight = targetComponent.position.y;
        }

        // Check if the flag is set to show the menu and if the Y position is within the height tolerance range
        if (showMenuFlag && Mathf.Abs(transform.position.y - targetHeight) <= heightTolerance)
        {
            // Show the context menu
            if (contextMenu != null && !contextMenu.activeSelf)
            {
                contextMenu.SetActive(true);
            }
        }
        else
        {
            // Hide the context menu if the conditions are not met
            if (contextMenu != null && contextMenu.activeSelf)
            {
                contextMenu.SetActive(false);
            }
        }
    }

    // Optional: Method to toggle the menu visibility flag from other scripts or interactions
    public void ToggleMenuFlag(bool showMenu)
    {
        showMenuFlag = showMenu;
    }
}
