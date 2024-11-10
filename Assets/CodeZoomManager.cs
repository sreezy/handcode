using UnityEngine;
using TMPro; 
using UnityEngine.XR; // For XR-specific types (optional depending on the setup)
using UnityEngine.XR.Interaction.Toolkit; // For XR Interaction Toolkit (if used)
using Oculus.Interaction; // For Oculus and Meta SDK-specific components

public class CodeZoomManager : MonoBehaviour
{
    public TextMeshProUGUI codeDisplay; // Link this to your Text or TextMeshPro component

    private string level0 = "void ExampleFunction() {\n    int a = 5;\n    int b = 10;\n    int result = a + b;\n    Debug.Log(result);\n}";
    private string level1 = "ExampleFunction:\n    int a = 5;\n    int b = 10;\n    int result = a + b;\n    Debug.Log(result);";
    private string[] level2 = {
        "int a = 5;",
        "int b = 10;",
        "int result = a + b;",
        "Debug.Log(result);"
    };
    private string[][] level3 = {
        new string[] { "int", "a", "=", "5;" },
        new string[] { "int", "b", "=", "10;" },
        new string[] { "int", "result", "=", "a", "+", "b;" },
        new string[] { "Debug.Log", "(", "result", ");" }
    };

    private int currentLevel = 0;

    public void SetZoomLevel(int level)
    {
        currentLevel = Mathf.Clamp(level, 0, 3); // Adjust max level as needed
        UpdateCodeDisplay();
    }

    private void UpdateCodeDisplay()
    {
        switch (currentLevel)
        {
            case 0:
                codeDisplay.text = level0;
                break;
            case 1:
                codeDisplay.text = level1;
                break;
            case 2:
                codeDisplay.text = string.Join("\n", level2);
                break;
            case 3:
                codeDisplay.text = string.Join("\n", FlattenArray(level3));
                break;
        }
    }

    private string FlattenArray(string[][] array)
    {
        string result = "";
        foreach (string[] line in array)
        {
            result += string.Join(" ", line) + "\n";
        }
        return result.TrimEnd('\n');
    }
}